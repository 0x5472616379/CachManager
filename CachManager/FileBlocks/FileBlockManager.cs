using CacheManager;
using CachManager.Idx;

namespace CachManager.FileBlocks;

public class FileBlockManager
{
    private static FileStream _dataFile;

    public static void Initialize(string cacheDirectory)
    {
        string dataFilePath = Path.Combine(cacheDirectory, CacheConstants.DataFile);
        _dataFile = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    }

    /// <summary>
    /// Gets the buffer of any file.
    /// </summary>
    /// <param name="entry">The IndexEntry which points to the file.</param>
    /// <returns>The entire buffer of that file.</returns>
    /// <exception cref="Exception">hrows an exception if the data length read doesn't match with the FileSize property.</exception>
    public static byte[] GetFileBuffer(IndexEntry entry)
    {
        var data = ReadFileBlocks(entry);
        if (data.Length != entry.FileSize)
            throw new IOException(
                $"Data length mismatch. Expected {entry.FileSize} bytes but read {data.Length} bytes. The file may be corrupted.");

        return data;
    }

    /// <summary>
    /// Reads all the file blocks starting from the specified entry's first block.
    /// </summary>
    /// <param name="entry">The index entry containing file metadata and starting block.</param>
    /// <returns>The complete file data as a byte array.</returns>
    /// <exception cref="IOException">Thrown when file reading fails or data is corrupted.</exception>
    public static byte[] ReadFileBlocks(IndexEntry entry)
    {
        byte[] output = new byte[entry.FileSize];
        int bytesRead = 0;
        int currentBlock = entry.FirstBlock;
        while (bytesRead < entry.FileSize)
        {
            byte[] header = new byte[CacheConstants.HeaderSize];
            long blockPosition = currentBlock * CacheConstants.BlockSize;

            _dataFile.Seek(blockPosition, SeekOrigin.Begin);
            int headerBytesRead = _dataFile.Read(header, 0, header.Length);

            if (headerBytesRead != header.Length)
                throw new IOException($"Failed to read header at block {currentBlock}");

            int NextFileId = (header[0] << 8) | header[1];
            int CurrentFilePartId = (header[2] << 8) | header[3];
            int nextBlock = (header[4] << 16) | (header[5] << 8) | header[6];
            int NextFileTypeId = header[7];

            int remainingBytes = entry.FileSize - bytesRead;
            int bytesToRead = Math.Min(remainingBytes, CacheConstants.ChunkSize);
            int dataBytesRead = _dataFile.Read(output, bytesRead, bytesToRead);

            if (dataBytesRead != bytesToRead)
                throw new IOException($"Failed to read data at block {currentBlock}");

            bytesRead += bytesToRead;
            currentBlock = nextBlock;

            if (currentBlock == 0 && bytesRead < entry.FileSize)
                throw new IOException($"Unexpected end of file chain at block {currentBlock}");
        }

        return output;
    }

    /// <summary>
    /// Appends a file to the cache by writing its data into 520-byte blocks,
    /// building an 8-byte header for each block, and updating the associated index entry.
    /// Each block contains up to 512 bytes of file data, with a header specifying:
    /// the file ID, part number, next block, and cache index (+1).
    /// All blocks are 520 bytes, and the last block may contain padding if the file
    /// data does not fill the full 512 bytes. A next block value of 0 indicates
    /// the end of the file chain.
    /// </summary>
    /// <param name="datFile">The file stream representing the cache data file.</param>
    /// <param name="entry">The index entry to update with file metadata and starting block.</param>
    /// <param name="fileData">The file data to write to the cache.</param>
    public static void WriteFileBlocks(FileStream datFile, IndexEntry entry, byte[] fileData)
    {
        int size = fileData.Length;
        int written = 0;
        int part = 0;

        int sector = (int)((datFile.Length + 519) / 520); // round up to next sector
        if (sector == 0) sector = 1; // first sector is 1
        
        
        entry.FirstBlock = sector;
        entry.FileSize = size;

        while (written < size)
        {
            int remaining = size - written;
            int chunkSize = Math.Min(remaining, 512);
            int nextSector = (remaining > 512) ? sector + 1 : 0;

            int blockSize = 8 + chunkSize; // Only full block if not last
            Span<byte> block = stackalloc byte[520]; // still allocate max for safety

            block[0] = (byte)((entry.EntryId >> 8) & 0xFF);
            block[1] = (byte)(entry.EntryId & 0xFF);
            block[2] = (byte)((part >> 8) & 0xFF);
            block[3] = (byte)(part & 0xFF);
            block[4] = (byte)((nextSector >> 16) & 0xFF);
            block[5] = (byte)((nextSector >> 8) & 0xFF);
            block[6] = (byte)(nextSector & 0xFF);
            block[7] = (byte)(entry.CacheIndex + 1);

            for (int i = 0; i < chunkSize; i++)
                block[8 + i] = fileData[written + i];

            datFile.Seek(sector * 520L, SeekOrigin.Begin);
            datFile.Write(block.Slice(0, blockSize)); // write exact bytes, no padding for last

            written += chunkSize;
            part++;
            sector = nextSector;
        }
    }
}