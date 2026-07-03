using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UN5ModdingWorkshop
{
    public partial class CheatsMenu : Form
    {
        public CheatsMenu()
        {
            InitializeComponent();
            CheckNTSCMode();
            CheckNoLinkedCharacter();
        }

        private void CheckNTSCMode()
        {
            int videoMode = Util.ReadProcessMemoryInt16(0x106750);
            if(videoMode == 0x2) //NTSC
            {
                chkNTSCMode.Enabled = false;
                chkNTSCMode.Checked = true;
            }
            else //PAL
            {
                chkNTSCMode.Checked = false;
            }
        }

        private void CheckNoLinkedCharacter()
        {
            int linkedMode = Util.ReadProcessMemoryInt8(0x8E9576);
            if (linkedMode == 0x25) //Has No Linked Character
            {
                chkNoLinkedCharacter.Checked = true;
            }
            else
            {
                chkNoLinkedCharacter.Checked = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            WriteNTSCMode();
            WriteNoLinkedCharacter();
        }

        private void WriteNTSCMode()
        {
            if (chkNTSCMode.Checked && chkNTSCMode.Enabled)
            {
                var (elfBytes, btlBytes) = GAME.ApplyPnachPatch(@"cheats\NTSCMode.pnach");
                File.WriteAllBytes(GAME.elfPath, elfBytes);
                File.WriteAllBytes(GAME.btlPath, btlBytes);
            }
        }

        private void WriteNoLinkedCharacter()
        {
            if (chkNoLinkedCharacter.Checked)
            {
                var (elfBytes, btlBytes) = GAME.ApplyPnachPatch(@"cheats\NoLinkedCharacter_Enable.pnach");
                File.WriteAllBytes(GAME.elfPath, elfBytes);
                File.WriteAllBytes(GAME.btlPath, btlBytes);
            }
        }
    }
}
