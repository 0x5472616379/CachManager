using ICSharpCode.SharpZipLib.BZip2;

namespace CacheManager;

public static class BZip2Helper
{
    private static readonly byte[] BZip2Header = "BZh1"u8.ToArray();

    public static byte[] Decompress(byte[] compressedData)
    {
        // Add standard BZIP2 header since Jagex strips it
        byte[] withHeader = new byte[compressedData.Length + BZip2Header.Length];
        Buffer.BlockCopy(BZip2Header, 0, withHeader, 0, BZip2Header.Length);
        Buffer.BlockCopy(compressedData, 0, withHeader, BZip2Header.Length, compressedData.Length);
    
        using var input = new MemoryStream(withHeader);
        using var output = new MemoryStream();
        using var decompressor = new BZip2InputStream(input);
        decompressor.CopyTo(output);
        return output.ToArray();
    }
    public static byte[] Compress(byte[] data, bool stripHeader = true, int level = 1)
    {
        using var input = new MemoryStream(data);
        using var output = new MemoryStream();
    
        BZip2.Compress(input, output, isStreamOwner: false, level: level);
    
        byte[] compressed = output.ToArray();
    
        if (!stripHeader) return compressed;

        byte[] withoutHeader = new byte[compressed.Length - 4];
        Buffer.BlockCopy(compressed, 4, withoutHeader, 0, withoutHeader.Length);
        return withoutHeader;
    }
}