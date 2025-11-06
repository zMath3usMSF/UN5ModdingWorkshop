using CCSFileExplorerWV;
using DiscUtils.Iso9660;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static string caminhoELF;
        public static bool openedELF;
        public static AwakeningParameters awakeningParameters;
        public static MovesetParameters movesetParameters;
        public static GeneralParameters generalParameters;
        public static ulong eeAddress;
        public static int lastSelectedID = 0;
        public static string cvm_toolPath = @"CVM Tool\cvm_tool.exe";
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
                using (FileStream isoStream = File.OpenRead(gameFile))
                {
                    CDReader cd = new CDReader(isoStream, true);
                    ExtractFiles(cd, @"\", gamePath);
                }

                Main.instance.Invoke(new Action(() =>
                {
                    Main.instance.lblProgress.Text = "Extracting CVM data...";
                }));
                cvm = Path.Combine(gamePath, "DATA\\DATA.CVM");
                iso = Path.Combine(gamePath, "DATA\\data.iso");
                rofs = Path.Combine(gamePath, "DATA\\ROFS");
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = cvm_toolPath,
                    Arguments = $"split \"{cvm}\" \"{iso}\" data.hdr -p cc2fuku",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process process = Process.Start(psi);
                process.WaitForExit();

                using (FileStream isoStream = File.OpenRead(iso))
                {
                    CDReader cd = new CDReader(isoStream, true);
                    ExtractFiles(cd, @"\", rofs);
                }

                File.Delete(cvm);
                File.Delete(iso);

                foreach (string file in Directory.GetFiles(rofs, "*.ccs", SearchOption.AllDirectories))
                {
                    using (FileStream fs = File.OpenRead(file))
                    {
                        byte[] header = new byte[2];
                        fs.Read(header, 0, 2);
                        if (header[0] != 0x1F || header[1] != 0x8B)
                        {
                            continue;
                        }
                    }

                    MemoryStream ms = new MemoryStream();
                    GZipStream gzipStream = new GZipStream(new MemoryStream(File.ReadAllBytes(file)), CompressionMode.Decompress);
                    gzipStream.CopyTo(ms);
                    File.WriteAllBytes(file, ms.ToArray());

                    Main.instance.Invoke(new Action(() =>
                    {
                        Main.instance.lblProgress.Text = $"Decompressing CVM files: {file}";
                    }));
                }

                MessageBox.Show("Game successfully extracted!");
                Main.instance.Invoke(new Action(() =>
                {
                    Main.instance.lblProgress.Text = "";
                }));
            });
        }

        public static void ExtractFiles(CDReader cd, string cdPath, string destinoBase)
        {
            foreach (var dir in cd.GetDirectories(cdPath))
            {
                ExtractFiles(cd, dir, destinoBase);
            }

            foreach (var file in cd.GetFiles(cdPath))
            {
                string relativePath = file.TrimStart('\\');
                if (relativePath.EndsWith(";1"))
                    relativePath = relativePath.Substring(0, relativePath.Length - 2);
                string destino = Path.Combine(destinoBase, relativePath);

                Main.instance.Invoke(new Action(() =>
                {
                    Main.instance.lblProgress.Text = "Extracting: " + destino;
                }));
                Directory.CreateDirectory(Path.GetDirectoryName(destino));

                using (var source = cd.OpenFile(file, FileMode.Open))
                using (var dest = File.Create(destino))
                {
                    source.CopyTo(dest);
                }
            }
        }

        public static async void Build()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            string sourceFolder = fbd.SelectedPath;
            string isoPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "GAME.ISO");
            string cvm = Path.Combine(sourceFolder, "DATA\\DATA.CVM");
            string iso = Path.Combine(sourceFolder, "DATA\\data.iso");
            string rofs = Path.Combine(sourceFolder, "DATA\\ROFS");

            await MakeGzlist();

            var mkcvm = new ProcessStartInfo
            {
                FileName = cvm_toolPath,
                Arguments = $"mkcvm \"{cvm}\" \"{iso}\" data.hdr",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var CVMTool = Process.Start(mkcvm))
            {
                string output = await CVMTool.StandardOutput.ReadToEndAsync();
                string error = await CVMTool.StandardError.ReadToEndAsync();
                CVMTool.WaitForExit();
            }

            Directory.Delete(rofs);
            Directory.Delete(iso);
            Main.instance.lblProgress.Text = "";
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
            gamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "GAME");
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
