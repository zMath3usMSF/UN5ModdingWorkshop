using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using static IOextent;
using System.Threading;

/// <summary>
/// Classe para leitura de um sistema ISO9660+UDF, alvo principal: Estrutura Sony Playstation®2.
/// </summary>
/// Bit.Raiden/Dev C-Sharp, uso não comercial no momento.
/// Existe conhecimento, mas apenas o conhecimento de Cristo é poder.
/// Novembro/2021
public class ISO9660
{
    public Stream ISOfile
    {
        get;
        set;
    }
    public int mode = 1;
    public int Tamanho_Setor = 0x800;

    public string FileName, Name;

    public List<Setor> Setores;

    public UDF UDFSession;
    public ISO9660(Stream ISO)
    {
        ISOfile = ISO;

        //Name = Path.GetFileNameWithoutExtension(FileName);

        if (mode == 2)
            Tamanho_Setor = 0x930;

        #region Ler Setores
        Setores = new List<Setor>();
        int lba = 0;
        while (lba < 20)
        {
            Setor setor = Setor.ReadSector(ISOfile, lba, Tamanho_Setor);
            setor.iso = ISOfile;
            Setores.Add(setor);
            lba++;
        }
        #endregion

        #region UDFVerify

        if (Setores.Any(x => x.NomeSeção != null && x.NomeSeção.Contains("NSR")))
            UDFSession = new UDF(this);

        #endregion
    }
    public ISO9660(string ISOpath)
    {
        ISOfile = File.Open(ISOpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        FileName = ISOpath;
        Name = Path.GetFileNameWithoutExtension(FileName);

        if (mode == 2)
            Tamanho_Setor = 0x930;

        #region Ler Setores
        Setores = new List<Setor>();
        int lba = 0;
        while (lba < 20)
        {
            Setor setor = Setor.ReadSector(ISOfile, lba, Tamanho_Setor);
            setor.iso = ISOfile;
            Setores.Add(setor);
            lba++;
        }
        #endregion

        #region UDFVerify

        if (Setores.Any(x => x.NomeSeção != null && x.NomeSeção.Contains("NSR")))
            UDFSession = new UDF(this);

        #endregion
    }
    public byte[] GetFirst16Sectors()
    {
        var outbin = new List<byte>();
        for (int i = 0; i < 16; i++)
            outbin.AddRange(Setores[i].GetSectorData);
        return outbin.ToArray();
    }

    public struct PathName
    {
        public string Name;
        public int ParentCount;
    }

    public static bool BuildISO(string isofolder, string outputpath
     , string VolumeID, string Author, string Data, string VolumeName, string AplicationName, string CopyrightName, string Resumo, string Bibliographic, bool Hidden = false, bool UDF = true, int tamanhosetor = 0x800, byte[] first16Sectors = null, string executable = "")
    {
        Stream outISO = File.Open(outputpath, FileMode.OpenOrCreate);

        var log = new StringBuilder();

        var foldersNames = new List<PathName>();
        var rootRecords = new List<Arquivo>();
        var FolderRecords = new List<(Arquivo Key, List<Arquivo> Value)>();
        var Path_Tables = new List<Path_Table>();

        int PathTable_LBA = UDF ? 257 : 18;
        int PathTableBE_LBA = 0;
        int FileEntries_LBA = 0;

        int PartitionLBA = 0;
        int FELBA = 0;
        var rootFileIDs = new List<FI>();
        var FolderFileIDs = new List<(FI Key, List<FI> Value)>();
        int StreamStartLBA = 0;

        #region Build File/Dir Records

        foreach (var folder in Directory.EnumerateDirectories(isofolder, "*.*", SearchOption.AllDirectories).OrderBy(x => x))
        {
            var FileRecords = new List<Arquivo>();
            string folderName = new DirectoryInfo(folder).Name;
            foldersNames.Add(new PathName()
            {
                Name = folderName,
                ParentCount = (int)folder.GetParentsNumber(new DirectoryInfo(isofolder).Name)
            });

            foreach (var folderFolder in Directory.EnumerateDirectories(folder).OrderBy(x => x))
                FileRecords.Add(new Arquivo()
                {
                    LBA = 0,
                    Tamanho = 0,
                    Gravação = new DirectoryInfo(folderFolder).LastWriteTime,
                    Flags = new Arquivo.Regras[] { Arquivo.Regras.SubDiretório, (Hidden ? Arquivo.Regras.Oculto : Arquivo.Regras.Normal) },
                    VolumeSequencialNumber = 1,
                    Name = new DirectoryInfo(folderFolder).Name,
                    FullPath = folderFolder
                });

            foreach (var folderFile in Directory.EnumerateFiles(folder).OrderBy(x => x))
                FileRecords.Add(new Arquivo()
                {
                    LBA = 0,
                    Tamanho = (uint)new FileInfo(folderFile).Length,
                    Gravação = new FileInfo(folderFile).LastWriteTime,
                    Flags = new Arquivo.Regras[] { (Hidden ? Arquivo.Regras.Oculto : Arquivo.Regras.Normal) },
                    VolumeSequencialNumber = 1,
                    Name = new FileInfo(folderFile).Name + ";1",
                    FullPath = folderFile
                });

            FolderRecords.Add((new Arquivo()
            {
                LBA = 0,
                Tamanho = (uint)FileRecords.GetFiles().Count(),
                Gravação = new DirectoryInfo(folder).LastWriteTime,
                Flags = new Arquivo.Regras[] { Arquivo.Regras.SubDiretório, (Hidden ? Arquivo.Regras.Oculto : Arquivo.Regras.Normal) },
                VolumeSequencialNumber = 1,
                Name = folderName,
                FullPath = folder
            }, FileRecords));
        }

        foreach (var rootFolder in Directory.EnumerateDirectories(isofolder).OrderBy(x => x))
        {
            string folderName = new DirectoryInfo(rootFolder).Name;
            var entry = FolderRecords.FirstOrDefault(x => x.Key.Name == folderName);
            rootRecords.Add(new Arquivo()
            {
                LBA = 0,
                Tamanho = (uint)entry.Value.GetFiles().Length,
                Gravação = new DirectoryInfo(rootFolder).LastWriteTime,
                Flags = new Arquivo.Regras[] { Arquivo.Regras.SubDiretório, (Hidden ? Arquivo.Regras.Oculto : Arquivo.Regras.Normal) },
                VolumeSequencialNumber = 1,
                Name = folderName,
                FullPath = rootFolder
            });
        }

        foreach (var rootFile in Directory.EnumerateFiles(isofolder).OrderBy(x => x))
            rootRecords.Add(new Arquivo()
            {
                LBA = 0,
                Tamanho = (uint)new FileInfo(rootFile).Length,
                Gravação = new FileInfo(rootFile).LastWriteTime,
                Flags = new Arquivo.Regras[] { (Hidden ? Arquivo.Regras.Oculto : Arquivo.Regras.Normal) },
                VolumeSequencialNumber = 1,
                Name = new FileInfo(rootFile).Name + ";1",
                FullPath = rootFile
            });

        #endregion

        #region Build Path Table

        Path_Tables.Add(new Path_Table() { DirLBA = 0, ParenteDirNumber = 1, NomePasta = "" });
        foreach (var folder in foldersNames)
            Path_Tables.Add(new Path_Table() { DirLBA = 0, ParenteDirNumber = (uint)folder.ParentCount, NomePasta = folder.Name });

        #endregion

        #region Calc LBAs

        var PathTable = new List<byte>();
        PathTable.AddRange(Path_Tables.GetTables(tamanhosetor));
        while (PathTable.Count % tamanhosetor != 0 || PathTable.Count < tamanhosetor)
            PathTable.Add(0);

        PathTableBE_LBA = PathTable_LBA + ((PathTable.Count / tamanhosetor) * 2);
        FileEntries_LBA = PathTableBE_LBA + ((PathTable.Count / tamanhosetor) * 2);

        // Calcula setores dos directory records ISO9660 (usado sempre)
        // Calcula setores dos directory records ISO9660 — simula com "." e ".." incluídos
        var tempRecords = new List<byte>();

        // root: simula as 2 entradas extras
        var tempRoot = new List<Arquivo>();
        tempRoot.Add(new Arquivo() { LBA = 0, Tamanho = 0, Gravação = DateTime.Now, Flags = new Arquivo.Regras[] { Arquivo.Regras.SubDiretório }, VolumeSequencialNumber = 1, Name = "", FullPath = "ROOT" });
        tempRoot.Add(new Arquivo() { LBA = 0, Tamanho = 0, Gravação = DateTime.Now, Flags = new Arquivo.Regras[] { Arquivo.Regras.SubDiretório }, VolumeSequencialNumber = 1, Name = "", FullPath = "PARENT" });
        tempRoot.AddRange(rootRecords);
        tempRecords.AddRange(tempRoot.GetFiles());
        while (tempRecords.Count % tamanhosetor != 0 || tempRecords.Count < tamanhosetor)
            tempRecords.Add(0);

        // subpastas: simula as 2 entradas extras em cada uma
        foreach (var record in FolderRecords)
        {
            var tempFol = new List<Arquivo>();
            tempFol.Add(new Arquivo() { LBA = 0, Tamanho = 0, Gravação = DateTime.Now, Flags = new Arquivo.Regras[] { Arquivo.Regras.SubDiretório }, VolumeSequencialNumber = 1, Name = "", FullPath = record.Key.FullPath });
            tempFol.Add(new Arquivo() { LBA = 0, Tamanho = 0, Gravação = DateTime.Now, Flags = new Arquivo.Regras[] { Arquivo.Regras.SubDiretório }, VolumeSequencialNumber = 1, Name = "", FullPath = "PARENT" });
            tempFol.AddRange(record.Value);
            tempRecords.AddRange(tempFol.GetFiles());
            while (tempRecords.Count % tamanhosetor != 0 || tempRecords.Count < tamanhosetor)
                tempRecords.Add(0);
        }
        int iso9660RecordSectors = tempRecords.Count / tamanhosetor;
        tempRecords = null;

        // PartitionLBA e sectorCountRecords só existem com UDF
        int sectorCountRecords = 0;
        if (UDF)
        {
            sectorCountRecords = iso9660RecordSectors;
            PartitionLBA = FileEntries_LBA + iso9660RecordSectors;
        }

        #endregion

        #region UDF Structures (only when UDF = true)

        var File_Entries_A = new List<FE>();
        var File_Entries_B = new List<FE>();
        var File_Entries = new List<FE>();

        if (UDF)
        {
            #region Build File Identifiers

            rootFileIDs.Add(new FI()
            {
                FullPath = "--CYCLIC_DIRECTORY--",
                FileVersionNumber = 1,
                FileCaracteristics = new FI.FileCaracteristic[] { FI.FileCaracteristic.Directory, FI.FileCaracteristic.Parent, (Hidden ? FI.FileCaracteristic.Hidden : FI.FileCaracteristic.Null) },
                ICB = new Descritor.long_ad() { TamanhoExtent = 0x13c, LocalizaçãoExtent = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 }, UsoImplementação = new byte[6] },
                TamanhoUsoImplementação = 0,
                FileIdentifier = new Descritor.OSTAcompressedUnicode() { CompressionID = 0, Dados = "" },
                FileIDSize = 0
            });

            foreach (var rootRecord in rootRecords)
                rootFileIDs.Add(new FI()
                {
                    FullPath = rootRecord.FullPath,
                    FileVersionNumber = 1,
                    FileCaracteristics = new FI.FileCaracteristic[] { rootRecord.Flags.Contains(Arquivo.Regras.SubDiretório) ? FI.FileCaracteristic.Directory : FI.FileCaracteristic.Null, (Hidden ? FI.FileCaracteristic.Hidden : FI.FileCaracteristic.Null) },
                    ICB = new Descritor.long_ad() { TamanhoExtent = 0x13c, LocalizaçãoExtent = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 }, UsoImplementação = new byte[6] },
                    TamanhoUsoImplementação = 0,
                    FileIdentifier = new Descritor.OSTAcompressedUnicode()
                    {
                        CompressionID = 0x10,
                        Dados = rootRecord.Flags.Contains(Arquivo.Regras.SubDiretório) ? rootRecord.Name : rootRecord.Name.Substring(0, rootRecord.Name.Length - 2)
                    },
                    FileIDSize = (byte)((rootRecord.Name.Length * 2) + 1)
                });

            var sizeMeasure = new List<byte>();
            int FileIdsLBA = 2;
            foreach (var rid in rootFileIDs)
            {
                FileIdsLBA += sizeMeasure.Count / tamanhosetor;
                rid.lba = FileIdsLBA;
                sizeMeasure.AddRange(rid.SectorToBin());
            }
            while (sizeMeasure.Count % tamanhosetor != 0 || sizeMeasure.Count < tamanhosetor)
                sizeMeasure.Add(0);
            FileIdsLBA = 2 + (sizeMeasure.Count / tamanhosetor);
            sizeMeasure.Clear();

            foreach (var fold in FolderRecords)
            {
                var foldFI = new FI()
                {
                    FullPath = fold.Key.FullPath,
                    FileVersionNumber = 1,
                    FileCaracteristics = new FI.FileCaracteristic[] { fold.Key.Flags.Contains(Arquivo.Regras.SubDiretório) ? FI.FileCaracteristic.Directory : FI.FileCaracteristic.Null, (Hidden ? FI.FileCaracteristic.Hidden : FI.FileCaracteristic.Null) },
                    ICB = new Descritor.long_ad() { TamanhoExtent = 0x13c, LocalizaçãoExtent = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 }, UsoImplementação = new byte[6] },
                    TamanhoUsoImplementação = 0,
                    FileIdentifier = new Descritor.OSTAcompressedUnicode() { CompressionID = 0x10, Dados = fold.Key.Name },
                    FileIDSize = (byte)((fold.Key.Name.Length * 2) + 1)
                };

                var foldFIs = new List<FI>();
                foldFIs.Add(new FI()
                {
                    FullPath = "--CYCLIC_DIRECTORY--",
                    FileVersionNumber = 1,
                    FileCaracteristics = new FI.FileCaracteristic[] { FI.FileCaracteristic.Directory, FI.FileCaracteristic.Parent, (Hidden ? FI.FileCaracteristic.Hidden : FI.FileCaracteristic.Null) },
                    ICB = new Descritor.long_ad() { TamanhoExtent = 0x13c, LocalizaçãoExtent = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 }, UsoImplementação = new byte[6] },
                    TamanhoUsoImplementação = 0,
                    FileIdentifier = new Descritor.OSTAcompressedUnicode() { CompressionID = 0, Dados = "" },
                    FileIDSize = 0
                });

                foreach (var Frecord in fold.Value)
                    foldFIs.Add(new FI()
                    {
                        FullPath = Frecord.FullPath,
                        FileVersionNumber = 1,
                        FileCaracteristics = new FI.FileCaracteristic[] { Frecord.Flags.Contains(Arquivo.Regras.SubDiretório) ? FI.FileCaracteristic.Directory : FI.FileCaracteristic.Null, (Hidden ? FI.FileCaracteristic.Hidden : FI.FileCaracteristic.Null) },
                        ICB = new Descritor.long_ad() { TamanhoExtent = 0x13c, LocalizaçãoExtent = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 }, UsoImplementação = new byte[6] },
                        TamanhoUsoImplementação = 0,
                        FileIdentifier = new Descritor.OSTAcompressedUnicode()
                        {
                            CompressionID = 0x10,
                            Dados = Frecord.Flags.Contains(Arquivo.Regras.SubDiretório) ? Frecord.Name : Frecord.Name.Substring(0, Frecord.Name.Length - 2)
                        },
                        FileIDSize = (byte)((Frecord.Name.Length * 2) + 1)
                    });

                FolderFileIDs.Add((foldFI, foldFIs));
            }

            foreach (var folid in FolderFileIDs)
            {
                int olderValue = FileIdsLBA;
                foreach (var fid in folid.Value)
                {
                    FileIdsLBA += sizeMeasure.Count / tamanhosetor;
                    fid.lba = FileIdsLBA;
                    sizeMeasure.AddRange(fid.SectorToBin());
                }
                while (sizeMeasure.Count % tamanhosetor != 0 || sizeMeasure.Count < tamanhosetor)
                    sizeMeasure.Add(0);
                FileIdsLBA = olderValue + (sizeMeasure.Count / tamanhosetor);
                sizeMeasure.Clear();
            }

            #endregion

            #region Build File Entries

            EA MakeDefaultEA() => new EA()
            {
                OffsetImplementationUse = 0x18,
                OffsetApplicationUse = 0x84,
                UsoImplementação = new EA.ImplementationUse[]
                {
                new EA.ImplementationUse()
                {
                    TipodeAtributo = 0x800, SubTipodeAtributo = 1, Reservado = new byte[3], TamanhoAtributo = 0x34, TamanhoImplementationUse = 4,
                    ID = new Descritor.regid() { Regras = new Descritor.regid.Flag[] { }, ID = "*UDF FreeEASpace\0\0\0\0\0\0\0", IDSufixo = new byte[]{ 0x02,0x01,0,0,0,0,0,0 } },
                    UsoImplementaçãoFreeSpace = new EA.FreeEASpace() { HeaderChecksum = 0x561, FreeEaSpace = new byte[2]{ 0,0 } }
                },
                new EA.ImplementationUse()
                {
                    TipodeAtributo = 0x800, SubTipodeAtributo = 1, Reservado = new byte[3], TamanhoAtributo = 0x38, TamanhoImplementationUse = 8,
                    ID = new Descritor.regid() { Regras = new Descritor.regid.Flag[] { }, ID = "*UDF DVD CGMS Info\0\0\0\0\0", IDSufixo = new byte[]{ 0x02,0x01,0,0,0,0,0,0 } },
                    UsoImplementaçãoGGMS = new EA.DVDGGMSInfo() { HeaderChecksum = 0x549, GGMSInformation = 0, TipoDeDadosEstrutura = 0, InformaçãoProtetivadeSistema = new byte[4] }
                }
                }
            };

            Descritor.regid MakeImplID() => new Descritor.regid()
            {
                Regras = new Descritor.regid.Flag[] { Descritor.regid.Flag.Sujo },
                ID = "DVD-ROM GENERATOR\0\0\0\0\0\0",
                IDSufixo = new byte[8]
            };

            var rootBin = new List<byte>();
            rootBin.AddRange(rootFileIDs.GetFiles());

            File_Entries_A.Add(new FE()
            {
                Name = "ROOT",
                FullPath = "ROOT",
                tamanhosetor = tamanhosetor,
                TagICB = new Descritor.icbtag()
                {
                    PreviousDirectEntryNumber = 0,
                    StrategyType = Descritor.icbtag.Strategy.FA5,
                    StrategyParameter = new byte[] { 0, 0 },
                    MaxEntryNumber = 1,
                    FileTyp = Descritor.icbtag.FileType.Directory,
                    ICBParentLocation = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 },
                    Regras = new Descritor.icbtag.Flag[] { Descritor.icbtag.Flag.File, Descritor.icbtag.Flag.NonRelocTable, Descritor.icbtag.Flag.Res1, Descritor.icbtag.Flag.Contiguous }
                },
                UID = 0xffffffff,
                GID = 0xffffffff,
                Permissions = 0x14a5,
                Acesso = new Descritor.time_stamp() { UTCspecs = 0x1f4c, datetime = DateTime.Now },
                Modificação = new Descritor.time_stamp() { UTCspecs = 0x1f4c, datetime = DateTime.Now },
                Atributo = new Descritor.time_stamp() { UTCspecs = 0x1f4c, datetime = DateTime.Now },
                LinkedFileCount = (uint)sectorCountRecords,
                RecordFormat = 0,
                RecordAttrs = 0,
                RecordSize = 0,
                InfoSize = (uint)rootBin.Count,
                LogicalBlocksWrited = 1,
                Checkpoint = 1,
                ICBAttrExtendido = new Descritor.long_ad() { LocalizaçãoExtent = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 }, TamanhoExtent = 0, UsoImplementação = new byte[6] },
                IDImplementação = MakeImplID(),
                UniqueID = 0,
                TamanhoAttrExtendidos = 0x84,
                TamanhoDescritoresAloc = 8,
                AtributosExtendidos = MakeDefaultEA(),
                DescritoresAlocação = new Descritor.long_ad()
                {
                    TamanhoExtent = (uint)rootBin.Count,
                    LocalizaçãoExtent = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 },
                    UsoImplementação = new byte[6]
                }
            });

            foreach (var rootRecord in rootRecords)
                if (!rootRecord.Flags.Contains(Arquivo.Regras.SubDiretório))
                    File_Entries_B.Add(new FE()
                    {
                        Name = rootRecord.Name,
                        FullPath = rootRecord.FullPath,
                        tamanhosetor = tamanhosetor,
                        TagICB = new Descritor.icbtag()
                        {
                            PreviousDirectEntryNumber = 0,
                            StrategyType = Descritor.icbtag.Strategy.FA5,
                            StrategyParameter = new byte[] { 0, 0 },
                            MaxEntryNumber = 1,
                            FileTyp = Descritor.icbtag.FileType.RandomAccessBytes,
                            ICBParentLocation = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 },
                            Regras = new Descritor.icbtag.Flag[] { Descritor.icbtag.Flag.File, Descritor.icbtag.Flag.NonRelocTable, Descritor.icbtag.Flag.Res1, Descritor.icbtag.Flag.Contiguous }
                        },
                        UID = 0xffffffff,
                        GID = 0xffffffff,
                        Permissions = 0x14a5,
                        Acesso = new Descritor.time_stamp() { UTCspecs = 0x1f4c, datetime = rootRecord.Gravação },
                        Modificação = new Descritor.time_stamp() { UTCspecs = 0x1f4c, datetime = rootRecord.Gravação },
                        Atributo = new Descritor.time_stamp() { UTCspecs = 0x1f4c, datetime = rootRecord.Gravação },
                        LinkedFileCount = 1,
                        RecordFormat = 0,
                        RecordAttrs = 0,
                        RecordSize = 0,
                        InfoSize = rootRecord.Tamanho,
                        LogicalBlocksWrited = 1,
                        Checkpoint = 1,
                        ICBAttrExtendido = new Descritor.long_ad() { LocalizaçãoExtent = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 }, TamanhoExtent = 0, UsoImplementação = new byte[6] },
                        IDImplementação = MakeImplID(),
                        UniqueID = 0,
                        TamanhoAttrExtendidos = 0x84,
                        TamanhoDescritoresAloc = 8,
                        AtributosExtendidos = MakeDefaultEA(),
                        DescritoresAlocação = new Descritor.long_ad()
                        {
                            TamanhoExtent = rootRecord.Tamanho,
                            LocalizaçãoExtent = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 },
                            UsoImplementação = new byte[6]
                        }
                    });

            foreach (var Frecord in FolderRecords)
            {
                var folderFIDs = FolderFileIDs.First(x => x.Key.FullPath == Frecord.Key.FullPath).Value;

                File_Entries_A.Add(new FE()
                {
                    Name = Frecord.Key.Name,
                    FullPath = Frecord.Key.FullPath,
                    tamanhosetor = tamanhosetor,
                    TagICB = new Descritor.icbtag()
                    {
                        PreviousDirectEntryNumber = 0,
                        StrategyType = Descritor.icbtag.Strategy.FA5,
                        StrategyParameter = new byte[] { 0, 0 },
                        MaxEntryNumber = 1,
                        FileTyp = Descritor.icbtag.FileType.Directory,
                        ICBParentLocation = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 },
                        Regras = new Descritor.icbtag.Flag[] { Descritor.icbtag.Flag.File, Descritor.icbtag.Flag.NonRelocTable, Descritor.icbtag.Flag.Res1, Descritor.icbtag.Flag.Contiguous }
                    },
                    UID = 0xffffffff,
                    GID = 0xffffffff,
                    Permissions = 0x14a5,
                    Acesso = new Descritor.time_stamp() { UTCspecs = 0x1f4c, datetime = Frecord.Key.Gravação },
                    Modificação = new Descritor.time_stamp() { UTCspecs = 0x1f4c, datetime = Frecord.Key.Gravação },
                    Atributo = new Descritor.time_stamp() { UTCspecs = 0x1f4c, datetime = Frecord.Key.Gravação },
                    LinkedFileCount = 1,
                    RecordFormat = 0,
                    RecordAttrs = 0,
                    RecordSize = 0,
                    InfoSize = (uint)folderFIDs.GetFiles().Length,
                    LogicalBlocksWrited = 1,
                    Checkpoint = 1,
                    ICBAttrExtendido = new Descritor.long_ad() { LocalizaçãoExtent = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 }, TamanhoExtent = 0, UsoImplementação = new byte[6] },
                    IDImplementação = MakeImplID(),
                    UniqueID = 0,
                    TamanhoAttrExtendidos = 0x84,
                    TamanhoDescritoresAloc = 8,
                    AtributosExtendidos = MakeDefaultEA(),
                    DescritoresAlocação = new Descritor.long_ad()
                    {
                        TamanhoExtent = (uint)folderFIDs.GetFiles().Length,
                        LocalizaçãoExtent = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 },
                        UsoImplementação = new byte[6]
                    }
                });

                foreach (var record in Frecord.Value)
                    if (!record.Flags.Contains(Arquivo.Regras.SubDiretório))
                        File_Entries_B.Add(new FE()
                        {
                            Name = record.Name,
                            FullPath = record.FullPath,
                            tamanhosetor = tamanhosetor,
                            TagICB = new Descritor.icbtag()
                            {
                                PreviousDirectEntryNumber = 0,
                                StrategyType = Descritor.icbtag.Strategy.FA5,
                                StrategyParameter = new byte[] { 0, 0 },
                                MaxEntryNumber = 1,
                                FileTyp = Descritor.icbtag.FileType.RandomAccessBytes,
                                ICBParentLocation = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 },
                                Regras = new Descritor.icbtag.Flag[] { Descritor.icbtag.Flag.File, Descritor.icbtag.Flag.NonRelocTable }
                            },
                            UID = 0xffffffff,
                            GID = 0xffffffff,
                            Permissions = 0x14a5,
                            Acesso = new Descritor.time_stamp() { UTCspecs = 0x1f4c, datetime = record.Gravação },
                            Modificação = new Descritor.time_stamp() { UTCspecs = 0x1f4c, datetime = record.Gravação },
                            Atributo = new Descritor.time_stamp() { UTCspecs = 0x1f4c, datetime = record.Gravação },
                            LinkedFileCount = 1,
                            RecordFormat = 0,
                            RecordAttrs = 0,
                            RecordSize = 0,
                            InfoSize = record.Tamanho,
                            LogicalBlocksWrited = 1,
                            Checkpoint = 1,
                            ICBAttrExtendido = new Descritor.long_ad() { LocalizaçãoExtent = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 }, TamanhoExtent = 0, UsoImplementação = new byte[6] },
                            IDImplementação = MakeImplID(),
                            UniqueID = 0,
                            TamanhoAttrExtendidos = 0x84,
                            TamanhoDescritoresAloc = 8,
                            AtributosExtendidos = MakeDefaultEA(),
                            DescritoresAlocação = new Descritor.long_ad()
                            {
                                TamanhoExtent = record.Tamanho,
                                LocalizaçãoExtent = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 },
                                UsoImplementação = new byte[6]
                            }
                        });
            }

            File_Entries.AddRange(File_Entries_A);
            File_Entries.AddRange(File_Entries_B);

            #endregion

            #region Calc FI/FE LBAs

            var saveFI = new List<byte>();
            saveFI.AddRange(rootFileIDs.GetFiles());
            while (saveFI.Count % tamanhosetor != 0 || saveFI.Count < tamanhosetor)
                saveFI.Add(0);

            uint fiLBA = 2;
            rootFileIDs[0].ICB.LocalizaçãoExtent.LogicalBlockNumber = fiLBA;
            fiLBA += (uint)(saveFI.Count / tamanhosetor);

            foreach (var fi in FolderFileIDs)
            {
                fi.Key.ICB.LocalizaçãoExtent.LogicalBlockNumber = fiLBA;
                var fiIN = new List<byte>();
                fiIN.AddRange(fi.Value.GetFiles());
                while (fiIN.Count % tamanhosetor != 0 || fiIN.Count < tamanhosetor)
                    fiIN.Add(0);
                fiLBA += (uint)(fiIN.Count / tamanhosetor);
                saveFI.AddRange(fiIN.ToArray());
            }

            FELBA = PartitionLBA + 2 + (saveFI.Count / tamanhosetor);
            StreamStartLBA = File_Entries.Count + FELBA;

            #endregion

        } // end if (UDF)

        #endregion

        // Calcula tamanho total dos arquivos
        long totalFileDataSize = 0;
        foreach (string fp in Directory.EnumerateFiles(isofolder, "*.*", SearchOption.AllDirectories))
        {
            long flen = new FileInfo(fp).Length;
            totalFileDataSize += flen;
            if (flen % tamanhosetor != 0)
                totalFileDataSize += tamanhosetor - (flen % tamanhosetor);
        }

        long metadataSize = UDF
            ? (long)(FELBA + File_Entries.Count) * tamanhosetor
            : (long)(FileEntries_LBA + iso9660RecordSectors) * tamanhosetor;

        long totalISOSize = metadataSize + totalFileDataSize;

        log.AppendLine("ISO MOUNTING STARTED...\r\n");

        outISO.SetLength(totalISOSize);
        outISO.Position = 0;
        outISO.Write(new byte[16 * tamanhosetor], 0, 16 * tamanhosetor);
        outISO.Position = metadataSize;

        var orderedFiles = Directory.EnumerateFiles(isofolder, "*.*", SearchOption.AllDirectories)
            .OrderBy(x => x).ToList();

        if (!string.IsNullOrEmpty(executable))
        {
            var exeFiles = orderedFiles.Where(x => x.Contains(executable)).ToList();
            var nonExeFiles = orderedFiles.Where(x => !x.Contains(executable)).ToList();
            orderedFiles = nonExeFiles.Concat(exeFiles).ToList();
        }

        foreach (string file_Path in orderedFiles)
        {
            var file_Stream = File.OpenRead(file_Path);
            int file_Length = (int)file_Stream.Length;

            for (int r = 0; r < rootRecords.Count; r++)
                if (rootRecords[r].FullPath == file_Path)
                {
                    rootRecords[r].LBA = (uint)(outISO.Position / tamanhosetor);
                    log.AppendLine($"File: {rootRecords[r].Name}, LBA: {rootRecords[r].LBA}");
                }

            foreach (var folder in FolderRecords)
                foreach (var file in folder.Value)
                    if (file.FullPath == file_Path)
                    {
                        file.LBA = (uint)(outISO.Position / tamanhosetor);
                        log.AppendLine($"File: {file.Name}, LBA: {file.LBA}");
                    }

            if (UDF)
                foreach (var File_Entry in File_Entries)
                    if (File_Entry.FullPath == file_Path)
                    {
                        File_Entry.DescritoresAlocação.LocalizaçãoExtent.LogicalBlockNumber = (uint)((outISO.Position / tamanhosetor) - PartitionLBA);
                        File_Entry.DescritoresAlocação.TamanhoExtent = (uint)file_Length;
                        File_Entry.InfoSize = (ulong)file_Length;
                    }

            file_Stream.CopyTo(outISO);
            while (outISO.Position % tamanhosetor != 0)
                outISO.WriteByte(0);
            file_Stream.Close();
        }

        if (UDF)
        {
            #region Write UDF structures

            for (int lba = FELBA - PartitionLBA, i = 0; i < File_Entries.Count; i++, lba++)
            {
                File_Entries[i].lba = lba;
                File_Entries[i].AtributosExtendidos.lba = lba;

                if (File_Entries[i].TagICB.FileTyp == Descritor.icbtag.FileType.Directory && File_Entries[i].FullPath != "ROOT")
                    File_Entries[i].DescritoresAlocação.LocalizaçãoExtent.LogicalBlockNumber =
                        FolderFileIDs.First(f => f.Key.FullPath == File_Entries[i].FullPath).Key.ICB.LocalizaçãoExtent.LogicalBlockNumber;
                else if (File_Entries[i].TagICB.FileTyp == Descritor.icbtag.FileType.Directory && File_Entries[i].FullPath == "ROOT")
                    File_Entries[i].DescritoresAlocação.LocalizaçãoExtent.LogicalBlockNumber = 2;

                foreach (var rootID in rootFileIDs)
                    if (rootID.FullPath == File_Entries[i].FullPath)
                        rootID.ICB.LocalizaçãoExtent.LogicalBlockNumber = (uint)lba;
                    else if (rootID.FullPath == "--CYCLIC_DIRECTORY--")
                        rootID.ICB.LocalizaçãoExtent.LogicalBlockNumber = (uint)File_Entries.First(e => e.FullPath == "ROOT").lba;

                foreach (var folderID in FolderFileIDs)
                    foreach (var fileID in folderID.Value)
                        if (fileID.FullPath == File_Entries[i].FullPath)
                            fileID.ICB.LocalizaçãoExtent.LogicalBlockNumber = (uint)lba;
                        else if (fileID.FullPath == "--CYCLIC_DIRECTORY--")
                            fileID.ICB.LocalizaçãoExtent.LogicalBlockNumber = (uint)File_Entries.First(e => e.FullPath == folderID.Key.FullPath).lba;
            }

            var saveFIwrite = new List<byte>();
            saveFIwrite.AddRange(rootFileIDs.GetFiles());
            while (saveFIwrite.Count % tamanhosetor != 0 || saveFIwrite.Count < tamanhosetor)
                saveFIwrite.Add(0);

            foreach (var fi in FolderFileIDs)
            {
                var folderFi = new List<byte>();
                folderFi.AddRange(fi.Value.GetFiles());
                while (folderFi.Count % tamanhosetor != 0 || folderFi.Count < tamanhosetor)
                    folderFi.Add(0);
                saveFIwrite.AddRange(folderFi.ToArray());
            }

            outISO.Position = (PartitionLBA + 2) * tamanhosetor;
            outISO.Write(saveFIwrite.ToArray(), 0, saveFIwrite.Count);

            var saveFE = new List<byte>();
            foreach (var fe in File_Entries)
                saveFE.AddRange(fe.SectorToBin());
            outISO.Write(saveFE.ToArray(), 0, saveFE.Count);

            var VolID = new List<byte>();
            VolID.Add(8);
            byte[] text = new byte[0x1e];
            string name = VolumeName;
            if (string.IsNullOrEmpty(name) || name.Length > 29) name = " ";
            Array.Copy(Encoding.Default.GetBytes(name), text, name.Length);
            VolID.AddRange(text);
            VolID.Add((byte)(name.Length + 1));

            PVD pVD = new PVD()
            {
                tamanhosetor = tamanhosetor,
                lba = 32,
                DescritorPrimaryVolumeSequencialNumber = 0,
                DescritorVolumeSequencialNumber = 0,
                VolumeID = Encoding.Default.GetString(VolID.ToArray()),
                VolumeSequenceNumber = 1,
                MaxVolumeSequenceNumber = 1,
                InterchangeLevel = 2,
                MaxInterchangeLevel = 2,
                CharacterSetList = 1,
                MaxCharacterSetList = 1,
                VolumeSetID = new Descritor.VolSetID()
                {
                    First16Bytes = new byte[0x10] { 8, 0, 0, 0, 0, 0, 0, 0, 0x33, 0x53, 0x43, 0x45, 0x49, 0x20, 0x20, 0x20 },
                    VolumeID = "                                 ",
                    LastCode = 0x31
                },
                DescritorCharSet = new Descritor.CharSet() { Flag = Descritor.CharSet.TipoSet.CS0, Info = "OSTA Compressed Unicode" },
                ExplanatoryCharSet = new Descritor.CharSet() { Flag = Descritor.CharSet.TipoSet.CS0, Info = "OSTA Compressed Unicode" },
                VolumeAbstrato = new Descritor.extent_ad() { ExtentSize = 0, LBAExtent = 0 },
                VolumeCopyrightNotice = new Descritor.extent_ad() { ExtentSize = 0, LBAExtent = 0 },
                ID_Aplicativo = new Descritor.regid() { Regras = new Descritor.regid.Flag[] { }, ID = "PLAYSTATION            ", IDSufixo = new byte[8] },
                DataHoraGravação = new Descritor.time_stamp() { UTCspecs = 0x1f4c, datetime = DateTime.Now },
                ID_Implementation = new Descritor.regid() { Regras = new Descritor.regid.Flag[] { }, ID = "DVD-ROM GENERATOR", IDSufixo = new byte[8] },
                UsoImplementação = new byte[0x40],
                LBASequenciaDescritorVolumePredecessor = 0
            };
            outISO.Position = (int)(32 * tamanhosetor);
            outISO.Write(pVD.SectorToBin(), 0, 0x800);

            byte[] pathtableloc = BitConverter.GetBytes((UInt64)(PathTable_LBA + (Path_Tables.GetTables(tamanhosetor).Length / tamanhosetor)));
            IUVD iUVD = new IUVD()
            {
                tamanhosetor = tamanhosetor,
                lba = 33,
                DescritorVolumeSequencialNumber = 1,
                ImplementationID = new Descritor.regid() { Regras = new Descritor.regid.Flag[] { }, ID = "*UDF LV Info", IDSufixo = pathtableloc },
                UsoImplementação = new Descritor.LVInformation()
                {
                    LVICharset = new Descritor.CharSet() { Info = "OSTA Compressed Unicode", Flag = Descritor.CharSet.TipoSet.CS0 },
                    LVIIdentifier = name,
                    ImplementationID = new Descritor.regid() { Regras = new Descritor.regid.Flag[] { }, ID = "DVD-ROM GENERATOR", IDSufixo = new byte[8] },
                    ImplementationUse = new byte[0x80]
                }
            };
            outISO.Write(iUVD.SectorToBin(), 0, 0x800);

            PD partition = new PD()
            {
                tamanhosetor = tamanhosetor,
                lba = 34,
                DescritorVolumeSequencialNumber = 2,
                NúmeroPartições = 0,
                ConteúdodePartição = new Descritor.regid() { ID = "+NSR02", Regras = new Descritor.regid.Flag[] { Descritor.regid.Flag.Protegido }, IDSufixo = new byte[8] },
                UsoPartição = new byte[0x80],
                TipoAcesso = 1,
                LBAPartição = (uint)PartitionLBA,
                TamanhoPartiçãoBlocks = (uint)((outISO.Length / tamanhosetor) - PartitionLBA),
                IdImplementação = new Descritor.regid() { ID = "DVD-ROM GENERATOR", Regras = new Descritor.regid.Flag[] { }, IDSufixo = new byte[8] },
                UsoImplementação = new byte[0x80]
            };
            outISO.Write(partition.SectorToBin(), 0, 0x800);

            LV logicVOL = new LV()
            {
                tamanhosetor = tamanhosetor,
                lba = 35,
                DescritorVolumeSequencialNumber = 3,
                DescCharSet = new Descritor.CharSet() { Flag = Descritor.CharSet.TipoSet.CS0, Info = "OSTA Compressed Unicode" },
                LVIdentifier = name,
                VolBlockSize = (uint)tamanhosetor,
                DomainIdentifier = new Descritor.regid() { Regras = new Descritor.regid.Flag[] { }, ID = "*OSTA UDF Compliant", IDSufixo = new byte[8] { 2, 1, 3, 0, 0, 0, 0, 0 } },
                ContentUse = new byte[0x10] { 0, 0x10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                PartitionMapTableSize = 6,
                MapNumber = 1,
                ImplementationIdentifier = new Descritor.regid() { Regras = new Descritor.regid.Flag[] { }, ID = "DVD-ROM GENERATOR", IDSufixo = new byte[8] },
                UsoImplementação = new byte[0x80],
                IntegritySequenceExtent = new Descritor.extent_ad() { LBAExtent = 64, ExtentSize = 0x1000 },
                PartitionMaps = new Descritor.PartitionMap[] { new Descritor.PartitionMap() { Type = 1, MapLength = 6, VolumeSequencialNumber = 1, PartitionNumber = 0 } }
            };
            outISO.Write(logicVOL.SectorToBin(), 0, 0x800);

            USD usd = new USD() { tamanhosetor = tamanhosetor, lba = 36, DescritorVolumeSequencialNumber = 4, AllocDescripNumber = 0, DescritoresAlocação = new Descritor.extent_ad[] { } };
            outISO.Write(usd.SectorToBin(), 0, 0x800);

            TD TerminatorDescript_p = new TD() { tamanhosetor = tamanhosetor, lba = 37 };
            outISO.Write(TerminatorDescript_p.SectorToBin(), 0, 0x800);

            pVD.lba = 48; outISO.Position = (int)(48 * tamanhosetor);
            outISO.Write(pVD.SectorToBin(), 0, 0x800);
            iUVD.lba = 49; outISO.Write(iUVD.SectorToBin(), 0, 0x800);
            partition.lba = 50; outISO.Write(partition.SectorToBin(), 0, 0x800);
            logicVOL.lba = 51; outISO.Write(logicVOL.SectorToBin(), 0, 0x800);
            usd.lba = 52; outISO.Write(usd.SectorToBin(), 0, 0x800);
            TerminatorDescript_p.lba = 53; outISO.Write(TerminatorDescript_p.SectorToBin(), 0, 0x800);

            LVI lvi = new LVI()
            {
                tamanhosetor = tamanhosetor,
                lba = 64,
                DataHoraGravação = new Descritor.time_stamp() { datetime = DateTime.Now, UTCspecs = 0x1f4c },
                Tipo = 1,
                Próximo = new Descritor.extent_ad() { ExtentSize = 0, LBAExtent = 0 },
                NúmeroPartições = 1,
                TamanhoUsoImplementação = 0x30,
                TabelaEspaçoLivre = new uint[1] { 0 },
                TabelaTamanhos = new uint[1] { (uint)((outISO.Length / tamanhosetor) - PartitionLBA) },
                UsoImplementação = new LVI.ImplementationUse()
                {
                    ID = new Descritor.regid() { Regras = new Descritor.regid.Flag[] { }, ID = "DVD-ROM GENERATOR", IDSufixo = new byte[8] },
                    FileNumber = (uint)new DirectoryInfo(isofolder).GetFiles("*.*", SearchOption.AllDirectories).Length,
                    DirectoryNumber = (uint)(new DirectoryInfo(isofolder).GetDirectories("*.*", SearchOption.AllDirectories).Length + 1),
                    MinUDFReadRev = 258,
                    MaxUDFWriteRev = 258,
                    MinUDFWriteRev = 258,
                    UsoImplementação = new byte[2]
                }
            };
            outISO.Position = (int)(64 * tamanhosetor);
            outISO.Write(lvi.SectorToBin(), 0, 0x800);
            TD TerminatorDescript_i = new TD() { tamanhosetor = tamanhosetor, lba = 65 };
            outISO.Write(TerminatorDescript_i.SectorToBin(), 0, 0x800);

            AVDP aVDP = new AVDP()
            {
                tamanhosetor = tamanhosetor,
                lba = 256,
                VolumePrincipal = new AVDP.Extensor() { Tamanho_Dados = 0x8000, LBA_Dados = 32 },
                VolumeReserva = new AVDP.Extensor() { Tamanho_Dados = 0x8000, LBA_Dados = 48 }
            };
            outISO.Position = (int)(256 * tamanhosetor);
            byte[] avdpBIN = aVDP.SectorToBin();
            outISO.Write(avdpBIN, 0, avdpBIN.Length);
            aVDP.lba = (int)(outISO.Length / tamanhosetor);
            avdpBIN = aVDP.SectorToBin();
            outISO.Position = outISO.Length;
            outISO.Write(avdpBIN, 0, avdpBIN.Length);

            var fsID = new List<byte>();
            fsID.Add(8);
            fsID.AddRange(Encoding.Default.GetBytes("PLAYSTATION2 DVD-ROM FILE SET"));
            fsID.Add(0); fsID.Add(0x1E);

            FSD FileSetDescriptor = new FSD()
            {
                tamanhosetor = tamanhosetor,
                lba = 0,
                DataHoraGravação = new Descritor.time_stamp() { datetime = DateTime.Now, UTCspecs = 0x1f4c },
                InterchangeLevel = 3,
                MaxInterchangeLevel = 3,
                CharacterSetList = 1,
                MaxCharacterSetList = 1,
                FileSetNumber = 1,
                FileSetDescriptorNumber = 1,
                LogicalVolumeIdentifierCharSet = new Descritor.CharSet() { Flag = Descritor.CharSet.TipoSet.CS0, Info = "OSTA Compressed Unicode" },
                LogicalVolumeIdentifier = "",
                FileSetCharSet = new Descritor.CharSet() { Flag = Descritor.CharSet.TipoSet.CS0, Info = "OSTA Compressed Unicode" },
                FileSetIdentifier = Encoding.Default.GetString(fsID.ToArray()),
                CopyrightFileIdentifier = "",
                AbstractFileIdentifier = "",
                RootDirectoryICB = new Descritor.long_ad()
                {
                    TamanhoExtent = 0x13c,
                    LocalizaçãoExtent = new Descritor.lb_addr() { LogicalBlockNumber = (uint)(FELBA - PartitionLBA), PartitionReferenceNumber = 0 },
                    UsoImplementação = new byte[6]
                },
                DomainIdentifier = new Descritor.regid() { Regras = new Descritor.regid.Flag[] { }, ID = "*OSTA UDF Compliant", IDSufixo = new byte[8] { 2, 1, 3, 0, 0, 0, 0, 0 } },
                NextExtent = new Descritor.long_ad() { TamanhoExtent = 0, LocalizaçãoExtent = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 }, UsoImplementação = new byte[6] },
                StreamDirectoryICB = new Descritor.long_ad() { TamanhoExtent = 0, LocalizaçãoExtent = new Descritor.lb_addr() { LogicalBlockNumber = 0, PartitionReferenceNumber = 0 }, UsoImplementação = new byte[6] }
            };

            TD TerminatorDescript = new TD() { tamanhosetor = tamanhosetor, lba = (int)(PartitionLBA + 1) };
            outISO.Position = (int)(PartitionLBA * tamanhosetor);
            outISO.Write(FileSetDescriptor.SectorToBin(), 0, FileSetDescriptor.SectorToBin().Length);
            outISO.Flush();
            outISO.Write(TerminatorDescript.SectorToBin(), 0, TerminatorDescript.SectorToBin().Length);

            #endregion
        } // end if (UDF) write

        #region ISO9660

        #region File Records Part 1

        // ── PASSO 1: pré-calcula LBAs de todos os diretórios ANTES de qualquer Insert ──
        var dirLBAMap = new Dictionary<string, uint>();
        if (!UDF)
        {
            uint tempLBA = (uint)FileEntries_LBA;

            // root: simula com "." e ".." incluídos
            var tempRootList = new List<Arquivo>();
            tempRootList.Add(new Arquivo() { LBA = 0, Tamanho = 0, Gravação = DateTime.Now, Flags = new Arquivo.Regras[] { Arquivo.Regras.SubDiretório }, VolumeSequencialNumber = 1, Name = "", FullPath = "ROOT" });
            tempRootList.Add(new Arquivo() { LBA = 0, Tamanho = 0, Gravação = DateTime.Now, Flags = new Arquivo.Regras[] { Arquivo.Regras.SubDiretório }, VolumeSequencialNumber = 1, Name = "", FullPath = "PARENT" });
            tempRootList.AddRange(rootRecords);
            uint tempRootSize = (uint)tempRootList.GetFiles().Length;
            dirLBAMap["ROOT"] = tempLBA;
            while (tempRootSize % tamanhosetor != 0 || tempRootSize < tamanhosetor)
                tempRootSize++;
            tempLBA += tempRootSize / (uint)tamanhosetor;

            // subpastas: simula com "." e ".." incluídos
            foreach (var folRec in FolderRecords)
            {
                var tempFolList = new List<Arquivo>();
                tempFolList.Add(new Arquivo() { LBA = 0, Tamanho = 0, Gravação = DateTime.Now, Flags = new Arquivo.Regras[] { Arquivo.Regras.SubDiretório }, VolumeSequencialNumber = 1, Name = "", FullPath = folRec.Key.FullPath });
                tempFolList.Add(new Arquivo() { LBA = 0, Tamanho = 0, Gravação = DateTime.Now, Flags = new Arquivo.Regras[] { Arquivo.Regras.SubDiretório }, VolumeSequencialNumber = 1, Name = "", FullPath = "PARENT" });
                tempFolList.AddRange(folRec.Value);
                uint tempFolSize = (uint)tempFolList.GetFiles().Length;
                dirLBAMap[folRec.Key.FullPath] = tempLBA;
                while (tempFolSize % tamanhosetor != 0 || tempFolSize < tamanhosetor)
                    tempFolSize++;
                tempLBA += tempFolSize / (uint)tamanhosetor;
            }
        }

        // ── PASSO 2: Insert dos "." e ".." do root e atribuição de LBAs ──
        uint RecordsLBA = (uint)FileEntries_LBA;
        uint RootRecordsSize = (uint)(rootRecords.GetFiles().Length + 0x60);

        rootRecords.Insert(0, new Arquivo() { LBA = 0, Tamanho = RootRecordsSize, Gravação = DateTime.Now, Flags = new Arquivo.Regras[] { }, VolumeSequencialNumber = 1, Name = "", FullPath = "ROOT" });
        rootRecords.Insert(1, new Arquivo() { LBA = 0, Tamanho = RootRecordsSize, Gravação = DateTime.Now, Flags = new Arquivo.Regras[] { }, VolumeSequencialNumber = 1, Name = "", FullPath = "PARENT" });

        foreach (var rootRec in rootRecords)
        {
            if (rootRec.FullPath == "ROOT" || rootRec.FullPath == "PARENT")
                rootRec.LBA = RecordsLBA;
            else if (UDF)
            {
                foreach (var fiEntryUDF in File_Entries)
                    if (fiEntryUDF.FullPath == rootRec.FullPath)
                        rootRec.LBA = (uint)(fiEntryUDF.DescritoresAlocação.LocalizaçãoExtent.LogicalBlockNumber + PartitionLBA);
            }
            else if (rootRec.Flags.Contains(Arquivo.Regras.SubDiretório))
                rootRec.LBA = dirLBAMap[rootRec.FullPath];
            // arquivos sem UDF: LBA já definido no loop de escrita
        }

        while (RootRecordsSize % tamanhosetor != 0 || RootRecordsSize < tamanhosetor)
            RootRecordsSize++;
        RecordsLBA += (uint)(RootRecordsSize / tamanhosetor);

        // ── PASSO 3: Insert dos "." e ".." de cada subpasta ──
        foreach (var folRec in FolderRecords)
        {
            uint folSize = (uint)(folRec.Value.GetFiles().Length + 0x60);

            // resolve LBAs de arquivos e subpastas dentro desta pasta
            foreach (var filRec in folRec.Value)
            {
                if (UDF)
                {
                    foreach (var fiEntryUDF in File_Entries)
                        if (fiEntryUDF.FullPath == filRec.FullPath)
                            filRec.LBA = (uint)(fiEntryUDF.DescritoresAlocação.LocalizaçãoExtent.LogicalBlockNumber + PartitionLBA);
                }
                else if (filRec.Flags.Contains(Arquivo.Regras.SubDiretório))
                    filRec.LBA = dirLBAMap[filRec.FullPath];
                // arquivos sem UDF: LBA já definido no loop de escrita
            }

            uint thisDirLBA = UDF ? RecordsLBA : dirLBAMap[folRec.Key.FullPath];

            folRec.Value.Insert(0, new Arquivo() { LBA = thisDirLBA, Tamanho = folSize, Gravação = DateTime.Now, Flags = new Arquivo.Regras[] { }, VolumeSequencialNumber = 1, Name = "", FullPath = folRec.Key.FullPath });
            folRec.Value.Insert(1, new Arquivo() { LBA = 0, Tamanho = 0, Gravação = DateTime.Now, Flags = new Arquivo.Regras[] { }, VolumeSequencialNumber = 1, Name = "", FullPath = "PARENT" });

            if (UDF)
            {
                while (folSize % tamanhosetor != 0 || folSize < tamanhosetor)
                    folSize++;
                RecordsLBA += folSize / (uint)tamanhosetor;
            }
        }

        #endregion

        #region Write Path_Tables
        outISO.Position = (int)(PathTable_LBA * tamanhosetor);
        Path_Tables[0].DirLBA = (uint)FileEntries_LBA;

        foreach (var recFol in FolderRecords)
        {
            var path = Path_Tables.Where(x => x.NomePasta == Path.GetFileName(recFol.Key.FullPath)).ToArray();
            int index = Path_Tables.IndexOf(path[0]);

            string parentFOLDER = new DirectoryInfo(recFol.Value[0].FullPath).Parent.FullName;
            if (parentFOLDER == isofolder)
            {
                recFol.Value[1].LBA = rootRecords[0].LBA;
                recFol.Value[1].Tamanho = (uint)rootRecords.GetFiles().Length;
            }
            else
            {
                var recEQ = FolderRecords.First(x => x.Value[0].FullPath == parentFOLDER);
                recFol.Value[1].LBA = recEQ.Value[0].LBA;
                recFol.Value[1].Tamanho = (uint)recEQ.Value.GetFiles().Length;
            }

            if (index != -1)
                Path_Tables[index].DirLBA = (uint)recFol.Value[0].LBA;
        }

        foreach (var rootR in rootRecords)
            if (rootR.Flags.Contains(Arquivo.Regras.SubDiretório))
            {
                var recFind = FolderRecords.First(x => x.Value[0].FullPath == rootR.FullPath);
                rootR.LBA = recFind.Value[0].LBA;
                rootR.Tamanho = recFind.Value[0].Tamanho;
            }

        foreach (var folRec in FolderRecords)
            foreach (var recF in folRec.Value)
                if (recF.Flags.Contains(Arquivo.Regras.SubDiretório))
                {
                    var recFind = FolderRecords.First(x => x.Value[0].FullPath == recF.FullPath);
                    recF.LBA = recFind.Value[0].LBA;
                    recF.Tamanho = recFind.Value[0].Tamanho;
                }

        outISO.Write(Path_Tables.GetTables(tamanhosetor), 0, Path_Tables.GetTables(tamanhosetor).Length);
        outISO.Write(Path_Tables.GetTables(tamanhosetor), 0, Path_Tables.GetTables(tamanhosetor).Length);
        outISO.Write(Path_Tables.GetTables(tamanhosetor, true), 0, Path_Tables.GetTables(tamanhosetor, true).Length);
        outISO.Write(Path_Tables.GetTables(tamanhosetor, true), 0, Path_Tables.GetTables(tamanhosetor, true).Length);
        #endregion

        #region File Records Part Final (Write)
        outISO.Position = (int)(FileEntries_LBA * tamanhosetor);

        var saveRecords = new List<byte>();
        rootRecords[0].Flags = new Arquivo.Regras[] { Arquivo.Regras.SubDiretório };
        rootRecords[1].Flags = new Arquivo.Regras[] { Arquivo.Regras.SubDiretório };
        saveRecords.AddRange(rootRecords.GetFiles());
        while (saveRecords.Count % tamanhosetor != 0 || saveRecords.Count < tamanhosetor)
            saveRecords.Add(0);

        foreach (var record in FolderRecords)
        {
            record.Value[0].Flags = new Arquivo.Regras[] { Arquivo.Regras.SubDiretório };
            record.Value[1].Flags = new Arquivo.Regras[] { Arquivo.Regras.SubDiretório };
            saveRecords.AddRange(record.Value.GetFiles());
            while (saveRecords.Count % tamanhosetor != 0 || saveRecords.Count < tamanhosetor)
                saveRecords.Add(0);
        }
        outISO.Write(saveRecords.ToArray(), 0, saveRecords.Count);
        #endregion

        // Primary Volume Descriptor ISO9660 (LBA 16)
        outISO.Position = (int)(16 * tamanhosetor);
        outISO.Write(Volume_Primário.GetPrimaryVolume("PLAYSTATION", VolumeName,
            (uint)(outISO.Length / tamanhosetor), (uint)tamanhosetor, 1,
            (uint)Path_Tables.GetTables(tamanhosetor, false, true).Length,
            (uint)PathTable_LBA, (uint)PathTableBE_LBA,
            (uint)FileEntries_LBA, (uint)rootRecords.GetFiles().Length,
            VolumeID, Author, Data, AplicationName, CopyrightName, Resumo, Bibliographic,
            DateTimeOffset.Now,
            (uint)(PathTable_LBA + (Path_Tables.GetTables(tamanhosetor).Length / tamanhosetor)),
            (uint)(PathTableBE_LBA + (Path_Tables.GetTables(tamanhosetor).Length / tamanhosetor))), 0, 0x800);

        outISO.Write(Setor.GetStringEmptySector(0xFF, "CD001", tamanhosetor), 0, 0x800);

        if (UDF)
        {
            outISO.Write(Setor.GetStringEmptySector(0, "BEA01", tamanhosetor), 0, 0x800);
            outISO.Write(Setor.GetStringEmptySector(0, "NSR02", tamanhosetor), 0, 0x800);
            outISO.Write(Setor.GetStringEmptySector(0, "TEA01", tamanhosetor), 0, 0x800);
        }

        #endregion

        log.AppendLine("\n=== DIR LBA MAP ===");
        foreach (var kv in dirLBAMap)
            log.AppendLine($"  {kv.Key} -> LBA {kv.Value}");

        log.AppendLine("\n=== ROOT RECORDS ===");
        foreach (var r in rootRecords)
            log.AppendLine($"  [{r.FullPath}] LBA={r.LBA} Tamanho={r.Tamanho} Flags={string.Join(",", r.Flags)}");

        log.AppendLine("\n=== FOLDER RECORDS ===");
        foreach (var f in FolderRecords)
        {
            log.AppendLine($"  PASTA: {f.Key.FullPath} LBA={f.Key.LBA}");
            foreach (var v in f.Value)
                log.AppendLine($"    [{v.FullPath}] LBA={v.LBA} Tamanho={v.Tamanho} Flags={string.Join(",", v.Flags)}");
        }

        log.AppendLine("\n=== PATH TABLES ===");
        foreach (var pt in Path_Tables)
            log.AppendLine($"  [{pt.NomePasta}] DirLBA={pt.DirLBA} Parent={pt.ParenteDirNumber}");

        log.AppendLine($"\nmetadataSize: {metadataSize} ({metadataSize / tamanhosetor} setores)");
        log.AppendLine($"totalISOSize: {totalISOSize}");

        File.WriteAllText(outputpath + ".log.txt", log.ToString());

        outISO.Close();

        log.AppendLine($"\nPath_Table LBA: {PathTable_LBA}");
        log.AppendLine($"Path_Table BigEndian LBA: {PathTableBE_LBA}");
        log.AppendLine($"FileEntries LBA: {FileEntries_LBA}");
        log.AppendLine($"Records Sectors: {sectorCountRecords}");
        log.AppendLine($"[UDF]Partition LBA: {PartitionLBA}");
        log.AppendLine($"[UDF]Partition File EntriesLBA: {FELBA}");
        log.AppendLine($"Stream LBA: {StreamStartLBA}");

        return true;
    }

    public void PatchISO(string pathmodfiles, string outisopath,
        string customVol = "", string Team = "", string temp = "",
        bool android = false, string deleteFiles = "")
    {
        string[] pathFiles = Directory.GetFiles(pathmodfiles, "*.*",
    SearchOption.AllDirectories).Select(x => x.Split(new string[] { pathmodfiles }, StringSplitOptions.RemoveEmptyEntries)[0]).ToArray();

        #region Extract ISO tempfolder
        if (temp == "")
            temp = Path.GetTempPath();
        string pathiso = temp + "ISO";

        if (Directory.Exists(pathiso))
            Directory.Delete(pathiso, true);

        CriarPasta(pathiso, true);
        pathiso += !android ? @"\" : @"/";

        //Delete Files
        if (deleteFiles != "")
        {
            var listDel = new List<string>();

            foreach (string file in deleteFiles.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                listDel.Add((!android ? RegulateFPath(file) : RegulateFPath(file).Replace(@"\", @"/")));
            }
            ExtrairNaPasta(pathiso, pathFiles, android, listDel.ToArray());//Extrair na pasta temp
        }
        else
        {
            ExtrairNaPasta(pathiso, pathFiles, android);//Extrair na pasta temp
        }

        #endregion

        #region Copiar Modificados para ISO
        foreach (var filetomove in Directory.GetFiles(pathmodfiles, "*.*",
    SearchOption.AllDirectories))
        {
            string dirsFILE = filetomove.Split(new string[] { pathmodfiles }, StringSplitOptions.RemoveEmptyEntries)[0];

            if (File.Exists(pathiso + dirsFILE))
                File.Delete(pathiso + dirsFILE);
            File.Move(filetomove, pathiso + dirsFILE);
        }
        #endregion
        Directory.Delete(pathmodfiles, true);

        #region Criar nova ISO
        Volume_Primário prim = Setores[16] as Volume_Primário;
        if (Team.Length > 29)
            Team = " ";
        if (customVol.Length > 29)
            customVol = " ";
        BuildISO(pathiso.Substring(0, pathiso.Length - 1).ToString(), outisopath, customVol != "" ? customVol : prim.VolumeID, Team != "" ? Team : prim.Autor, "RAIDENPATCHER", customVol != "" ? customVol : prim.VolumeID, prim.AppID, prim.Copyright,
                            "", "", false, true, prim.tamanhosetor, GetFirst16Sectors()); ;
        #endregion

        //Delete work folders
        Directory.Delete(pathiso, true);

    }

    public void SetArchiveEntries9660(Path_Table[] path_Table, uint pathlba, Arquivo.EntradaArquivo[] Infos, Stream ISO)
    {
        foreach (var table in path_Table)
            foreach (var inf in Infos)
                if (!table.Conteúdo.Any(x => x.Name == inf.Nome) &&
                    Path.GetDirectoryName(inf.NomecomPasta).Contains(table.NomePasta))
                {
                    var arquivos = table.Conteúdo.ToList();
                    arquivos.Add(new Arquivo(Arquivo.GetRecordData(inf.Nome, (uint)inf.LBA, (uint)inf.Tamanho, inf.TimeInfo,
                        new Arquivo.Regras[] { }), 0));
                    table.Conteúdo = arquivos.ToArray();

                }

        //HERE IS THE PROBLEM, IS JUST REWRITING EXISTANT DATA
        foreach (var table in path_Table)
            foreach (var item in table.Conteúdo)
                foreach (var info in Infos)
                    if (item.Name == info.Nome)
                    {
                        ISO.WriteBytes(new MemoryStream(((uint)(info.LBA)).ToLEBE(32)), item.OffsetinSector + (Tamanho_Setor * table.DirLBA) + 2, 0, 8);
                        ISO.WriteBytes(new MemoryStream(((uint)(info.Tamanho)).ToLEBE(32)), item.OffsetinSector + (Tamanho_Setor * table.DirLBA) + 10, 0, 8);
                        ISO.WriteBytes(new MemoryStream(info.TimeInfo.GetDateTimeData()), item.OffsetinSector + (Tamanho_Setor * table.DirLBA) + 0x12, 0, 7);
                    }
    }
    public string GetFilePath(string name, string root)
    {
        foreach (var file in Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories))
        {
            if (Path.GetFileName(file) + ";1" == name)
                return file;
        }
        return "";
    }
    public int GetFileStartLBA()
    {
        var integers = new List<int>();
        foreach (var identifierList in UDFSession.Principal.Partição.FileSet.FileIdentifiers)
        {
            foreach (var identifier in identifierList)
            {
                integers.Add(identifier.FileEntry.lba);
            }
        }

        return integers.Max();
    }
    public void CriarPasta(string path, bool hidden = false)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        if (hidden)
        {
            var info = new DirectoryInfo(path);
            info.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

        }
    }
    public void ExtrairNaPasta(string savepath, string[] checkpath = null, bool android = false, string[] deleteList = null)
    {
        string path = savepath;// + @"\" + Name;
        //CriarPasta(path);
        //path += @"\";

        var dirs = (Setores[16] as Volume_Primário).path_Tables;

        int d = 0;
        foreach (var pasta in dirs)
        {
            for (int i = 2; i < pasta.Conteúdo.Length; i++)
            {
                string pastap = path;
                string name = pasta.PathDirectoryWithParents(dirs);
                if (name != "\0")
                {
                    if (android)
                        if (name.Length > 0)
                            name = name.Substring(1, name.Length - 1);
                    pastap += name;
                    CriarPasta(pastap);
                }
                pastap += !android ? @"\" : @"/";
                if (!pasta.Conteúdo[i].Flags.Contains(Arquivo.Regras.SubDiretório))
                {
                    string FileNam = pasta.Conteúdo[i].Name.Substring(0, pasta.Conteúdo[i].Name.Length - 2).ToString();
                    string FileInside = pastap + FileNam;
                    if (android)
                        FileInside = FileInside.Replace(@"\", @"/");
                    string Gname = (pasta.NomePasta != "\0" ? pasta.NomePasta + (android ? @"/" : @"\") : "") + FileNam;
                    if (checkpath != null &&
                        !checkpath.Contains(Gname) &&
                        deleteList != null && !deleteList.Contains(Gname))
                    {
                        if (File.Exists(FileInside))
                            File.Delete(FileInside);

                        var filewrite = new FileStream(FileInside, FileMode.CreateNew);
                        filewrite.WriteBytes(ISOfile, 0, pasta.Conteúdo[i].LBA * (uint)Tamanho_Setor, (int)pasta.Conteúdo[i].Tamanho);
                        filewrite.Close();

                        #region File Date/Time
                        File.SetLastWriteTime(FileInside,
                            pasta.Conteúdo[i].Gravação);

                        File.SetLastAccessTime(FileInside,
                            pasta.Conteúdo[i].Gravação);

                        File.SetCreationTime(FileInside,
                            pasta.Conteúdo[i].Gravação);
                        #endregion
                    }
                    //else
                    //{
                    //    if (File.Exists(FileInside))
                    //        File.Delete(FileInside);

                    //    var filewrite = new FileStream(FileInside, FileMode.CreateNew);
                    //    filewrite.WriteBytes(ISOfile, 0, pasta.Conteúdo[i].LBA * (uint)Tamanho_Setor, (int)pasta.Conteúdo[i].Tamanho);
                    //    filewrite.Close();

                    //    #region File Date/Time
                    //    File.SetLastWriteTime(FileInside,
                    //        pasta.Conteúdo[i].Gravação);

                    //    File.SetLastAccessTime(FileInside,
                    //        pasta.Conteúdo[i].Gravação);

                    //    File.SetCreationTime(FileInside,
                    //        pasta.Conteúdo[i].Gravação);
                    //    #endregion
                    //}
                }
            }
            d++;
        }
    }
}

