namespace CachManager.Archive;

public class SubArchiveFile
{
    public int Id { get; }
    public byte[] Data { get; set; }
    public byte[] RawData { get; }
    public int CompressedSize { get; }
    public int DecompressedSize { get; }
    public bool WasCompressed { get; }
    public byte[] OriginalHeader { get; set; }

    public SubArchiveFile(int id, byte[] data, byte[] rawData, int compressedSize, int decompressedSize, bool wasCompressed)
    {
        Id = id;
        Data = data;
        RawData = rawData;
        CompressedSize = compressedSize;
        DecompressedSize = decompressedSize;
        WasCompressed = wasCompressed;
    }
}
