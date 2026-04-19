using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

/// <summary>
/// Classe para leitura de um sistema ISO9660+UDF, alvo principal: Estrutura Sony Playstation®2.
/// </summary>
/// Bit.Raiden/Dev C-Sharp, uso não comercial no momento.
/// Existe conhecimento, mas apenas o conhecimento de Cristo é poder.
/// Novembro/2021

//File Entry
public class FE: Descritor
{
    public icbtag TagICB;
    public uint UID, GID, Permissions;
    public uint LinkedFileCount;

    public byte RecordFormat;
    public byte RecordAttrs;
    public uint RecordSize;

    public ulong InfoSize;
    public ulong LogicalBlocksWrited;

    public time_stamp Acesso;
    public time_stamp Modificação;
    public time_stamp Atributo;

    public uint Checkpoint;

    public long_ad ICBAttrExtendido;
    public regid IDImplementação;
    public ulong UniqueID;

    public uint TamanhoAttrExtendidos;
    public uint TamanhoDescritoresAloc;

    public EA AtributosExtendidos;
    public long_ad DescritoresAlocação;

    //Special
    public string Name, FullPath;
    public override byte[] SectorToBin()
    {
        var outBin = new List<byte>();
        var outSector = new List<byte>();

        outBin.AddRange(TagICB.GetData());

        outBin.AddRange(BitConverter.GetBytes(UID));
        outBin.AddRange(BitConverter.GetBytes(GID));
        outBin.AddRange(BitConverter.GetBytes(Permissions));

        outBin.AddRange(BitConverter.GetBytes((UInt16)LinkedFileCount));

        outBin.Add(RecordFormat);
        outBin.Add(RecordAttrs);
        outBin.AddRange(BitConverter.GetBytes(RecordSize));

        outBin.AddRange(BitConverter.GetBytes(InfoSize));
        outBin.AddRange(BitConverter.GetBytes(LogicalBlocksWrited));

        outBin.AddRange(Acesso.GetData());
        outBin.AddRange(Modificação.GetData());
        outBin.AddRange(Atributo.GetData());

        outBin.AddRange(BitConverter.GetBytes(Checkpoint));

        outBin.AddRange(ICBAttrExtendido.GetData());
        outBin.AddRange(IDImplementação.GetData());

        outBin.AddRange(BitConverter.GetBytes(UniqueID));

        outBin.AddRange(BitConverter.GetBytes(TamanhoAttrExtendidos));
        outBin.AddRange(BitConverter.GetBytes(TamanhoDescritoresAloc));

        outBin.AddRange(AtributosExtendidos.SectorToBin());
        outBin.AddRange(DescritoresAlocação.GetData());

        outBin.RemoveRange(outBin.Count - 8, 8);

        //Tag
        outSector.AddRange(new Descritor.Tag_Descritor()
        {
            ID_de_Descritor = 0x105,
            LBA_This_Descritor = (uint)lba,
            Tamanho_CRC_Descritor = (uint)outBin.Count,
            Versão = 2,
            Reservado = 0,
            VolumeSerialNumber = 0,
            CRC_Descritor = UDFUtils.ComputeCrc(outBin.ToArray(), outBin.Count)
        }.GetTag());
        byte tagchecksum = UDFUtils.TagChecksum(outSector.ToArray());

        outSector.Clear();
        outSector.AddRange(new Descritor.Tag_Descritor()
        {
            ID_de_Descritor = 0x105,
            LBA_This_Descritor = (uint)lba,
            Tamanho_CRC_Descritor = (uint)outBin.Count,
            Versão = 2,
            Reservado = 0,
            VolumeSerialNumber = 0,
            CRC_Descritor = UDFUtils.ComputeCrc(outBin.ToArray(), outBin.Count),
            TagChecksum = tagchecksum
        }.GetTag());

        outSector.AddRange(outBin);

        while (outSector.Count % tamanhosetor != 0 || outSector.Count() < tamanhosetor)
            outSector.Add(0);
        return outSector.ToArray();
    }
    public FE() { }
    public FE(byte[] Sector)
    {
        ReadDTAG(Sector);

        TagICB.ReadfromData(Sector.ReadBytes(0x10, 0x13));

        UID = Sector.ReadUInt(0x24, 32);
        GID = Sector.ReadUInt(0x28, 32);
        Permissions = Sector.ReadUInt(0x2C, 32);

        LinkedFileCount = Sector.ReadUInt(0x30, 16);

        RecordFormat = Sector[0x32];
        RecordAttrs = Sector[0x33];
        RecordSize = Sector.ReadUInt(0x34, 32);

        InfoSize = Sector.ReadULong(0x38);
        LogicalBlocksWrited = Sector.ReadULong(0x40);

        Acesso.GetTimeStamp(Sector.ReadBytes(0x48, 0xC));
        Modificação.GetTimeStamp(Sector.ReadBytes(0x54, 0xC));
        Atributo.GetTimeStamp(Sector.ReadBytes(0x60, 0xC));

        Checkpoint = Sector.ReadUInt(0x6C, 32);

        ICBAttrExtendido.ReadfromData(Sector.ReadBytes(0x70, 0x10));
        IDImplementação.ReadFromData(Sector.ReadBytes(0x80, 0x20));
        UniqueID = Sector.ReadULong(0xA0);

        TamanhoAttrExtendidos = Sector.ReadUInt(0xA8, 32);
        TamanhoDescritoresAloc = Sector.ReadUInt(0xAC, 32);

        AtributosExtendidos = new EA(Sector.ReadBytes(0xB0, (int)TamanhoAttrExtendidos));
        DescritoresAlocação.ReadfromData(Sector.ReadBytes(0xB0 + (int)TamanhoAttrExtendidos, 0x10));
    }
}
