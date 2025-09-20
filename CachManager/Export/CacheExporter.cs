using CacheManager;
using CachManager.Archive;

namespace CachManager.Export;

// public class CacheExporter
// {
//     public static readonly Dictionary<int, string> ArchiveNamesIdx0 = new()
//     {
//         { 1, "title.dat" },
//         { 2, "config.dat" },
//         { 3, "interface.dat" },
//         { 4, "media.dat" },
//         { 5, "versionlist.dat" },
//         { 6, "texture.dat" },
//         { 7, "wordenc.dat" },
//         { 8, "sound.dat" }
//     };
//
//     private readonly ArchiveManager _archiveManager;
//
//     public CacheExporter(ArchiveManager archiveManager)
//     {
//         _archiveManager = archiveManager;
//     }
//
//     public void ExportAll(string outputRoot)
//     {
//         Directory.CreateDirectory(outputRoot);
//
//         for (int idx = 0; idx <= 4; idx++)
//         {
//             Console.WriteLine($"Exporting idx{idx}...");
//             ExportIndex(idx, Path.Combine(outputRoot, $"idx{idx}"));
//         }
//     }
//
//     private void ExportIndex(int idx, string outputPath)
//     {
//         Directory.CreateDirectory(outputPath);
//
//         var buffers = _archiveManager.GetArchiveFileBuffers(idx);
//         for (int fileId = 0; fileId < buffers.Count; fileId++)
//         {
//             var buffer = buffers[fileId];
//             if (buffer == null || buffer.Length == 0)
//                 continue;
//
//             // Give special names to top-level archives in idx0
//             string friendlyName;
//             if (idx == 0 && ArchiveNamesIdx0.TryGetValue(fileId, out var archiveName))
//                 friendlyName = archiveName;
//             else
//             {
//                 friendlyName = EntryDictionary.Lookup(fileId);
//                 if (friendlyName == "Unknown")
//                     friendlyName = $"{fileId:D4}.dat";
//             }
//
//             string filePath = Path.Combine(outputPath, friendlyName);
//             File.WriteAllBytes(filePath, buffer);
//
//             // Parse as sub-archive only if it looks like one
//             try
//             {
//                 var subFiles = _archiveManager.ParseSubArchive(buffer);
//                 if (subFiles.Count > 0)
//                 {
//                     string subDir = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(friendlyName));
//                     Directory.CreateDirectory(subDir);
//
//                     foreach (var sub in subFiles)
//                     {
//                         string subFriendlyName = EntryDictionary.Lookup(sub.Id);
//                         if (subFriendlyName == "Unknown")
//                             subFriendlyName = $"{sub.Id:D8}.bin"; // keep big ID format
//
//                         string subPath = Path.Combine(subDir, subFriendlyName);
//                         File.WriteAllBytes(subPath, sub.RawData);
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 //Console.WriteLine($"Failed to parse archive file {fileId} in idx{idx}: {ex.Message}");
//             }
//         }
//     }
// }