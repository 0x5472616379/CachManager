using CacheManager;
using CachManager.Idx;

namespace CachManager.Export;

// public class CacheRebuilder
// {
//     public void Rebuild(string exportedDirectory, string outputDirectory)
//     {
//         Directory.CreateDirectory(outputDirectory);
//
//         // Create the data file
//         string datPath = Path.Combine(outputDirectory, "main_file_cache.dat");
//         using (FileStream datStream = new FileStream(datPath, FileMode.Create))
//         {
//             // Process each index directory
//             for (int idx = 0; idx <= 4; idx++)
//             {
//                 string indexDir = Path.Combine(exportedDirectory, $"idx{idx}");
//                 if (!Directory.Exists(indexDir)) continue;
//
//                 RebuildIndex(idx, indexDir, datStream, outputDirectory);
//             }
//         }
//     }
//
//     private static readonly Dictionary<string, int> ArchiveNamesIdx0Reverse =
//         CacheExporter.ArchiveNamesIdx0.ToDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);
//
//
//     private void RebuildIndex(int idx, string indexDir, FileStream datStream, string outputDirectory)
//     {
//         var indexEntries = new List<IndexEntry>();
//         var files = Directory.GetFiles(indexDir, "*", SearchOption.TopDirectoryOnly);
//
//         foreach (var filePath in files)
//         {
//             string fileName = Path.GetFileName(filePath);
//             int fileId = -1;
//
//             // First try numeric filenames
//             string numericPart = Path.GetFileNameWithoutExtension(fileName);
//             if (int.TryParse(numericPart, out int parsedId))
//             {
//                 fileId = parsedId;
//             }
//             else if (idx == 0)
//             {
//                 if (!ArchiveNamesIdx0Reverse.TryGetValue(fileName, out fileId))
//                 {
//                     Console.WriteLine(
//                         $"Skipping file {fileName} in idx{idx} because it’s not a recognized top-level archive.");
//                     continue;
//                 }
//             }
//
//             if (fileId == -1)
//             {
//                 Console.WriteLine($"Skipping file {fileName} in idx{idx} due to invalid file ID format.");
//                 continue;
//             }
//
//             byte[] fileData = File.ReadAllBytes(filePath);
//             long startBlock = datStream.Position / 520; // 520 = full block size
//             WriteFileData(datStream, fileData, fileId, idx);
//             indexEntries.Add(new IndexEntry(fileData.Length, (int)startBlock, fileId));
//         }
//
//         // Create the index file
//         CreateIndexFile(outputDirectory, idx, indexEntries);
//     }
//
//
//     private void WriteFileData(FileStream datStream, byte[] data, int fileId, int indexId)
//     {
//         const int blockSize = 520; // 8 header + 512 data
//         const int payloadSize = 512;
//         const int headerSize = 8;
//
//         int totalBlocks = (data.Length + payloadSize - 1) / payloadSize;
//
//         for (int chunk = 0; chunk < totalBlocks; chunk++)
//         {
//             long currentBlock = datStream.Position / blockSize;
//             long nextBlock = (chunk == totalBlocks - 1) ? 0 : currentBlock + 1;
//
//             // Build header (8 bytes)
//             byte[] header = new byte[headerSize];
//             header[0] = (byte)(fileId >> 8);
//             header[1] = (byte)(fileId);
//             header[2] = (byte)(chunk >> 8);
//             header[3] = (byte)(chunk);
//             header[4] = (byte)(nextBlock >> 16);
//             header[5] = (byte)(nextBlock >> 8);
//             header[6] = (byte)(nextBlock);
//             header[7] = (byte)indexId;
//
//             datStream.Write(header, 0, header.Length);
//
//             // Payload
//             int offset = chunk * payloadSize;
//             int length = Math.Min(payloadSize, data.Length - offset);
//             datStream.Write(data, offset, length);
//
//             // Pad remaining space in the block
//             if (length < payloadSize)
//             {
//                 datStream.Write(new byte[payloadSize - length], 0, payloadSize - length);
//             }
//         }
//     }
//
//
//     private void CreateIndexFile(string outputDirectory, int idx, List<IndexEntry> entries)
//     {
//         string idxPath = Path.Combine(outputDirectory, string.Format(CacheConstants.IndexFilePattern, idx));
//         using (FileStream fs = new FileStream(idxPath, FileMode.Create))
//         using (BinaryWriter writer = new BinaryWriter(fs))
//         {
//             int maxFileId = (idx == 0) ? CacheExporter.ArchiveNamesIdx0.Keys.Max() : entries.Max(e => e.FileId);
//
//             for (int fileId = 0; fileId <= maxFileId; fileId++)
//             {
//                 var entry = entries.FirstOrDefault(e => e.FileId == fileId);
//                 if (entry == null)
//                 {
//                     // pad empty slot (important for idx0[0])
//                     writer.Write(new byte[6]);
//                 }
//                 else
//                 {
//                     writer.Write((byte)(entry.Size >> 16));
//                     writer.Write((byte)(entry.Size >> 8));
//                     writer.Write((byte)entry.Size);
//                     writer.Write((byte)(entry.StartBlock >> 16));
//                     writer.Write((byte)(entry.StartBlock >> 8));
//                     writer.Write((byte)entry.StartBlock);
//                 }
//             }
//         }
//     }
// }