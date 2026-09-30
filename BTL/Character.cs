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
    public class Character
    {
        public static List<Character> List = new List<Character>();
        public static List<Character> ListBkp = new List<Character>();

        public uint ID = 0x0;
        public string Name = "";
        public string CCS = "";
        public List<string> Cluts = new List<string>();
        public List<string> Textures = new List<string>();
        public string Model = "";
        public string HoldBonne = "";
        public List<int> MovesetFunctionsAddr = new List<int>();
        public float Unk = 0f;
        public float Unk1 = 0f;

        public uint AtkCount = 0x0;
        public List<Attack> Attacks = new List<Attack>();
        public uint AtkOffset = 0x0;
        public uint AtkListOffset = 0x0;
        public uint AtkListMemOffset = 0x0;
        public float Unk2 = 0f;
        public uint AnmCount = 0x0;
        public uint AnmListOffset = 0x0;
        public uint AnmListMemOffset = 0x0;
        public uint AnmNameCount = 0x0;
        public uint AnmNameListOffset = 0x0;
        public uint AnmNameListMemOffset = 0x0;

        public uint AcessoryCount = 0x0;
        public uint AccessoryListOffset = 0x0;

        public float Height = 110f;
        public float Width = 140f;
        public float Speed = 1f;
        public float RunningStartSpeed = 0.3f;
        public float Slide = 0.3f;
        public float Weight = 1f;
        public float Gravity = 11.5f;
        public float SpeedAir = 16f;
        public float WallJmpRecoilDistance = 0.04f;
        public float WallJmpDistanceLimit = 0.02f;
        public uint FirstJumpDelay = 3;
        public float FirstJumpHeight = 280f;
        public float SecondJumpHeight = 210f;
        public uint DashDelay = 9;
        public uint DashDuration = 12;
        public float DashSpeed = 400f;
        public float DashDistance = 600f;
        public float BackDashHeight = 40f;
        public float BackDashWeight = 30f;
        public float BackDashDistance = 0.02f;
        public float BackDashHeight2 = 2f;
        public float AirAtkXAdjLimit = 250f;
        public float AirAtkYAdjLimit = 150f;
        public float Unk3 = 0f;
        public float Unk4 = 0f;
        public float Strength = 1f;
        public float Defense = 1f;
        public float DamageKnockback = 1f;
        public float AttackKnockback = 1f;
        public float StatusDuration = 1f;
        public float QuantityProjectiles = 1f;
        public float HealingMultiplier = 1f;
        public float ChakraSpeed = 1f;

        public Character Clone() => (Character)MemberwiseClone();

        public void Read(byte[] Input)
        {
            ID = Input.ReadUInt(0x0, 32);
            Name = Util.ReadMemoryFixedLenString(Input.ReadInt(0x4, 32), 0x20, '\0');
            CCS = Util.ReadMemoryFixedLenString(Input.ReadInt(0x8, 32), 0x20, '\0');

            int clutsOffs = Input.ReadInt(0xC, 32);
            Util.ReadStringList(Cluts, clutsOffs, 0x1E);

            int texturesOffs = Input.ReadInt(0x10, 32);
            Util.ReadStringList(Textures, texturesOffs, 0x1E);

            Model = Util.ReadMemoryFixedLenString(Input.ReadInt(0x14, 32), 0x20, '\0');
            HoldBonne = Util.ReadMemoryFixedLenString(Input.ReadInt(0x18, 32), 0x20, '\0');
            for(int i = 0; i < 8; i++)
            {
                int funcAddr = Input.ReadInt(0x1C, 32);
                funcAddr = Util.ReadProcessMemoryInt32(funcAddr + (i * 4));
                MovesetFunctionsAddr.Add(funcAddr);
            }

            Unk = Input.ReadSingle(0x20);
            Unk1 = Input.ReadSingle(0x24);

            AtkCount = Input.ReadUInt(0x28, 32);
            AtkListOffset = Input.ReadUInt(0x2C, 32);
            for(int i = 0; i < AtkCount; i++)
            {
                int atkAddr = (int)AtkListOffset + (i * 0x54);
                Attack atk = new Attack();
                atk.Read(Util.ReadProcessMemoryBytes(atkAddr, 0x54));
                Attacks.Add(atk);
            }
            AtkListMemOffset = Input.ReadUInt(0x30, 32);

            Unk2 = Input.ReadSingle(0x34);
            AnmCount = Input.ReadUInt(0x38, 32);
            AnmListOffset = Input.ReadUInt(0x3C, 32);
            AnmListMemOffset = Input.ReadUInt(0x40, 32);
            AnmNameCount = Input.ReadUInt(0x44, 32);
            AnmNameListOffset = Input.ReadUInt(0x48, 32);
            AnmNameListMemOffset = Input.ReadUInt(0x4C, 32);
            AcessoryCount = Input.ReadUInt(0x50, 32);
            AccessoryListOffset = Input.ReadUInt(0x54, 32);

            Height = Input.ReadSingle(0x58);
            Width = Input.ReadSingle(0x5c);
            Speed = Input.ReadSingle(0x60);
            RunningStartSpeed = Input.ReadSingle(0x64);
            Slide = Input.ReadSingle(0x68);
            Weight = Input.ReadSingle(0x6c);
            Gravity = Input.ReadSingle(0x70);
            SpeedAir = Input.ReadSingle(0x74);
            WallJmpRecoilDistance = Input.ReadSingle(0x78);
            WallJmpDistanceLimit = Input.ReadSingle(0x7C);
            FirstJumpDelay = Input.ReadUInt(0x80, 32);
            FirstJumpHeight = Input.ReadSingle(0x84);
            SecondJumpHeight = Input.ReadSingle(0x88);
            DashDelay = Input.ReadUInt(0x8c, 32);
            DashDuration = Input.ReadUInt(0x90, 32);
            DashSpeed = Input.ReadSingle(0x94);
            DashDistance = Input.ReadSingle(0x98);
            BackDashHeight = Input.ReadSingle(0x9c);
            BackDashWeight = Input.ReadSingle(0xa0);
            BackDashDistance = Input.ReadSingle(0xa4);
            BackDashHeight2 = Input.ReadSingle(0xa8);

            AirAtkXAdjLimit = Input.ReadSingle(0xac);
            AirAtkYAdjLimit = Input.ReadSingle(0xb0);
            Unk3 = Input.ReadSingle(0xb4);
            Unk4 = Input.ReadSingle(0xb8);

            Strength = Input.ReadSingle(0xbc);
            Defense = Input.ReadSingle(0xc0);
            DamageKnockback = Input.ReadSingle(0xc4);
            AttackKnockback = Input.ReadSingle(0xc8);
            StatusDuration = Input.ReadSingle(0xcc);
            QuantityProjectiles = Input.ReadSingle(0xd0);
            HealingMultiplier = Input.ReadSingle(0xd4);
            ChakraSpeed = Input.ReadSingle(0xd8);
        }

        public static void PopulateForm(GeneralParameters genForm, Character charGenPrm)
        {
            genForm.numCharHeight.Value = (decimal)charGenPrm.Height;
            genForm.numCharWidth.Value = (decimal)charGenPrm.Width;
            genForm.numCharSpeed.Value = (decimal)charGenPrm.Speed;
            genForm.numRunningStartSpeed.Value = (decimal)charGenPrm.RunningStartSpeed;
            genForm.numCharSlide.Value = (decimal)charGenPrm.Slide;
            genForm.numCharWeight.Value = (decimal)charGenPrm.Weight;
            genForm.numCharGravity.Value = (decimal)charGenPrm.Gravity;
            genForm.numCharSpeedAir.Value = (decimal)charGenPrm.SpeedAir;
            genForm.numWallJmpRecoilDistance.Value = (decimal)charGenPrm.WallJmpRecoilDistance;
            genForm.numWallJmpDistanceLimit.Value = (decimal)charGenPrm.WallJmpDistanceLimit;
            genForm.numCharFirstJumpDelay.Value = (decimal)charGenPrm.FirstJumpDelay;
            genForm.numCharFirstJumpHeight.Value = (decimal)charGenPrm.FirstJumpHeight;
            genForm.numCharSecondJumpHeight.Value = (decimal)charGenPrm.SecondJumpHeight;
            genForm.numCharDashDelay.Value = (decimal)charGenPrm.DashDelay;
            genForm.numCharDashDuration.Value = (decimal)charGenPrm.DashDuration;
            genForm.numCharDashSpeed.Value = (decimal)charGenPrm.DashSpeed;
            genForm.numCharDashDistance.Value = (decimal)charGenPrm.DashDistance;
            genForm.numCharBackDashHeight.Value = (decimal)charGenPrm.BackDashHeight;
            genForm.numCharBackDashWeight.Value = (decimal)charGenPrm.BackDashWeight;
            genForm.numCharBackDashDistance.Value = (decimal)charGenPrm.BackDashDistance;
            genForm.numCharBackDashHeight2.Value = (decimal)charGenPrm.BackDashHeight2;
            genForm.numAirAtkXAdjLimit.Value = (decimal)charGenPrm.AirAtkXAdjLimit;
            genForm.numAirAtkYAdjLimit.Value = (decimal)charGenPrm.AirAtkYAdjLimit;
            genForm.numCharStrength.Value = (decimal)charGenPrm.Strength;
            genForm.numCharDefense.Value = (decimal)charGenPrm.Defense;
            genForm.numCharDamageKnockback.Value = (decimal)charGenPrm.DamageKnockback;
            genForm.numCharAttackKnockback.Value = (decimal)charGenPrm.AttackKnockback;
            genForm.numCharStatus.Value = (decimal)charGenPrm.StatusDuration;
            genForm.numCharQuantityProjectiles.Value = (decimal)charGenPrm.QuantityProjectiles;
            genForm.numCharHealingMultiplier.Value = (decimal)charGenPrm.HealingMultiplier;
            genForm.numCharChakraRecoverySpeed.Value = (decimal)charGenPrm.ChakraSpeed;
            genForm.grpGeneralConfig.Visible = true;
            genForm.grpMovementConfig.Visible = true;
            genForm.grpMovementAirConfig.Visible = true;
        }

        public static byte[] Update(GeneralParameters genForm, int charID)
        {
            List<byte> GenData = new List<byte>();
            Character character = List[charID];

            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharHeight.Value)));
            character.Height = Convert.ToSingle(genForm.numCharHeight.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharWidth.Value)));
            character.Width = Convert.ToSingle(genForm.numCharWidth.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharSpeed.Value)));
            character.Speed = Convert.ToSingle(genForm.numCharSpeed.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numRunningStartSpeed.Value)));
            character.RunningStartSpeed = Convert.ToSingle(genForm.numRunningStartSpeed.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharSlide.Value)));
            character.Slide = Convert.ToSingle(genForm.numCharSlide.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharWeight.Value)));
            character.Weight = Convert.ToSingle(genForm.numCharWeight.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharGravity.Value)));
            character.Gravity = Convert.ToSingle(genForm.numCharGravity.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharSpeedAir.Value)));
            character.SpeedAir = Convert.ToSingle(genForm.numCharSpeedAir.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numWallJmpRecoilDistance.Value)));
            character.WallJmpRecoilDistance = Convert.ToSingle(genForm.numWallJmpRecoilDistance.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numWallJmpDistanceLimit.Value)));
            character.WallJmpDistanceLimit = Convert.ToSingle(genForm.numWallJmpDistanceLimit.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToInt32(genForm.numCharFirstJumpDelay.Value)));
            character.FirstJumpDelay = Convert.ToUInt32(genForm.numCharFirstJumpDelay.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharFirstJumpHeight.Value)));
            character.FirstJumpHeight = Convert.ToSingle(genForm.numCharFirstJumpHeight.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharSecondJumpHeight.Value)));
            character.SecondJumpHeight = Convert.ToSingle(genForm.numCharSecondJumpHeight.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToInt32(genForm.numCharDashDelay.Value)));
            character.DashDelay = Convert.ToUInt32(genForm.numCharDashDelay.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToInt32(genForm.numCharDashDuration.Value)));
            character.DashDuration = Convert.ToUInt32(genForm.numCharDashDuration.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharDashSpeed.Value)));
            character.DashSpeed = Convert.ToSingle(genForm.numCharDashSpeed.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharDashDistance.Value)));
            character.DashDistance = Convert.ToSingle(genForm.numCharDashDistance.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharBackDashHeight.Value)));
            character.BackDashHeight = Convert.ToSingle(genForm.numCharBackDashHeight.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharBackDashWeight.Value)));
            character.BackDashWeight = Convert.ToSingle(genForm.numCharBackDashWeight.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharBackDashDistance.Value)));
            character.BackDashDistance = Convert.ToSingle(genForm.numCharBackDashDistance.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharBackDashHeight2.Value)));
            character.BackDashHeight2 = Convert.ToSingle(genForm.numCharBackDashHeight2.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numAirAtkXAdjLimit.Value)));
            character.AirAtkXAdjLimit = Convert.ToSingle(genForm.numAirAtkXAdjLimit.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numAirAtkYAdjLimit.Value)));
            character.AirAtkYAdjLimit = Convert.ToSingle(genForm.numAirAtkYAdjLimit.Value);
            GenData.AddRange(BitConverter.GetBytes(character.Unk3));
            GenData.AddRange(BitConverter.GetBytes(character.Unk4));
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharStrength.Value)));
            character.Strength = Convert.ToSingle(genForm.numCharStrength.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharDefense.Value)));
            character.Defense = Convert.ToSingle(genForm.numCharDefense.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharDamageKnockback.Value)));
            character.DamageKnockback = Convert.ToSingle(genForm.numCharDamageKnockback.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharAttackKnockback.Value)));
            character.AttackKnockback = Convert.ToSingle(genForm.numCharAttackKnockback.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharStatus.Value)));
            character.StatusDuration = Convert.ToSingle(genForm.numCharStatus.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharQuantityProjectiles.Value)));
            character.QuantityProjectiles = Convert.ToSingle(genForm.numCharQuantityProjectiles.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharHealingMultiplier.Value)));
            character.HealingMultiplier = Convert.ToSingle(genForm.numCharHealingMultiplier.Value);
            GenData.AddRange(BitConverter.GetBytes(Convert.ToSingle(genForm.numCharChakraRecoverySpeed.Value)));
            character.ChakraSpeed = Convert.ToSingle(genForm.numCharChakraRecoverySpeed.Value);

            return GenData.ToArray();
        }

        public static void WriteToMemory(byte[] charGeneralDataBlock, int charID)
        {
            int P1AtributtesOffs = Util.BTL_GetPlayer1MemoryOffs() + 0x8C;

            Util.WriteProcessMemoryBytes(P1AtributtesOffs + 0x58, charGeneralDataBlock);
            Util.WriteProcessMemoryBytes(BTL.charMainAreaOffsets[charID] + 0x58, charGeneralDataBlock);
        }

        public static void WriteToELF(byte[] resultBytes, int charID)
        {
            if (!File.Exists(GAME.elfPath))
            {
                MessageBox.Show("Unable to save, check if the file has been deleted or moved.", string.Empty, MessageBoxButtons.OK);
            }
            else
            {
                using (FileStream fs = new FileStream(GAME.elfPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
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
    }
}