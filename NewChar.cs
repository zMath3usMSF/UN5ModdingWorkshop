using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UN5ModdingWorkshop
{
    public partial class NewChar : Form
    {
        public NewChar()
        {
            InitializeComponent();
            textBox1.KeyPress += TextBox1_KeyPress;
            textBox1.TextChanged += TextBox1_TextChanged;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            string charID = textBox1.Text;
            textBox2.Text = "CLT_2" + charID + "body";
            textBox3.Text = "TEX_2" + charID + "body";
            textBox4.Text = "MDL_2" + charID + "00t0 body";
            textBox5.Text = "OBJ_eff_dummy_" + charID + "hol0";
            textBox7.Text = "1" + charID.ToUpper() + "BOD1.CCS";
            textBox6.Text = "2" + charID.ToUpper() + "BOD1.CCS";
            textBox8.Text = "3" + charID.ToUpper() + "3EYE.CCS";
            textBox9.Text = "3" + charID.ToUpper() + "3PCT.CCS";
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verifica se a tecla pressionada é um número (0 a 9)
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Bloqueia o caractere, não deixa aparecer no TextBox
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show("Character ID cannot be null!");
                return;
            }
            string charID = textBox1.Text;
            textBox2.Text = "CLT_2" + charID + "body";
            textBox3.Text = "TEX_2" + charID + "body";
            textBox4.Text = "MDL_2" + charID + "00t0 body";
            textBox5.Text = "OBJ_eff_dummy_" + charID + "hol0";
            textBox7.Text = "1" + charID.ToUpper() + "BOD1.CCS";
            textBox6.Text = "2" + charID.ToUpper() + "BOD1.CCS";
            textBox8.Text = "3" + charID.ToUpper() + "3EYE.CCS";
            textBox9.Text = "3" + charID.ToUpper() + "3PCT.CCS";
        }
    }
}
