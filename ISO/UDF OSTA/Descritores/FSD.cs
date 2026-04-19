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

//File Set Descriptor
public class FSD : Descritor
{
    public time_stamp DataHoraGravação;
    public uint InterchangeLevel;
    public uint MaxInterchangeLevel;

    public uint CharacterSetList;
    public uint MaxCharacterSetList;

    public uint FileSetNumber;
    public uint FileSetDescriptorNumber;

    public CharSet LogicalVolumeIdentifierCharSet;
    public string LogicalVolumeIdentifier;

    public CharSet FileSetCharSet;
    public string FileSetIdentifier;

    public string CopyrightFileIdentifier;
    public string AbstractFileIdentifier;

    public long_ad RootDirectoryICB;
    public regid DomainIdentifier;
    public long_ad NextExtent;
    public long_ad StreamDirectoryICB;

    public List<FI[]> FileIdentifiers;
    public override byte[] SectorToBin()
    {
        var outBin = new List<byte>();
        var outSector = new List<byte>();

        outBin.AddRange(DataHoraGravação.GetData());
        outBin.AddRange(BitConverter.GetBytes((UInt16)InterchangeLevel));
        outBin.AddRange(BitConverter.GetBytes((UInt16)MaxInterchangeLevel));

        outBin.AddRange(BitConverter.GetBytes((UInt32)CharacterSetList));
        outBin.AddRange(BitConverter.GetBytes((UInt32)MaxCharacterSetList));

        outBin.AddRange(BitConverter.GetBytes((UInt32)FileSetNumber));
        outBin.AddRange(BitConverter.GetBytes((UInt32)FileSetDescriptorNumber));

        outBin.AddRange(LogicalVolumeIdentifierCharSet.GetData());
        byte[] LogicVolumeID = new byte[0x80];
        Array.Copy(Encoding.Default.GetBytes(LogicalVolumeIdentifier), LogicVolumeID, LogicalVolumeIdentifier.Length);
        outBin.AddRange(LogicVolumeID);

        outBin.AddRange(FileSetCharSet.GetData());
        byte[] FileSetID = new byte[0x20];
        byte[] CopyrightSetID = new byte[0x20];
        byte[] AbstractSetID = new byte[0x20];
        Array.Copy(Encoding.Default.GetBytes(FileSetIdentifier), FileSetID, FileSetIdentifier.Length);
        Array.Copy(Encoding.Default.GetBytes(CopyrightFileIdentifier), CopyrightSetID, CopyrightFileIdentifier.Length);
        Array.Copy(Encoding.Default.GetBytes(AbstractFileIdentifier), AbstractSetID, AbstractFileIdentifier.Length);
        outBin.AddRange(FileSetID);
        outBin.AddRange(CopyrightSetID);
        outBin.AddRange(AbstractSetID);

        outBin.AddRange(RootDirectoryICB.GetData());
        outBin.AddRange(DomainIdentifier.GetData());
        outBin.AddRange(NextExtent.GetData());
        outBin.AddRange(StreamDirectoryICB.GetData());

        outBin.AddRange(new byte[0x20]);//Reserved
        while (outBin.Count % (tamanhosetor - 0x10) != 0 || outBin.Count() < (tamanhosetor - 0x10))
            outBin.Add(0);
        //Tag
        outSector.AddRange(new Descritor.Tag_Descritor()
        {
            ID_de_Descritor = 0x100,
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
            ID_de_Descritor = 0x100,
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

    public FSD() { }
    public FSD(byte[] Sector, Stream ISO, PD Partição)
    {
        ReadDTAG(Sector);

        DataHoraGravação.GetTimeStamp(Sector.ReadBytes(0x10, 0xC));
        InterchangeLevel = Sector.ReadUInt(0x1c, 16);
        MaxInterchangeLevel = Sector.ReadUInt(0x1e, 16);

        CharacterSetList = Sector.ReadUInt(0x20, 32);
        MaxCharacterSetList = Sector.ReadUInt(0x24, 32);

        FileSetNumber = Sector.ReadUInt(0x28, 32);
        FileSetDescriptorNumber = Sector.ReadUInt(0x2c, 32);

        LogicalVolumeIdentifierCharSet.ReadfromData(Sector.ReadBytes(0x30, 0x40));
        LogicalVolumeIdentifier = Sector.ReadBytes(0x70, 0x80).ConvertTo(Encoding.Default);

        FileSetCharSet.ReadfromData(Sector.ReadBytes(0xF0, 0x40));
        FileSetIdentifier = Sector.ReadBytes(0x130, 0x20).ConvertTo(Encoding.Default);

        CopyrightFileIdentifier = Sector.ReadBytes(0x150, 0x20).ConvertTo(Encoding.Default);
        AbstractFileIdentifier = Sector.ReadBytes(0x170, 0x20).ConvertTo(Encoding.Default);

        RootDirectoryICB.ReadfromData(Sector.ReadBytes(0x190, 0x10));
        DomainIdentifier.ReadFromData(Sector.ReadBytes(0x1a0, 0x20));
        NextExtent.ReadfromData(Sector.ReadBytes(0x1c0, 0x10));
        StreamDirectoryICB.ReadfromData(Sector.ReadBytes(0x1d0, 0x10));
        //byte[0x20] null

    }
    public void ReadFIS(Stream ISO, PD Partição)
    {
        #region Leitura de FIs
        FileIdentifiers = new List<FI[]>();
        int lbaFIS = lba + 2;
        while (ISO.ReadSector(lbaFIS, tamanhosetor).ReadUInt(0, 16) == 0x101)
        {
            byte[] SectorFI = ISO.ReadSector(lbaFIS, tamanhosetor);
            FileIdentifiers.Add(FI.SplitFromSector(SectorFI, ISO, Partição, lbaFIS));
            lbaFIS++;
        }
        #endregion
    }
}
