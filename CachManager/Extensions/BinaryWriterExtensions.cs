using System.Text;

namespace CacheManager;


public static class BinaryWriterExtensions
{
    public static void WriteInt16BigEndian(this BinaryWriter writer, short value)
    {
        writer.Write((byte)(value >> 8));
        writer.Write((byte)value);
    }
    
    public static void WriteUInt16BigEndian(this BinaryWriter writer, ushort value)
    {
        writer.Write((byte)(value >> 8));
        writer.Write((byte)value);
    }
    
    public static void WriteInt32BigEndian(this BinaryWriter writer, int value)
    {
        writer.Write((byte)(value >> 24));
        writer.Write((byte)(value >> 16));
        writer.Write((byte)(value >> 8));
        writer.Write((byte)value);
    }
    
    public static void WriteUnsignedMedium(this BinaryWriter writer, int value)
    {
        if (value < 0 || value > 0xFFFFFF)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Unsigned Medium must be between 0 and 16,777,215");
        }
        writer.Write((byte)(value >> 16));
        writer.Write((byte)(value >> 8));
        writer.Write((byte)value);
    }
    
    public static void WriteCacheString(this BinaryWriter writer, string value) {
        var bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(value);
        writer.Write(bytes);
        writer.Write((byte)10);
    }
}