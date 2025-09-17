using CacheManager;
using CachManager.Archive;

namespace CachManager.Export;

public class CacheExporter
    {
        private readonly ArchiveManager _archiveManager;

        public CacheExporter(ArchiveManager archiveManager)
        {
            _archiveManager = archiveManager;
        }

        public void ExportAll(string outputRoot)
        {
            Directory.CreateDirectory(outputRoot);

            for (int idx = 0; idx <= 4; idx++)
            {
                Console.WriteLine($"Exporting idx{idx}...");
                ExportIndex(idx, Path.Combine(outputRoot, $"idx{idx}"));
            }
        }

        private void ExportIndex(int idx, string outputPath)
        {
            Directory.CreateDirectory(outputPath);

            var buffers = _archiveManager.GetArchiveFileBuffers(idx);
            for (int fileId = 0; fileId < buffers.Count; fileId++)
            {
                var buffer = buffers[fileId];

                if (buffer == null || buffer.Length == 0)
                    continue;

                string friendlyName = EntryDictionary.Lookup(fileId);
                if (friendlyName == "Unknown")
                    friendlyName = $"{fileId:D4}.dat";

                string filePath = Path.Combine(outputPath, friendlyName);
                File.WriteAllBytes(filePath, buffer);

                try
                {
                    var subFiles = _archiveManager.ParseArchiveData(buffer);
                    if (subFiles.Count > 0)
                    {
                        string subDir = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(friendlyName));
                        Directory.CreateDirectory(subDir);

                        foreach (var sub in subFiles)
                        {
                            string subName = EntryDictionary.Lookup(sub.Id);
                            if (subName == "Unknown")
                                subName = $"{sub.Id}.bin";

                            string subPath = Path.Combine(subDir, subName);
                            File.WriteAllBytes(subPath, sub.RawData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to parse archive file {fileId} in idx{idx}: {ex.Message}");
                }
            }
        }
    }