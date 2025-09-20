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
    
    public static readonly Dictionary<int, string> ArchiveNames = new()
    {
        { 0, "archive" },
        { 1, "model" },
        { 2, "anim" },
        { 3, "music" },
        { 4, "map" },
    };
    
    public static readonly Dictionary<int, string> SubArchiveNames = new()
    {
        { 0, "empty.dat" },
        { 1, "title.dat" },
        { 2, "config.dat" },
        { 3, "interface.dat" },
        { 4, "media.dat" },
        { 5, "versionlist.dat" },
        { 6, "texture.dat" },
        { 7, "wordenc.dat" },
        { 8, "sound.dat" }
    };
}