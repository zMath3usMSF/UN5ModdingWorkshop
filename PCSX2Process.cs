using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace UN5ModdingWorkshop
{
    public class PCSX2Process
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        public const int PROCESS_VM_WRITE = 0x0020;
        public const int PROCESS_VM_OPERATION = 0x0008;
        public const int PROCESS_VM_READ = 0x0010;
        public const int PROCESS_ALL_ACCESS = 0x1F0FFF;
        public const int PROCESS_VM_READWRITE = PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION;

        public static IntPtr processHandle = OpenProcess(PROCESS_VM_READ, false, ID);
        public int GetProcessId(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                return processes[0].Id;
            }
            return -1;
        }
        public class ProcessDetails
        {
            public string Name { get; set; }
            public int Id { get; set; }

            public ProcessDetails(string name, int id)
            {
                Name = name;
                Id = id;
            }
            public override string ToString()
            {
                return $"{Name} (ID: {Id})";
            }
        }
        public static int ID = 0;
        public static void GetEEAdress()
        {
            string path = @"pcsx2_offsetreader.exe";
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = path,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    if (line.Contains("EEmem"))
                    {
                        string eeOffsStr = line.Split(new string[] { "->" }, StringSplitOptions.None)[1];
                        GAME.eeAddress = ulong.Parse(eeOffsStr, System.Globalization.NumberStyles.HexNumber);
                    }
                }
                process.WaitForExit();
            }
        }

        public static void ReadMainBTLMemory()
        {
            Main.instance.picBackground.Visible = false;
            Main.instance.tabControl1.Visible = true;
            processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, ID);
            if (processHandle != IntPtr.Zero)
            {
                int currentMemoryStart = Util.ReadProcessMemoryInt32(0x617EF4);
                int originalMemoryStart = 0xBD4560;

                GAME.memoryDif = currentMemoryStart - originalMemoryStart;

                GAME.charCount = Util.ReadProcessMemoryInt16(0x1EDA20);
                if (Util.ReadStringWithOffset(0x417CD0, false) == "2nrtbod1.ccs")
                {
                    BTL.Clear();

                    int charStringTblOffset = 0x5BA570;
                    if (GAME.charCount != 94) //Verifica se é o UN6 usando quantidade de personagens presentes originalmente no jogo como base.
                    {
                        GAME.isUN6 = true;
                    }
                    int charProgTblOffset = 0x5AC8C0;

                    BTL.ReadCharProgDataTbl(processHandle, charProgTblOffset);

                    BTL.ReadCharNameTbl(processHandle, charStringTblOffset);

                    Main.instance.btnEditGeneralParameters.Visible = true;
                    Main.instance.btnEditMovesetParameters.Visible = true;
                    Main.instance.btnEditAwekeningParameters.Visible = true;
                }
                else
                {
                    MessageBox.Show("Error reading process memory, check if the game has already started or if the PCSX2 version is 1.6 or earlier and try again.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Unable to open the process.");
            }
        }
    }
}
