using CacheManager;

namespace CachManager.Idx;

public class IndexManager
{
    private FileStream[] _indexFiles;

    public void Initialize(string cacheDirectory)
    {
        _indexFiles = new FileStream[5];
        for (int i = 0; i < 5; i++)
        {
            string indexPath = Path.Combine(cacheDirectory, string.Format(CacheConstants.IndexFilePattern, i));
            _indexFiles[i] = new FileStream(indexPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }

    private int GetEntryCount(int idx) => (int)_indexFiles[idx].Length / CacheConstants.IndexEntrySize;

    public RSIndex CreateIndex(int idx)
    {
        var indexCount = GetEntryCount(idx);
        var index = new RSIndex();

        for (int fileId = 0; fileId < indexCount; fileId++)
        {
            long position = fileId * CacheConstants.IndexEntrySize;
            var buffer = new byte[CacheConstants.IndexEntrySize];

            _indexFiles[idx].Seek(position, SeekOrigin.Begin);
            int bytesRead = _indexFiles[idx].Read(buffer, 0, buffer.Length);

            if (bytesRead != buffer.Length)
                throw new IOException($"Failed to read index entry at position {position}");

            index.Entries.Add(new IndexEntry(fileId, buffer, idx));
        }

        return index;
    }
}