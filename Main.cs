using CCSFileExplorerWV;
using DiscUtils.Iscsi;
using DiscUtils.Iso9660;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using UN5ModdingWorkshop;

namespace WindowsFormsApp1
{
    public partial class Main : Form
    {
        public static Main instance;
        public Main()
        {
            InitializeComponent();
            instance = this;
            picBackground.Location = new Point(0, 27);
            lblProgress.Text = "";
            Config.Load(this);
            Process[] processes = Process.GetProcesses();
            for (int i = 0; i < processes.Count(); i++)
            {
                if (processes[i].ProcessName.ToLower().Contains("pcsx2"))
                {
                    PCSX2Process.ID = processes[i].Id;
                    PCSX2Process.GetEEAdress();
                    PCSX2Process.ReadMainBTLMemory();
                }
            }
            if (PCSX2Process.ID == 0) MessageBox.Show("Unable to automatically detect any running PCSX2 process. " +
            "Please make sure the open PCSX2 is version 1.6 or higher, then manually select it in Open > PCSX2 Process.");
        }

        private void btnEditGeneralParameters_Click(object sender, EventArgs e)
        {
            GeneralParameters genForm = new GeneralParameters();
            int charID = CharSel.CharSelID[CharSel.SelectedID];
            string charName = BTL.charNameList[charID];

            genForm.timer1.Enabled = true;
            genForm.UpdateLabels(charName, charID);
            var charGenPrm = PlGen.CharGenPrm[charID];
            PlGen.SendTextToGenForm(genForm, charGenPrm);
            genForm.Show();
        }

        private void OpenELF()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Title = "Select the ELF.",
                Filter = "ELF Files|*.05;*.06;*.37|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                GAME.openedELF = false;
            }
            else
            {
                GAME.caminhoELF = openFileDialog.FileName;
                GAME.openedELF = true;
                GAME.generalParameters.btnSaveELF.Enabled = true;
            }
        }

        private void pCSX2MemoryProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectProcess selectProcess = new SelectProcess();
            selectProcess.ListBox1.Items.Clear();
            selectProcess.Owner = this;
            SelectProcess.GetPCSX2Process();
            PCSX2Process.ReadMainBTLMemory();
        }

        private void openELFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenELF();
        }

        private void btnEditMovesetParameters_Click(object sender, EventArgs e)
        {
            GeneralParameters genForm = new GeneralParameters();
            int charID = CharSel.CharSelID[CharSel.SelectedID];
            string charName = BTL.charNameList[charID];

            MovesetParameters movForm = new MovesetParameters();
            movForm.timer1.Enabled = true;
            PlAtk.AddCharComboList(movForm, charID, charName);
            movForm.UpdateLabels(charName, charID.ToString());
            movForm.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("UN5CharPrmEditor, version 1.4. \n\nMade by zMath3usMSF.");
        }

        private void changeP1CharacterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateMatch updateMatch = new UpdateMatch(this);
            updateMatch.lblPlayerID.Text = "P1:";
            bool isP1 = true;
            updateMatch.SendText(isP1);
            updateMatch.Show();
        }

        private void changeP2CharacterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateMatch updateMatch = new UpdateMatch(this);
            updateMatch.lblPlayerID.Text = "P2:";
            bool isP1 = false;
            updateMatch.SendText(isP1);
            updateMatch.Show();
        }

        private void btnEditAwekeningParameters_Click(object sender, EventArgs e)
        {
            int charID = CharSel.CharSelID[CharSel.SelectedID];
            string charName = BTL.charNameList[charID];

            AwakeningParameters awkForm = new AwakeningParameters();
            awkForm.timer1.Enabled = true;
            PlAwk.AddItemsToListBox(awkForm, charID);
            awkForm.UpdateLabels(charName, charID.ToString());
            awkForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int charID = CharSel.CharSelID[CharSel.SelectedID];
            string charName = BTL.charNameList[charID];

            JutsuParameters jtsForm = new JutsuParameters();
            jtsForm.timer1.Enabled = true;
            jtsForm.UpdateLabels(charName, charID.ToString());
            jtsForm.AddToListBox(int.Parse(charID.ToString()));
            jtsForm.Show();
        }

        private void addNewCharacterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewChar ok = new NewChar();
            ok.Show();
        }

        private void btnSelectGamePath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog() == DialogResult.OK)
            {
                string gamePath = fbd.SelectedPath;
                txtGamePath.Text = gamePath;
                CharSel.Create(this, gamePath);
                Config.Data.GamePath = gamePath;
                Config.Save();
            }
        }

        private void txtGamePath_TextChanged(object sender, EventArgs e)
        {
        
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void makeGzlistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GAME.MakeGzlist();
        }

        private void infoADVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InfoADV form = new InfoADV();
            form.Show();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void gameToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void extractCVMToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            GAME.Extract();
            GAME.MakeHostFS();
        }

        private void makeGzlistToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            GAME.Build();
        }
    }
}