using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace UN5ModdingWorkshop
{
    public partial class AwakeningParameters : Form
    {
        public static int p1IDFromForm1;
        public AwakeningParameters()
        {
            InitializeComponent();
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
        }
        public void VerifyOpenedELF(object sender, EventArgs e)
        {
            btnSaveELF.Enabled = GAME.openedELF;
            if (BTL.P1ID != int.Parse(lblCharID2.Text))
            {
                btnUpdateP1.Enabled = false;
            }
            else
            {
                btnUpdateP1.Enabled = true;
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

            listBox1.SelectedIndex = 0;
        }
        public void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int charID = int.Parse(lblCharID2.Text);
            int selectedIndex = listBox1.SelectedIndex;
            int selectedAwk = Convert.ToInt32(listBox1.SelectedItem.ToString().Split(':')[0]);

            lblSelectedAwakening2.Text = listBox1.SelectedItem.ToString().Split(':')[0];
            cmbSwitchToAwakening.SelectedIndexChanged -= cmbSwitchToAwakening_SelectedIndexChanged;
            PlAwk.SendTextAwk(this, PlAwk.GetCharAwk(selectedAwk, false), selectedAwk, charID);
            cmbSwitchToAwakening.SelectedIndexChanged += cmbSwitchToAwakening_SelectedIndexChanged;
        }
        public void cmbSwitchToAwakening_SelectedIndexChanged(object sender, EventArgs e)
        {
            int charID = int.Parse(lblCharID2.Text);
            int selectedIndex = listBox1.SelectedIndex;
            int selectedAwk = cmbSwitchToAwakening.SelectedIndex;

            listBox1.SelectedIndexChanged -= listBox1_SelectedIndexChanged;
            lblSelectedAwakening2.Text = Convert.ToString(cmbSwitchToAwakening.SelectedIndex);
            listBox1.Items[selectedIndex] = $"{selectedAwk}: Char Awakening {selectedIndex + 1}";
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            PlAwk.SendTextAwk(this, PlAwk.GetCharAwk(selectedAwk, false), selectedAwk, charID);
        }

        private void btnUpdateP1_Click(object sender, EventArgs e)
        {
            int charID = int.Parse(lblCharID2.Text);
            int selectedAwk = Convert.ToInt32(listBox1.SelectedItem.ToString().Split(':')[0]);
            int awkPos = listBox1.SelectedIndex;
            var result = PlAwk.UpdateCharAwkPrm(this, selectedAwk, charID, false);
            PlAwk.UpdateP1AwkPrm(result.charAwkPrmBlock, result.charAwkAct, selectedAwk, charID, awkPos);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            int charID = int.Parse(lblCharID2.Text);
            int selectedAwk = Convert.ToInt32(listBox1.SelectedItem.ToString().Split(':')[0]);
            int awkPos = listBox1.SelectedIndex;
            PlAwk.SendTextAwk(this, PlAwk.GetCharAwk(selectedAwk, true), selectedAwk, charID);
            var result = PlAwk.UpdateCharAwkPrm(this, selectedAwk, charID, true);
            PlAwk.UpdateP1AwkPrm(result.charAwkPrmBlock, result.charAwkAct, selectedAwk, charID, awkPos);
        }

        private void btnSaveELF_Click(object sender, EventArgs e)
        {
            int charID = int.Parse(lblCharID2.Text);
            int selectedAwk = Convert.ToInt32(listBox1.SelectedItem.ToString().Split(':')[0]);
            int awkPos = listBox1.SelectedIndex;
            var result = PlAwk.UpdateCharAwkPrm(this, selectedAwk, charID, false);
            PlAwk.WriteELFAwkPrm(result.charAwkPrmBlock, result.charAwkAct, selectedAwk, charID, awkPos);
        }
    }
}
