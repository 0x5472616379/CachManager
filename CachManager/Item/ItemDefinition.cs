namespace CacheManager.Items;


public class ItemDefinition
{
    public int Id { get; set; } = -1;

    // Core
    public int ModelId { get; set; }
    public string Name { get; set; }
    public string Examine { get; set; }

    // Icon
    public ushort IconZoom { get; set; } = 2000;
    public ushort IconPitch { get; set; }
    public ushort IconYaw { get; set; }
    public short IconOffsetX { get; set; }
    public short IconOffsetY { get; set; }
    public int OpCode10 { get; set; }
    public int IconRoll { get; set; }

    public bool Stackable { get; set; }
    public int Cost { get; set; } = 1;
    public bool Members { get; set; }

    // Model properties (U16 for IDs, signed byte for offsets)
    public int MaleModelId0 { get; set; } = -1; // -1 in Java
    public int MaleModelId1 { get; set; } = -1;
    public int MaleModelId2 { get; set; } = -1;
    public int MaleHeadModelId0 { get; set; } = -1;
    public int MaleHeadModelId1 { get; set; } = -1;
    public sbyte MaleOffsetY { get; set; }

    public int FemaleModelId0 { get; set; } = -1;
    public int FemaleModelId1 { get; set; } = -1;
    public int FemaleModelId2 { get; set; } = -1;
    public int FemaleHeadModelId0 { get; set; } = -1;
    public int FemaleHeadModelId1 { get; set; } = -1;
    public sbyte FemaleOffsetY { get; set; }

    // Options
    public string[] Options { get; set; }
    public string[] InventoryOptions { get; set; }

    // Recolor
    public int[] SrcColor { get; set; }
    public int[] DstColor { get; set; }

    // Stacks
    public int[] StackId { get; set; }
    public int[] StackCount { get; set; }

    // Scale
    public int ScaleX { get; set; } = 128;
    public int ScaleY { get; set; } = 128;
    public int ScaleZ { get; set; } = 128;

    // Lighting
    public byte LightAmbient { get; set; }
    public int LightAttenuation { get; set; }

    public byte Team { get; set; }

    // Links
    public int LinkedId { get; set; } = -1;
    public int CertificateId { get; set; } = -1;

    public void Reset()
    {
        ModelId = -1;
        Name = null;
        Examine = null;
        SrcColor = null;
        DstColor = null;
        IconZoom = 2000;
        IconPitch = 0;
        IconYaw = 0;
        IconRoll = 0;
        IconOffsetX = 0;
        IconOffsetY = 0;
        Stackable = false;
        Cost = 1;
        Members = false;
        Options = null;
        InventoryOptions = null;

        MaleModelId0 = MaleModelId1 = MaleModelId2 = -1;
        FemaleModelId0 = FemaleModelId1 = FemaleModelId2 = -1;
        MaleHeadModelId0 = MaleHeadModelId1 = -1;
        FemaleHeadModelId0 = FemaleHeadModelId1 = -1;
        MaleOffsetY = 0;
        FemaleOffsetY = 0;

        StackId = null;
        StackCount = null;

        LinkedId = -1;
        CertificateId = -1;

        ScaleX = ScaleY = ScaleZ = 128;
        LightAmbient = 0;
        LightAttenuation = 0;
        Team = 0;
    }
    
    public override string ToString()
    {
        return $"Item {Id}: Name={Name}, Certificate={CertificateId}, Linked={LinkedId}";
    }
}
