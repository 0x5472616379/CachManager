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

    public List<byte[]> GetArchiveFileBuffers(int idx)
    {
        var entries = _indexManager.GetEntriesForIdx(idx);
        var idxFiles = new List<byte[]>();

        foreach (var entry in entries)
        {
            var fileBuffer = ReadFileBlocks(entry);
            idxFiles.Add(fileBuffer);
        }

        return idxFiles;
    }

    public byte[] ReadFileBlocks(IndexEntry entry)
    {
        byte[] output = new byte[entry.Size];
        int bytesRead = 0;
        int currentBlock = entry.StartBlock;

        while (bytesRead < entry.Size)
        {
            byte[] header = new byte[CacheConstants.HeaderSize];
            long blockPosition = currentBlock * CacheConstants.BlockSize;

            _dataFile.Seek(blockPosition, SeekOrigin.Begin);
            int headerBytesRead = _dataFile.Read(header, 0, header.Length);

            if (headerBytesRead != header.Length)
                throw new IOException($"Failed to read header at block {currentBlock}");

            int nextBlock = (header[4] << 16) | (header[5] << 8) | header[6];
            int remainingBytes = entry.Size - bytesRead;
            int bytesToRead = Math.Min(remainingBytes, CacheConstants.ChunkSize);

            int dataBytesRead = _dataFile.Read(output, bytesRead, bytesToRead);
            if (dataBytesRead != bytesToRead)
                throw new IOException($"Failed to read data at block {currentBlock}");

            bytesRead += bytesToRead;
            currentBlock = nextBlock;

            if (currentBlock == 0 && bytesRead < entry.Size)
                throw new IOException($"Unexpected end of file chain at block {currentBlock}");
        }

        return output;
    }

    public List<SubArchiveFile> ParseArchiveData(byte[] data)
    {
        var files = new List<SubArchiveFile>();
        if (data == null || data.Length == 0)
            return files;

        byte[] headerBytes = data[..6];

        using var ms = new MemoryStream(data[6..]);
        using var br = new BinaryReader(ms);

        int dcSize = (headerBytes[0] << 16) | (headerBytes[1] << 8) | headerBytes[2];
        int cSize = (headerBytes[3] << 16) | (headerBytes[4] << 8) | headerBytes[5];

        byte[] archiveData;

        if (dcSize != cSize)
        {
            byte[] compressedData = br.ReadBytes(cSize);
            archiveData = BZip2Helper.Decompress(compressedData);
        }
        else
        {
            archiveData = br.ReadBytes(dcSize);
        }

        using var contentStream = new MemoryStream(archiveData);
        using var contentReader = new BinaryReader(contentStream);
        files = ParseArchiveContents(contentReader, files);

        // Attach header for rebuilding
        foreach (var f in files)
            f.OriginalHeader = headerBytes;

        return files;
    }

    
    
    private List<SubArchiveFile> ParseArchiveContents(BinaryReader reader, List<SubArchiveFile> files)
    {
        try
        {
            int fileCount = reader.ReadUInt16BigEndian();
            Console.WriteLine($"Found {fileCount} files in archive");
            
            var entries = new List<(int id, int decompressedSize, int compressedSize, bool isCompressed)>();
            for (int i = 0; i < fileCount; i++)
            {
                int id = reader.ReadInt32BigEndian();
                int decompressedSize = reader.ReadUnsignedMedium();
                int compressedSize = reader.ReadUnsignedMedium();
                bool isCompressed = decompressedSize != compressedSize;
                
                entries.Add((id, decompressedSize, compressedSize, isCompressed));
            }

            foreach (var (id, decompressedSize, compressedSize, isCompressed) in entries)
            {
                try
                {
                    byte[] rawData = reader.ReadBytes(compressedSize);
                    byte[] fileData = rawData;
                    if (isCompressed)
                    {
                        fileData = BZip2Helper.Decompress(fileData);
                        if (fileData.Length != decompressedSize)
                            Console.WriteLine($"Size mismatch for {id}: Expected {decompressedSize}, got {fileData.Length}");
                    }
                    files.Add(new SubArchiveFile(id, fileData, rawData, compressedSize, decompressedSize, isCompressed));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to read file {id}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing archive data: {ex.Message}");
        }
    
        return files;
    }
}