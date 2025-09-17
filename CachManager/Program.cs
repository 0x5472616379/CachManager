using CacheManager;
using CachManager.Archive;
using CachManager.Export;
using CachManager.Idx;

string cacheDirectory = "cache";
string idxOutputDirectory = "output/";

IndexManager idxManager = new IndexManager();
idxManager.Initialize(cacheDirectory);

//Gets all the entries for a specific main_file_cache.idx file.
var entries = idxManager.GetEntriesForIdx(0);

ArchiveManager archiveManager = new ArchiveManager();
archiveManager.Initialize(cacheDirectory);

//Gets all the Sub-Archives buffers with the help of main_file_cache.idx0 (idx0 is the only one that points to a sub-archive)
var archiveFileBuffers =
    archiveManager.GetArchiveFileBuffers(CacheConstants.ArchiveIndex); //Archive Files (title, config, interface..)

//Gets all the Sub-Archive files for a specific sub archive index.
//Some Sub-Archive Buffers are compressed, hence why we also handle decompression in there for those specific ones.
var subArchiveFiles =
    archiveManager.ParseArchiveData(
        archiveFileBuffers[CacheConstants.SubArchiveConfigIndex]); //config archive contains obj.idx, obj.dat..

var exporter = new CacheExporter(archiveManager);
exporter.ExportAll("output");

Console.WriteLine("Done!");

// File.WriteAllBytes("OriginalConfig.dat", archiveFileBuffers[CacheConstants.SubArchiveConfigIndex]);
//
// var newData = BuildSubArchive(subArchiveFiles);
// File.WriteAllBytes("NewConfig.dat", newData);

byte[] BuildSubArchive(List<SubArchiveFile> files)
{
    using var ms = new MemoryStream();
    using var bw = new BinaryWriter(ms);

    // Write original 6-byte header since we don't plan on adding any new sub-archives.
    //This header contains the compressed / decompressed size
    if (files.Count > 0 && files[0].OriginalHeader != null)
        bw.Write(files[0].OriginalHeader);

    // Header: number of files
    bw.WriteUInt16BigEndian((ushort)files.Count);

    // First pass: file entries
    foreach (var file in files)
    {
        bw.WriteInt32BigEndian(file.Id);
        bw.WriteUnsignedMedium(file.DecompressedSize);
        bw.WriteUnsignedMedium(file.WasCompressed ? file.CompressedSize : file.DecompressedSize);
    }

    // Second pass: file data
    foreach (var file in files)
    {
        byte[] dataToWrite = file.WasCompressed ? file.RawData : file.Data;
        bw.Write(dataToWrite);
    }

    return ms.ToArray();
}