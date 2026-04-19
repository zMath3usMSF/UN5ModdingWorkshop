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

//Logical Volume Descriptor
public class LV: Descritor
{
    public uint DescritorVolumeSequencialNumber;
    public CharSet DescCharSet;
    public string LVIdentifier;

    public uint VolBlockSize;

    public regid DomainIdentifier;

    public byte[] ContentUse;

    public uint PartitionMapTableSize;
    public uint MapNumber;
    public regid ImplementationIdentifier;

    public byte[] UsoImplementação;

    public extent_ad IntegritySequenceExtent;
    public LVI LogicVolumeIntegrity;//apenas se existir com o extent acima

    public PartitionMap[] PartitionMaps;

    public override byte[] SectorToBin()
    {
        var outBin = new List<byte>();
        var outSector = new List<byte>();

        outBin.AddRange(BitConverter.GetBytes((UInt32)DescritorVolumeSequencialNumber));
        outBin.AddRange(DescCharSet.GetData());

        var implbin = new List<byte>();
        implbin.Add(8);
        byte[] text = new byte[0x7e];
        Array.Copy(Encoding.Default.GetBytes(LVIdentifier), text, LVIdentifier.Length);
        implbin.AddRange(text);
        implbin.Add((byte)(LVIdentifier.Length+1));

        outBin.AddRange(implbin.ToArray());

        outBin.AddRange(BitConverter.GetBytes((UInt32)VolBlockSize));
        outBin.AddRange(DomainIdentifier.GetData());
        outBin.AddRange(ContentUse);

        outBin.AddRange(BitConverter.GetBytes((UInt32)PartitionMapTableSize));
        outBin.AddRange(BitConverter.GetBytes((UInt32)MapNumber));
        outBin.AddRange(ImplementationIdentifier.GetData());
        outBin.AddRange(UsoImplementação);

        outBin.AddRange(IntegritySequenceExtent.GetData());
        foreach(var map in PartitionMaps)
            outBin.AddRange(map.GetData());

        while (outBin.Count % (tamanhosetor - 0x10) != 0 || outBin.Count() < (tamanhosetor - 0x10))
            outBin.Add(0);
        //Tag
        outSector.AddRange(new Descritor.Tag_Descritor()
        {
            ID_de_Descritor = 6,
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
            ID_de_Descritor = 6,
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

    public LV() { }
    public LV(byte[] Sector)
    {
        ReadDTAG(Sector);

        DescritorVolumeSequencialNumber = Sector.ReadUInt(0x10, 32);

        DescCharSet.ReadfromData(Sector.ReadBytes(0x14, 0x40));
        LVIdentifier = Sector.ReadBytes(0x54, 0x80).ConvertTo(Encoding.Default);

        VolBlockSize = Sector.ReadUInt(0xD4, 32);
        DomainIdentifier.ReadFromData(Sector.ReadBytes(0xD8, 0x20));

        ContentUse = Sector.ReadBytes(0xF8, 0x10);

        PartitionMapTableSize = Sector.ReadUInt(0x108, 32);
        MapNumber = Sector.ReadUInt(0x10C, 32);
        ImplementationIdentifier.ReadFromData(Sector.ReadBytes(0x110, 0x20));

        UsoImplementação = Sector.ReadBytes(0x130, 0x80);

        IntegritySequenceExtent.ReadfromData(Sector.ReadBytes(0x1b0, 8));

        #region Mapas de Partição
        var maps = new List<PartitionMap>();
        for (uint i = 0x1b8;i< MapNumber* PartitionMapTableSize; i+= PartitionMapTableSize)
        {
            byte[] partition = Sector.ReadBytes((int)i, (int)PartitionMapTableSize);
            PartitionMap mapp = new PartitionMap();
            mapp.ReadFromData(partition);
            maps.Add(mapp);
        }
        PartitionMaps = maps.ToArray();
        #endregion
    }
}


