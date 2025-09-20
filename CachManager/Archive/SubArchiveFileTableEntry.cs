namespace CachManager.Archive;

public class SubArchiveFileTableEntry(int id, string name, int decompressedSize, int compressedSize, bool isCompressed)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public int DecompressedSize { get; } = decompressedSize;
    public int CompressedSize { get; } = compressedSize;
    public bool IsCompressed { get; } = isCompressed;
}