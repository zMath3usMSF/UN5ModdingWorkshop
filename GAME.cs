using CCSFileExplorerWV;
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
        public static int charSelCount = 0;
        public static string elfPath = "";
        public static string btlPath = "";
        public static AwakeningParameters awakeningParameters;
        public static MovesetParameters movesetParameters;
        public static GeneralParameters generalParameters;
        public static ulong eeAddress;
        public static int lastSelectedID = 0;
        public static string cvm_toolPath = @"CVM Tool\cvm_tool.exe";
        public static string cvm_hdrPath = @"CVM Tool\data.hdr";
        public static string ps2isoPath = @"tools\ps2iso.exe";
        public static string mkisofsPath = @"tools\mkisofs.exe"; // ou genisoimage.exe, conforme o nome do binário que você baixou
        public static string sevenZipPath = @"tools\7z.exe";
        public static string cvm;
        public static string iso;
        public static string rofs;
        public static string gamePath;
        private static async Task BuildIso9660Async(string sourceFolder, string outputIso)
        {
            if (File.Exists(outputIso)) File.Delete(outputIso);

            // -V         : volume label
            // -iso-level 2 : permite nomes de arquivo mais longos (até 30 chars)
            // -no-iso-translate : não substitui caracteres especiais em nomes de arquivo
            // -o         : caminho de saída do ISO
            string args = $"-V \"GAME\" -iso-level 2 -no-iso-translate -o \"{outputIso}\" \"{sourceFolder}\"";

            await RunProcessAsync(mkisofsPath, args, "Erro ao criar DATA.ISO com mkisofs");
        }
        public class WrongCvmPasswordException : Exception
        {
            public WrongCvmPasswordException() : base("Senha do CVM incorreta.") { }
        }
        public static Task Extract()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "ISO files (*.iso)|*.iso|All files (*.*)|*.*";
            if (ofd.ShowDialog() != DialogResult.OK) return Task.Run(() => { });

            string cvmPassword = "";
            Main.instance.Invoke(new Action(() =>
            {
                cvmPassword = PromptCvmPassword();
            }));

            return Task.Run(async () =>
            {
                try
                {
                    Main.instance.Invoke(new Action(() =>
                        Main.instance.lblProgress.Text = "Extracting ISO..."));

                    string gameFile = ofd.FileName;

                    string unpackFolder = Path.Combine(
                        Path.GetDirectoryName(gameFile),
                        "UNPACK_" + Path.GetFileName(gameFile));

                    // ISO principal → ps2iso (UDF)
                    RunProcessSync(ps2isoPath, $"unpack \"{gameFile}\"", "Error extracting ISO");

                    // ps2iso gera UNPACK_<nome>.iso/FILES/
                    string filesFolder = Path.Combine(unpackFolder, "FILES");

                    gamePath = Path.Combine(Path.GetDirectoryName(gameFile), "GAME");
                    if (Directory.Exists(gamePath)) Directory.Delete(gamePath, true);
                    Directory.CreateDirectory(gamePath);

                    foreach (string file in Directory.EnumerateFiles(filesFolder, "*", SearchOption.AllDirectories))
                    {
                        string rel = file.Substring(filesFolder.Length).TrimStart('\\', '/');
                        string dest = Path.Combine(gamePath, rel);
                        Directory.CreateDirectory(Path.GetDirectoryName(dest));
                        File.Move(file, dest);
                    }
                    Directory.Delete(unpackFolder, true);

                    elfPath = GetELFPathInSystemCNF(gamePath);
                    if (!IsValidELF(Path.GetFileName(elfPath)))
                    {
                        MessageBox.Show("Invalid ELF file: " + Path.GetFileName(elfPath) + "\nExpected: SLES_556.05.ELF or SLUS_556.06.ELF");
                        Main.instance.Invoke(new Action(() =>
                        Main.instance.lblProgress.Text = ""));
                        return;
                    }

                    Main.instance.Invoke(new Action(() =>
                        Main.instance.lblProgress.Text = "Extracting CVM data..."));

                    cvm = Path.Combine(gamePath, @"DATA\DATA.CVM");
                    iso = Path.Combine(gamePath, @"DATA\data.iso");
                    rofs = Path.Combine(gamePath, @"DATA\ROFS");

                    // Divide CVM → data.iso (ISO9660) via cvm_tool
                    string splitArgs = string.IsNullOrEmpty(cvmPassword)
                        ? $"split \"{cvm}\" \"{iso}\" \"{cvm_hdrPath}\""
                        : $"split \"{cvm}\" \"{iso}\" \"{cvm_hdrPath}\" -p {cvmPassword}";

                    RunProcessSync(cvm_toolPath, splitArgs, "Error splitting CVM");
                    File.Delete(cvm);

                    // data.iso é ISO9660 → extrai usando 7-Zip (externo)
                    Directory.CreateDirectory(rofs);
                    await ExtractIso9660With7ZipAsync(iso, rofs);
                    File.Delete(iso);

                    // Descomprime .ccs gzipados dentro de ROFS
                    foreach (string file in Directory.GetFiles(rofs, "*.ccs", SearchOption.AllDirectories))
                    {
                        using (FileStream fs = File.OpenRead(file))
                        {
                            byte[] header = new byte[2];
                            fs.Read(header, 0, 2);
                            if (header[0] != 0x1F || header[1] != 0x8B)
                                continue;
                        }

                        Main.instance.Invoke(new Action(() =>
                            Main.instance.lblProgress.Text = $"Decompressing: {Path.GetFileName(file)}"));

                        using (MemoryStream ms = new MemoryStream())
                        using (GZipStream gz = new GZipStream(
                            new MemoryStream(File.ReadAllBytes(file)), CompressionMode.Decompress))
                        {
                            gz.CopyTo(ms);
                            File.WriteAllBytes(file, ms.ToArray());
                        }
                    }

                    Main.instance.Invoke(new Action(() =>
                        Main.instance.lblProgress.Text = ""));

                    MakeHostFS();
                    MessageBox.Show("Game successfully extracted!");
                }
                catch (WrongCvmPasswordException)
                {
                    try { if (File.Exists(iso)) File.Delete(iso); } catch { }
                    try { if (File.Exists(cvm)) File.Delete(cvm); } catch { }
                    try { if (Directory.Exists(gamePath)) Directory.Delete(gamePath, true); } catch { }

                    Main.instance.Invoke(new Action(() =>
                    {
                        Main.instance.lblProgress.Text = "";
                        MessageBox.Show("Senha do CVM incorreta. A extração foi cancelada.",
                            "Senha inválida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                catch (Exception ex)
                {
                    Main.instance.Invoke(new Action(() =>
                    {
                        Main.instance.lblProgress.Text = "";
                        MessageBox.Show("Erro durante a extração:\n" + ex.Message,
                            "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
            });
        }

        public static async Task Build()
        {
            string sourceFolder = null;

            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() != DialogResult.OK) return;
                sourceFolder = fbd.SelectedPath;
            }

            elfPath = GetELFPathInSystemCNF(sourceFolder);
            if (!IsValidELF(Path.GetFileName(elfPath)))
            {
                MessageBox.Show("Invalid ELF file: " + Path.GetFileName(elfPath) + "\nExpected: SLES_556.05.ELF or SLUS_556.06.ELF");
                Main.instance.Invoke(new Action(() =>
                Main.instance.lblProgress.Text = ""));
                return;
            }

            string cvmPassword = PromptCvmPassword();

            await Task.Run(async () =>
            {
                // Caminhos ORIGINAIS — nunca serão apagados ou modificados
                string originalRofs = Path.Combine(sourceFolder, @"DATA\ROFS");

                // Pasta temporária de trabalho (fora da pasta extraída)
                string tempBuildFolder = Path.Combine(
                    Path.GetTempPath(),
                    "UN5_BUILD_" + Guid.NewGuid().ToString("N"));

                string tempRofs = Path.Combine(tempBuildFolder, "ROFS");
                string tempIso = Path.Combine(tempBuildFolder, "data.iso");
                string tempCvm = Path.Combine(tempBuildFolder, "DATA.CVM");

                try
                {
                    Main.instance.Invoke(new Action(() =>
                        Main.instance.lblProgress.Text = "Copiando ROFS para área temporária..."));

                    // Copia TODO o conteúdo de ROFS para a pasta temporária,
                    // preservando o original intacto
                    Directory.CreateDirectory(tempRofs);
                    foreach (string dirPath in Directory.GetDirectories(originalRofs, "*", SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(originalRofs, tempRofs));

                    foreach (string filePath in Directory.GetFiles(originalRofs, "*", SearchOption.AllDirectories))
                        File.Copy(filePath, filePath.Replace(originalRofs, tempRofs), overwrite: true);

                    // Compressão .ccs e gzlist.txt acontecem só na cópia temporária
                    await MakeGzlist(tempRofs);

                    Main.instance.Invoke(new Action(() =>
                        Main.instance.lblProgress.Text = "Criando DATA.ISO..."));

                    await BuildIso9660Async(tempRofs, tempIso);

                    Main.instance.Invoke(new Action(() =>
                        Main.instance.lblProgress.Text = "Convertendo DATA.ISO para DATA.CVM..."));

                    string mkcvmArgs = string.IsNullOrEmpty(cvmPassword)
                        ? $"mkcvm \"{tempCvm}\" \"{tempIso}\" \"{cvm_hdrPath}\""
                        : $"mkcvm \"{tempCvm}\" \"{tempIso}\" \"{cvm_hdrPath}\" -p {cvmPassword}";

                    await RunProcessAsync(cvm_toolPath, mkcvmArgs, "Erro ao criar CVM");

                    // ── Monta estrutura que o ps2iso espera ──────────────────────────
                    Main.instance.Invoke(new Action(() =>
                        Main.instance.lblProgress.Text = "Preparando estrutura para ps2iso..."));

                    string packStaging = Path.Combine(tempBuildFolder, "ps2iso_staging");
                    Directory.CreateDirectory(packStaging);

                    string stagingFiles = Path.Combine(packStaging, "FILES");
                    Directory.CreateDirectory(stagingFiles);

                    string retailElfPath = GetELFPathInSystemCNF(sourceFolder);   // ex.: sourceFolder\SCES_123.45
                    string enabledElfPath = retailElfPath;                // arquivo real que existe na pasta
                    string originalCvmPath = Path.Combine(sourceFolder, @"DATA\DATA.CVM"); // CVM antigo, não deve ir pro staging

                    // Copia todos os arquivos ORIGINAIS de sourceFolder → staging/FILES,
                    // exceto: .ELF de trabalho (tratado abaixo), pasta ROFS (não vai pro ISO final)
                    // e o DATA.CVM antigo (será substituído pelo recém-gerado em tempCvm)
                    foreach (string srcFile in Directory.EnumerateFiles(sourceFolder, "*", SearchOption.AllDirectories))
                    {
                        if (string.Equals(srcFile, enabledElfPath, StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (string.Equals(srcFile, originalCvmPath, StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (srcFile.StartsWith(originalRofs + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                            continue;

                        string rel = srcFile.Substring(sourceFolder.Length).TrimStart('\\', '/');
                        string dest = Path.Combine(stagingFiles, rel);
                        Directory.CreateDirectory(Path.GetDirectoryName(dest));
                        File.Copy(srcFile, dest, overwrite: true);
                    }

                    // Copia o DATA.CVM recém-gerado (com as modificações) para o staging
                    string relCvm = originalCvmPath.Substring(sourceFolder.Length).TrimStart('\\', '/');
                    string destCvm = Path.Combine(stagingFiles, relCvm);
                    Directory.CreateDirectory(Path.GetDirectoryName(destCvm));
                    File.Copy(tempCvm, destCvm, overwrite: true);

                    // Reverte o patch HostFS (Enable → Disable) numa cópia temporária em memória
                    // e grava no staging já com o nome retail original (sem o ".ELF")
                    if (File.Exists(enabledElfPath))
                    {
                        string relRetailElf = retailElfPath.Substring(sourceFolder.Length).TrimStart('\\', '/');
                        string destRetailElf = Path.Combine(stagingFiles, Path.GetFileNameWithoutExtension(relRetailElf));
                        Directory.CreateDirectory(Path.GetDirectoryName(destRetailElf));
                        File.WriteAllBytes(destRetailElf, ApplyPnachPatch(@"cheats\HostFSMode_Disable.pnach").elfBytes);
                    }
                    else
                    {
                        MessageBox.Show(Path.GetFileName(enabledElfPath) + " not found!");
                    }

                    // Gera METADATA.json dentro de packStaging
                    Main.instance.Invoke(new Action(() =>
                        Main.instance.lblProgress.Text = "Gerando METADATA.json..."));

                    string metadataPath = BuildPs2IsoMetadata(sourceFolder, packStaging, stagingFiles);

                    // Chama ps2iso pack → gera packStaging/OUTPUT.iso
                    Main.instance.Invoke(new Action(() =>
                        Main.instance.lblProgress.Text = "Criando ISO final do jogo..."));

                    await RunProcessAsync(ps2isoPath, $"pack \"{metadataPath}\"", "Erro ao criar ISO final");

                    string generatedIso = Path.Combine(packStaging, "OUTPUT.iso");

                    string finalISO = Path.Combine(
                        Path.GetDirectoryName(sourceFolder),
                        "MODDED_UN5.ISO");

                    if (File.Exists(finalISO)) File.Delete(finalISO);
                    File.Move(generatedIso, finalISO);

                    Main.instance.Invoke(new Action(() =>
                        Main.instance.lblProgress.Text = ""));

                    MessageBox.Show("Build concluído!\n" + finalISO);
                }
                finally
                {
                    // Só apaga a área TEMPORÁRIA — sourceFolder original nunca é tocado
                    if (Directory.Exists(tempBuildFolder))
                        Directory.Delete(tempBuildFolder, true);
                }
            });
        }

        /// <summary>
        /// Extrai todo o conteúdo de uma imagem ISO9660 usando o utilitário externo isoinfo.
        /// Faz UMA ÚNICA chamada de listagem (-l sem -p, que lista a árvore inteira recursivamente
        /// de uma vez), parseando os blocos "Directory listing of X". Evita recursão de processos
        /// (que causava loop infinito nesta build do isoinfo) e diferencia arquivos de pastas.
        /// </summary>
        /// <summary>
        /// Extrai todo o conteúdo de uma imagem ISO9660 usando o 7-Zip (externo), que lê
        /// ISO9660 nativamente e extrai tudo em UM ÚNICO processo — muito mais rápido que
        /// chamar isoinfo arquivo por arquivo.
        /// </summary>
        private static async Task ExtractIso9660With7ZipAsync(string isoPath, string destFolder)
        {
            Directory.CreateDirectory(destFolder);

            // x          : extrai mantendo estrutura de pastas
            // -y         : confirma overwrite automaticamente
            // -o<pasta>  : pasta de destino (sem espaço entre -o e o caminho)
            string args = $"x \"{isoPath}\" -o\"{destFolder}\" -y";

            await RunProcessAsync(sevenZipPath, args, "Erro ao extrair ISO com 7-Zip");

            // Remove o sufixo ";1" (versão ISO9660) dos nomes extraídos, já que o 7z
            // geralmente preserva os nomes como estão na ISO
            foreach (string path in Directory.EnumerateFileSystemEntries(destFolder, "*", SearchOption.AllDirectories)
                         .Where(p => p.Contains(";")).OrderByDescending(p => p.Length).ToList())
            {
                string cleanPath = System.Text.RegularExpressions.Regex.Replace(path, @";\d+$", "");
                if (!string.Equals(path, cleanPath, StringComparison.OrdinalIgnoreCase) && !File.Exists(cleanPath) && !Directory.Exists(cleanPath))
                {
                    if (Directory.Exists(path))
                        Directory.Move(path, cleanPath);
                    else if (File.Exists(path))
                        File.Move(path, cleanPath);
                }
            }
        }

        /// <summary>
        /// Gera METADATA.json no formato que ps2iso espera.
        /// </summary>
        private static string BuildPs2IsoMetadata(string sourceFolder, string packStaging, string stagingFiles)
        {
            string volumeLabel = "GAME";
            string sysCNF = Path.Combine(sourceFolder, "SYSTEM.CNF");
            if (File.Exists(sysCNF))
            {
                foreach (string line in File.ReadAllLines(sysCNF))
                {
                    string trimmed = line.Replace(" ", "");
                    if (trimmed.ToUpper().StartsWith("BOOT2="))
                    {
                        string elf = trimmed.Substring("BOOT2=".Length)
                            .Replace("cdrom0:\\", "").Replace(";1", "");
                        volumeLabel = Path.GetFileNameWithoutExtension(elf)
                            .Replace(".", "_").ToUpper();
                        break;
                    }
                }
            }

            var entries = Directory
                .EnumerateFiles(stagingFiles, "*", SearchOption.AllDirectories)
                .Select(f => f.Substring(stagingFiles.Length).TrimStart('\\', '/'))
                .Where(rel => !rel.Equals("METADATA.json", StringComparison.OrdinalIgnoreCase))
                .OrderBy(r => r, StringComparer.OrdinalIgnoreCase)
                .Select(r => r.Replace('/', '\\'))
                .ToList();

            string metadataPath = Path.Combine(packStaging, "METADATA.json");
            File.WriteAllText(metadataPath, SerializeMetadata(volumeLabel, entries));
            return metadataPath;
        }

        private static string SerializeMetadata(string volumeLabel, List<string> entries)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  \"VolumeLabel\": \"{EscapeJson(volumeLabel)}\",");
            sb.AppendLine("  \"Entries\": [");

            for (int i = 0; i < entries.Count; i++)
            {
                string comma = i < entries.Count - 1 ? "," : "";
                sb.AppendLine($"    \"{EscapeJson(entries[i])}\"{comma}");
            }

            sb.AppendLine("  ]");
            sb.Append("}");
            return sb.ToString();
        }

        private static string EscapeJson(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static string PromptCvmPassword()
        {
            using (Form prompt = new Form())
            {
                prompt.Text = "CVM Password";
                prompt.Width = 360;
                prompt.Height = 160;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.StartPosition = FormStartPosition.CenterParent;
                prompt.MaximizeBox = false;
                prompt.MinimizeBox = false;

                Label lbl = new Label
                {
                    Text = "Enter CVM password (leave blank for none):",
                    Left = 12,
                    Top = 14,
                    Width = 320
                };
                TextBox tb = new TextBox { Left = 12, Top = 38, Width = 320 };
                Button btnOK = new Button
                {
                    Text = "OK",
                    Left = 175,
                    Top = 72,
                    Width = 75,
                    DialogResult = DialogResult.OK
                };
                Button btnCancel = new Button
                {
                    Text = "Cancel",
                    Left = 257,
                    Top = 72,
                    Width = 75,
                    DialogResult = DialogResult.Cancel
                };

                prompt.Controls.AddRange(new Control[] { lbl, tb, btnOK, btnCancel });
                prompt.AcceptButton = btnOK;
                prompt.CancelButton = btnCancel;

                return prompt.ShowDialog() == DialogResult.OK ? tb.Text.Trim() : "";
            }
        }

        private static void RunProcessSync(string fileName, string arguments, string errorPrefix)
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

                string stdout = process.StandardOutput.ReadToEnd();
                string stderr = process.StandardError.ReadToEnd();
                process.WaitForExit();

                // cvm_tool imprime essa mensagem quando a senha está incorreta,
                // mesmo que o exit code não indique erro
                if (stdout.IndexOf("bad decryption key", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    stderr.IndexOf("bad decryption key", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    throw new WrongCvmPasswordException();
                }

                if (process.ExitCode != 0)
                    throw new Exception($"{errorPrefix} (exit {process.ExitCode}):\nSTDOUT: {stdout}\nSTDERR: {stderr}");
            }
        }

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

                Task<string> stdoutTask = process.StandardOutput.ReadToEndAsync();
                Task<string> stderrTask = process.StandardError.ReadToEndAsync();

                await Task.Run(() => process.WaitForExit());

                string stdout = await stdoutTask;
                string stderr = await stderrTask;

                if (process.ExitCode != 0)
                    throw new Exception($"{errorPrefix} (exit {process.ExitCode}):\nSTDOUT: {stdout}\nSTDERR: {stderr}");
            }
        }

        public static Task MakeGzlist(string rofsPath)
        {
            return Task.Run(() =>
            {
                List<string> output = new List<string>();
                output.Add("#\t\tname\t\t\tnum");

                List<string> folders = Directory.GetDirectories(rofsPath, "*", SearchOption.AllDirectories).ToList();
                folders.Sort();
                folders.Insert(0, rofsPath);

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
                        if (Path.GetExtension(filePath).ToLower() != ".ccs") continue;

                        string fileCVMPath = filePath.Replace(rofsPath.TrimEnd('\\') + "\\", "").ToLower();

                        Main.instance.Invoke(new Action(() =>
                            Main.instance.lblProgress.Text = "Reading: " + Path.GetFileName(filePath)));

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

                    foldersFilesCount.Add(dirCount + currentFileList.Length);
                }

                foldersFilesCount[0] += 1;
                folders[0] = "root";
                for (int i = 1; i < folders.Count; i++)
                {
                    folders[i] = folders[i]
                        .Replace(rofsPath.TrimEnd('\\') + "\\", "")
                        .Replace("\\", "/")
                        .ToLower() + "/";
                }

                for (int i = 0; i < folders.Count; i++)
                    output.Add("\t\t" + folders[i].PadRight(16) + "\t" + foldersFilesCount[i]);

                output.Add("\t\tbinEnd");
                output.Add("#\t\tname\t\t\tsize\t\tgzip");
                output.Add("\tdata\t0x0");

                for (int i = 0; i < files.Count; i++)
                    output.Add($"\t\t{files[i]} \t{filesGzip[i]}\t{filesSize[i]}");

                output.Add("\t\tbinEnd");
                output.Add("binFileEnd");

                File.WriteAllLines(Path.Combine(rofsPath, "gzlist.txt"), output);
            });
        }
        public static byte[] ReadAllBytesBuffered(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
            using (MemoryStream ms = new MemoryStream())
            {
                fs.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static (byte[] elfBytes, byte[] btlBytes) ApplyPnachPatch(string pnachPath)
        {
            if (!File.Exists(elfPath))
            {
                MessageBox.Show(Path.GetFileName(elfPath) + " not found!");
                return (new byte[0], new byte[0]);
            }
            byte[] elfBytes = File.ReadAllBytes(elfPath);

            if (!File.Exists(btlPath))
            {
                MessageBox.Show(Path.GetFileName(btlPath) + " not found!");
                return (new byte[0], new byte[0]);
            }
            byte[] btlBytes = File.ReadAllBytes(btlPath);

            string[] patchLines = File.ReadAllLines(pnachPath);

            using (MemoryStream msELF = new MemoryStream(elfBytes))
            using (BinaryWriter bwELF = new BinaryWriter(msELF))
            using (MemoryStream msBTL = new MemoryStream(btlBytes))
            using (BinaryWriter bwBTL = new BinaryWriter(msBTL))
            {
                foreach (string line in patchLines)
                {
                    if (!line.StartsWith("patch=", StringComparison.OrdinalIgnoreCase))
                        continue;

                    string[] parts = line.Split(',');
                    if (parts.Length < 5) continue; // linha malformada, ignora

                    uint currentOffset = Convert.ToUInt32(parts[2].Substring(2), 16);
                    char sizeType = parts[2][0];
                    string value = parts[4];

                    BinaryWriter bw = null;
                    uint baseOffset = 0;

                    if (currentOffset > 0x100000 && currentOffset < 0x6C6D00)
                    {
                        bw = bwELF;
                        baseOffset = 0xFFE80;
                    }
                    else if (currentOffset > 0x6C6D00 && currentOffset < 0x8F3D00)
                    {
                        bw = bwBTL;
                        baseOffset = 0x6C6D00;
                    }
                    else
                    {
                        continue;
                    }

                    bw.BaseStream.Position = currentOffset - baseOffset;
                    switch (sizeType)
                    {
                        case '0':
                            bw.Write(Convert.ToByte(value.Substring(6), 16));
                            break;
                        case '1':
                            bw.Write(BitConverter.GetBytes(Convert.ToUInt16(value.Substring(4), 16)));
                            break;
                        case '2':
                            bw.Write(BitConverter.GetBytes(Convert.ToUInt32(value, 16)));
                            break;
                    }
                }

                return (msELF.ToArray(), msBTL.ToArray());
            }
        }

        public static void MakeHostFS()
        {
            File.WriteAllBytes(elfPath, ApplyPnachPatch(@"cheats\HostFSMode_Enable.pnach").elfBytes);
        }

        public static string GetELFPathInSystemCNF(string basePath)
        {
            string systemCNFPath = Path.Combine(basePath, "SYSTEM.CNF");
            if (File.Exists(systemCNFPath))
            {
                foreach (string line in File.ReadAllLines(systemCNFPath))
                {
                    if (line.Split('=')[0].Replace(" ", "").ToUpper() == "BOOT2")
                    {
                        elfPath = line.Split('=')[1].Replace(" ", "")
                            .Replace("cdrom0:\\", "").Replace(";1", "");
                        try
                        {
                            File.Move(Path.Combine(basePath, elfPath), Path.Combine(basePath, elfPath + ".ELF"));

                        }
                        catch (Exception ex)
                        {
                            
                        }
                        elfPath = Path.Combine(basePath, elfPath + ".ELF");
                        btlPath = Path.Combine(gamePath, "PRG\\BTL.BIN");
                    }
                }
            }
            else
            {
                MessageBox.Show("SYSTEM.CNF not found!");
            }
            return elfPath;
        }

        public static bool IsValidELF(string elfName)
        {
            switch (elfName)
            {
                case "SLES_556.05.ELF":
                case "SLUS_556.06.ELF":
                    return true;
                default:
                    return false;
            }
        } 
    }
}