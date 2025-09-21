using System.IO.Compression;

namespace CacheManager;

public class GZipHelper
{
    public static byte[] Compress(byte[] data)
    {
        using (var outputStream = new MemoryStream())
        {
            using (var zipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
            }
            return outputStream.ToArray();
        }
    }
    
    public static byte[] Decompress(byte[] data)
    {
        using (var compressedStream = new MemoryStream(data))
        using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
        using (var resultStream = new MemoryStream())
        {
            zipStream.CopyTo(resultStream);
            return resultStream.ToArray();
        }
    }
}