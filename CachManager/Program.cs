using CacheManager;
using CachManager.Archive;
using CachManager.FileBlocks;
using CachManager.Idx;

string cacheDirectory = "cache";

var allEntries = new List<RSIndex>();

IndexManager idxManager = new IndexManager();
ArchiveManager archiveManager = new ArchiveManager();

FileBlockManager.Initialize(cacheDirectory);
idxManager.Initialize(cacheDirectory);
archiveManager.Initialize(cacheDirectory);

//Gets all the entries for a specific main_file_cache.idxN file.
for (int i = 0; i <= 4; i++)
    allEntries.Add(idxManager.CreateIndex(i));

// Example of how to read a file buffer (could be a song allEntries[3].Entries[0], or a subarchive etc).
var subArchiveConfigIndexEntry = allEntries[CacheConstants.ArchiveIndex].Entries[CacheConstants.SubArchiveConfigIndex];
var subArchiveConfigBuffer = FileBlockManager.GetFileBuffer(subArchiveConfigIndexEntry);
var preparedSubArchiveBuffer = archiveManager.PrepareSubArchiveBuffer(subArchiveConfigBuffer);
var subArchiveFiles = archiveManager.ReadSubArchiveFiles(preparedSubArchiveBuffer);

//Load entire cache in memory.
byte[][][] data = new byte[5][][];

for (int x = 0; x <= 4; x++)
{
    var index = idxManager.CreateIndex(x);
    data[x] = new byte[index.Entries.Count][];

    for (int y = 0; y < index.Entries.Count; y++)
    {
        var entry = allEntries[x].Entries[y];
        data[x][y] = FileBlockManager.GetFileBuffer(entry);
    }
}

Export();

void Export()
{
    string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
    Directory.CreateDirectory(outputDir);

    // 1. Rebuild main_file_cache.dat sequentially
    string dataPath = Path.Combine(outputDir, "main_file_cache.dat");
    using var dataFile = new FileStream(dataPath, FileMode.Create, FileAccess.Write);
    for (int i = 0; i <= 4; i++)
    {
        for (int y = 0; y < allEntries[i].Entries.Count; y++)
        {
            var entry = allEntries[i].Entries[y];
            var fileData = data[i][y];

            // Append each file to the .dat and update IndexEntry (FirstBlock, FileSize)
            FileBlockManager.WriteFileBlocks(dataFile, entry, fileData);
        }
    }

    // 2. Rebuild all .idx files
    for (int i = 0; i <= 4; i++)
    {
        string idxPath = Path.Combine(outputDir, $"main_file_cache.idx{i}");
        using var fs = new FileStream(idxPath, FileMode.Create, FileAccess.Write);

        foreach (var entry in allEntries[i].Entries)
        {
            byte[] buffer = new byte[CacheConstants.IndexEntrySize];

            buffer[0] = (byte)((entry.FileSize >> 16) & 0xFF);
            buffer[1] = (byte)((entry.FileSize >> 8) & 0xFF);
            buffer[2] = (byte)(entry.FileSize & 0xFF);

            buffer[3] = (byte)((entry.FirstBlock >> 16) & 0xFF);
            buffer[4] = (byte)((entry.FirstBlock >> 8) & 0xFF);
            buffer[5] = (byte)(entry.FirstBlock & 0xFF);

            fs.Write(buffer, 0, buffer.Length);
        }
    }

    Console.WriteLine("Cache successfully exported to 'output' folder.");
}