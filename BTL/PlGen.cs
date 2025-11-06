using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace UN5ModdingWorkshop
{
    public class PlGen
    {
        public static List<PlGen> CharGenPrm = new List<PlGen>();
        public static List<PlGen> CharGenPrmBkp = new List<PlGen>();
        #region GeneralParameters

        public float Width, Height, Speed, RunningStartSpeed, Slide, Weight, Gravity, SpeedAir, WallJmpRecoilDistance, WallJmpDistanceLimit, 
                     FirstJumpHeight, SecondJumpHeight,  DashSpeed, DashDistance, BackDashHeight, BackDashWeight, BackDashDistance, 
                     BackDashHeight2, AirAtkXAdjLimit, AirAtkYAdjLimit, Strength, Defense, DamageKnockback, AttackKnockback, StatusDuration, QuantityProjectiles, 
                     HealingMultiplier, ChakraSpeed;

        public int ID, cUnk1, cUnk2, AtkCount, AnmCount, AnmNameCount, AcessoryCount, FirstJumpDelay, DashDelay, DashDuration;

        public byte[] NameOffset, CCSOffset, CLTOffset, TEXOffset, MDLOffset, HolOBJOffset, PrgOffset, AtkListOffset, AtkListMemOffset, 
                      AnmListOffset, AnmListMemOffset, AnmNameListOffset, AnmNameListMemOffset, AccessoryListOffset;

        public float Unk, Unk1, Unk2, Unk3, Unk4, Unk5, Unk6;

        #endregion
        internal static PlGen ReadCharGenPrm(byte[] Input) => new PlGen
        {
            ID = (int)Input.ReadUInt(0x0, 32),
            NameOffset = Input.ReadBytes(0x4, 4),
            CCSOffset = Input.ReadBytes(0x8, 4),
            CLTOffset = Input.ReadBytes(0xC, 4),
            TEXOffset = Input.ReadBytes(0x10, 4),
            MDLOffset = Input.ReadBytes(0x14, 4),
            HolOBJOffset = Input.ReadBytes(0x18, 4),
            PrgOffset = Input.ReadBytes(0x1C, 4),
            Unk = Input.ReadSingle(0x20),
            Unk1 = Input.ReadSingle(0x24),
            AtkCount = (int)Input.ReadUInt(0x28, 32),
            AtkListOffset = Input.ReadBytes(0x2C, 4),
            AtkListMemOffset = Input.ReadBytes(0x30, 4),
            Unk2 = Input.ReadSingle(0x34),
            AnmCount = (int)Input.ReadUInt(0x38, 32),
            AnmListOffset = Input.ReadBytes(0x3C, 4),
            AnmListMemOffset = Input.ReadBytes(0x40, 4),
            AnmNameCount = (int)Input.ReadUInt(0x44, 32),
            AnmNameListOffset = Input.ReadBytes(0x48, 4),
            AnmNameListMemOffset = Input.ReadBytes(0x4C, 4),
            AcessoryCount = (int)Input.ReadUInt(0x50, 32),
            AccessoryListOffset =  Input.ReadBytes(0x54, 4),

            Height = Input.ReadSingle(0x58),
            Width = Input.ReadSingle(0x5c),
            Speed = Input.ReadSingle(0x60),
            RunningStartSpeed = Input.ReadSingle(0x64),
            Slide = Input.ReadSingle(0x68),
            Weight = Input.ReadSingle(0x6c),
            Gravity = Input.ReadSingle(0x70),
            SpeedAir = Input.ReadSingle(0x74),
            WallJmpRecoilDistance = Input.ReadSingle(0x78),
            WallJmpDistanceLimit = Input.ReadSingle(0x7C),
            FirstJumpDelay = (int)Input.ReadUInt(0x80, 32),
            FirstJumpHeight = Input.ReadSingle(0x84),
            SecondJumpHeight = Input.ReadSingle(0x88),
            DashDelay = (int)Input.ReadUInt(0x8c, 32),
            DashDuration = (int)Input.ReadUInt(0x90, 32),
            DashSpeed = Input.ReadSingle(0x94),
            DashDistance = Input.ReadSingle(0x98),
            BackDashHeight = Input.ReadSingle(0x9c),
            BackDashWeight = Input.ReadSingle(0xa0),
            BackDashDistance = Input.ReadSingle(0xa4),
            BackDashHeight2 = Input.ReadSingle(0xa8),

            AirAtkXAdjLimit = Input.ReadSingle(0xac),
            AirAtkYAdjLimit = Input.ReadSingle(0xb0),
            Unk3 = Input.ReadSingle(0xb4),
            Unk4 = Input.ReadSingle(0xb8),

            Strength = Input.ReadSingle(0xbc),
            Defense = Input.ReadSingle(0xc0),
            DamageKnockback = Input.ReadSingle(0xc4),
            AttackKnockback = Input.ReadSingle(0xc8),
            StatusDuration = Input.ReadSingle(0xcc),
            QuantityProjectiles = Input.ReadSingle(0xd0),
            HealingMultiplier = Input.ReadSingle(0xd4),
            ChakraSpeed = Input.ReadSingle(0xd8)
        };
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public static void UpdateP1GenPrm(byte[] charGeneralDataBlock, int charID)
        {
            int P1AtributtesOffs = Util.BTL_GetPlayer1MemoryOffs() + 0x8C;

            Util.WriteProcessMemoryBytes(P1AtributtesOffs + 0x58, charGeneralDataBlock);
            Util.WriteProcessMemoryBytes(BTL.charMainAreaOffsets[charID] + 0x58, charGeneralDataBlock);
        }
        public static void WriteELFCharPrm(byte[] resultBytes, int charID)
        {
            if (!File.Exists(GAME.caminhoELF))
            {
                MessageBox.Show("Unable to save, check if the file has been deleted or moved.", string.Empty, MessageBoxButtons.OK);
            }
            else
            {
                using (FileStream fs = new FileStream(GAME.caminhoELF, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    int skipChars = charID * 0x8 + 0x4;
                    int mainAreaOffset = GAME.isUN6 == true ? 0x317E80 : 0x4ACA40;
                    fs.Seek(mainAreaOffset + skipChars, SeekOrigin.Begin);

                    byte[] charMainPointer = new byte[4];
                    fs.Read(charMainPointer, 0, charMainPointer.Length);
                    int charMainOffset = BitConverter.ToInt32(charMainPointer, 0);
                    int subValue = 0xFFE80;
                    charMainOffset = charMainOffset - subValue + 0x58;

                    fs.Seek(charMainOffset, SeekOrigin.Begin);
                    fs.Write(resultBytes, 0, resultBytes.Length);

                    MessageBox.Show("The changes were saved successfully!", string.Empty, MessageBoxButtons.OK);
                }
            }
        }
        public static byte[] UpdateCharGenPrm(GeneralParameters genForm, int charID)
        {
            List<byte> resultBytesList = new List<byte>();

            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharHeight.Text)));
            CharGenPrm[charID].Height = Convert.ToSingle(genForm.txtCharHeight.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharWidth.Text)));
            CharGenPrm[charID].Width = Convert.ToSingle(genForm.txtCharWidth.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharSpeed.Text)));
            CharGenPrm[charID].Speed = Convert.ToSingle(genForm.txtCharSpeed.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtRunningStartSpeed.Text)));
            CharGenPrm[charID].RunningStartSpeed = Convert.ToSingle(genForm.txtRunningStartSpeed.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharSlide.Text)));
            CharGenPrm[charID].Slide = Convert.ToSingle(genForm.txtCharSlide.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharWeight.Text)));
            CharGenPrm[charID].Weight = Convert.ToSingle(genForm.txtCharWeight.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharGravity.Text)));
            CharGenPrm[charID].Gravity = Convert.ToSingle(genForm.txtCharGravity.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharSpeedAir.Text)));
            CharGenPrm[charID].SpeedAir = Convert.ToSingle(genForm.txtCharSpeedAir.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtWallJmpRecoilDistance.Text)));
            CharGenPrm[charID].WallJmpRecoilDistance = Convert.ToSingle(genForm.txtWallJmpRecoilDistance.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtWallJmpDistanceLimit.Text)));
            CharGenPrm[charID].WallJmpDistanceLimit = Convert.ToSingle(genForm.txtWallJmpDistanceLimit.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToInt32(genForm.txtCharFirstJumpDelay.Text)));
            CharGenPrm[charID].FirstJumpDelay = Convert.ToInt32(genForm.txtCharFirstJumpDelay.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharFirstJumpHeight.Text)));
            CharGenPrm[charID].FirstJumpHeight = Convert.ToSingle(genForm.txtCharFirstJumpHeight.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharSecondJumpHeight.Text)));
            CharGenPrm[charID].SecondJumpHeight = Convert.ToSingle(genForm.txtCharSecondJumpHeight.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToInt32(genForm.txtCharDashDelay.Text)));
            CharGenPrm[charID].DashDelay = Convert.ToInt32(genForm.txtCharDashDelay.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToInt32(genForm.txtCharDashDuration.Text)));
            CharGenPrm[charID].DashDuration = Convert.ToInt32(genForm.txtCharDashDuration.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharDashSpeed.Text)));
            CharGenPrm[charID].DashSpeed = Convert.ToSingle(genForm.txtCharDashSpeed.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharDashDistance.Text)));
            CharGenPrm[charID].DashDistance = Convert.ToSingle(genForm.txtCharDashDistance.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharBackDashHeight.Text)));
            CharGenPrm[charID].BackDashHeight = Convert.ToSingle(genForm.txtCharBackDashHeight.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharBackDashWeight.Text)));
            CharGenPrm[charID].BackDashWeight = Convert.ToSingle(genForm.txtCharBackDashWeight.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharBackDashDistance.Text)));
            CharGenPrm[charID].BackDashDistance = Convert.ToSingle(genForm.txtCharBackDashDistance.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharBackDashHeight2.Text)));
            CharGenPrm[charID].BackDashHeight2 = Convert.ToSingle(genForm.txtCharBackDashHeight2.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtAirAtkXAdjLimit.Text)));
            CharGenPrm[charID].AirAtkXAdjLimit = Convert.ToSingle(genForm.txtAirAtkXAdjLimit.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtAirAtkYAdjLimit.Text)));
            CharGenPrm[charID].AirAtkYAdjLimit = Convert.ToSingle(genForm.txtAirAtkYAdjLimit.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(PlGen.CharGenPrm[charID].Unk3));
            resultBytesList.AddRange(BitConverter.GetBytes(PlGen.CharGenPrm[charID].Unk4));
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharStrength.Text)));
            CharGenPrm[charID].Strength = Convert.ToSingle(genForm.txtCharStrength.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharDefense.Text)));
            CharGenPrm[charID].Defense = Convert.ToSingle(genForm.txtCharDefense.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharDamageKnockback.Text)));
            CharGenPrm[charID].DamageKnockback = Convert.ToSingle(genForm.txtCharDamageKnockback.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharAttackKnockback.Text)));
            CharGenPrm[charID].AttackKnockback = Convert.ToSingle(genForm.txtCharAttackKnockback.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharStatus.Text)));
            CharGenPrm[charID].StatusDuration = Convert.ToSingle(genForm.txtCharStatus.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharQuantityProjectiles.Text)));
            CharGenPrm[charID].QuantityProjectiles = Convert.ToSingle(genForm.txtCharQuantityProjectiles.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharHealingMultiplier.Text)));
            CharGenPrm[charID].HealingMultiplier = Convert.ToSingle(genForm.txtCharHealingMultiplier.Text);
            resultBytesList.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.txtCharChakraRecoverySpeed.Text)));
            CharGenPrm[charID].ChakraSpeed = Convert.ToSingle(genForm.txtCharChakraRecoverySpeed.Text);

            byte [] resultBytes = resultBytesList.ToArray();
            return resultBytes;
        }
        public static void SendTextToGenForm(GeneralParameters genForm, PlGen charGenPrm)
        {
            genForm.txtCharHeight.Text = ($"{charGenPrm.Height}");
            genForm.txtCharWidth.Text = ($"{charGenPrm.Width}");
            genForm.txtCharSpeed.Text = ($"{charGenPrm.Speed}");
            genForm.txtRunningStartSpeed.Text = $"{charGenPrm.RunningStartSpeed}";
            genForm.txtCharSlide.Text = ($"{charGenPrm.Slide}");
            genForm.txtCharWeight.Text = ($"{charGenPrm.Weight}");
            genForm.txtCharGravity.Text = ($"{charGenPrm.Gravity}");
            genForm.txtCharSpeedAir.Text = ($"{charGenPrm.SpeedAir}");
            genForm.txtWallJmpRecoilDistance.Text = $"{charGenPrm.WallJmpRecoilDistance}";
            genForm.txtWallJmpDistanceLimit.Text = $"{charGenPrm.WallJmpDistanceLimit}";
            genForm.txtCharFirstJumpDelay.Text = ($"{charGenPrm.FirstJumpDelay}");
            genForm.txtCharFirstJumpHeight.Text = ($"{charGenPrm.FirstJumpHeight}");
            genForm.txtCharSecondJumpHeight.Text = ($"{charGenPrm.SecondJumpHeight}");
            genForm.txtCharDashDelay.Text = ($"{charGenPrm.DashDelay}");
            genForm.txtCharDashDuration.Text = ($"{charGenPrm.DashDuration}");
            genForm.txtCharDashSpeed.Text = ($"{charGenPrm.DashSpeed}"); ;
            genForm.txtCharDashDistance.Text = ($"{charGenPrm.DashDistance}");
            genForm.txtCharBackDashHeight.Text = ($"{charGenPrm.BackDashHeight}");
            genForm.txtCharBackDashWeight.Text = ($"{charGenPrm.BackDashWeight}");
            genForm.txtCharBackDashDistance.Text = ($"{charGenPrm.BackDashDistance}");
            genForm.txtCharBackDashHeight2.Text = ($"{charGenPrm.BackDashHeight2}");
            genForm.txtAirAtkXAdjLimit.Text = $"{charGenPrm.AirAtkXAdjLimit}";
            genForm.txtAirAtkYAdjLimit.Text = $"{charGenPrm.AirAtkYAdjLimit}";
            genForm.txtCharStrength.Text = ($"{charGenPrm.Strength}");
            genForm.txtCharDefense.Text = ($"{charGenPrm.Defense}");
            genForm.txtCharDamageKnockback.Text = ($"{charGenPrm.DamageKnockback}");
            genForm.txtCharAttackKnockback.Text = ($"{charGenPrm.AttackKnockback}");
            genForm.txtCharStatus.Text = ($"{charGenPrm.StatusDuration}");
            genForm.txtCharQuantityProjectiles.Text = ($"{charGenPrm.QuantityProjectiles}");
            genForm.txtCharHealingMultiplier.Text = ($"{charGenPrm.HealingMultiplier}");
            genForm.txtCharChakraRecoverySpeed.Text = ($"{charGenPrm.ChakraSpeed}");
            genForm.grpGeneralConfig.Visible = true;
            genForm.grpMovementConfig.Visible = true;
            genForm.grpMovementAirConfig.Visible = true;
        }
    }
}