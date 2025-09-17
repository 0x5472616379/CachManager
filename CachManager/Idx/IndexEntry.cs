namespace CachManager.Idx;

public class IndexEntry
{
    public int Size { get; private set; }
    public int StartBlock { get; private set; }
    public int FileId { get; set; }

    public IndexEntry(int size, int startBlock, int fileId = -1)
    {
        Size = size;
        StartBlock = startBlock;
        FileId = fileId;
    }
}