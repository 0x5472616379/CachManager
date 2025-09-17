using System.Text;

namespace CacheManager;

public static class BinaryReaderExtensions
{
    public static int ReadInt16BigEndian(this BinaryReader reader)
    {
        byte[] bytes = reader.ReadBytes(2);
        return (short)((bytes[0] << 8) | bytes[1]);
    }
    
    public static ushort ReadUInt16BigEndian(this BinaryReader reader)
    {
        byte[] bytes = reader.ReadBytes(2);
        return (ushort)((bytes[0] << 8) | bytes[1]);
    }

    public static int ReadInt32BigEndian(this BinaryReader reader)
    {
        byte[] bytes = reader.ReadBytes(4);
        return (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];
    }
    
    public static int ReadUnsignedMedium(this BinaryReader reader)
    {
        byte[] bytes = reader.ReadBytes(3);
        return (bytes[0] << 16) | (bytes[1] << 8) | bytes[2];
    }
    
    public static string ReadCacheString(this BinaryReader reader) {
        var bytes = new List<byte>();
        byte b;
        while ((b = reader.ReadByte()) != 10) {
            bytes.Add(b);
        }
        return Encoding.GetEncoding("ISO-8859-1").GetString(bytes.ToArray());
    }
}