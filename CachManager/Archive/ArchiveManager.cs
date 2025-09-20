using CacheManager;
using CachManager.Idx;

namespace CachManager.Archive;

public class ArchiveManager
{
    private IndexManager _indexManager;
    private FileStream _dataFile;

    public void Initialize(string cacheDirectory)
    {
        _indexManager = new IndexManager();
        _indexManager.Initialize(cacheDirectory);
        string dataFilePath = Path.Combine(cacheDirectory, CacheConstants.DataFile);
        _dataFile = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    }

    /// <summary>
    /// Prepares and extracts data from a sub-archive buffer by processing the header
    /// and decompressing the data if necessary.
    /// </summary>
    /// <param name="data">The raw sub-archive data containing header and compressed/uncompressed content.</param>
    /// <returns>The extracted and processed archive data as a byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input data is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the input data is empty or too short to contain a valid header.</exception>
    /// <exception cref="IOException">Thrown when decompression fails or the data format is invalid.</exception>
    /// <exception cref="InvalidDataException">Thrown when the header contains invalid size information.</exception>
    public byte[] PrepareSubArchiveBuffer(byte[] data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data), "Input data cannot be null");

        if (data.Length < 6)
            throw new ArgumentException("Input data is too short to contain a valid header", nameof(data));

        ReadOnlySpan<byte> headerBytes = data.AsSpan(0, 6);
        int decompressedSize = (headerBytes[0] << 16) | (headerBytes[1] << 8) | headerBytes[2];
        int compressedSize = (headerBytes[3] << 16) | (headerBytes[4] << 8) | headerBytes[5];

        if (decompressedSize <= 0)
            throw new InvalidDataException($"Invalid decompressed size: {decompressedSize}");

        if (compressedSize <= 0)
            throw new InvalidDataException($"Invalid compressed size: {compressedSize}");

        int remainingDataLength = data.Length - 6;
        if (compressedSize > remainingDataLength)
            throw new InvalidDataException(
                $"Compressed size ({compressedSize}) exceeds available data ({remainingDataLength})");

        using var memoryStream = new MemoryStream(data, 6, remainingDataLength);
        using var binaryReader = new BinaryReader(memoryStream);

        byte[] archiveData;

        if (decompressedSize != compressedSize)
        {
            byte[] compressedData = binaryReader.ReadBytes(compressedSize);
            archiveData = BZip2Helper.Decompress(compressedData);

            if (archiveData.Length != decompressedSize)
                throw new IOException(
                    $"Decompressed size mismatch. Expected {decompressedSize}, got {archiveData.Length}");
        }
        else
            archiveData = binaryReader.ReadBytes(decompressedSize);

        return archiveData;
    }

    public List<SubArchiveFile> ReadSubArchiveFiles(byte[] dataBuffer)
    {
        var files = new List<SubArchiveFile>();

        if (dataBuffer == null || dataBuffer.Length == 0)
            return files;

        using (var ms = new MemoryStream(dataBuffer))
        using (var reader = new BinaryReader(ms))
        {
            try
            {
                int fileCount = reader.ReadUInt16BigEndian();
                Console.WriteLine($"Found {fileCount} files in archive");

                // First Pass: Read Table Entries
                var entries = new List<SubArchiveFileTableEntry>();
                for (int i = 0; i < fileCount; i++)
                {
                    int id = reader.ReadInt32BigEndian();
                    int decompressedSize = reader.ReadUnsignedMedium();
                    int compressedSize = reader.ReadUnsignedMedium();
                    bool isCompressed = decompressedSize != compressedSize;
                    
                    var lookedUpName = EntryDictionary.Lookup(id);
                    var name = lookedUpName == "Unknown" ? id.ToString() : lookedUpName;

                    entries.Add(new SubArchiveFileTableEntry(id, name, decompressedSize, compressedSize, isCompressed));
                }

                // Second Pass: Read File Data
                foreach (var entry in entries)
                {
                    try
                    {
                        byte[] rawData = reader.ReadBytes(entry.CompressedSize);

                        files.Add(new SubArchiveFile(entry.Id,
                                                     entry.Name,
                                                     rawData,
                                                     entry.DecompressedSize,
                                                     entry.CompressedSize,
                                                     entry.IsCompressed));
                    }
                    catch (Exception ex)
                    {
                        throw new IOException($"Failed to read file data for entry {entry.Id}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing archive data: {ex.Message}");
                throw new InvalidDataException("Failed to parse sub-archive file structure", ex);
            }
        }

        return files;
    }
}