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

//Extended Atribute Descriptor
public class EA: Descritor
{
    public uint OffsetImplementationUse;
    public uint OffsetApplicationUse;

    public ImplementationUse[] UsoImplementação;

    public override byte[] SectorToBin()
    {
        var outSector = new List<byte>(); 
        var outBin = new List<byte>();
        outBin.AddRange(BitConverter.GetBytes(OffsetImplementationUse));
        outBin.AddRange(BitConverter.GetBytes(OffsetApplicationUse));

        //Tag
        outSector.AddRange(new Descritor.Tag_Descritor()
        {
            ID_de_Descritor = 0x106,
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
            ID_de_Descritor = 0x106,
            LBA_This_Descritor = (uint)lba,
            Tamanho_CRC_Descritor = (uint)outBin.Count,
            Versão = 2,
            Reservado = 0,
            VolumeSerialNumber = 0,
            CRC_Descritor = UDFUtils.ComputeCrc(outBin.ToArray(), outBin.Count),
            TagChecksum = tagchecksum
        }.GetTag());
        outBin.AddRange(UsoImplementação[0].GetData());
        outBin.AddRange(UsoImplementação[1].GetData());
        outSector.AddRange(outBin);

        return outSector.ToArray();
    }
    public EA() { }
    public EA(byte[] Sector)
    {
        ReadDTAG(Sector);

        OffsetImplementationUse = Sector.ReadUInt(0x10, 32);
        OffsetApplicationUse = Sector.ReadUInt(0x14, 32);

        UsoImplementação = new ImplementationUse[2];
        UsoImplementação[0].ReadfromData(Sector.ReadBytes((int)OffsetImplementationUse, (int)(OffsetApplicationUse - 0x18)));
        UsoImplementação[1].ReadfromData(Sector.ReadBytes((int)(OffsetImplementationUse + UsoImplementação[0].TamanhoAtributo), (int)Sector.ReadUInt((int)(OffsetImplementationUse + UsoImplementação[0].TamanhoAtributo + 8),16)));
    }

    public struct ImplementationUse
    {
        public uint TipodeAtributo;
        public byte SubTipodeAtributo;
        public byte[] Reservado;

        public uint TamanhoAtributo;
        public uint TamanhoImplementationUse;

        public regid ID;
        public FreeEASpace UsoImplementaçãoFreeSpace;
        public DVDGGMSInfo UsoImplementaçãoGGMS;
        public byte[] GetData()
        {
            var outBin = new List<byte>();
            outBin.AddRange(BitConverter.GetBytes(TipodeAtributo));
            outBin.Add(SubTipodeAtributo);
            outBin.AddRange(Reservado);

            outBin.AddRange(BitConverter.GetBytes(TamanhoAtributo));
            outBin.AddRange(BitConverter.GetBytes(TamanhoImplementationUse));

            outBin.AddRange(ID.GetData());

            if (UsoImplementaçãoFreeSpace!=null)
            {
                outBin.AddRange(UsoImplementaçãoFreeSpace.GetData());
            }
            else if (UsoImplementaçãoGGMS!=null)
            {
                outBin.AddRange(UsoImplementaçãoGGMS.GetData());
            }
            return outBin.ToArray();
        }
        public void ReadfromData(byte[] data)
        {
            TipodeAtributo = data.ReadUInt(0, 32);
            SubTipodeAtributo = data[4];
            Reservado = data.ReadBytes(5, 3);

            TamanhoAtributo = data.ReadUInt(8, 32);
            TamanhoImplementationUse = data.ReadUInt(0xC, 32);

            ID.ReadFromData(data.ReadBytes(0x10, 0x20));

            if(ID.ID.Contains("*UDF FreeEASpace"))
            {
                UsoImplementaçãoFreeSpace = new FreeEASpace();
                UsoImplementaçãoFreeSpace.ReadfromData(data.ReadBytes(0x30, (int)TamanhoImplementationUse));
            }
            else if(ID.ID.Contains("*UDF DVD CGMS Info"))
            {
                UsoImplementaçãoGGMS = new DVDGGMSInfo();
                UsoImplementaçãoGGMS.ReadfromData(data.ReadBytes(0x30, (int)TamanhoImplementationUse));
            }
        }

    }
    public class FreeEASpace
    {
        public uint HeaderChecksum;
        public byte[] FreeEaSpace;

        public byte[] GetData()
        {
            var outBin = new List<byte>();
            outBin.AddRange(BitConverter.GetBytes((UInt16)HeaderChecksum));
            outBin.AddRange(FreeEaSpace);
            return outBin.ToArray();
        }
        public void ReadfromData(byte[] data)
        {
            HeaderChecksum = data.ReadUInt(0, 16);
            FreeEaSpace = data.ReadBytes(2, data.Length - 2);
        }
    }
    public class DVDGGMSInfo
    {
        public uint HeaderChecksum;
        public byte GGMSInformation;
        public byte TipoDeDadosEstrutura;
        public byte[] InformaçãoProtetivadeSistema;
        public byte[] GetData()
        {
            var outBin = new List<byte>();
            outBin.AddRange(BitConverter.GetBytes((UInt16)HeaderChecksum));
            outBin.Add(GGMSInformation);
            outBin.Add(TipoDeDadosEstrutura);
            outBin.AddRange(InformaçãoProtetivadeSistema);
            return outBin.ToArray();
        }
        public void ReadfromData(byte[] data)
        {
            HeaderChecksum = data.ReadUInt(0, 16);
            GGMSInformation = data[2];
            TipoDeDadosEstrutura = data[3];
            InformaçãoProtetivadeSistema = data.ReadBytes(4, 4);
        }
    }
    public struct ApplicationUse
    {
        public uint TipodeAtributo;
        public byte SubTipodeAtributo;
        public byte[] Reservado;

        public uint TamanhoAtributo;
        public uint TamanhoAplicaçãoUse;

        public regid ID;
        public byte[] UsoAplicação;
        public byte[] GetData()
        {
            var outBin = new List<byte>();

            return outBin.ToArray();
        }
        public void ReadfromData(byte[] data)
        {
            TipodeAtributo = data.ReadUInt(0, 32);
            SubTipodeAtributo = data[4];
            Reservado = data.ReadBytes(5, 3);

            TamanhoAtributo = data.ReadUInt(8, 32);
            TamanhoAplicaçãoUse = data.ReadUInt(0xC, 32);

            ID.ReadFromData(data.ReadBytes(0x10, 0x20));
            UsoAplicação = data.ReadBytes(0x30, (int)TamanhoAplicaçãoUse);
        }
    }
}
