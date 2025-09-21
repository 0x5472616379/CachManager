namespace CacheManager.Items;

public class ItemDefinition
{
    public int Id { get; set; } = -1;
    public int ModelId { get; set; }
    public string Name { get; set; }
    public string Examine { get; set; }
    public int IconZoom { get; set; } = 2000;
    public int IconPitch { get; set; }
    public int IconYaw { get; set; }
    public int IconRoll { get; set; }
    public int IconOffsetX { get; set; }
    public int IconOffsetY { get; set; }
    public bool Stackable { get; set; }
    public int Cost { get; set; } = 1;
    public bool Members { get; set; }

    // Model properties
    public int MaleModelId0 { get; set; } = -1;
    public int MaleModelId1 { get; set; } = -1;
    public int MaleModelId2 { get; set; } = -1;
    public int MaleHeadModelId0 { get; set; } = -1;
    public int MaleHeadModelId1 { get; set; } = -1;
    public byte MaleOffsetY { get; set; }

    public int FemaleModelId0 { get; set; } = -1;
    public int FemaleModelId1 { get; set; } = -1;
    public int FemaleModelId2 { get; set; } = -1;
    public int FemaleHeadModelId0 { get; set; } = -1;
    public int FemaleHeadModelId1 { get; set; } = -1;
    public byte FemaleOffsetY { get; set; }

    public string[] Options { get; set; }
    public string[] InventoryOptions { get; set; }

    public int[] SrcColor { get; set; }
    public int[] DstColor { get; set; }

    public int[] StackId { get; set; }
    public int[] StackCount { get; set; }

    public int ScaleX { get; set; } = 128;
    public int ScaleY { get; set; } = 128;
    public int ScaleZ { get; set; } = 128;

    public int LightAmbient { get; set; }
    public int LightAttenuation { get; set; }

    public int Team { get; set; }

    public int LinkedId { get; set; } = -1;
    public int CertificateId { get; set; } = -1;
    
    public void Reset()
    {
        ModelId = 0;
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
        MaleModelId0 = -1;
        MaleModelId1 = -1;
        MaleOffsetY = 0;
        FemaleModelId0 = -1;
        FemaleModelId1 = -1;
        FemaleOffsetY = 0;
        MaleModelId2 = -1;
        FemaleModelId2 = -1;
        MaleHeadModelId0 = -1;
        MaleHeadModelId1 = -1;
        FemaleHeadModelId0 = -1;
        FemaleHeadModelId1 = -1;
        StackId = null;
        StackCount = null;
        LinkedId = -1;
        CertificateId = -1;
        ScaleX = 128;
        ScaleY = 128;
        ScaleZ = 128;
        LightAmbient = 0;
        LightAttenuation = 0;
        Team = 0;
    }
}