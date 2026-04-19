using CCSFileExplorerWV;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace UN5ModdingWorkshop
{
    public class GAME
    {
        public static bool isUN6;
        public static int memoryDif = 0;
        public static int Global_Pointer = 0x617EF0;
        public static List<int> charInvalid = new List<int> { 0, 8, 9, 20, 21, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 44, 45, 74, 88 };
        public static int charCount = 0;
        public static int charSelCount = 0;
        public static string caminhoELF;
        public static bool openedELF;
        public static AwakeningParameters awakeningParameters;
        public static MovesetParameters movesetParameters;
        public static GeneralParameters generalParameters;
        public static ulong eeAddress;
        public static int lastSelectedID = 0;
        public static string cvm_toolPath = @"CVM Tool\cvm_tool.exe";
        public static string cvm_hdrPath = @"CVM Tool\data.hdr";
        public static string cvm;
        public static string iso;
        public static string rofs;
        public static string gamePath;

        public static Task Extract()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK) return Task.Run(() => { });

            return Task.Run(() =>
            {
                Main.instance.Invoke(new Action(() =>
                {
                    Main.instance.lblProgress.Text = "Extracting ISO...";
                }));

                string gameFile = ofd.FileName;
                gamePath = Path.Combine(Path.GetDirectoryName(gameFile), "GAME");
                Directory.CreateDirectory(gamePath);

                // Extrai o conteúdo da ISO usando DiscUtils
                using (FileStream isoStream = File.OpenRead(gameFile))
                {
                    var cdReader = new DiscUtils.Iso9660.CDReader(isoStream, joliet: false);
                    ExtractDirectory(cdReader.Root, gamePath);
                }

                Main.instance.Invoke(new Action(() =>
                {
                    Main.instance.lblProgress.Text = "Extracting CVM data...";
                }));

                cvm = Path.Combine(gamePath, "DATA\\DATA.CVM");
                iso = Path.Combine(gamePath, "DATA\\data.iso");
                rofs = Path.Combine(gamePath, "DATA\\ROFS");

                // Divide o CVM com o cvm_tool
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = cvm_toolPath,
                    Arguments = $"split \"{cvm}\" \"{iso}\" \"{cvm_hdrPath}\" -p cc2fuku",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process process = Process.Start(psi);
                process.WaitForExit();

                // Extrai o data.iso usando DiscUtils também
                Directory.CreateDirectory(rofs);

                using (FileStream dataIsoStream = File.OpenRead(iso))
                {
                    var cdReader = new DiscUtils.Iso9660.CDReader(dataIsoStream, joliet: false);
                    ExtractDirectory(cdReader.Root, rofs);
                }

                File.Delete(cvm);
                File.Delete(iso);

                // Descompressão dos arquivos CCS
                foreach (string file in Directory.GetFiles(rofs, "*.ccs", SearchOption.AllDirectories))
                {
                    using (FileStream fs = File.OpenRead(file))
                    {
                        byte[] header = new byte[2];
                        fs.Read(header, 0, 2);
                        if (header[0] != 0x1F || header[1] != 0x8B)
                            continue;
                    }

                    using (MemoryStream ms = new MemoryStream())
                    using (GZipStream gzipStream = new GZipStream(
                        new MemoryStream(File.ReadAllBytes(file)), CompressionMode.Decompress))
                    {
                        gzipStream.CopyTo(ms);
                        File.WriteAllBytes(file, ms.ToArray());
                    }

                    Main.instance.Invoke(new Action(() =>
                    {
                        Main.instance.lblProgress.Text = $"Decompressing CVM files: {file}";
                    }));
                }

                Main.instance.Invoke(new Action(() =>
                {
                    Main.instance.lblProgress.Text = "";
                }));

                MakeHostFS();
                MessageBox.Show("Game successfully extracted!");
            });
        }
        // Extrai recursivamente todos os arquivos e pastas de um diretório DiscUtils
        private static void ExtractDirectory(DiscUtils.DiscDirectoryInfo dir, string destPath)
        {
            Directory.CreateDirectory(destPath);

            // Arquivos
            foreach (var file in dir.GetFiles())
            {
                // Remove o sufixo ";1" que o ISO9660 adiciona
                string cleanName = file.Name.Contains(";")
                    ? file.Name.Substring(0, file.Name.LastIndexOf(';'))
                    : file.Name;

                string destFile = Path.Combine(destPath, cleanName);

                using (Stream src = file.OpenRead())
                using (FileStream dst = File.Create(destFile))
                    src.CopyTo(dst);

                // Preserva data de modificação
                File.SetLastWriteTime(destFile, file.LastWriteTime);
                File.SetCreationTime(destFile, file.LastWriteTime);
            }

            // Subpastas recursivo
            foreach (var subDir in dir.GetDirectories())
            {
                string destSub = Path.Combine(destPath, subDir.Name);
                ExtractDirectory(subDir, destSub);
            }
        }
        public static async Task Build()
        {
            string sourceFolder = null;

            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() != DialogResult.OK) return;
                sourceFolder = fbd.SelectedPath;
            }

            await Task.Run(async () =>
            {
                cvm = Path.Combine(sourceFolder, @"DATA\DATA.CVM");
                iso = Path.Combine(sourceFolder, @"DATA\data.iso");
                rofs = Path.Combine(sourceFolder, @"DATA\ROFS");

                // ── 1. Comprime CCS e gera gzlist ──────────────────────────
                await MakeGzlist();

                // ── 2. Cria data.iso a partir do ROFS ──────────────────────
                Main.instance.Invoke(new Action(() =>
                    Main.instance.lblProgress.Text = "Criando DATA.ISO..."));

                await Task.Run(() =>
                {
                    ISO9660.BuildISO(
                        isofolder: rofs,
                        outputpath: iso,
                        VolumeID: "GAME",
                        Author: "",
                        Data: "",
                        VolumeName: "GAME",
                        AplicationName: "PLAYSTATION",
                        CopyrightName: "",
                        Resumo: "",
                        Bibliographic: "",
                        Hidden: false,
                        UDF: false,
                        tamanhosetor: 0x800,
                        first16Sectors: null,
                        executable: ""
                    );
                });

                // ── 3. Empacota data.iso no DATA.CVM ───────────────────────
                Main.instance.Invoke(new Action(() =>
                    Main.instance.lblProgress.Text = "Convertendo DATA.ISO para DATA.CVM..."));

                await RunProcessAsync(
                    cvm_toolPath,
                    $"mkcvm \"{cvm}\" \"{iso}\" \"{cvm_hdrPath}\" -p cc2fuku",
                    "Erro ao criar CVM"
                );

                // Limpa temporários do ROFS
                if (Directory.Exists(rofs)) Directory.Delete(rofs, true);
                if (File.Exists(iso)) File.Delete(iso);

                // ── 4. Cria a ISO final do jogo (sem UN5.ELF) ──────────────
                Main.instance.Invoke(new Action(() =>
                    Main.instance.lblProgress.Text = "Criando ISO final do jogo..."));

                // Arquivos a ignorar na ISO final
                var excludeFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "UN5.ELF"
        };

                string tempFolder = Path.Combine(sourceFolder, "_iso_temp");
                if (Directory.Exists(tempFolder)) Directory.Delete(tempFolder, true);

                await Task.Run(() =>
                {
                    foreach (string file in Directory.EnumerateFiles(
                        sourceFolder, "*.*", SearchOption.AllDirectories))
                    {
                        if (file.StartsWith(tempFolder, StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (excludeFiles.Contains(Path.GetFileName(file)))
                            continue;

                        if (file.Equals(iso, StringComparison.OrdinalIgnoreCase))
                            continue;

                        string relativePath = file.Substring(sourceFolder.Length).TrimStart('\\', '/');
                        string destFile = Path.Combine(tempFolder, relativePath);

                        Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                        File.Copy(file, destFile, overwrite: true);
                    }

                    string elfName = "";
                    string sysCNF = Path.Combine(tempFolder, "SYSTEM.CNF");
                    if (File.Exists(sysCNF))
                    {
                        foreach (string line in File.ReadAllLines(sysCNF))
                        {
                            if (line.Replace(" ", "").ToUpper().StartsWith("BOOT2="))
                            {
                                elfName = line.Split('=')[1].Trim()
                                    .Replace("cdrom0:\\", "")
                                    .Replace(";1", "")
                                    .Trim();
                                break;
                            }
                        }
                    }

                    string finalISO = Path.Combine(
                        Path.GetDirectoryName(sourceFolder),
                        Path.GetFileName(sourceFolder) + "_output.iso");

                    ISO9660.BuildISO(
                        isofolder: tempFolder,
                        outputpath: finalISO,
                        VolumeID: "GAME",
                        Author: "",
                        Data: "",
                        VolumeName: "GAME",
                        AplicationName: "PLAYSTATION",
                        CopyrightName: "",
                        Resumo: "",
                        Bibliographic: "",
                        Hidden: false,
                        UDF: true,
                        tamanhosetor: 0x800,
                        first16Sectors: null,
                        executable: elfName
                    );
                });

                if (Directory.Exists(tempFolder)) Directory.Delete(tempFolder, true);

                Main.instance.Invoke(new Action(() =>
                    Main.instance.lblProgress.Text = ""));

                MessageBox.Show("Build concluído!");
            });
        }

        // Método auxiliar que resolve o deadlock lendo stdout/stderr em paralelo
        private static async Task RunProcessAsync(string fileName, string arguments, string errorPrefix)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(psi))
            {
                if (process == null)
                    throw new Exception($"{errorPrefix}: falha ao iniciar o processo.");

                var stdout = new StringBuilder();
                var stderr = new StringBuilder();

                // Lê em paralelo via eventos — mais seguro que ReadToEndAsync
                process.OutputDataReceived += (s, e) => { if (e.Data != null) stdout.AppendLine(e.Data); };
                process.ErrorDataReceived += (s, e) => { if (e.Data != null) stderr.AppendLine(e.Data); };

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await Task.Run(() => process.WaitForExit());

                if (process.ExitCode != 0)
                    throw new Exception($"{errorPrefix} (exit {process.ExitCode}):\n{stderr}");
            }
        }

        public static Task MakeGzlist()
        {
            return Task.Run(() =>
            {
                List<string> output = new List<string>();
                output.Add("#\t\tname\t\t\tnum");

                List<string> folders = Directory.GetDirectories(rofs, "*", SearchOption.AllDirectories).ToList();
                folders.Sort();
                folders.Insert(0, rofs);

                List<int> foldersFilesCount = new List<int>();
                List<string> files = new List<string>();
                List<string> filesSize = new List<string>();
                List<string> filesGzip = new List<string>();

                foreach (string folder in folders)
                {
                    string[] currentFileList = Directory.GetFiles(folder);
                    int dirCount = Directory.GetDirectories(folder).Length;

                    foreach (string filePath in currentFileList)
                    {
                        string fileName = Path.GetFileName(filePath).ToLower();
                        if (Path.GetExtension(fileName) == ".ccs")
                        {
                            string fileCVMPath = filePath.Replace(rofs + "\\", "").ToLower();
                            Main.instance.Invoke(new Action(() =>
                            {
                                Main.instance.lblProgress.Text = "Reading: " + Path.GetFileName(filePath);
                            }));

                            string nameNoExt = Path.GetFileNameWithoutExtension(filePath).ToLower();

                            byte[] data = ReadAllBytesBuffered(filePath);
                            string originalSize = "0x" + data.Length.ToString("X8").ToLower();

                            data = FileHelper.zipArray(data, nameNoExt);
                            string gzipSize = "0x" + data.Length.ToString("X8").ToLower();

                            File.WriteAllBytes(filePath, data);

                            files.Add(fileCVMPath);
                            filesSize.Add(originalSize);
                            filesGzip.Add(gzipSize);
                        }
                    }

                    foldersFilesCount.Add(dirCount + currentFileList.Length);
                }
                foldersFilesCount[0] += 1;
                folders[0] = "root";
                for (int i = 1; i < folders.Count; i++)
                {
                    folders[i] = folders[i]
                        .Replace(rofs + "\\", "")
                        .Replace("\\", "/")
                        .ToLower() + "/";
                }

                for (int i = 0; i < folders.Count; i++)
                {
                    string nome = folders[i].PadRight(16);
                    output.Add("\t\t" + nome + "\t" + foldersFilesCount[i]);
                }

                output.Add("\t\tbinEnd");
                output.Add("#\t\tname\t\t\tsize\t\tgzip");
                output.Add("\tdata\t0x0");

                for (int i = 0; i < files.Count; i++)
                {
                    output.Add($"\t\t{files[i]} \t{filesGzip[i]}\t{filesSize[i]}");
                }
                output.Add("\t\tbinEnd");
                output.Add("binFileEnd");
                string outPath = Path.Combine(rofs, "gzlist.txt");
                File.WriteAllLines(outPath, output);
            });
        }
        public static byte[] ReadAllBytesBuffered(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        public static void MakeHostFS()
        {
            string elfPath = GetELFPathInSystemCNF();
            if(File.Exists(elfPath))
            {
                string[] hostFSPatch = File.ReadAllLines("cheats\\enable_hostfs.pnach");
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(elfPath)))
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    foreach (string line in hostFSPatch)
                    {
                        bw.BaseStream.Position = Convert.ToUInt32(line.Split(',')[2].Substring(2), 16) - 0xFFE80;
                        bw.Write(BitConverter.GetBytes(Convert.ToUInt32(line.Split(',')[4], 16)));
                    }
                    string path = Path.GetDirectoryName(elfPath);
                    File.WriteAllBytes(Path.Combine(path, "UN5.ELF"), ms.ToArray());
                }
            }
            else
            {
                MessageBox.Show(Path.GetFileName(elfPath) + "not found!");
            }
        }
        public static string GetELFPathInSystemCNF()
        {
            string elfPath = "";
            string systemCNFPath = Path.Combine(gamePath, "SYSTEM.CNF");
            if (File.Exists(systemCNFPath))
            {
                string[] systemCNF = File.ReadAllLines(systemCNFPath);
                foreach (string line in systemCNF)
                {
                    if (line.Split('=')[0].Replace(" ", "").ToUpper() == "BOOT2")
                    {
                        elfPath = line.Split('=')[1].Replace(" ", "");
                        elfPath = elfPath.Replace("cdrom0:\\", "").Replace(";1", "");
                        elfPath = Path.Combine(gamePath, elfPath);
                    }
                }
            }
            else
            {
                MessageBox.Show("SYSTEM.CNF not found!");
            }
            return elfPath;
        }
    }
}
