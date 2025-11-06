using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UN5ModdingWorkshop;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static UN5ModdingWorkshop.PCSX2Process;
using static WindowsFormsApp1.Main;

namespace WindowsFormsApp1
{
    public partial class SelectProcess : Form
    {
        public ListBox ListBox1 { get { return listBox1; } }
        public SelectProcess()
        {
            InitializeComponent();
            ListBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBox1.SelectedItem != null)
            {
                ProcessDetails selectedProcess = (ProcessDetails)ListBox1.SelectedItem;
                GetEEAdress();

                PCSX2Process.ID = selectedProcess.Id;
                this.Close();
            }
        }

        public static void GetPCSX2Process()
        {
            Process[] processes = Process.GetProcesses();
            SelectProcess selectProcess = new SelectProcess();
            for (int i = 0; i < processes.Count(); i++)
            {
                if (processes[i].ProcessName.ToLower().Contains("pcsx2"))
                {
                    ProcessDetails processDetails = new ProcessDetails(processes[i].ProcessName, processes[i].Id);

                    selectProcess.AdicionarItemListBox(processDetails);
                }
                if (i == processes.Count() - 1 & processes[i].ProcessName.ToLower().Contains("pcsx2") == false)
                {
                    if (selectProcess.ListBox1.Items.Count == 0 & processes[i].ProcessName.ToLower().Contains("pcsx2") == false)
                    {
                        MessageBox.Show("PCSX2 process not found, open PCSX2 with the game running and try again.");
                        return;
                    }
                    selectProcess.ShowDialog();
                }
            }
            return;
        }
        public void AdicionarItemListBox(object item)
        {
            listBox1.Items.Add(item);
        }
    }
}
