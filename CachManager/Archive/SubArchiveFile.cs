using CacheManager;

namespace CachManager.Archive;

public class SubArchiveFile(int id, string name, byte[] rawData, int decompressedSize, int compressedSize, bool isCompressed)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public byte[] RawData { get; set; } = rawData;
    public int DecompressedSize { get; set; } = decompressedSize;
    public int CompressedSize { get; set; } = compressedSize;
    public bool IsCompressed { get; set; } = isCompressed;

    public byte[] GetDecompressedData()
    {
        if (!IsCompressed) return RawData;
        
        var decompressed = BZip2Helper.Decompress(RawData);
        if (decompressed.Length != DecompressedSize)
            throw new InvalidDataException($"Decompression size mismatch for file {Id}");
        
        return decompressed;
    }
}