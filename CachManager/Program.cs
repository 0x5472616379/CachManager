using CacheManager;
using CacheManager.Items;
using CachManager.Archive;
using CachManager.FileBlocks;
using CachManager.Idx;

string cacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "CacheTool317",
    "cache");

string outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CacheTool317",
    "output");

string testDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CacheTool317",
    "test");

Directory.CreateDirectory(cacheDirectory);
Directory.CreateDirectory(outputDir);
Directory.CreateDirectory(testDir);

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

ModifyItemData();
Export();

void Export()
{
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



void ModifyItemData()
{
    // Get the config sub-archive files
    var objIdx = subArchiveFiles.FirstOrDefault(x => x.Id == EntryDictionary.Hash("obj.idx"));
    var objDat = subArchiveFiles.FirstOrDefault(x => x.Id == EntryDictionary.Hash("obj.dat"));
    
    if (objIdx == null || objDat == null)
    {
        Console.WriteLine("Could not find obj.idx or obj.dat in config archive");
        return;
    }

    // Decompress and decode
    var decompressedIdx = objIdx.IsCompressed ? BZip2Helper.Decompress(objIdx.RawData) : objIdx.RawData;
    var decompressedDat = objDat.IsCompressed ? BZip2Helper.Decompress(objDat.RawData) : objDat.RawData;
    
    ItemDefDecoder decoder = new ItemDefDecoder();
    decoder.Run(decompressedIdx, decompressedDat);
    var defs = decoder.Definitions;
    
    // Modify index 1007
    if (defs.Length > 1007)
    {
        var redCape = defs[1007];
        Console.WriteLine($"Original name: {redCape.Name}");
        redCape.Name = "Cape of neitiznot";
        Console.WriteLine($"Modified name: {redCape.Name}");
    }
    else
    {
        Console.WriteLine($"Item index 1007 not found. Total items: {defs.Length}");
        return;
    }
    
     // Re-encode the definitions
     var (newIdxData, newDatData) = ItemDefEncoder.Encode(defs);
    
     File.WriteAllBytes(Path.Combine(testDir, "originalObjIdx.bin"), decompressedIdx);
     File.WriteAllBytes(Path.Combine(testDir, "originalObjDat.bin"), decompressedDat);
     
     File.WriteAllBytes(Path.Combine(testDir, "newObjIdx.bin"), newIdxData);
     File.WriteAllBytes(Path.Combine(testDir, "newObjDat.bin"), newDatData);
     
     // Compress the data
     byte[] compressedIdx = BZip2Helper.Compress(newIdxData);
     byte[] compressedDat = BZip2Helper.Compress(newDatData);
    
      // Update the sub-archive files
      objIdx.RawData = compressedIdx;
      objIdx.CompressedSize = compressedIdx.Length;
      objIdx.DecompressedSize = newIdxData.Length;
      objIdx.IsCompressed = true;
     
      objDat.RawData = compressedDat;
      objDat.CompressedSize = compressedDat.Length;
      objDat.DecompressedSize = newDatData.Length;
      objDat.IsCompressed = true;
    
    // Now rebuild the config sub-archive buffer (should not be compressed)
    byte[] newConfigBuffer = RebuildSubArchiveBuffer(subArchiveFiles);

    var length = newConfigBuffer.Length;
    var fullConfigBuffer = CreateSubArchiveWithHeader(length, length, newConfigBuffer);
    
    // Update the main cache data
    data[CacheConstants.ArchiveIndex][CacheConstants.SubArchiveConfigIndex] = fullConfigBuffer;
    
    Console.WriteLine("Item data modified successfully");
}

byte[] CreateSubArchiveWithHeader(int decompressedSize, int compressedSize, byte[] compressedData)
{
    using (var ms = new MemoryStream())
    using (var writer = new BinaryWriter(ms))
    {
        // Write the sub-archive header (6 bytes)
        writer.WriteUnsignedMedium(decompressedSize);
        writer.WriteUnsignedMedium(compressedSize);
        
        // Write the data
        writer.Write(compressedData);
        
        return ms.ToArray();
    }
}

byte[] RebuildSubArchiveBuffer(List<SubArchiveFile> files)
{
    using (var ms = new MemoryStream())
    using (var writer = new BinaryWriter(ms))
    {
        // Write file count
        writer.WriteUInt16BigEndian((ushort)files.Count);
        
        // Write table entries
        foreach (var file in files)
        {
            writer.WriteInt32BigEndian(file.Id);
            writer.WriteUnsignedMedium(file.DecompressedSize);
            writer.WriteUnsignedMedium(file.CompressedSize);
        }
        
        // Write file data
        foreach (var file in files)
        {
            writer.Write(file.RawData);
        }
        
        return ms.ToArray();
    }
}