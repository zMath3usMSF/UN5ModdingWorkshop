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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Main : Form
    {
        public static Main instance;
        public Main()
        {
            InitializeComponent();

            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Remove(tabPage3);
            tabControl1.TabPages.Remove(tabPage4);
            tabControl1.TabPages.Remove(tabPage5);

            instance = this;
            picBackground.Location = new Point(0, 27);
            lblProgress.Text = "";
            Process[] processes = Process.GetProcesses();
            for (int i = 0; i < processes.Count(); i++)
            {
                if (processes[i].ProcessName.ToLower().Contains("pcsx2"))
                {
                    PCSX2Process.ID = processes[i].Id;
                    PCSX2Process.GetEEAdress();
                    PCSX2Process.ReadMainBTLMemory(this);
                }
            }
            if (PCSX2Process.ID == 0) MessageBox.Show("Unable to automatically detect any running PCSX2 process. " +
            "Please make sure the open PCSX2 is version 1.6 or higher, then manually select it in Open > PCSX2 Process.");
        }

        private void btnEditGeneralParameters_Click(object sender, EventArgs e)
        {
            GeneralParameters genForm = new GeneralParameters();
            int charID = CharSel.CharSelID[CharSel.SelectedID];
            string charName = charID < 93 ? BTL.charNameList[charID] : "???";

            genForm.timer1.Enabled = true;
            genForm.UpdateLabels(charName, charID);
            var charGenPrm = Character.List[charID];
            Character.PopulateForm(genForm, charGenPrm);
            genForm.Show();
        }

        private void pCSX2MemoryProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectProcess selectProcess = new SelectProcess();
            selectProcess.ListBox1.Items.Clear();
            selectProcess.Owner = this;
            SelectProcess.GetPCSX2Process();
            PCSX2Process.ReadMainBTLMemory(this);
        }

        private void btnEditMovesetParameters_Click(object sender, EventArgs e)
        {
            GeneralParameters genForm = new GeneralParameters();
            int charID = CharSel.CharSelID[CharSel.SelectedID];
            string charName = "";

            MovesetParameters movForm = new MovesetParameters();
            movForm.timer1.Enabled = true;
            Attack.AddCharComboList(movForm, charID, charName);
            movForm.UpdateLabels(charName, charID.ToString());
            movForm.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var about = new AboutForm())
            {
                about.ShowDialog(this);
            }
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
                GAME.gamePath = gamePath;
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
            GAME.MakeGzlist(GAME.rofs);
        }

        private void infoADVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Util.GetCurrentGameMode() == "Master")
            {
                InfoADV form = new InfoADV();
                form.Show();
            }
            else
            {
                MessageBox.Show("You need to be in Master Mode first!");
            }
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
        }

        private void makeGzlistToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            GAME.Build();
        }

        private void buildGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GAME.Build();
        }

        private void btnEditJutsusParameters_Click(object sender, EventArgs e)
        {
            int charID = CharSel.CharSelID[CharSel.SelectedID];
            string charName = "";

            JutsuParameters sklForm = new JutsuParameters();
            sklForm.timer1.Enabled = true;
            sklForm.UpdateLabels(charName, charID.ToString());
            sklForm.Show();
        }

        private void openELFToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cheatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheatsMenu cheatsMenu = new CheatsMenu();
            cheatsMenu.ShowDialog();
        }

        private void cCSEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CCSF_Editor CCSF_Editor = new CCSF_Editor();
            CCSF_Editor.Show();
        }

        private void reallocToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReallocCharacter(0x1, 0xA14B60);
        }
        private const int ALIGNMENT = 0x10;
        private const int NAME_ENTRY_LEN = 0x1E;
        private const int ANM_NAME_LEN = 0x10;
        private const int OBJ_ATK_NAME_LEN = 0x20;
        private const int PRG_POINTERS_SIZE = 0x20;
        private const int NULL_ANM_STRING_PTR = 0x6105C0;
        private const int CHAR_TABLE_LOW = 0x5AC8C0;
        private const int CHAR_TABLE_HIGH = 0x956100;
        private const int CHAR_TABLE_SPLIT_INDEX = 0x5E;
        private const int CHAR_TABLE_ENTRY_SIZE = 8;
        private const int CHAR_TABLE_OFFSET_FIELD = 4;

        private void ReallocCharacter(int charIndex, int baseOffset)
        {
            MemoryStream msCharData = new MemoryStream();
            BinaryWriter bwCharData = new BinaryWriter(msCharData);

            MemoryStream msCharANMStringData = new MemoryStream();
            BinaryWriter bwCharANMStringData = new BinaryWriter(msCharANMStringData);

            MemoryStream msCharANMData = new MemoryStream();
            BinaryWriter bwCharANMData = new BinaryWriter(msCharANMData);

            MemoryStream msCharATKData = new MemoryStream();
            BinaryWriter bwCharATKData = new BinaryWriter(msCharATKData);

            MemoryStream msCharDataHeader = new MemoryStream();
            BinaryWriter bwCharDataHeader = new BinaryWriter(msCharDataHeader);

            Character charGen = Character.List[charIndex];
            int count = 0;

            int newCltOffset = (int)(baseOffset + bwCharData.BaseStream.Length);
            foreach(string clutName in charGen.Cluts)
            {
                Util.WriteFixedLenString(bwCharData, clutName, NAME_ENTRY_LEN);
            }
            Util.AlignBinaryWriter(bwCharData, ALIGNMENT);

            int newTexOffset = (int)(baseOffset + bwCharData.BaseStream.Length);
            foreach (string textureName in charGen.Textures)
            {
                Util.WriteFixedLenString(bwCharData, textureName, NAME_ENTRY_LEN);
            }
            Util.AlignBinaryWriter(bwCharData, ALIGNMENT);

            count = 0;
            int newMdlOffset = (int)(baseOffset + bwCharData.BaseStream.Length);
            string mdlName = charGen.Model;
            Util.WriteFixedLenString(bwCharData, mdlName, NAME_ENTRY_LEN);
            Util.AlignBinaryWriter(bwCharData, ALIGNMENT);

            int newHolObjOffset = (int)(baseOffset + bwCharData.BaseStream.Length);
            string holObjName = charGen.HoldBonne;
            Util.WriteFixedLenString(bwCharData, holObjName, NAME_ENTRY_LEN);
            Util.AlignBinaryWriter(bwCharData, ALIGNMENT);

            int newPrgOffset = (int)(baseOffset + bwCharData.BaseStream.Length);
            foreach(int funcAddr in charGen.MovesetFunctionsAddr)
            {
                bwCharData.Write(funcAddr);
            }
            Util.AlignBinaryWriter(bwCharData, ALIGNMENT);

            List<long> AnmStringOffsList = new List<long>();
            for (int i = 0; i < charGen.AnmNameCount; i++)
            {
                int AnmStringOffs = Util.ReadProcessMemoryInt32((int)charGen.AnmNameListOffset + (i * 4));
                string AnmName = Util.ReadMemoryFixedLenString(AnmStringOffs, ANM_NAME_LEN, '\0');
                if (AnmName != "")
                {
                    AnmStringOffsList.Add(bwCharANMStringData.BaseStream.Length);
                    Util.WriteFixedLenString(bwCharANMStringData, AnmName, ANM_NAME_LEN);
                }
                else
                {
                    AnmStringOffsList.Add(NULL_ANM_STRING_PTR);
                }
            }

            int startAnmStringOffs = (int)(baseOffset + bwCharData.BaseStream.Length);
            bwCharData.Write(msCharANMStringData.ToArray());
            int newAnmStringOffs = (int)(baseOffset + bwCharData.BaseStream.Length);
            for (int i = 0; i < AnmStringOffsList.Count; i++)
            {
                if (AnmStringOffsList[i] == NULL_ANM_STRING_PTR)
                {
                    bwCharData.Write(NULL_ANM_STRING_PTR);
                }
                else
                {
                    bwCharData.Write((int)(startAnmStringOffs + AnmStringOffsList[i]));
                }
            }

            List<(string BonneName, int BonneNameOffs)> BonneNameList = new();

            for (int i = 0; i < charGen.AnmCount; i++)
            {
                Animation plAnm = Animation.Get(charIndex, i);

                if (plAnm.ObjAtk.SequenceEqual(new byte[4] { 0x00, 0x00, 0x00, 0x00 })) plAnm.ObjAtk = new byte[4] { 0xC0, 0x05, 0x61, 0x00 };
                if (plAnm.ObjAtk2.SequenceEqual(new byte[4] { 0x00, 0x00, 0x00, 0x00 })) plAnm.ObjAtk2 = new byte[4] { 0xC0, 0x05, 0x61, 0x00 };

                string ObjAtkName = Util.ReadMemoryFixedLenString(
                    BitConverter.ToInt32(plAnm.ObjAtk, 0), OBJ_ATK_NAME_LEN, '\0');

                string ObjAtkName2 = Util.ReadMemoryFixedLenString(
                    BitConverter.ToInt32(plAnm.ObjAtk2, 0), OBJ_ATK_NAME_LEN, '\0');

                if (Animation.CommonBonnesList.ContainsKey(ObjAtkName))
                {
                    plAnm.ObjAtk = Animation.CommonBonnesList[ObjAtkName];
                }
                else if (!BonneNameList.Any(x => x.BonneName == ObjAtkName))
                {
                    BonneNameList.Add((ObjAtkName, (int)bwCharData.BaseStream.Length));

                    plAnm.ObjAtk = BitConverter.GetBytes(
                        (int)(baseOffset + bwCharData.BaseStream.Length));

                    Util.WriteFixedLenString(bwCharData, ObjAtkName, OBJ_ATK_NAME_LEN);
                }
                else
                {
                    var existingBonne = BonneNameList.First(x => x.BonneName == ObjAtkName);

                    plAnm.ObjAtk = BitConverter.GetBytes(
                        (int)(baseOffset + existingBonne.BonneNameOffs));
                }

                if (Animation.CommonBonnesList.ContainsKey(ObjAtkName2))
                {
                    plAnm.ObjAtk2 = Animation.CommonBonnesList[ObjAtkName2];
                }
                else if (!BonneNameList.Any(x => x.BonneName == ObjAtkName2))
                {
                    BonneNameList.Add((ObjAtkName2, (int)bwCharData.BaseStream.Length));

                    plAnm.ObjAtk2 = BitConverter.GetBytes(
                        (int)(baseOffset + bwCharData.BaseStream.Length));

                    Util.WriteFixedLenString(bwCharData, ObjAtkName2, OBJ_ATK_NAME_LEN);
                }
                else
                {
                    var existingBonne = BonneNameList.First(x => x.BonneName == ObjAtkName2);

                    plAnm.ObjAtk2 = BitConverter.GetBytes(
                        (int)(baseOffset + existingBonne.BonneNameOffs));
                }
            }

            int newAnmOffs = (int)(baseOffset + bwCharData.BaseStream.Length);
            for (int i = 0; i < charGen.AnmCount; i++)
            {
                Animation plAnm = Animation.Get(charIndex, i);
                byte[] AnmData = Animation.UpdateAllCharAnmPrm(charIndex, plAnm, false);
                bwCharANMData.Write(AnmData);
            }
            bwCharData.Write(msCharANMData.ToArray());

            int newAtkOffs = (int)(baseOffset + bwCharData.BaseStream.Length);
            for (int i = 0; i < charGen.AtkCount; i++)
            {
                List<byte> AtkData = new List<byte>();
                Attack.SerializeAtk(AtkData, Character.List[charIndex].Attacks[i], true);
                bwCharATKData.Write(AtkData.ToArray());
            }
            bwCharData.Write(msCharATKData.ToArray());
            Util.AlignBinaryWriter(bwCharData, ALIGNMENT);

            string charID = "";
            if (charIndex <= 0x5E)
            {
                int charIDTableOffset = Util.ReadProcessMemoryInt32(0x419550 + (charIndex * 4));
                charID = Util.ReadMemoryFixedLenString(charIDTableOffset, 0x4, '\0');
            }
            else
            {
                int charIDTableOffset = Util.ReadProcessMemoryInt32(0x954A30 + ((charIndex - 0x5E) * 4));
                charID = Util.ReadMemoryFixedLenString(charIDTableOffset, 0x4, '\0');
            }

            int newCharCCSOffs = (int)(baseOffset + bwCharData.BaseStream.Length);
            Util.WriteFixedLenString(bwCharData, $"2{charID}bod1.ccs", 0x10);
            int newCharDataHeaderOffs = (int)(baseOffset + bwCharData.BaseStream.Length);
            bwCharDataHeader.Write(charIndex);
            bwCharDataHeader.Write(-1);
            bwCharDataHeader.Write(newCharCCSOffs);
            bwCharDataHeader.Write(newCltOffset);
            bwCharDataHeader.Write(newTexOffset);
            bwCharDataHeader.Write(newMdlOffset);
            bwCharDataHeader.Write(newHolObjOffset);
            bwCharDataHeader.Write(newPrgOffset);

            bwCharDataHeader.Write(charGen.Unk);
            bwCharDataHeader.Write(charGen.Unk1);
            bwCharDataHeader.Write(charGen.AtkCount);
            bwCharDataHeader.Write(newAtkOffs);
            bwCharDataHeader.Write(0);
            bwCharDataHeader.Write(charGen.Unk2);

            bwCharDataHeader.Write(charGen.AnmCount);
            bwCharDataHeader.Write(newAnmOffs);
            bwCharDataHeader.Write(0);

            bwCharDataHeader.Write(charGen.AnmNameCount);
            bwCharDataHeader.Write(newAnmStringOffs);
            bwCharDataHeader.Write(0);

            bwCharDataHeader.Write(0);
            bwCharDataHeader.Write(0);

            bwCharDataHeader.Write(charGen.Height);
            bwCharDataHeader.Write(charGen.Width);
            bwCharDataHeader.Write(charGen.Speed);
            bwCharDataHeader.Write(charGen.RunningStartSpeed);
            bwCharDataHeader.Write(charGen.Slide);
            bwCharDataHeader.Write(charGen.Weight);
            bwCharDataHeader.Write(charGen.Gravity);
            bwCharDataHeader.Write(charGen.SpeedAir);
            bwCharDataHeader.Write(charGen.WallJmpRecoilDistance);
            bwCharDataHeader.Write(charGen.WallJmpDistanceLimit);
            bwCharDataHeader.Write(charGen.FirstJumpDelay);
            bwCharDataHeader.Write(charGen.FirstJumpHeight);
            bwCharDataHeader.Write(charGen.SecondJumpHeight);
            bwCharDataHeader.Write(charGen.DashDelay);
            bwCharDataHeader.Write(charGen.DashDuration);
            bwCharDataHeader.Write(charGen.DashSpeed);
            bwCharDataHeader.Write(charGen.DashDistance);
            bwCharDataHeader.Write(charGen.BackDashHeight);
            bwCharDataHeader.Write(charGen.BackDashWeight);
            bwCharDataHeader.Write(charGen.BackDashDistance);
            bwCharDataHeader.Write(charGen.BackDashHeight2);

            bwCharDataHeader.Write(charGen.AirAtkXAdjLimit);
            bwCharDataHeader.Write(charGen.AirAtkYAdjLimit);
            bwCharDataHeader.Write(charGen.Unk3);
            bwCharDataHeader.Write(charGen.Unk4);

            bwCharDataHeader.Write(charGen.Strength);
            bwCharDataHeader.Write(charGen.Defense);
            bwCharDataHeader.Write(charGen.DamageKnockback);
            bwCharDataHeader.Write(charGen.AttackKnockback);
            bwCharDataHeader.Write(charGen.StatusDuration);
            bwCharDataHeader.Write(charGen.QuantityProjectiles);
            bwCharDataHeader.Write(charGen.HealingMultiplier);
            bwCharDataHeader.Write(charGen.ChakraSpeed);
            bwCharDataHeader.Write(0);

            bwCharDataHeader.Write(newCharDataHeaderOffs);
            bwCharDataHeader.Write(0);
            bwCharDataHeader.Write(0);
            bwCharDataHeader.Write(0);

            bwCharDataHeader.Write(0);
            bwCharDataHeader.Write(0);
            bwCharDataHeader.Write((short)0);
            bwCharDataHeader.Write((short)-1);
            bwCharDataHeader.Write(1f);

            bwCharDataHeader.Write(15f);
            bwCharDataHeader.Write(0);
            bwCharDataHeader.Write(0);
            bwCharDataHeader.Write(0);

            bwCharDataHeader.Write(0);
            bwCharDataHeader.Write(0);
            bwCharDataHeader.Write(0);
            bwCharDataHeader.Write(1f);

            bwCharDataHeader.Write(0);
            bwCharDataHeader.Write(0);
            bwCharDataHeader.Write(0);
            bwCharDataHeader.Write(0);

            bwCharData.Write(msCharDataHeader.ToArray());

            File.WriteAllBytes(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CharacterData.BIN"),
                msCharData.ToArray());

            Util.WriteProcessMemoryBytes(baseOffset, msCharData.ToArray());

            if (charIndex <= CHAR_TABLE_SPLIT_INDEX)
            {
                int oldOffs = Util.ReadProcessMemoryInt32(CHAR_TABLE_LOW + (charIndex * CHAR_TABLE_ENTRY_SIZE) + CHAR_TABLE_OFFSET_FIELD);
                Util.WriteProcessMemoryBytes(oldOffs, msCharDataHeader.ToArray());
                Util.WriteProcessMemoryInt32(CHAR_TABLE_LOW + (charIndex * CHAR_TABLE_ENTRY_SIZE) + CHAR_TABLE_OFFSET_FIELD, newCharDataHeaderOffs);
            }
            else
            {
                int oldOffs = Util.ReadProcessMemoryInt32(CHAR_TABLE_HIGH + ((charIndex - CHAR_TABLE_SPLIT_INDEX) * CHAR_TABLE_ENTRY_SIZE) + CHAR_TABLE_OFFSET_FIELD);
                Util.WriteProcessMemoryBytes(oldOffs, msCharDataHeader.ToArray());
                Util.WriteProcessMemoryInt32(CHAR_TABLE_HIGH + ((charIndex - CHAR_TABLE_SPLIT_INDEX) * CHAR_TABLE_ENTRY_SIZE) + CHAR_TABLE_OFFSET_FIELD, newCharDataHeaderOffs);
            }
        }
    }
}