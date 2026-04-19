using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

/// <summary>
/// Classe para leitura de um sistema ISO9660+UDF, alvo principal: Estrutura Sony Playstation®2.
/// </summary>
/// Bit.Raiden/Dev C-Sharp, uso não comercial no momento.
/// Existe conhecimento, mas apenas o conhecimento de Cristo é poder.
/// Novembro/2021
public class Path_Table: Setor
{
    public uint DirLBA,ParenteDirNumber;
    public string NomePasta;
    public Arquivo[] Conteúdo;
    public Path_Table()
    {

    }
    public Path_Table(byte[] entry, bool bigendian, Stream reader)
    {
        int nomesize = entry[0];
        DirLBA = entry.ReadUInt(2, 32, bigendian);
        ParenteDirNumber = entry.ReadUInt(6, 16);
        NomePasta = entry.ReadBytes(8, nomesize).ConvertTo(Encoding.Default);

        Conteúdo = Arquivo.LerPastas(reader.ReadFiles((int)DirLBA));
    }

    public byte[] GetTable(bool bigendian = false)
    {
        var outTableBin = new List<byte>();

        outTableBin.Add((byte)(NomePasta.Length > 0 ? NomePasta.Length : 1)); //Name Length(1 for empty/ Root)
        outTableBin.Add(0); //Extended atribute length(normally 0)

        outTableBin.AddRange(bigendian ? BitConverter.GetBytes(DirLBA).Reverse() : 
            BitConverter.GetBytes(DirLBA)); //LBA
        outTableBin.AddRange(bigendian ? BitConverter.GetBytes((UInt16)ParenteDirNumber).Reverse() :
            BitConverter.GetBytes((UInt16)ParenteDirNumber)); //Number of Parent Directory

        if (NomePasta.Length > 0)
            outTableBin.AddRange(Encoding.Default.GetBytes(NomePasta)); //Name
        else
            outTableBin.Add(0);

        while (outTableBin.Count % 2 != 0)
            outTableBin.Add(0);

        return outTableBin.ToArray();
    }
    public static Path_Table[] LerTabelas(byte[] sector, bool bigendian, Stream reader)
    {
        var paths = new List<Path_Table>();
        for(int i =0;i< sector.Length;)
        {
            byte[] readed = sector.ReadBytes(i+8,sector[i]);
            int size = 8 + readed.Length;
            #region Zero Skip
            try
            {
                if (sector[i + size] == 0)
                    while (sector[i + size] == 0)
                        size++;
            }
            catch (IndexOutOfRangeException) { }
            #endregion
            byte[] entr = sector.ReadBytes(i, size);
            #region Zero Array Skip
            if (!entr.All(x=>x==0))
               paths.Add(new Path_Table(entr, bigendian, reader));
            #endregion
            i += entr.Length;
        }
        return paths.ToArray();
    }
}

