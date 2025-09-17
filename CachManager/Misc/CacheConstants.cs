namespace CacheManager;

public static class CacheConstants
{
    public const string DataFile = "main_file_cache.dat";
    public const string IndexFilePattern = "main_file_cache.idx{0}";

    public const int ChunkSize = 512;
    public const int HeaderSize = 8;
    public const int BlockSize = HeaderSize + ChunkSize;
    public const int IndexEntrySize = 6;
    
    //ArchiveNames
    public const int ArchiveIndex = 0;
    public const int ModelIndex = 1;
    public const int AnimIndex = 2;
    public const int MusicIndex = 3;
    public const int MapIndex = 4;
    
    //SubArchiveNames
    public const int SubArchiveEmptyIndex = 0;
    public const int SubArchiveTitleIndex = 1;
    public const int SubArchiveConfigIndex = 2;
    public const int SubArchiveInterfaceIndex = 3;
    public const int SubArchiveMediaIndex = 4;
    public const int SubArchiveVersionlistIndex = 5;
    public const int SubArchiveTextureIndex = 6;
    public const int SubArchiveWordencIndex = 7;
    public const int SubArchiveSoundIndex = 8;
}