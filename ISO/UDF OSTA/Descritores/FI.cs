using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System.Text;

/// <summary>
/// Classe para leitura de um sistema ISO9660+UDF, alvo principal: Estrutura Sony Playstation®2.
/// </summary>
/// Bit.Raiden/Dev C-Sharp, uso não comercial no momento.
/// Existe conhecimento, mas apenas o conhecimento de Cristo é poder.
/// Novembro/2021

//File Identifier

//This descriptor can be in groups of non-complete sectors, logical blocks.
public class FI : Descritor
{
    public uint FileVersionNumber;
    public FileCaracteristic[] FileCaracteristics;//bit[]

    public byte FileIDSize;

    public FE FileEntry;

    public long_ad ICB;
    public uint TamanhoUsoImplementação;

    public byte[] UsoImplementação;
    public OSTAcompressedUnicode FileIdentifier;

    //Specials
    public string FullPath;
    public override byte[] SectorToBin()
    {
        var outSector = new List<byte>();
        var outBin = new List<byte>();
        outBin.AddRange(BitConverter.GetBytes((UInt16)FileVersionNumber));

        //File Caracteristics
        bool[] entriesFlags = new bool[8];
        foreach (var regra in FileCaracteristics)
        {
            switch (regra)
            {
                case FileCaracteristic.Hidden:
                    entriesFlags[0] = true;
                    break;
                case FileCaracteristic.Directory:
                    entriesFlags[1] = true;
                    break;
                case FileCaracteristic.Deleted:
                    entriesFlags[2] = true;
                    break;
                case FileCaracteristic.Parent:
                    entriesFlags[3] = true;
                    break;

                case FileCaracteristic.Metadata:
                    entriesFlags[4] = true;
                    break;
            }
        }
        BitArray outflag = new BitArray(entriesFlags);
        outBin.Add(outflag.ToByte());

        outBin.Add((byte)((FileIdentifier.Dados.Length * 2) +1));
        outBin.AddRange(ICB.GetData());
        outBin.AddRange(BitConverter.GetBytes((UInt16)TamanhoUsoImplementação));
        if (UsoImplementação != null && TamanhoUsoImplementação > 0)
            outBin.AddRange(UsoImplementação);
        outBin.AddRange(FileIdentifier.GetData());

        while (outBin.Count % 4 != 0)
            outBin.Add(0);

        //Tag
        outSector.AddRange(new Descritor.Tag_Descritor()
        {
            ID_de_Descritor = 0x101,
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
            ID_de_Descritor = 0x101,
            LBA_This_Descritor = (uint)lba,
            Tamanho_CRC_Descritor = (uint)outBin.Count,
            Versão = 2,
            Reservado = 0,
            VolumeSerialNumber = 0,
            CRC_Descritor = UDFUtils.ComputeCrc(outBin.ToArray(), outBin.Count),
            TagChecksum = tagchecksum
        }.GetTag());

        outSector.AddRange(outBin);
        
        return outSector.ToArray();
    }
    public FI() { }
    public FI(byte[] Entry, Stream ISO, PD Partição)
    {
        tamanhosetor = Partição.tamanhosetor;

        ReadDTAG(Entry);

        FileVersionNumber = Entry.ReadUInt(0x10, 16);

        #region FileCaracteristics
        var caracs = new List<FileCaracteristic>();
        byte flags = Entry[0x12];
        int index = 0;
        foreach (bool bit in flags.ReadBits())
        {
            switch (index)
            {
                case 0:
                    if (bit)
                        caracs.Add(FileCaracteristic.Hidden);
                    else
                        caracs.Add(FileCaracteristic.Exists);
                    break;
                case 1:
                    if (bit)
                        caracs.Add(FileCaracteristic.Directory);
                    else
                        caracs.Add(FileCaracteristic.Archive);
                    break;
                case 2:
                    if (bit)
                        caracs.Add(FileCaracteristic.Deleted);
                    break;
                case 3:
                    if (bit)
                        caracs.Add(FileCaracteristic.Parent);
                    break;
                case 4:
                    if (bit)
                        caracs.Add(FileCaracteristic.Metadata);
                    break;
            }
            index++;
        }
        FileCaracteristics = caracs.ToArray();
        #endregion

        FileIDSize = Entry[0x13];

        ICB.ReadfromData(Entry.ReadBytes(0x14, 0x10));
        uint lbafe = ICB.LocalizaçãoExtent.LogicalBlockNumber + Partição.LBAPartição;
        FileEntry = Descritor.ReadSector(ISO, (int)lbafe, tamanhosetor) as FE;//new FE(ISO.ReadBytes((int)(lbafe * tamanhosetor), tamanhosetor));

        TamanhoUsoImplementação = Entry.ReadUInt(0x24, 16);

        if (TamanhoUsoImplementação > 0)
            UsoImplementação = Entry.ReadBytes(0x26, (int)TamanhoUsoImplementação);

        if (FileIDSize > 0)
            FileIdentifier.ReadfromData(Entry.ReadBytes(0x26 + (int)TamanhoUsoImplementação,(int)FileIDSize));
    }
    public static FI[] SplitFromSector(byte[] Sector, Stream ISO, PD Partição, int lba)
    {

        var fis = new List<FI>();
        for (int i = 0; i < Sector.Length;)
        {
            byte[] readed = Sector.ReadBytes(i + 8, Sector[i + 0x13] + 0x26);
            int size = readed.Length;
            #region Zero Skip
            try
            {
                if (Sector[i + size] == 0)
                    while (Sector[i + size] == 0)
                        size++;
            }
            catch (IndexOutOfRangeException) { }
            #endregion
            byte[] entr = Sector.ReadBytes(i, size);
            #region Zero Array Skip
            if (!entr.All(x => x == 0))
            {
                FI fileid = new FI(entr, ISO, Partição);
                fileid.lba = lba;
                int offsetnosetor = i + (lba * Partição.tamanhosetor);
                fileid.offsetsetor = offsetnosetor;
                fileid.tamanhosetor = Partição.tamanhosetor;
                fis.Add(fileid);
            }
            #endregion
            i += entr.Length;
        }
        return fis.ToArray();

    }
    public enum FileCaracteristic
    {
        Exists,
        Hidden,
        Directory,
        Archive,
        Deleted,
        Parent,
        Metadata,
        Null
    };
}
