namespace CacheManager.Items;

public class ItemDefEncoder
{
    public static (byte[] idxData, byte[] datData) Encode(ItemDefinition[] definitions)
    {
        using var idxStream = new MemoryStream();
        using var datStream = new MemoryStream();
        using var idxWriter = new BinaryWriter(idxStream);
        using var datWriter = new BinaryWriter(datStream);

        // ---- Write headers ----
        // .idx: first 2 bytes = count
        idxWriter.WriteInt16BigEndian((short)definitions.Length);

        // .dat: first 2 bytes = count
        datWriter.WriteInt16BigEndian((short)definitions.Length);

        for (int i = 0; i < definitions.Length; i++)
        {
            var def = definitions[i];

            long startPos = datStream.Position;
            EncodeDefinition(def, datWriter);
            long endPos = datStream.Position;

            int length = (int)(endPos - startPos);

            if (length > short.MaxValue)
                throw new InvalidOperationException($"Definition {i} too large: {length} bytes");

            // .idx stores each definition length as a 2-byte big-endian short
            idxWriter.WriteInt16BigEndian((short)length);
        }

        return (idxStream.ToArray(), datStream.ToArray());
    }

    
    private static void EncodeDefinition(ItemDefinition def, BinaryWriter writer)
    {
        if (def.ModelId > 0)
        {
            writer.Write((byte)1);
            writer.WriteUInt16BigEndian((ushort)def.ModelId);
        }

        if (!string.IsNullOrEmpty(def.Name))
        {
            writer.Write((byte)2);
            writer.WriteCacheString(def.Name);
        }

        if (!string.IsNullOrEmpty(def.Examine))
        {
            writer.Write((byte)3);
            writer.WriteCacheString(def.Examine);
        }

        if (def.IconZoom != 2000)
        {
            writer.Write((byte)4);
            writer.WriteUInt16BigEndian(def.IconZoom);
        }

        if (def.IconPitch != 0)
        {
            writer.Write((byte)5);
            writer.WriteUInt16BigEndian(def.IconPitch);
        }

        if (def.IconYaw != 0)
        {
            writer.Write((byte)6);
            writer.WriteUInt16BigEndian(def.IconYaw);
        }

        if (def.IconOffsetX != 0)
        {
            writer.Write((byte)7);
            writer.WriteInt16BigEndian(def.IconOffsetX);
        }

        if (def.IconOffsetY != 0)
        {
            writer.Write((byte)8);
            writer.WriteInt16BigEndian(def.IconOffsetY);
        }

        if (def.OpCode10 > 0)
        {
            writer.Write((byte)10);
            writer.WriteUInt16BigEndian((ushort)def.OpCode10);
        }

        if (def.Stackable)
        {
            writer.Write((byte)11);
        }

        if (def.Cost > 1)
        {
            writer.Write((byte)12);
            writer.WriteInt32BigEndian(def.Cost);
        }

        if (def.Members)
        {
            writer.Write((byte)16);
        }

        if (def.MaleModelId0 > 0)
        {
            writer.Write((byte)23);
            writer.WriteUInt16BigEndian((ushort)def.MaleModelId0);
            writer.Write((sbyte)def.MaleOffsetY);
        }

        if (def.MaleModelId1 > 0)
        {
            writer.Write((byte)24);
            writer.WriteUInt16BigEndian((ushort)def.MaleModelId1);
        }

        if (def.FemaleModelId0 > 0)
        {
            writer.Write((byte)25);
            writer.WriteUInt16BigEndian((ushort)def.FemaleModelId0);
            writer.Write((sbyte)def.FemaleOffsetY);
        }

        if (def.FemaleModelId1 > 0)
        {
            writer.Write((byte)26);
            writer.WriteUInt16BigEndian((ushort)def.FemaleModelId1);
        }

        if (def.Options != null)
        {
            for (int i = 0; i < def.Options.Length; i++)
            {
                if (!string.IsNullOrEmpty(def.Options[i]))
                {
                    writer.Write((byte)(30 + i));
                    writer.WriteCacheString(def.Options[i]);
                }
            }
        }

        if (def.InventoryOptions != null)
        {
            for (int i = 0; i < def.InventoryOptions.Length; i++)
            {
                if (!string.IsNullOrEmpty(def.InventoryOptions[i]))
                {
                    writer.Write((byte)(35 + i));
                    writer.WriteCacheString(def.InventoryOptions[i]);
                }
            }
        }

        if (def.SrcColor != null && def.DstColor != null && def.SrcColor.Length > 0)
        {
            writer.Write((byte)40);
            writer.Write((byte)def.SrcColor.Length);
            for (int i = 0; i < def.SrcColor.Length; i++)
            {
                writer.WriteUInt16BigEndian((ushort)def.SrcColor[i]);
                writer.WriteUInt16BigEndian((ushort)def.DstColor[i]);
            }
        }

        if (def.MaleModelId2 > 0)
        {
            writer.Write((byte)78);
            writer.WriteUInt16BigEndian((ushort)def.MaleModelId2);
        }

        if (def.FemaleModelId2 > 0)
        {
            writer.Write((byte)79);
            writer.WriteUInt16BigEndian((ushort)def.FemaleModelId2);
        }

        if (def.MaleHeadModelId0 > 0)
        {
            writer.Write((byte)90);
            writer.WriteUInt16BigEndian((ushort)def.MaleHeadModelId0);
        }

        if (def.FemaleHeadModelId0 > 0)
        {
            writer.Write((byte)91);
            writer.WriteUInt16BigEndian((ushort)def.FemaleHeadModelId0);
        }

        if (def.MaleHeadModelId1 > 0)
        {
            writer.Write((byte)92);
            writer.WriteUInt16BigEndian((ushort)def.MaleHeadModelId1);
        }

        if (def.FemaleHeadModelId1 > 0)
        {
            writer.Write((byte)93);
            writer.WriteUInt16BigEndian((ushort)def.FemaleHeadModelId1);
        }

        if (def.IconRoll != 0)
        {
            writer.Write((byte)95);
            writer.WriteUInt16BigEndian((ushort)def.IconRoll);
        }

        if (def.LinkedId > 0)
        {
            writer.Write((byte)97);
            writer.WriteUInt16BigEndian((ushort)def.LinkedId);
        }

        if (def.CertificateId > 0)
        {
            writer.Write((byte)98);
            writer.WriteUInt16BigEndian((ushort)def.CertificateId);
        }

        if (def.StackId != null)
        {
            for (int i = 0; i < def.StackId.Length; i++)
            {
                if (def.StackId[i] > 0)
                {
                    writer.Write((byte)(100 + i));
                    writer.WriteUInt16BigEndian((ushort)def.StackId[i]);
                    writer.WriteUInt16BigEndian((ushort)def.StackCount[i]);
                }
            }
        }

        if (def.ScaleX != 128)
        {
            writer.Write((byte)110);
            writer.WriteUInt16BigEndian((ushort)def.ScaleX);
        }

        if (def.ScaleZ != 128)
        {
            writer.Write((byte)111);
            writer.WriteUInt16BigEndian((ushort)def.ScaleZ);
        }

        if (def.ScaleY != 128)
        {
            writer.Write((byte)112);
            writer.WriteUInt16BigEndian((ushort)def.ScaleY);
        }

        if (def.LightAmbient != 0)
        {
            writer.Write((byte)113);
            writer.Write((byte)def.LightAmbient);
        }

        if (def.LightAttenuation != 0)
        {
            writer.Write((byte)114);
            writer.Write((byte)(def.LightAttenuation / 5));
        }

        if (def.Team != 0)
        {
            writer.Write((byte)115);
            writer.Write((byte)def.Team);
        }

        // End of definition
        writer.Write((byte)0);
    }
}