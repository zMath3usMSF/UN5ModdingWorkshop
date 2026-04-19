using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

/// <summary>
/// Classe para leitura de um sistema ISO9660+UDF, alvo principal: Estrutura Sony Playstation®2.
/// </summary>
/// Bit.Raiden/Dev C-Sharp, uso não comercial no momento.
/// Existe conhecimento, mas apenas o conhecimento de Cristo é poder.
/// Novembro/2021
public class Arquivo
{
    public enum Regras
    {
        Normal,
        Oculto,
        ArquivoAssoc,
        Atributado,
        AtributadoExtendido,
        Continuado,
        SubDiretório
    }

    public uint LBA, Tamanho, VolumeSequencialNumber, OffsetinSector;
    public string Name, FullPath;
    public DateTime Gravação;
    public Regras[] Flags;

    public Arquivo()
    {

    }
    public Arquivo(byte[] entrada, int offs)
    {
        OffsetinSector = (uint)offs;
        try
        {
            LBA = entrada.ReadUInt(2, 32);
            Tamanho = entrada.ReadUInt(0xA, 32);
            byte[] datetimedir = entrada.ReadBytes(0x12, 7);
            Gravação = datetimedir.GetDateTimeDir();
            #region Regras
            var regras = new List<Regras>(8);
            byte flags = entrada.ReadBytes(0x19, 1)[0];
            int index = 0;
            foreach (bool bit in flags.ReadBits())
            {
                switch (index)
                {
                    case 0:
                        if (bit)
                            regras.Add(Regras.Oculto);
                        break;
                    case 1:
                        if (bit)
                            regras.Add(Regras.SubDiretório);
                        break;
                    case 2:
                        if (bit)
                            regras.Add(Regras.ArquivoAssoc);
                        break;
                    case 3:
                        if (bit)
                            regras.Add(Regras.Atributado);
                        break;
                    case 4:
                        if (bit)
                            regras.Add(Regras.AtributadoExtendido);

                        break;

                    case 7:
                        if (bit)
                            regras.Add(Regras.Continuado);

                        break;
                }
                index++;
            }
            Flags = regras.ToArray();
            #endregion
            VolumeSequencialNumber = entrada.ReadUInt(0x1C, 16);
            int namesize = entrada.ReadBytes(0x20, 1)[0];
            Name = entrada.ReadBytes(0x21, namesize).ConvertTo(Encoding.Default);
        }
        catch (Exception) { }
    }
    public byte[] GetFile()
    {
        var outBin = new List<byte>();//Final Record output

        var record = new List<byte>();

        record.AddRange(LBA.ToLEBE(32));//LBA
        record.AddRange(Tamanho.ToLEBE(32));//File Size
        record.AddRange(Gravação.GetDateTimeData());//File Write Time

        //FLAGS
        bool[] entriesFlags = new bool[8];
        foreach (Regras regra in Flags)
        {
            switch (regra)
            {
                case Regras.Oculto:
                    entriesFlags[0] = true;
                    break;
                case Regras.SubDiretório:
                    entriesFlags[1] = true;
                    break;

                case Regras.ArquivoAssoc:
                    entriesFlags[2] = true;
                    break;

                case Regras.Atributado:
                    entriesFlags[3] = true;
                    break;

                case Regras.AtributadoExtendido:
                    entriesFlags[4] = true;
                    break;

                case Regras.Continuado:
                    entriesFlags[7] = true;
                    break;
            }
        }
        BitArray outflag = new BitArray(entriesFlags);

        record.Add(outflag.ToByte());//Binary Flags

        record.Add(0);//Unit size for interleave mode (0 for nope)
        record.Add(0);//Interleave mode gap size (0 for nope)

        record.AddRange(VolumeSequencialNumber.ToLEBE(16));//Volume Sequence Index
        record.Add(Name.Length == 0 ? (byte)1 : (byte)Name.Length);//Name size
        record.AddRange(Encoding.Default.GetBytes(Name.ToUpper()));//File/Dir Name

        //Padded to 2
        while ((record.Count) % 2 != 0 || record.Count < 0x2e)
        {
            record.Add(0);
        }

        outBin.Add((byte)(record.Count + 2));//Record Size
        outBin.Add(0);//Record Extended atribute Size
        outBin.AddRange(record.ToArray());

        return outBin.ToArray();
    }
    public static List<EntradaArquivo> GetInfos(string rootdir, int tamanhosetor,int lba = 261, bool patchmode = false, uint udfblocksahead = 0)
    {
        var entradas = new List<EntradaArquivo>();

        if (!patchmode)
        {
            int alldirblockuse = GetDirectoriesRecordsBlockCount(rootdir, tamanhosetor);
            //MessageBox.Show("Gasto de Setores-AllDir: " + alldirblockuse.ToString());
            lba += alldirblockuse + (int)udfblocksahead;
        }
        

        long LBAFile = lba;

        foreach (string file in Directory.EnumerateFiles(rootdir, "*.*",SearchOption.AllDirectories))
        {
            long Filesize = new FileInfo(file).Length;

            entradas.Add(new EntradaArquivo
            {
                Nome = Path.GetFileName(file) + ";1",
                NomecomPasta = file.Split(new string[] { rootdir },StringSplitOptions.RemoveEmptyEntries)[0],
                Tamanho = Filesize,
                Tipo = 1,
                LBA = LBAFile,
                TimeInfo = new FileInfo(file).LastWriteTime
            });

            LBAFile += GetSectorUseFileSize(Filesize, tamanhosetor);
        }

        return entradas;
    }

    public static int GetSectorUseFileSize(long fileSize, int tamanhosetor)
    {
        int sectorsused = 0;
        long subsize = fileSize;

        while (subsize % tamanhosetor != 0)
            subsize++;

        sectorsused = (int)(subsize / tamanhosetor);


        return sectorsused;
    }

    public static int GetDirectoriesRecordsBlockCount(string rootdir, int tamanhosetor)
    {
        int result = 1;//Root Included
        foreach (string dir in Directory.EnumerateDirectories(rootdir, "*.*", SearchOption.AllDirectories))
        {
            result += GetRecordsDirBlockCount(dir, tamanhosetor);
        }
        return result;
    }

    public static List<byte> GetRecords(string root, List<EntradaArquivo> Infos,int tamanhosetor, uint udfblocksahead = 0)
    {
        var outList = new List<byte>();

        uint lba = 261;//After the two Path_Tables(LE+BE) + Optional ones-Root

        //Root
        outList.AddRange(GetDirectoryRecords(root, 261, lba, Infos, (uint)GetDirectoryRecords(root, 261, lba, Infos, 0, tamanhosetor).Length, tamanhosetor));
        lba++;


        foreach (string Dir in Directory.EnumerateDirectories(root, "*.*", SearchOption.AllDirectories))
        {
            outList.AddRange(GetDirectoryRecords(Dir, GetParentLBA(Dir, root,tamanhosetor), lba, Infos, GetParentSize(Dir,tamanhosetor), tamanhosetor));
            lba += (uint)GetRecordsDirBlockCount(Dir, tamanhosetor);
        }
        return outList;
    }

    public static uint GetParentLBA(string Dir, string root, int tamanhosetor)
    {
        uint lba = 261;//Root

        var records = new List<byte>();

        string parent = Directory.GetParent(Dir).FullName;

        if (parent+@"\" == root)
            return lba;

        foreach(var dirsub in Directory.EnumerateDirectories(root, "*.*", SearchOption.AllDirectories))
        {
            records.AddRange(GetDirectoryRecords(dirsub, 0, 0, null, 0, tamanhosetor));
            //MessageBox.Show(dirsub + "\n" + Directory.GetParent(Dir).FullName);

            if(dirsub==Directory.GetParent(Dir).FullName)
            {
                return (uint)(lba+records.Count / tamanhosetor);
            }
        }

        return lba;
    }
    public static uint GetParentSize(string Dir,  int tamanhosetor)
    {
        uint size = 0;

        byte[] record = GetDirectoryRecords(Directory.GetParent(Dir).FullName, 0, 0, null, 0, tamanhosetor);

        size = (uint)record.Length;

        return size;
    }
    public static byte[] GetDirectoryRecords(string dir, uint parentlba, uint dirlba, List<EntradaArquivo> Entradas, uint parentsize, int tamanhosetor)
    {
        var bytess = new List<byte>();//Usagem Volátil
        var outDir = new List<byte>();

        //./(cd) and ../(cd..)
        bytess.AddRange(GetRecordData("", dirlba, 0, DateTime.Now, new Regras[] { }));//CD-This Dir
        bytess.AddRange(GetRecordData("", parentlba, parentsize, DateTime.Now, new Regras[] { }));//CD.. Root Dir

        var entries = Directory.EnumerateFileSystemEntries(dir).ToArray();
        EntradaArquivo entrada = new EntradaArquivo();
        for (int i = 0; i < entries.Length;)
        {
            if (!Directory.Exists(Path.GetFileName(entries[i]))&&Entradas!=null)
            {
                try
                {
                    entrada = Entradas.Where(x => x.Nome == Path.GetFileName(entries[i]) + ";1").ToArray()[0];
                }
                catch (IndexOutOfRangeException) { }
            }
            else if(Entradas == null|| Directory.Exists(Path.GetFileName(entries[i])))
            {
                entrada.LBA = 0;
                entrada.Tamanho = 0;
            }
            byte[] record = GetRecordData(Directory.Exists(entries[i]) ? Path.GetFileName(entries[i]) : Path.GetFileName(entries[i]) + ";1",
                (uint)entrada.LBA, (uint)entrada.Tamanho, DateTime.Now, new Regras[] { });

            if ((bytess.Count + record.Length) < tamanhosetor)
            {
                bytess.AddRange(record);
                if (i == entries.Length - 1)
                {
                    while (bytess.Count % tamanhosetor != 0)
                        bytess.Add(0);
                    outDir.AddRange(bytess.ToArray());
                    i++;
                }
                else
                    i++;
            }
            else
            {
                while (bytess.Count % tamanhosetor != 0)
                    bytess.Add(0);
                outDir.AddRange(bytess.ToArray());
                bytess.Clear();
            }
        }

        #region Fix Repeated blocks and remove them
        //for (int i = 0; i < outDir.Count / tamanhosetor; i++)
        //{
        //    if ((i + 1) < outDir.Count)
        //    {
        //        byte[] sector = outDir.ToArray().ReadSector(i, tamanhosetor);
        //        byte[] nextsector = outDir.ToArray().ReadSector(i + 1, tamanhosetor);

        //        if (nextsector.SequenceEqual(sector) || nextsector.All(x => x == 0))
        //            outDir.RemoveRange((i + 1) * tamanhosetor, tamanhosetor);

        //    }
        //}//cada setor
        #endregion

        //Fix CD entry size
        //Array.Copy(((uint)outfinal.Length).ToLEBE(32), 0, outfinal, 0xa, 8);


        return outDir.ToArray();
    }
    public static int GetRecordsDirBlockCount(string dir, int tamanhosetor)
    {
        var bytess = new List<byte>();//Usagem Volátil
        var outSectors = new List<byte>();//Coleção final

        //./(cd) and ../(cd..)
        bytess.AddRange(GetRecordData("", 0, 0, DateTime.Now, new Regras[] { }));
        bytess.AddRange(GetRecordData("", 0, 0, DateTime.Now, new Regras[] { }));

        var entries = Directory.EnumerateFileSystemEntries(dir, "*.*", SearchOption.TopDirectoryOnly).ToArray();

        for (int i = 0; i < entries.Length;)
        {
            byte[] record = GetRecordData(Directory.Exists(entries[i]) ? Path.GetFileName(entries[i]) : Path.GetFileName(entries[i]) + ";1",
                0, 0, DateTime.Now, new Regras[] { });

            if ((bytess.Count + record.Length) < tamanhosetor)
            {
                bytess.AddRange(record);
                if (i == entries.Length - 1)
                {
                    while (bytess.Count % tamanhosetor != 0)
                        bytess.Add(0);
                    outSectors.AddRange(bytess.ToArray());
                    i++;
                }
                else
                    i++;
            }
            else
            {
                while (bytess.Count % tamanhosetor != 0)
                    bytess.Add(0);
                outSectors.AddRange(bytess.ToArray());
                bytess.Clear();
            }
        }

        #region Fix Repeated blocks and remove them
        //for (int i = 0; i < outSectors.Count / tamanhosetor; i++)
        //{
        //    if ((i + 1) < outSectors.Count)
        //    {
        //        byte[] sector = outSectors.ToArray().ReadSector(i, tamanhosetor);
        //        byte[] nextsector = outSectors.ToArray().ReadSector(i + 1, tamanhosetor);

        //        if (nextsector.SequenceEqual(sector) || nextsector.All(x => x == 0))
        //            outSectors.RemoveRange((i + 1) * tamanhosetor, tamanhosetor);

        //    }
        //}//cada setor
        #endregion

        return outSectors.Count / tamanhosetor;
    }

    public static byte[] GetRecordData(string Name, uint lba, uint size,
        DateTime Gravação, Regras[] flags, uint volumeseqnumber = 1)
    {
        var outBinary = new List<byte>();

        //Size Extended Atribute Record
        //0, inexistent
        outBinary.Add(0);

        //LE+BE LBA
        outBinary.AddRange(lba.ToLEBE(32));

        //LE+BE Size
        outBinary.AddRange(size.ToLEBE(32));

        //DateTime
        outBinary.AddRange(Gravação.GetDateTimeData());

        //FLAGS
        bool[] entriesFlags = new bool[8];
        foreach (Regras regra in flags)
        {
            switch (regra)
            {
                case Regras.Oculto:
                    entriesFlags[0] = true;
                    break;
                case Regras.SubDiretório:
                    entriesFlags[1] = true;
                    break;

                case Regras.ArquivoAssoc:
                    entriesFlags[2] = true;
                    break;

                case Regras.Atributado:
                    entriesFlags[3] = true;
                    break;

                case Regras.AtributadoExtendido:
                    entriesFlags[4] = true;
                    break;

                case Regras.Continuado:
                    entriesFlags[7] = true;
                    break;
            }
        }
        BitArray outflag = new BitArray(entriesFlags);
        outBinary.Add(outflag.ToByte());

        //Intercaled Mode
        outBinary.Add(0);
        outBinary.Add(0);//intercaled files size

        //Volume Sequencial number(usually 1)
        outBinary.AddRange(volumeseqnumber.ToLEBE(16));

        //Name Size and Name string
        outBinary.Add(Name.Length == 0 ? (byte)1 : (byte)Name.Length);
        outBinary.AddRange(Encoding.Default.GetBytes(Name));

        //Some Padding
        while ((outBinary.Count + 1) % 2 != 0)
        {
            outBinary.Add(0);
        }
        //outBinary.Add(0);

        //OUTFINAL ARRAY
        var finalout = new List<byte>();
        byte[] part = outBinary.ToArray();
        finalout.Add((byte)(part.Length + 1));
        finalout.AddRange(part);
        return finalout.ToArray();
    }
    public static Arquivo[] LerPastas(byte[] setorDIR)
    {
        var pastas = new List<Arquivo>();
        try
        {

            bool stop = false;
            for (int i = 0; stop == false;)
            {
                byte[] entrada = setorDIR.ReadBytes(i, setorDIR[i]);
                if (entrada.All(x => x == 0))
                {
                    stop = true;
                }
                if (stop == false)
                {
                    pastas.Add(new Arquivo(entrada,i));
                    i += entrada.Length;
                }
            }

        }
        catch (Exception) { }
        return pastas.ToArray();
    }

    public struct EntradaArquivo
    {
        public string Nome, NomecomPasta;
        public long LBA, Tamanho;
        public int Tipo;
        public DateTime TimeInfo;
        //1 = Arquivo
        //0 = Pasta
    }
}

