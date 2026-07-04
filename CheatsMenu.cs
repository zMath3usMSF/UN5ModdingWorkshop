using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
            CheckNoExtraHit();
            CheckNoJankenpon();
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

        private void CheckNoExtraHit()
        {
            int extraHitMode = Util.ReadProcessMemoryInt32(0x243918);
            if (extraHitMode == 0x10430015) //Has No Extra Hit
            {
                chkNoExtraHit.Checked = true;
            }
            else
            {
                chkNoExtraHit.Checked = false;
            }
        }

        private void CheckNoJankenpon()
        {
            int jankenponMode = Util.ReadProcessMemoryInt32(0x24DD58);
            if (jankenponMode == 0x10000088) //Has No Jankenpon
            {
                chkNoJankenpon.Checked = true;
            }
            else
            {
                chkNoJankenpon.Checked = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            WriteNTSCMode();
            WriteNoLinkedCharacter();
            WriteNoExtraHit();
            WriteNoJankenpon();

            MessageBox.Show("Cheat changes saved. restart the game for them to take effect.");
            this.Close();
        }

        private void WriteNTSCMode()
        {
            if (chkNTSCMode.Checked && chkNTSCMode.Enabled)
            {
                var (elfBytes, btlBytes) = GAME.ApplyPnachPatch(@"cheats\NTSCMode.pnach");
                File.WriteAllBytes(GAME.elfPath, elfBytes);
                File.WriteAllBytes(GAME.btlPath, btlBytes);

                DialogResult result = MessageBox.Show(
                    "When switching the video mode from PAL to NTSC, the game's frame rate changes, " +
                    "causing the Ultimate Jutsus to become desynchronized from their audio.\n\n" +
                    "To fix this, replace the Ultimate Jutsu files in the \"STR\" folder " +
                    "(UN5/DATA/ROFS/STR) with the ones from Accel 2.\n\n" +
                    "Do you want to download the Accel 2 STR files now?",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://drive.google.com/file/d/1Cnnu4J8zhBg-AW3mjXDPbc9_airIXkRu/view?usp=sharing",
                        UseShellExecute = true
                    });
                }
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
            else
            {
                var (elfBytes, btlBytes) = GAME.ApplyPnachPatch(@"cheats\NoLinkedCharacter_Disable.pnach");
                File.WriteAllBytes(GAME.elfPath, elfBytes);
                File.WriteAllBytes(GAME.btlPath, btlBytes);
            }
        }

        private void WriteNoExtraHit()
        {
            if (chkNoExtraHit.Checked)
            {
                var (elfBytes, btlBytes) = GAME.ApplyPnachPatch(@"cheats\NoExtraHit_Enable.pnach");
                File.WriteAllBytes(GAME.elfPath, elfBytes);
                File.WriteAllBytes(GAME.btlPath, btlBytes);
            }
            else
            {
                var (elfBytes, btlBytes) = GAME.ApplyPnachPatch(@"cheats\NoExtraHit_Disable.pnach");
                File.WriteAllBytes(GAME.elfPath, elfBytes);
                File.WriteAllBytes(GAME.btlPath, btlBytes);
            }
        }

        private void WriteNoJankenpon()
        {
            if (chkNoJankenpon.Checked)
            {
                var (elfBytes, btlBytes) = GAME.ApplyPnachPatch(@"cheats\NoJankenpon_Enable.pnach");
                File.WriteAllBytes(GAME.elfPath, elfBytes);
                File.WriteAllBytes(GAME.btlPath, btlBytes);
            }
            else
            {
                var (elfBytes, btlBytes) = GAME.ApplyPnachPatch(@"cheats\NoJankenpon_Disable.pnach");
                File.WriteAllBytes(GAME.elfPath, elfBytes);
                File.WriteAllBytes(GAME.btlPath, btlBytes);
            }
        }
    }
}
