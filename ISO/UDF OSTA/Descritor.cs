using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System.Text;
using static UDFUtils;

/// <summary>
/// Classe para leitura de um sistema ISO9660+UDF, alvo principal: Estrutura Sony Playstation®2.
/// </summary>
/// Bit.Raiden/Dev C-Sharp, uso não comercial no momento.
/// Existe conhecimento, mas apenas o conhecimento de Cristo é poder.
/// Novembro/2021
public class Descritor
{
    public int lba, tamanhosetor, offsetsetor;
    public bool ChecksumPass = false, CRCPass = false;
    public Tag_Descritor tag;

    public byte[] SectorData;

    public virtual byte[] SectorToBin()
    {
        var outBin = new List<byte>();

        //Tag
        outBin.AddRange(tag.GetTag());



        return outBin.ToArray();
    }

    public static Descritor ReadSector(Stream input, int lba, int tamanho, PD Partição = null, bool FIentry = false)
    {
        Descritor sektor = null;
        byte[] sector = input.ReadSector(lba, tamanho);
        if (FIentry)
            sector = input.ReadBytes(lba, tamanho);
        uint tagid = sector.ReadUInt(0, 16);
        switch (tagid)
        {
            case 1:
                sektor = new PVD(sector);
                break;
            case 2:
                sektor = new AVDP(sector);
                break;
            case 4:
                sektor = new IUVD(sector);
                break;
            case 5:
                sektor = new PD(sector);
                break;
            case 6:
                sektor = new LV(sector);
                break;
            case 7:
                sektor = new USD(sector);
                break;
            case 8:
                sektor = new TD(sector);
                break;
            case 9:
                sektor = new LVI(sector);
                break;
            case 0x100:
                sektor = new FSD(sector, input, Partição);
                break;
            case 0x101:
                sektor = new FI(sector, input, Partição);
                break;
            case 0x105:
                sektor = new FE(sector);
                break;
        }
        if (sektor != null)
        {
            sektor.lba = lba;
            sektor.tamanhosetor = tamanho;
            sektor.offsetsetor = lba * tamanho;
        }
        return sektor;
    }

    public struct Tag_Descritor
    {
        public uint ID_de_Descritor;//uint16
        public uint Versão;//uint16
        public byte TagChecksum;//byte sum+module 256
        public byte Reservado;//byte
        public uint VolumeSerialNumber;//uint16
        public uint CRC_Descritor;//uint16
        public uint Tamanho_CRC_Descritor;//uint16
        public uint LBA_This_Descritor;//uint32

        public byte[] GetTag()
        {
            var outBIN = new List<byte>();
            outBIN.AddRange(BitConverter.GetBytes((UInt16)ID_de_Descritor));
            outBIN.AddRange(BitConverter.GetBytes((UInt16)Versão));
            outBIN.Add(TagChecksum);
            outBIN.Add(Reservado);
            outBIN.AddRange(BitConverter.GetBytes((UInt16)VolumeSerialNumber));
            outBIN.AddRange(BitConverter.GetBytes((UInt16)CRC_Descritor));
            outBIN.AddRange(BitConverter.GetBytes((UInt16)Tamanho_CRC_Descritor));
            outBIN.AddRange(BitConverter.GetBytes((UInt32)LBA_This_Descritor));
            return outBIN.ToArray();
        }
    }
    public struct time_stamp
    {
        public uint UTCspecs;
        public DateTime datetime;
        public byte[] GetData()
        {
            var outbin = new List<byte>();
            outbin.AddRange(BitConverter.GetBytes((UInt16)UTCspecs));
            outbin.AddRange(BitConverter.GetBytes((UInt16)datetime.Year));
            outbin.Add((byte)(datetime.Month));
            outbin.Add((byte)(datetime.Day));
            outbin.Add((byte)(datetime.Hour));
            outbin.Add((byte)(datetime.Minute));
            outbin.Add((byte)(datetime.Second));
            outbin.Add(0);
            outbin.Add((byte)(datetime.Millisecond));
            outbin.Add(0);
            return outbin.ToArray();
        }
        public void GetTimeStamp(byte[] file)
        {
            UTCspecs = file.ReadUInt(0, 16);
            try
            {
                int ano = (int)file.ReadUInt(2, 16);
                int mês = file[4];
                int dia = file[5];
                int hora = file[6];
                int min = file[7];
                int seg = file[8];
                int centseg = file[9];
                int miseg = file[0xA];
                int microseg = file[0xB];
                datetime = new DateTime(ano, mês, dia, hora, min, seg, miseg);
            }
            catch (Exception) { }
        }
    }
    public struct LVInformation
    {
        public CharSet LVICharset;
        public string LVIIdentifier;
        public string LVInfo1, LVInfo2, LVInfo3;
        public regid ImplementationID;
        public byte[] ImplementationUse;

        public byte[] GetData()
        {
            var outbin = new List<byte>();
            outbin.AddRange(LVICharset.GetData());

            byte[] lvi = new byte[0x7e];
            Array.Copy(Encoding.Default.GetBytes(LVIIdentifier), lvi, LVIIdentifier.Length);
            outbin.Add(8);
            outbin.AddRange(lvi);
            outbin.Add((byte)(LVIIdentifier.Length+1));

            var implEmpty = new List<byte>();
            implEmpty.Add(8);
            implEmpty.AddRange(new byte[0x22]);
            implEmpty.Add(1);

            outbin.AddRange(implEmpty.ToArray());
            outbin.AddRange(implEmpty.ToArray());
            outbin.AddRange(implEmpty.ToArray());

            outbin.AddRange(ImplementationID.GetData());
            outbin.AddRange(ImplementationUse);

            return outbin.ToArray();
        }
        public void ReadFromData(byte[] data)
        {
            LVICharset.ReadfromData(data.ReadBytes(0, 0x40));
            LVIIdentifier = data.ReadBytes(0x40, 0x80).ConvertTo(Encoding.Default);
            LVInfo1 = data.ReadBytes(0xC0, 0x24).ConvertTo(Encoding.Default);
            LVInfo2 = data.ReadBytes(0xE4, 0x24).ConvertTo(Encoding.Default);
            LVInfo3 = data.ReadBytes(0x108, 0x24).ConvertTo(Encoding.Default);

            ImplementationID.ReadFromData(data.ReadBytes(0x12c, 0x20));
            ImplementationUse = data.ReadBytes(0x14c, 0x80);
        }
    }
    public struct VolSetID
    {
        public byte[] First16Bytes;
        public string VolumeID;
        public byte LastCode;

        public byte[] GetData()
        {
            var outbin = new List<byte>();
            outbin.AddRange(First16Bytes);

            byte[] volid = new byte[0x6f];
            Array.Copy(Encoding.Default.GetBytes(VolumeID), volid, VolumeID.Length);
            outbin.AddRange(volid);

            outbin.Add(LastCode);
            return outbin.ToArray();
        }
        public void ReadfromData(byte[] data)
        {
            First16Bytes = data.ReadBytes(0, 16);
            string uncompressed = ReadCompressedUnicode(First16Bytes);
            VolumeID = data.ReadBytes(16, 0x6F).ConvertTo(Encoding.Default);
            LastCode = data[0x7f];
        }
    }
    public struct regid
    {
        public Flag[] Regras;
        public string ID;//0x17
        public byte[] IDSufixo;//8
        public byte[] GetData()
        {
            var outbin = new List<byte>();
            #region Regras
            var bitsFLAG = new BitArray(8);
            if (Regras.Contains(Flag.Sujo))
                bitsFLAG[0] = true;
            if(Regras.Contains(Flag.Protegido))
                bitsFLAG[1] = true;
            outbin.Add(bitsFLAG.ToByte());
            #endregion
            byte[] regstr = new byte[0x17];
            Array.Copy(Encoding.Default.GetBytes(ID), regstr, Encoding.Default.GetBytes(ID).Length);
            outbin.AddRange(regstr);
            outbin.AddRange(IDSufixo);
            return outbin.ToArray();
        }
        public void ReadFromData(byte[] data)
        {
            #region LER FLAGS
            byte flg = data[0];
            var regr = new List<Flag>();
            if (flg.ReadBit(0))
                regr.Add(Flag.Sujo);
            if (flg.ReadBit(1))
                regr.Add(Flag.Protegido);
            Regras = regr.ToArray();
            #endregion
            ID = data.ReadBytes(1, 0x17).ConvertTo(Encoding.Default);
            IDSufixo = data.ReadBytes(0x18, 8);
        }
        public enum Flag
        {
            Sujo,
            Protegido,
            Reservado
        };
    }
    public struct extent_ad
    {
        public uint ExtentSize;
        public uint LBAExtent;

        public byte[] GetData()
        {
            var outbin = new List<byte>();
            outbin.AddRange(BitConverter.GetBytes(ExtentSize));
            outbin.AddRange(BitConverter.GetBytes(LBAExtent));
            return outbin.ToArray();
        }
        public void ReadfromData(byte[] data)
        {
            ExtentSize = data.ReadUInt(0, 32);
            LBAExtent = data.ReadUInt(4, 32);
        }
    }
    public struct long_ad
    {
        public uint TamanhoExtent;
        public lb_addr LocalizaçãoExtent;
        public byte[] UsoImplementação;
        public byte[] GetData()
        {
            var outbin = new List<byte>();
            outbin.AddRange(BitConverter.GetBytes(TamanhoExtent));
            outbin.AddRange(LocalizaçãoExtent.GetData());
            outbin.AddRange(UsoImplementação);
            return outbin.ToArray();
        }
        public void ReadfromData(byte[] data)
        {
            TamanhoExtent = data.ReadUInt(0, 32);
            LocalizaçãoExtent.ReadfromData(data.ReadBytes(4, 6));
            UsoImplementação = data.ReadBytes(0xA, 6);
        }
    }
    public struct icbtag
    {
        public uint PreviousDirectEntryNumber;
        public uint StrategyTypeCode;
        public Strategy StrategyType;
        public byte[] StrategyParameter;

        public uint MaxEntryNumber;
        public FileType FileTyp;//Bit Array

        public lb_addr ICBParentLocation;
        public Flag[] Regras;

        public enum Flag
        {
            ShortAllocatDescrip,
            ExtendedAllocatDescrip,
            OrdenedDirectory,
            NotOrdenedDirectory,
            NonRelocTable,
            File,
            Res1,
            SetUID,
            SetGID,
            StickyC_ISVTX,
            Contiguous,
            Transformed,
            MultiVersions,
            Stream
        };
        public enum FileType: byte
        {
            NotSpecified = 0,
            NotAlocatedEntry = 1,
            PartitionIntegrityEntry = 2,
            IndirectEntry = 3,
            Directory = 4,
            RandomAccessBytes = 5,
            SpecialBlockDEV = 6,
            SpecialCharDEV = 7,
            ExtendedAtributes = 8,
            FIFOFile = 9,
            C_ISSOCK = 10,
            TerminalEntry = 11,
            SymbolicLink = 12,
            StreamDirectory = 13
        };
        public enum Strategy
        {
            NotSpecified,
            FA2,
            FA3,
            FA4,
            FA5
        };

        public byte[] GetData()
        {
            var outbin = new List<byte>();
            outbin.AddRange(BitConverter.GetBytes(PreviousDirectEntryNumber));
            outbin.AddRange(BitConverter.GetBytes((UInt16)StrategyType));
            outbin.AddRange(StrategyParameter);
            outbin.AddRange(BitConverter.GetBytes((UInt16)MaxEntryNumber));
            outbin.Add(0);
            outbin.Add((byte)FileTyp);
            outbin.AddRange(ICBParentLocation.GetData());
            #region Regras
            var flags1 = new BitArray(8);
            var flags2 = new BitArray(8);
            foreach (var flag in Regras)
            {
                switch (flag)
                {
                    case Flag.ShortAllocatDescrip:
                        flags1[0] = true;
                        flags1[1] = true;
                        break;
                    case Flag.ExtendedAllocatDescrip:
                        flags1[2] = true;
                        break;
                    case Flag.OrdenedDirectory:
                        flags1[3] = true;
                        break;
                    case Flag.NonRelocTable:
                        flags1[4] = true;
                        break;
                    case Flag.File:
                        flags1[5] = true;
                        break;
                    case Flag.SetUID:
                        flags1[6] = true;
                        break;
                    case Flag.SetGID:
                        flags1[7] = true;
                        break;
                    case Flag.StickyC_ISVTX:
                        flags2[0] = true;
                        break;
                    case Flag.Contiguous:
                        flags2[1] = true;
                        break;
                    case Flag.Res1:
                        flags2[2] = true;
                        break;
                    case Flag.Transformed:
                        flags2[3] = true;
                        break;
                    case Flag.MultiVersions:
                        flags2[4] = true;
                        break;
                    case Flag.Stream:
                        flags2[5] = true;
                        break;
                }
            }
            outbin.Add(flags1.ToByte());
            outbin.Add(flags2.ToByte());
            #endregion
            return outbin.ToArray();
        }
        public void ReadfromData(byte[] data)
        {
            PreviousDirectEntryNumber = data.ReadUInt(0, 32);
            StrategyTypeCode = data.ReadUInt(4, 16);
            #region Strategy
            switch (StrategyTypeCode)
            {
                case 0:
                    StrategyType = Strategy.NotSpecified;
                    break;
                case 1:
                    StrategyType = Strategy.FA2;
                    break;
                case 2:
                    StrategyType = Strategy.FA3;
                    break;
                case 3:
                    StrategyType = Strategy.FA4;
                    break;
                case 4:
                    StrategyType = Strategy.FA5;
                    break;

            }
            #endregion
            StrategyParameter = data.ReadBytes(6, 2);

            MaxEntryNumber = data.ReadUInt(8, 16);
            FileTyp = (FileType)data[0xB];

            ICBParentLocation.ReadfromData(data.ReadBytes(0xC, 6));

            #region Regras
            var flags = new List<Flag>();
            int index = 0;
            foreach (bool bit in data[0x11].ReadBits())
            {
                switch (index)
                {
                    case 0:
                        if (bit)
                            flags.Add(Flag.ShortAllocatDescrip);
                        break;
                    case 1:
                        if (bit)
                            flags.Add(Flag.ShortAllocatDescrip);
                        break;
                    case 2:
                        if (bit)
                            flags.Add(Flag.ExtendedAllocatDescrip);
                        break;
                    case 3:
                        if (bit)
                            flags.Add(Flag.OrdenedDirectory);
                        else
                            flags.Add(Flag.NotOrdenedDirectory);
                        break;
                    case 4:
                        if (bit)
                            flags.Add(Flag.NonRelocTable);
                        break;

                    case 5:
                        if (bit)
                            flags.Add(Flag.File);
                        break;
                    case 6:
                        if (bit)
                            flags.Add(Flag.SetUID);
                        break;
                    case 7:
                        if (bit)
                            flags.Add(Flag.SetGID);
                        break;
                }
                index++;
            }
            foreach (bool bit in data[0x12].ReadBits())
            {
                switch (index)
                {
                    case 0:
                        if (bit)
                            flags.Add(Flag.StickyC_ISVTX);
                        break;
                    case 1:
                        if (bit)
                            flags.Add(Flag.Contiguous);
                        break;
                    case 2:
                        if (bit)
                            flags.Add(Flag.Res1);
                        break;
                    case 3:
                        if (bit)
                            flags.Add(Flag.Transformed);
                        break;
                    case 4:
                        if (bit)
                            flags.Add(Flag.MultiVersions);
                        break;
                    case 5:
                        if (bit)
                            flags.Add(Flag.Stream);
                        break;
                }
                index++;
            }
            Regras = flags.ToArray();
            #endregion
        }
    }
    public struct OSTAcompressedUnicode
    {
        public byte CompressionID;
        public string Dados;
        public byte[] GetData()
        {
            var outbin = new List<byte>();
            outbin.Add(CompressionID);
            outbin.Add(0);
            switch (CompressionID)
            {
                case 8://1 byte por caractere -> UTF8
                    outbin.AddRange(Encoding.Default.GetBytes(Dados));
                    break;
                case 16://2 bytes por caractere -> UTF16
                    outbin.AddRange(Encoding.Unicode.GetBytes(Dados));
                    break;
                case 254://1 byte por caractere -> UTF8 (8)
                    outbin.AddRange(Encoding.Default.GetBytes(Dados));
                    break;
                case 255://2 bytes por caractere -> UTF16 (16)
                    outbin.AddRange(Encoding.Unicode.GetBytes(Dados));
                    break;
            }

            return outbin.ToArray();
        }
        public void ReadfromData(byte[] data)
        {
            CompressionID = data[0];
            byte[] text = data.ReadBytes(2, data.Length-1);
            Array.Resize(ref text, text.Length + 1);
            switch (CompressionID)
            {
                case 8://1 byte por caractere -> UTF8
                    Dados = text.ConvertTo(Encoding.Default);
                    break;
                case 16://2 bytes por caractere -> UTF16
                    //Array.Reverse(data);
                    Dados = text.ConvertTo(Encoding.Unicode);
                    break;
                case 254://1 byte por caractere -> UTF8 (8)
                    Dados = text.ConvertTo(Encoding.Default);
                    break;
                case 255://2 bytes por caractere -> UTF16 (16)
                    //Array.Reverse(data);
                    Dados = text.ConvertTo(Encoding.Unicode);
                    break;
            }
        }
    }
    public struct lb_addr
    {
        public uint LogicalBlockNumber;
        public uint PartitionReferenceNumber;

        public byte[] GetData()
        {
            var outbin = new List<byte>();
            outbin.AddRange(BitConverter.GetBytes(LogicalBlockNumber));
            outbin.AddRange(BitConverter.GetBytes((UInt16)PartitionReferenceNumber));
            return outbin.ToArray();
        }
        public void ReadfromData(byte[] data)
        {
            LogicalBlockNumber = data.ReadUInt(0, 32);
            PartitionReferenceNumber = data.ReadUInt(4, 16);
        }
    }
    public struct CharSet
    {
        public string Info;
        public TipoSet Flag;
        public enum TipoSet
        {
            CS0,
            CS1,
            CS2,
            CS3,
            CS4,
            CS5,
            CS6,
            CS7,
            CS8,
            Reservado
        };
        public byte[] GetData()
        {
            var outbin = new List<byte>();
            outbin.Add(0);//NO COMPRESSION
            byte[] set = new byte[0x3f];
            Array.Copy(Encoding.Default.GetBytes(Info), set, Encoding.Default.GetBytes(Info).Length);
            outbin.AddRange(set);
            return outbin.ToArray();
        }
        public void ReadfromData(byte[] data)
        {
            #region Ler FLAG CompressedSentence
            byte flag = data[0];
            if (flag != 0)
                throw new Exception("A FLAG do CHARSET é diferente de 0, aviso de comperssão CS.");
            #endregion

            Info = data.ReadBytes(1, 0x3f).ConvertTo(Encoding.Default);
        }
    }
    public struct PartitionMap
    {
        public uint Type;
        public uint MapLength;

        //Tipo 1
        public uint VolumeSequencialNumber;
        public uint PartitionNumber;

        //Tipo 2
        public byte[] PartitionID;
        public byte[] GetData()
        {
            var outbin = new List<byte>();
            outbin.Add((byte)Type);
            outbin.Add((byte)MapLength);
            switch (Type)
            {
                case 1:
                    outbin.AddRange(BitConverter.GetBytes((UInt16)VolumeSequencialNumber));
                    outbin.AddRange(BitConverter.GetBytes((UInt16)PartitionNumber));
                    break;

                case 2:
                    outbin.AddRange(PartitionID);
                    break;
            }
            return outbin.ToArray();
        }
        public void ReadFromData(byte[] data)
        {
            Type = data[0];
            MapLength = data[1];

            switch(Type)
            {
                case 1:
                    VolumeSequencialNumber = data.ReadUInt(2, 16);
                    PartitionNumber = data.ReadUInt(4, 16);
                    break;

                case 2:
                    PartitionID = data.ReadBytes(2, 0x3e);
                    break;
            }
        }
    }
    public void ReadDTAG(byte[] entry)
    {
        Tag_Descritor result = new Tag_Descritor();
        result.ID_de_Descritor = entry.ReadUInt(0, 16);
        result.Versão = entry.ReadUInt(2, 16);
        result.TagChecksum = entry[4];
        if (result.TagChecksum == TagChecksum(entry))
            ChecksumPass = true;
        result.Reservado = entry[5];
        result.VolumeSerialNumber = entry.ReadUInt(6, 16);
        result.CRC_Descritor = entry.ReadUInt(8, 16);
        result.Tamanho_CRC_Descritor = entry.ReadUInt(0xA, 16);
        if (ComputeCrc(entry.ReadBytes(0x10, (int)result.Tamanho_CRC_Descritor), (int)result.Tamanho_CRC_Descritor) == result.CRC_Descritor)
            CRCPass = true;
        result.LBA_This_Descritor = entry.ReadUInt(0xc, 32);
        tag = result;
    }
}

