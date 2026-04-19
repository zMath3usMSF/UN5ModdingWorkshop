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
 
//Primary Volume Descriptor
public class PVD : Descritor
{
    public uint DescritorVolumeSequencialNumber;
    public uint DescritorPrimaryVolumeSequencialNumber;

    public string VolumeID;//string[d][32]

    public uint VolumeSequenceNumber;
    public uint MaxVolumeSequenceNumber;
    public uint InterchangeLevel;
    public uint MaxInterchangeLevel;
    public uint CharacterSetList;
    public uint MaxCharacterSetList;

    public VolSetID VolumeSetID;//string[d][128]

    public CharSet DescritorCharSet;
    public CharSet ExplanatoryCharSet;
    public extent_ad VolumeAbstrato;
    public extent_ad VolumeCopyrightNotice;
    public regid ID_Aplicativo;

    public time_stamp DataHoraGravação;//Timestamp[0xC]

    public regid ID_Implementation;
    public byte[] UsoImplementação;//0x40
    public uint LBASequenciaDescritorVolumePredecessor;
    public Flag RegraComum = Flag.CommonAllVolumes;
    public byte[] Reservado;//22
    public enum Flag
    {
        CommonAllVolumes,
        NotCommonAllVolumes
    };
    public override byte[] SectorToBin()
    {
        var outBin = new List<byte>();
        var outSector = new List<byte>();

        outBin.AddRange(BitConverter.GetBytes((UInt32)DescritorVolumeSequencialNumber));
        outBin.AddRange(BitConverter.GetBytes((UInt32)DescritorPrimaryVolumeSequencialNumber));

        byte[] VolID = new byte[0x20];
        Array.Copy(Encoding.Default.GetBytes(VolumeID), VolID, VolumeID.Length);
        outBin.AddRange(VolID);

        outBin.AddRange(BitConverter.GetBytes((UInt16)VolumeSequenceNumber));
        outBin.AddRange(BitConverter.GetBytes((UInt16)MaxVolumeSequenceNumber));
        outBin.AddRange(BitConverter.GetBytes((UInt16)InterchangeLevel));
        outBin.AddRange(BitConverter.GetBytes((UInt16)MaxInterchangeLevel));

        outBin.AddRange(BitConverter.GetBytes((UInt32)CharacterSetList));
        outBin.AddRange(BitConverter.GetBytes((UInt32)MaxCharacterSetList));

        outBin.AddRange(VolumeSetID.GetData());

        outBin.AddRange(DescritorCharSet.GetData());
        outBin.AddRange(ExplanatoryCharSet.GetData());

        outBin.AddRange(VolumeAbstrato.GetData());
        outBin.AddRange(VolumeCopyrightNotice.GetData());

        outBin.AddRange(ID_Aplicativo.GetData());

        outBin.AddRange(DataHoraGravação.GetData());

        outBin.AddRange(ID_Implementation.GetData());

        outBin.AddRange(UsoImplementação);
        outBin.AddRange(BitConverter.GetBytes((UInt32)LBASequenciaDescritorVolumePredecessor));

        while (outBin.Count % (tamanhosetor - 0x10) != 0 || outBin.Count() < (tamanhosetor - 0x10))
            outBin.Add(0);

        //Tag
        outSector.AddRange(new Descritor.Tag_Descritor()
        {
            ID_de_Descritor = 1,
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
            ID_de_Descritor = 1,
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

    public PVD() { }
    public PVD(byte[] Sector)
    {
        ReadDTAG(Sector);

        DescritorVolumeSequencialNumber = Sector.ReadUInt(0x10, 32);
        DescritorPrimaryVolumeSequencialNumber = Sector.ReadUInt(0x14, 32);

        VolumeID = Sector.ReadBytes(0x18, 0x20).ConvertTo(Encoding.Default);

        VolumeSequenceNumber = Sector.ReadUInt(0x38, 16);//uint16
        MaxVolumeSequenceNumber = Sector.ReadUInt(0x3A, 16);//uint16
        InterchangeLevel = Sector.ReadUInt(0x3C, 16);//uint16
        MaxInterchangeLevel = Sector.ReadUInt(0x3E, 16);//uint16

        CharacterSetList = Sector.ReadUInt(0x40, 32);//uint32
        MaxCharacterSetList = Sector.ReadUInt(0x44, 32);//uint32

        VolumeSetID.ReadfromData(Sector.ReadBytes(0x48, 0x80));

        DescritorCharSet.ReadfromData(Sector.ReadBytes(0xC8, 0x40));
        ExplanatoryCharSet.ReadfromData(Sector.ReadBytes(0x108, 0x40));

        VolumeAbstrato.ReadfromData(Sector.ReadBytes(0x148, 8));
        VolumeCopyrightNotice.ReadfromData(Sector.ReadBytes(0x150, 8));

        ID_Aplicativo.ReadFromData(Sector.ReadBytes(0x158, 0x20));

        DataHoraGravação.GetTimeStamp(Sector.ReadBytes(0x178, 0xC));

        ID_Implementation.ReadFromData(Sector.ReadBytes(0x184, 0x20));
        UsoImplementação = Sector.ReadBytes(0x1A4, 0x40);

        LBASequenciaDescritorVolumePredecessor = Sector.ReadUInt(0x1E4, 32);

        #region Ler FLAG Comum
        uint flag = Sector.ReadUInt(0x1e8, 16);//uint16
        if (BitConverter.GetBytes(flag)[0].ReadBit(0))
            RegraComum = Flag.NotCommonAllVolumes;
        #endregion

        Reservado = Sector.ReadBytes(0x1eA, 22);
    }
}
