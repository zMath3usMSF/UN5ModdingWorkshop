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
    public partial class InfoADV : Form
    {
        bool debug = false;
        bool foundCameraInfoOffs = false;
        int CameraInfoOffs = Util.ReadProcessMemoryInt32(GAME.Global_Pointer + 0x16C) - 0x500;
        public InfoADV()
        {
            InitializeComponent();
        }

        private void UpdateInfo()
        {
            int PlayerInfoOffs = Util.ReadProcessMemoryInt32(GAME.Global_Pointer + 0x1A0);
            float PlayerPosX = Util.ReadProcessMemoryFloat(PlayerInfoOffs + 0x10);
            float PlayerPosY = Util.ReadProcessMemoryFloat(PlayerInfoOffs + 0x14);
            float PlayerPosZ = Util.ReadProcessMemoryFloat(PlayerInfoOffs + 0x18);
            float PlayerRotZ = Util.ReadProcessMemoryFloat(PlayerInfoOffs + 0x28) * (180f / (float)Math.PI);
            textBox1.Text = $"{Convert.ToInt32(PlayerPosX)} {Convert.ToInt32(PlayerPosY)} {Convert.ToInt32(PlayerPosZ)} {Convert.ToInt32(PlayerRotZ)}";

            while(foundCameraInfoOffs == false)
            {
                int currentValue = Util.ReadProcessMemoryInt32(CameraInfoOffs);
                if (currentValue == 0x60DD00)
                {
                    foundCameraInfoOffs=true;
                    break;
                }
                CameraInfoOffs += 4;
            }
            float CameraPosX = Util.ReadProcessMemoryFloat(CameraInfoOffs + 0x10);
            float CameraPosY = Util.ReadProcessMemoryFloat(CameraInfoOffs + 0x14);
            float CameraPosZ = Util.ReadProcessMemoryFloat(CameraInfoOffs + 0x18);
            float CameraRotX = Util.ReadProcessMemoryFloat(CameraInfoOffs + 0x30);
            float CameraRotY = Util.ReadProcessMemoryFloat(CameraInfoOffs + 0x34);
            float CameraRotZ = Util.ReadProcessMemoryFloat(CameraInfoOffs + 0x38);
            textBox2.Text = $"{Convert.ToInt32(CameraPosX)} {Convert.ToInt32(CameraPosY)} {Convert.ToInt32(CameraPosZ)} {Convert.ToInt32(CameraRotX)} {Convert.ToInt32(CameraRotY)} {Convert.ToInt32(CameraRotZ)}";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateInfo();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox2.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (debug == false)
            {
                debug = true;
                button3.Text = "Change to Shuriken Camera";
                Util.WriteProcessMemoryInt32(0x60DD20, 0x724AF0);
            }
            else
            {
                debug = false;
                button3.Text = "Change to Debug Camera";
                Util.WriteProcessMemoryInt32(0x60DD20, 0x724410);
            }
        }
    }
}
