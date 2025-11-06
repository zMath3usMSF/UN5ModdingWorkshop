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
    public partial class JutsuParameters : Form
    {
        int p1IDFromForm1 = 0;
        public JutsuParameters()
        {
            InitializeComponent();
            listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
            cmbSelectedGroup.SelectedIndexChanged +=CmbSelectedGroup_SelectedIndexChanged;
        }

        private void CmbSelectedGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            int charID = int.Parse(lblCharID2.Text);
            int selectedJutsu = int.Parse(listBox1.SelectedItem.ToString().Split(':')[0].Trim());
            int selectedGroup = cmbSelectedGroup.SelectedIndex;
            List<CharSkl> charSklPrm = CharSkl.GetCharSklPrm(selectedJutsu);

            CharSkl.SendTextSkl(this, charSklPrm[selectedGroup], selectedJutsu, charID);
        }

        public void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedJutsu = int.Parse(listBox1.SelectedItem.ToString().Split(':')[0].Trim());
            cmbSelectedGroup.Items.Clear();
            List<CharSkl> var = CharSkl.GetCharSklPrm(selectedJutsu);
            cmbSelectedGroup.Items.AddRange(Enumerable.Range(1, var.Count).Select(x => x.ToString()).ToArray());
            cmbSelectedGroup.SelectedIndex = 0;
            cmbSwitchToJutsu.SelectedIndex = selectedJutsu;
            lblSelectedAwakening2.Text = selectedJutsu.ToString();
        }
        public void AddToListBox(int charID)
        {
            var selectedJutsus = CharSkl.GetCharSelectedJutsu(charID);
            if(cmbSwitchToJutsu.Items.Count == 0)
            {
                CharSkl.ReadAllJutsuName();
                cmbSwitchToJutsu.Items.AddRange(CharSkl.charJutsuNameList.ToArray());
            }
            listBox1.Items.Add($"{selectedJutsus.selectedJutsu1}: {CharSkl.charJutsuNameList[selectedJutsus.selectedJutsu1]}");
            listBox1.Items.Add($"{selectedJutsus.selectedJutsu2}: {CharSkl.charJutsuNameList[selectedJutsus.selectedJutsu2]}");
            listBox1.SelectedIndex = 0;
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
        }

        private void btnUpdateP1_Click(object sender, EventArgs e)
        {
            int charID = int.Parse(lblCharID2.Text);
            int sklGroup = cmbSelectedGroup.SelectedIndex;
            int sklID = int.Parse(lblSelectedAwakening2.Text);
            byte[] sklBlock  = CharSkl.UpdateCharSklPrm(this, CharSkl.CharSklPrm[sklID][sklGroup]);
            CharSkl.WriteCharSkl(sklBlock, sklID);
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
