using System.Text;

namespace CacheManager;

public class RSCRC
{
    public static uint CalculateCrc32(byte[] data)
    {
        return Crc32.Compute(data);
    }

    public static uint CalculateCrc32FromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        return Crc32.Compute(fileData);
    }

    public static uint CalculateCrc32FromString(string text)
    {
        byte[] data = Encoding.UTF8.GetBytes(text);
        return Crc32.Compute(data);
    }

    public static uint CalculateCrc32Streaming(byte[] data)
    {
        using (var crc32 = new Crc32())
        {
            byte[] hash = crc32.ComputeHash(data);
            return BitConverter.ToUInt32(hash, 0);
        }
    }

    public static bool VerifyFileCrc(string filePath, uint expectedCrc)
    {
        uint actualCrc = CalculateCrc32FromFile(filePath);
        return actualCrc == expectedCrc;
    }

    public static bool VerifyModelCrc(byte[] modelData, int expectedCrcFromFile)
    {
        uint calculatedCrc = CalculateCrc32(modelData);
        return (int)calculatedCrc == expectedCrcFromFile;
    }
}