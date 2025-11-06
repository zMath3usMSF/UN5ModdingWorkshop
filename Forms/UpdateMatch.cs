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
    public partial class UpdateMatch : Form
    {
        Main mainInstance = new Main();
        public UpdateMatch(Main main)
        {
            InitializeComponent();
            mainInstance = main;
        }

        public void SendText(bool isP1)
        {
            int PlayerOffset = isP1 == true ? 0xBD7AAC + GAME.memoryDif : 0xBD7AD4 + GAME.memoryDif;
            int PlayerID = Util.ReadProcessMemoryInt8(PlayerOffset);
            if (cmbCharList.Items.Count == 0)
            {
                for (int i = 0; i <= 93; i++)
                {
                    if (!GAME.charInvalid.Contains(i))
                    {
                        cmbCharList.Items.Add($"{i}: {BTL.charNameList[i]}");
                    }
                }
            }
            for (int i = 0; i < cmbCharList.Items.Count; i++)
            {
                string[] teste = cmbCharList.Items[i].ToString().Split(':');
                if (teste[0] == PlayerID.ToString())
                {
                    cmbCharList.SelectedIndex = i;
                }
            }

            byte[] MapIDByte = new byte[1];
            PCSX2Process.ReadProcessMemory(PCSX2Process.processHandle, (IntPtr)(0xBD7AF8 + GAME.eeAddress + (ulong)GAME.memoryDif), MapIDByte, MapIDByte.Length, out var none5);
            int MapID = MapIDByte[0];

            if (cmbMapList.Items.Count == 0)
            {
                for (int i = 0; i < 24; i++)
                {
                    cmbMapList.Items.Add($"{i}: " + BTL.GetMapName(i));
                }
            }
            cmbMapList.SelectedIndex = MapID;
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            bool isP1;
            string[] PlayerIDString = cmbCharList.SelectedItem.ToString().Split(':');
            int PlayerID = int.Parse(PlayerIDString[0]);
            int MapID = cmbMapList.SelectedIndex;

            isP1 = lblPlayerID.Text == "P1:" ? true : false;

            BTL.UpdateMatch(isP1, PlayerID, MapID);
        }
    }
}
