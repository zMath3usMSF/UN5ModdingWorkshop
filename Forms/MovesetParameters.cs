using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UN5ModdingWorkshop;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class MovesetParameters : Form
    {
        public static int p1IDFromForm1;
        public Rectangle button;

        public MovesetParameters()
        {
            InitializeComponent();
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            chkHitBoxCharPos1.CheckedChanged += ChkHitBoxCharPos1_CheckedChanged;
            chkHitBoxCharPos2.CheckedChanged += ChkHitBoxCharPos2_CheckedChanged;
        }

        private void ChkHitBoxCharPos2_CheckedChanged(object sender, EventArgs e)
        {
            txtHitBone2.Text = "";
            txtHitBone2.Enabled = chkHitBoxCharPos2.Checked ? false : true;
        }

        private void ChkHitBoxCharPos1_CheckedChanged(object sender, EventArgs e)
        {
            txtHitBone1.Text = "";
            txtHitBone1.Enabled = chkHitBoxCharPos1.Checked ? false : true;
        }

        public void VerifyOpenedELF(object sender, EventArgs e)
        {
            btnSaveELF.Enabled = GAME.openedELF;
            Util.VerifyCurrentPlayersIDs();
            if (BTL.P1ID != int.Parse(lblCharID2.Text))
            {
                btnUpdateP1.Enabled = false;
            }
            else
            {
                btnUpdateP1.Enabled = true;
            }
        }
        public void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int charID = int.Parse(lblCharID2.Text);
            int selectedIndex = listBox1.SelectedIndex;

            if (btnEditAtkParameters.Visible == false)
            {
                lblSelectedAtk2.Text = listBox1.SelectedIndex.ToString();

                PlAtk.SendTextAtk(charID, this, PlAtk.GetCharAtk(charID, selectedIndex));
            }
            else
            {
                int selectedAnm = int.Parse(listBox1.SelectedItem.ToString().Split(':')[0]);
                PlAnm.SendTextAnm(this, PlAnm.Get(charID, selectedAnm));
            }
        }

        public void UpdateLabels(string txtCharNameForm1, string charIDForm1)
        {
            lblCharName2.Text = txtCharNameForm1;
            lblCharID2.Text = charIDForm1;
            int.TryParse(charIDForm1, out int CharIDForm2Int);
            Util.VerifyCurrentPlayersIDs();

            p1IDFromForm1 = BTL.P1ID;

            if (p1IDFromForm1 == CharIDForm2Int)
            {
                btnUpdateP1.Enabled = true;
            }
            if (GAME.openedELF == true)
            {
                btnSaveELF.Enabled = true;
            }

            listBox1.SelectedIndex = 21;
        }

        private void btnUpdateP1_Click(object sender, EventArgs e)
        {
            if (btnEditAtkParameters.Visible == false)
            {
                int charID = int.Parse(lblCharID2.Text);
                int atkID = int.Parse(lblSelectedAtk2.Text);
                byte[] resultBytes = PlAtk.UpdateCharAtkPrm(this, charID, atkID);
                int selectedAtk = listBox1.SelectedIndex;
                PlAtk.UpdateP1Atk(resultBytes, selectedAtk, charID);
                PlAtk.SendTextAtk(charID, this, PlAtk.GetCharAtk(charID, selectedAtk));
            }
            else
            {
                int charID = int.Parse(lblCharID2.Text);
                byte[] resultBytes = PlAnm.UpdateCharAnmPrm(this, charID);
                int selectedAnm = int.Parse(listBox1.SelectedItem.ToString().Split(':')[0]);
                PlAnm.UpdateP1Anm(resultBytes, selectedAnm, charID);
                PlAnm.SendTextAnm(this, PlAnm.Get(charID, selectedAnm));
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if(btnEditAtkParameters.Visible == false)
            {
                int charID = int.Parse(lblCharID2.Text);
                int selectedIndex = listBox1.SelectedIndex;
                var charAtkPrm = PlAtk.CharAtkPrmBkp[charID][selectedIndex];
                PlAtk.SendTextAtk(charID, this, charAtkPrm);
            }
            else
            {
                chkHitBoxCharPos1.CheckedChanged -= ChkHitBoxCharPos1_CheckedChanged;
                chkHitBoxCharPos2.CheckedChanged -= ChkHitBoxCharPos2_CheckedChanged;
                int charID = int.Parse(lblCharID2.Text);
                int selectedAnm = int.Parse(listBox1.SelectedItem.ToString().Split(':')[0]);
                var charAnmPrm = PlAnm.PlAnmPrmBkp[charID][selectedAnm];
                PlAnm.SendTextAnm(this, charAnmPrm);
                chkHitBoxCharPos1.CheckedChanged += ChkHitBoxCharPos1_CheckedChanged;
                chkHitBoxCharPos2.CheckedChanged += ChkHitBoxCharPos2_CheckedChanged;
            }
        }

        private void btnSaveELF_Click(object sender, EventArgs e)
        {
            if (btnEditAtkParameters.Visible == false)
            {
                int charID = int.Parse(lblCharID2.Text);
                byte[] resultBytes = PlAtk.UpdateAllCharAtkPrm(this, charID);
                PlAtk.WriteELFCharAtk(resultBytes, charID);
            }
            else
            {
                int charID = int.Parse(lblCharID2.Text);
                byte[] resultBytes = PlAnm.UpdateAllCharAnmPrm(this, charID);
                PlAnm.WriteELFCharAnm(resultBytes, charID);
            }
        }

        private void btnEditAnmParameters_Click(object sender, EventArgs e)
        {
            pnlAtkPrm.Visible = false;
            pnlAnmPrm.Visible = true;
            listBox1.Items.Clear();
            btnEditAnmParameters.Visible = false;
            btnEditAtkParameters.Visible = true;
            AddToListBox();
            listBox1.SelectedIndex = 0;
        }

        private void AddToListBox()
        {
            int SelectedAtk = int.Parse(lblSelectedAtk2.Text);
            int currentCharID = int.Parse(lblCharID2.Text);

            int AtkAnmBlock = (int)PlAtk.CharAtkPrm[currentCharID][SelectedAtk].AnimationIdx;

            for (int i = AtkAnmBlock; i < PlGen.List[currentCharID].AnmCount; i++)
            {
                int AnmID = PlAnm.Get(currentCharID, i).AnmID;

                if (-1 != AnmID)
                {
                    listBox1.Items.Add($"{i}: {PlAnm.GetPlAnmName(currentCharID, AnmID)}");
                }
                else
                {
                    break;
                }
            }
        }

        private void btnEditAtkParameters_Click(object sender, EventArgs e)
        {
            pnlAnmPrm.Visible = false;
            pnlAtkPrm.Visible = true;
            listBox1.Items.Clear();
            int currentCharID = int.Parse(lblCharID2.Text);
            string currentCharName = lblCharName2.Text;
            PlAtk.AddCharComboList(this, currentCharID, currentCharName);
            listBox1.SelectedIndex = int.Parse(lblSelectedAtk2.Text);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void lblDpad_Click(object sender, EventArgs e)
        {

        }

        private void grpAttackParameters_Enter(object sender, EventArgs e)
        {

        }

        private void numAnmStartHitFrame_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cmbDuration_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbDuration.SelectedIndex == 0)
            {
                numDuration.Value = 0;
                numDuration.Enabled = true;
            }
            else
            {
                numDuration.Value = 0;
                numDuration.Enabled = false;
            }
        }

        private void lblCharXDistance_Click(object sender, EventArgs e)
        {

        }

        private void numCharXDistance_ValueChanged(object sender, EventArgs e)
        {

        }

        private void lblCharYDistance_Click(object sender, EventArgs e)
        {

        }

        private void numCharYDistance_ValueChanged(object sender, EventArgs e)
        {

        }

        private void lblAnmHitBoxXPos2_Click(object sender, EventArgs e)
        {

        }

        private void numHitBoxXPos2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numAnmUnk8_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numAnmUnk7_ValueChanged(object sender, EventArgs e)
        {

        }

        private void lblAnmUnk7_Click(object sender, EventArgs e)
        {

        }

        private void lblAnmUnk8_Click(object sender, EventArgs e)
        {

        }

        private void lblAnmEndHitFrame2_Click(object sender, EventArgs e)
        {

        }

        private void numAnmEndHitFrame2_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
