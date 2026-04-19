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

//Logical Volume Integrity Descriptor
public class LVI: Descritor
{
    public time_stamp DataHoraGravação;
    public uint Tipo;
    public extent_ad Próximo;//0 para null
    public byte[] UsoVolumeLógico;

    public uint NúmeroPartições;
    public uint TamanhoUsoImplementação;

    public uint[] TabelaEspaçoLivre;//4 bytes por entrada
    public uint[] TabelaTamanhos;//4 bytes por entrada

    public ImplementationUse UsoImplementação;

    public override byte[] SectorToBin()
    {
        var outBin = new List<byte>();
        var outSector = new List<byte>();

        outBin.AddRange(DataHoraGravação.GetData());
        outBin.AddRange(BitConverter.GetBytes((UInt32)Tipo));
        outBin.AddRange(Próximo.GetData());
        outBin.AddRange(new byte[4] { 0xff,0xff,0xff,0xff});
        outBin.AddRange(new byte[0x1c]);

        outBin.AddRange(BitConverter.GetBytes((UInt32)NúmeroPartições));
        outBin.AddRange(BitConverter.GetBytes((UInt32)TamanhoUsoImplementação));

        foreach(var nump in TabelaEspaçoLivre)
            outBin.AddRange(BitConverter.GetBytes((UInt32)nump));
        foreach (var nump2 in TabelaTamanhos)
            outBin.AddRange(BitConverter.GetBytes((UInt32)nump2));

        outBin.AddRange(UsoImplementação.GetData());
        while (outBin.Count % (tamanhosetor - 0x10) != 0 || outBin.Count() < (tamanhosetor - 0x10))
            outBin.Add(0);
        //Tag
        outSector.AddRange(new Descritor.Tag_Descritor()
        {
            ID_de_Descritor = 9,
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
            ID_de_Descritor = 9,
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

    public LVI() { }
    public LVI(byte[] Sector)
    {
        ReadDTAG(Sector);

        DataHoraGravação.GetTimeStamp(Sector.ReadBytes(0x10, 0xC));

        Tipo = Sector.ReadUInt(0x1c, 32);

        Próximo.ReadfromData(Sector.ReadBytes(0x20, 8));

        UsoVolumeLógico = Sector.ReadBytes(0x28, 0x20);

        NúmeroPartições = Sector.ReadUInt(0x48, 32);
        TamanhoUsoImplementação = Sector.ReadUInt(0x4C, 32);


        int offset = 0x50;
        #region Tabela Espaço Livre
        TabelaEspaçoLivre = new uint[NúmeroPartições];
        for(int i =0;i< NúmeroPartições;i++)
        {
            TabelaEspaçoLivre[i] = Sector.ReadUInt(offset, 32);
            offset += 4;
        }
        #endregion

        #region Tabela Tamanhos
        TabelaTamanhos = new uint[NúmeroPartições];
        for (int i = 0; i < NúmeroPartições; i++)
        {
            TabelaTamanhos[i] = Sector.ReadUInt(offset, 32);
            offset += 4;
        }
        #endregion

        UsoImplementação.ReadfromData(Sector.ReadBytes(offset, (int)TamanhoUsoImplementação));
    }

    public struct ImplementationUse
    {
        public regid ID;
        public uint FileNumber;
        public uint DirectoryNumber;
        public uint MinUDFReadRev;
        public uint MinUDFWriteRev;
        public uint MaxUDFWriteRev;
        public byte[] UsoImplementação;

        public byte[] GetData()
        {
            var outbin = new List<byte>();
            outbin.AddRange(ID.GetData());
            outbin.AddRange(BitConverter.GetBytes((UInt32)FileNumber));
            outbin.AddRange(BitConverter.GetBytes((UInt32)DirectoryNumber));
            outbin.AddRange(BitConverter.GetBytes((UInt16)MinUDFReadRev));
            outbin.AddRange(BitConverter.GetBytes((UInt16)MinUDFWriteRev));
            outbin.AddRange(BitConverter.GetBytes((UInt16)MaxUDFWriteRev));
            outbin.AddRange(UsoImplementação);

            return outbin.ToArray();
        }
        public void ReadfromData(byte[] data)
        {
            ID.ReadFromData(data.ReadBytes(0, 0x20));
            FileNumber = data.ReadUInt(0x20, 32);
            DirectoryNumber = data.ReadUInt(0x24, 32);
            MinUDFReadRev = data.ReadUInt(0x26, 16);
            MinUDFWriteRev = data.ReadUInt(0x28, 16);
            MaxUDFWriteRev = data.ReadUInt(0x2C, 16);
            UsoImplementação = data.ReadBytes(0x2E, data.Length - 0x2e);
        }
    }
}


