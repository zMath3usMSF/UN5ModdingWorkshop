using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

/// <summary>
/// Classe para leitura de um sistema ISO9660+UDF, alvo principal: Estrutura Sony Playstation®2.
/// </summary>
/// Bit.Raiden/Dev C-Sharp, uso não comercial no momento.
/// Existe conhecimento, mas apenas o conhecimento de Cristo é poder.
/// Novembro/2021
public class Volume_Primário: Setor
{
    public string TwoSectorsBeforeID;
    public string VolumeID;

    public string VolumeSetID;
    public string Publicante;
    public string Autor;
    public string AppID;
    
    public string Copyright;
    public string Abstrato;
    public string Bibliografia;

    public DateTimeOffset Criação;
    public DateTimeOffset Modificação;
    public DateTimeOffset Validade;
    public DateTimeOffset Disponível;

    public uint SectorCount, DiskCount, DiskCountVolSet;
    public uint SectorsSize;
    public uint PathTableSize;

    public uint PathTableLBA, PathTableLBABE;
    public uint PathTableLBAextra, PathTableLBABEextra;

    public Path_Table[] path_Tables;
    public Path_Table[] path_Tables_Opt;
    public Path_Table[] path_Tables_BE;
    public Path_Table[] path_Tables_BE_Opt;

    public byte[] DirectoryRecord;
    public byte DirectoryRecordsVersion;
    public Volume_Primário(Stream reader, int lba, int tamanho)
    {
        this.lba = lba;
        this.tipo = Tipo_de_Descritor.VolumePrimário;
        this.tamanhosetor = tamanho;
        this.offsetsetor = lba * tamanho;
        this.NomeSeção = reader.ReadBytes(offsetsetor + 1, 5).ConvertTo(Encoding.Default);
        this.Versão = reader.ReadBytes(offsetsetor + 6, 1)[0];

        //7 não usado
        TwoSectorsBeforeID = reader.ReadBytes(offsetsetor + 8, 0x20).ConvertTo(Encoding.Default);
        VolumeID = reader.ReadBytes(offsetsetor + 0x28, 0x20).ConvertTo(Encoding.Default);

        SectorCount = reader.ReadUInt(offsetsetor + 0x50, 32);
        DiskCount = reader.ReadUInt(offsetsetor + 0x78, 16);
        DiskCountVolSet = reader.ReadUInt(offsetsetor + 0x7C, 16);

        SectorsSize = reader.ReadUInt(offsetsetor + 0x80, 16);
        PathTableSize = reader.ReadUInt(offsetsetor + 0x84, 32);

        //LittleEndian
        PathTableLBA = reader.ReadUInt(offsetsetor + 0x8C, 32);
        PathTableLBAextra = reader.ReadUInt(offsetsetor + 0x90, 32);
        //BigEndian
        PathTableLBABE = reader.ReadUInt(offsetsetor + 0x94, 32,true);
        PathTableLBABEextra = reader.ReadUInt(offsetsetor + 0x98, 32,true);

        //DirectoryRecord 0x22 bytes
        DirectoryRecord = reader.ReadBytes(offsetsetor + 0x9C, 0x22);


        #region Strings 2
        VolumeSetID = reader.ReadBytes(offsetsetor + 0xBE, 0x80).ConvertTo(Encoding.Default);
        Publicante = reader.ReadBytes(offsetsetor + 0x13E, 0x80).ConvertTo(Encoding.Default);
        Autor = reader.ReadBytes(offsetsetor + 0x1BE, 0x80).ConvertTo(Encoding.Default);
        AppID = reader.ReadBytes(offsetsetor + 0x23E, 0x80).ConvertTo(Encoding.Default);

        Copyright = reader.ReadBytes(offsetsetor + 0x2BE, 0x25).ConvertTo(Encoding.Default);
        Abstrato = reader.ReadBytes(offsetsetor + 0x2E3, 0x25).ConvertTo(Encoding.Default);
        Bibliografia = reader.ReadBytes(offsetsetor + 0x308, 0x25).ConvertTo(Encoding.Default);
        #endregion

        #region Datas e Tempos
        try
        {
            Criação = DateTimeOffset.ParseExact(reader.ReadBytes(offsetsetor + 0x32D, 0x10).ConvertTo(Encoding.Default), "yyyyMMddHHmmssff", CultureInfo.InvariantCulture);
            Modificação = DateTimeOffset.ParseExact(reader.ReadBytes(offsetsetor + 0x33E, 0x10).ConvertTo(Encoding.Default), "yyyyMMddHHmmssff", CultureInfo.InvariantCulture);
            Validade = DateTimeOffset.ParseExact(reader.ReadBytes(offsetsetor + 0x34F, 0x10).ConvertTo(Encoding.Default), "yyyyMMddHHmmssff", CultureInfo.InvariantCulture);
            Disponível = DateTimeOffset.ParseExact(reader.ReadBytes(offsetsetor + 0x360, 0x10).ConvertTo(Encoding.Default), "yyyyMMddHHmmssff", CultureInfo.InvariantCulture);
        }
        catch (Exception) { }
        #endregion

        DirectoryRecordsVersion = reader.ReadBytes(offsetsetor + 0x361, 1)[0];
        
        path_Tables = Path_Table.LerTabelas(reader.ReadBytes((int)PathTableLBA* tamanhosetor, (int)PathTableSize), false, reader);

        //if(PathTableLBAextra!=0)
        //    path_Tables_Opt = Path_Table.LerTabelas(reader.ReadBytes((int)PathTableLBAextra * tamanhosetor, (int)PathTableSize), false, reader);
        //if (PathTableLBABE != 0)
        //    path_Tables_BE = Path_Table.LerTabelas(reader.ReadBytes((int)PathTableLBABE * tamanhosetor, (int)PathTableSize), false, reader);
        //if (PathTableLBABEextra != 0)
        //    path_Tables_BE_Opt = Path_Table.LerTabelas(reader.ReadBytes((int)PathTableLBABEextra * tamanhosetor, (int)PathTableSize), false, reader);
    }

    public static byte[] GetPrimaryVolume(string IDsistema, string Volume, uint totalblocksonISO, uint tamanhosetor,uint disksnumber, uint pathtablesize, uint LEpathtablelba, uint BEpathtablelba, 
        uint rootdirLBA, uint rootdirSize,string IDvolume, string Autor, string Dados, string Aplicação, string Copyright, string Resumo, string Bibliografia,
        DateTimeOffset Criação, uint optionalLEpathtablelba = 0, uint optionalBEpathtablelba = 0)
    {
        List<byte> primaryout = new List<byte>();

        //Código Descritor+ID+Versão
        primaryout.Add(1);//Code
        primaryout.AddRange(Encoding.Default.GetBytes("CD001"));//ID
        primaryout.Add(1);//Version
        primaryout.Add(0);//Not Used

        //IDSISTEMA+VOLUME
        primaryout.AddRange(IDsistema.GetFilledString(0x20, 0x20));
        primaryout.AddRange(Volume.GetFilledString(0x20, 0x20));
        primaryout.AddRange(new byte[8]);//not used


        //TotalBlocks
        primaryout.AddRange(totalblocksonISO.ToLEBE(32));
        primaryout.AddRange(new byte[0x20]);//not used

        //Disks Number
        primaryout.AddRange(disksnumber.ToLEBE(16));//LogicalVolume disks number
        primaryout.AddRange(disksnumber.ToLEBE(16));//this disks number

        //Sector Size
        primaryout.AddRange(tamanhosetor.ToLEBE(16));//normalmente 0x800 ->2kb(DVD)

        //PathTable Size
        primaryout.AddRange(pathtablesize.ToLEBE(32));

        //PathTable variants LBA
        primaryout.AddRange(BitConverter.GetBytes((UInt32)LEpathtablelba));//LE
        primaryout.AddRange(BitConverter.GetBytes((UInt32)optionalLEpathtablelba));//LE Opcional

        primaryout.AddRange(BitConverter.GetBytes((UInt32)BEpathtablelba).Reverse().ToArray());//BE
        primaryout.AddRange(BitConverter.GetBytes((UInt32)optionalBEpathtablelba).Reverse().ToArray());//BE Opcional

        //Directory Record[0x22]
        primaryout.AddRange(Arquivo.GetRecordData("", rootdirLBA, rootdirSize, DateTime.Now, new Arquivo.Regras[] { Arquivo.Regras.SubDiretório }));

        //Infos 2
        primaryout.AddRange(IDvolume.GetFilledString(0x80, 0x20));
        primaryout.AddRange(Autor.GetFilledString(0x80, 0x20));
        primaryout.AddRange(Dados.GetFilledString(0x80, 0x20));
        primaryout.AddRange(Aplicação.GetFilledString(0x80, 0x20));

        primaryout.AddRange(Copyright.GetFilledString(0x25, 0x20));
        primaryout.AddRange(Resumo.GetFilledString(0x25, 0x20));
        primaryout.AddRange(Bibliografia.GetFilledString(0x25, 0x20));

        //Datas
        primaryout.AddRange(Criação.GetDateTimeOffsetData());//Criação
        primaryout.AddRange(DateTimeOffset.Now.GetDateTimeOffsetData());//Modificação
        byte[] empty = new byte[0x11].FillArray(0x30);
        empty[0x10] = 0;
        primaryout.AddRange(empty);//Validade
        primaryout.AddRange(empty);//Data para Uso

        //Outros
        primaryout.Add(1);//Verion PathTable
        primaryout.Add(0);//Always zero

        while (primaryout.Count % tamanhosetor != 0 || primaryout.Count() < tamanhosetor)
            primaryout.Add(0);

        return primaryout.ToArray();
    }
}

