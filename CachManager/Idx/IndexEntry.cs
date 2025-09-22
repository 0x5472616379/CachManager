using CacheManager;

namespace CachManager.Idx;

public class IndexEntry
{
    public int EntryId { get; set; }
    public int FileSize { get; set; }
    public int FirstBlock { get; set; }
    public int CacheIndex { get; set; }

    public IndexEntry(int id, byte[] buffer, int cacheIndex)
    {
        if (buffer.Length != CacheConstants.IndexEntrySize)
            throw new ArgumentException($"Index entry must be {CacheConstants.IndexEntrySize} bytes", nameof(buffer));

        FileSize = (buffer[0] << 16) | (buffer[1] << 8) | buffer[2];
        FirstBlock = (buffer[3] << 16) | (buffer[4] << 8) | buffer[5];
        EntryId = id;
        CacheIndex = cacheIndex;
    }
}