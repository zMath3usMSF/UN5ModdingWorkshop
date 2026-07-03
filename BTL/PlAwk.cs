using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace UN5ModdingWorkshop
{
    internal class PlAwk
    {
        public static List<PlAwk> CharAwkPrm = new List<PlAwk>();
        public static List<PlAwk> CharAwkPrmBkp = new List<PlAwk>();
        public static List<List<int>> CharAwkIDList = new List<List<int>>();
        public static List<int> CharAwkActivationType = new List<int>();
        public static List<int> CharAwkActivationTypeBkp = new List<int>();
        public static List<int> CharAwkActivationSound = new List<int>();
        public static int awkCount = 0;

        public uint NameOffset = 0x6105C0;
        public uint PrgOffset = 0x0;
        public int ID = 0;
        public int Duration = 0;
        public int InfiniteChakraFlag = 0;
        public float DamageBuff = 0;
        public float DefenseBuff = 0;
        public float SpeedBuff = 0;
        public float JumpHeightBuff = 0;
        public float KnockbackBuff = 0;
        public float CharSize = 0;
        public float HealingBuff = 0;
        public float AttackDefenseDamage = 0;
        public float ChakraChargeBuff = 0;
        public float AttackChakraDrain = 0;
        public float InitialHP = 0;
        public float FinalHP = 0;
        public float InitialChakra = 0;
        public float FinalChakra = 0;
        public float HPDrain = 0;
        public float HPDrainLimit = 0;
        public float ChakraDrain = 0;
        public float ChakraDrainLimit = 0;
        public uint ColorEffect = 0;
        public int AuraColor = 0;

        public Dictionary<string, int> AuraColorsDict = new Dictionary<string, int>()
        {
        {"White", 0},
        {"Dark Blue", 1},
        {"Red", 2},
        {"Green", 3},
        {"Pink", 4},
        {"None", 5},
        {"Light Blue", 6},
        {"White 2", 7},
        {"Yellow", 8},
        {"Yellow 2", 9},
        {"Black", 10},
        {"Bug", 11},
        {"Leaf", 12},
        {"Bubble", 13},
        {"Sand", 14},
        {"None 2", 15},
        {"Rose Petals (needs model)", 16},
        };

        public Dictionary<string, int> AwkActTypesDict = new Dictionary<string, int>()
        {
        {"0: ???", 0},
        {"1: ???", 1},
        {"2: ???", 2},
        {"3: ???", 3},
        {"5: ???", 4},
        {"8: ???", 5},
        {"9: ???", 6},
        {"16: Taunt", 7},
        {"17: Taunt", 8},
        {"64: ???", 9},
        {"65: ???", 10},
        };

        public Dictionary<string, uint> AwkCharColorEffsDict = new Dictionary<string, uint>()
        {
            {"Yellow",      0x005A8150},
            {"Yellow 2",    0x005A8160},
            {"Red",         0x005A8170},
            {"White",       0x005A8180},
            {"Dark Blue",   0x005A8190},
            {"Light Blue",  0x005A81A0},
            {"Dark Blue 2", 0x005A81B0},
            {"Purple",      0x005A81C0},
            {"Black",       0x005A81D0},
            {"White 2",     0x005A81E0},
            {"White 3",     0x005A81F0},
            {"White 4",     0x005A8200},
            {"White 5",     0x005A8210},
            {"Black 5",     0x005A8220},
            {"Black 6",     0x005A8230},
            {"Black 7",     0x005A8240},
            {"Black 8",     0x005A8250},
        };
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        internal static PlAwk ReadAwkGenPrm(byte[] Input) => new PlAwk
        {
            NameOffset = Input.ReadUInt(0x0, 32),
            PrgOffset = Input.ReadUInt(0x4, 32),
            ID = (int)Input.ReadUInt(0x8, 32),
            Duration = (int)Input.ReadUInt(0xC, 32),
            InfiniteChakraFlag = (int)Input.ReadUInt(0x10, 32),
            DamageBuff = Input.ReadSingle(0x14),
            DefenseBuff = Input.ReadSingle(0x18),
            SpeedBuff = Input.ReadSingle(0x1C),
            JumpHeightBuff = Input.ReadSingle(0x20),
            KnockbackBuff = Input.ReadSingle(0x24),
            CharSize = Input.ReadSingle(0x28),
            HealingBuff = Input.ReadSingle(0x2C),
            AttackDefenseDamage = Input.ReadSingle(0x30),
            ChakraChargeBuff = Input.ReadSingle(0x34),
            AttackChakraDrain = Input.ReadSingle(0x38),
            InitialHP = Input.ReadSingle(0x3C),
            FinalHP = Input.ReadSingle(0x40),
            InitialChakra = Input.ReadSingle(0x44),
            FinalChakra = Input.ReadSingle(0x48),
            HPDrain = Input.ReadSingle(0x4C),
            HPDrainLimit = Input.ReadSingle(0x50),
            ChakraDrain = Input.ReadSingle(0x54),
            ChakraDrainLimit = Input.ReadSingle(0x58),
            ColorEffect = Input.ReadUInt(0x5C, 32),
            AuraColor = (int)Input.ReadUInt(0x60, 32),
        };

        public static void ReadCharAwkIDList()
        {
            while(CharAwkIDList.Count <= 93)
            {
                CharAwkIDList.Add(new List<int>());
            }
            int charAwkIDListOffset = GAME.isUN6 == true ? 0x962170 : 0x5C91B0;
            int charAwkCountOffset = 0x30EFD0;
            awkCount = Util.ReadProcessMemoryInt16(charAwkCountOffset);

            byte[] awkIDBytes = new byte[4];
            for (int i = 0; i <= 93; i++)
            {
                int skipBytes = i * 8;
                int awkCount = Util.ReadProcessMemoryInt32(charAwkIDListOffset + skipBytes + 4);

                if (awkCount > 1)
                {
                    int awkIDAreaOffset = Util.ReadProcessMemoryInt32(charAwkIDListOffset + skipBytes);

                    for (int j = 0; j < awkCount; j++)
                    {
                        int awkID = Util.ReadProcessMemoryInt16(awkIDAreaOffset + j * 2);
                        CharAwkIDList[i].Add(awkID);
                    }
                }
                else
                {
                    int awkID = Util.ReadProcessMemoryInt32(charAwkIDListOffset + i * 8);
                    CharAwkIDList[i].Add(awkID);
                }

                int skipActBytes = i * 4;
                int charAwkActOffset = 0x5C8FD0;
                int charAwkActSound = Util.ReadProcessMemoryInt16(charAwkActOffset + skipActBytes);
                CharAwkActivationSound.Add(charAwkActSound);

                int charAwkActType = Util.ReadProcessMemoryInt16(charAwkActOffset + skipActBytes + 2);
                CharAwkActivationType.Add(charAwkActType);
                CharAwkActivationTypeBkp.Add(charAwkActType);
            }
        }
        public static PlAwk GetCharAwk(int selectedAwk, bool reset)
        {
            while (CharAwkPrm.Count <= awkCount)
            {
                CharAwkPrm.Add(null);
                CharAwkPrmBkp.Add(null);
            }
            if (CharAwkPrm[selectedAwk] == null)
            {
                int awkAreaOffset = GAME.isUN6 == true ? 0x94A000 : 0x5A8260;
                int skipAwk = selectedAwk * 0x64;
                byte[] currentAwkBlock = Util.ReadProcessMemoryBytes(awkAreaOffset + skipAwk, 0x64);

                var ninja = ReadAwkGenPrm(currentAwkBlock);
                var clone = (PlAwk)ninja.Clone();
                CharAwkPrm[selectedAwk] = ninja;
                CharAwkPrmBkp[selectedAwk] = clone;
            }

            return reset == true ? CharAwkPrmBkp[selectedAwk] : CharAwkPrm[selectedAwk];
        }
        public static void AddItemsToListBox(AwakeningParameters awkForm, int charID)
        {
            for(int i = 0; i < CharAwkIDList[charID].Count; i++)
            {
                awkForm.listBox1.Items.Add($"{CharAwkIDList[charID][i]}: Char Awakening {i + 1}");
            }
        }
        public static void SendTextAwk(AwakeningParameters awkForm, PlAwk charAwkPrm, int selectedIndex, int charID, bool reset)
        {
            if(awkForm.cmbSwitchToAwakening.Items.Count == 0)
            {
                for(int i = 0; i < awkCount; i++)
                {
                    awkForm.cmbSwitchToAwakening.Items.Add($"{i}");
                }
            }
            awkForm.cmbSwitchToAwakening.SelectedIndex = selectedIndex;
            awkForm.numAwkDuration.Value = charAwkPrm.Duration;
            awkForm.chkInfCkrFlag.Checked = charAwkPrm.InfiniteChakraFlag == 0x82 ? true : false;
            awkForm.numAwkDamage.Value = (decimal)charAwkPrm.DamageBuff;
            awkForm.numAwkDefense.Value = (decimal)charAwkPrm.DefenseBuff;
            awkForm.numAwkSpeed.Value = (decimal)charAwkPrm.SpeedBuff;
            awkForm.numJmpHeight.Value = (decimal)charAwkPrm.JumpHeightBuff;
            awkForm.numAwkAtkKnockback.Value = (decimal)charAwkPrm.KnockbackBuff;
            awkForm.numAwkCharSize.Value = (decimal)charAwkPrm.CharSize;
            awkForm.numAwkHealing.Value = (decimal)charAwkPrm.HealingBuff;
            awkForm.numAwkAtkDefDamage.Value = (decimal)charAwkPrm.AttackDefenseDamage;
            awkForm.numAwkCkrCharge.Value = (decimal)charAwkPrm.ChakraChargeBuff;
            awkForm.numAwkAtkCkrDrain.Value = (decimal)charAwkPrm.AttackChakraDrain;           
            awkForm.numAwkIniHP.Value = (decimal)charAwkPrm.InitialHP;
            awkForm.numAwkFinHP.Value = (decimal)charAwkPrm.FinalHP;
            awkForm.numAwkIniCkr.Value = (decimal)charAwkPrm.InitialChakra;
            awkForm.numAwkFinCkr.Value = (decimal)charAwkPrm.FinalChakra;
            awkForm.numAwkHPDrain.Value = (decimal)charAwkPrm.HPDrain;
            awkForm.numAwkHPDrainLimit.Value = (decimal)charAwkPrm.HPDrainLimit;
            awkForm.numAwkCkrDrain.Value = (decimal)charAwkPrm.ChakraDrain;
            awkForm.numAwkCkrDrainLimit.Value = (decimal)charAwkPrm.ChakraDrainLimit;
            var AwkColorDictionary = charAwkPrm.AwkCharColorEffsDict;
            if (awkForm.cmbAwkCharColorEff.Items.Count == 0)
            {
                awkForm.cmbAwkCharColorEff.Items.AddRange(AwkColorDictionary.Keys.ToArray());
                awkForm.cmbAwkCharColorEff.Items.Add("(None)");
            }
            List<uint> dicToList = AwkColorDictionary.Values.ToList();
            for(int i = 0; i < AwkColorDictionary.Count; i++)
            {
                uint currentAwkColorEffect = charAwkPrm.ColorEffect;
                uint currentListColorEffect = dicToList[i];
                if (currentAwkColorEffect == currentListColorEffect)
                {
                    awkForm.cmbAwkCharColorEff.SelectedIndex = i;
                    break;
                }
                else
                {
                    awkForm.cmbAwkCharColorEff.SelectedIndex = AwkColorDictionary.Count;
                }
            }
            if(awkForm.cmbAwkAuraColor.Items.Count == 0)
            {
                awkForm.cmbAwkAuraColor.Items.AddRange(charAwkPrm.AuraColorsDict.Keys.ToArray());
            }
            awkForm.cmbAwkAuraColor.SelectedIndex = charAwkPrm.AuraColor;

            var charAwkActivationType = reset == true ? CharAwkActivationTypeBkp[charID] : CharAwkActivationType[charID];
            for (int i = 0; i < 7; i++)
            {
                awkForm.chkAwkActType.SetItemChecked(i, (charAwkActivationType & (1 << i)) != 0);
            }
            int currentPLSound = CharAwkActivationSound[charID];
            awkForm.cmbPLSound.SelectedIndex = currentPLSound == -4 ? 0 : currentPLSound == -3 ? 1 : currentPLSound == -2 ? 2 : currentPLSound == -1 ? 3 : currentPLSound > 34 ? 0 : currentPLSound + 4;
        }

        public static (byte[] charAwkPrmBlock, byte[] charAwkAct)UpdateCharAwkPrm(AwakeningParameters awkForm, int selectedAwk, int charID, bool reset)
        {
            List<byte> AwkData = new List<byte>();
            List<byte> charAwkAct = new List<byte>();
            var awkPrm = reset == true ? CharAwkPrmBkp[selectedAwk] : CharAwkPrm[selectedAwk];

            AwkData.AddRange(BitConverter.GetBytes(Convert.ToUInt32(awkPrm.NameOffset)));
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToUInt32(awkPrm.PrgOffset)));
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToInt32(awkPrm.ID)));
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToInt32(awkForm.numAwkDuration.Value)));
            awkPrm.Duration = Convert.ToInt32(awkForm.numAwkDuration.Value);
            AwkData.AddRange(BitConverter.GetBytes(awkForm.chkInfCkrFlag.Checked == true ? 0x82 : 0x02));
            awkPrm.InfiniteChakraFlag = awkForm.chkInfCkrFlag.Checked == true ? 0x82 : 0x02;
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkDamage.Value)));
            awkPrm.DamageBuff = Convert.ToSingle(awkForm.numAwkDamage.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkDefense.Value)));
            awkPrm.DefenseBuff = Convert.ToSingle(awkForm.numAwkDefense.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkSpeed.Value)));
            awkPrm.SpeedBuff = Convert.ToSingle(awkForm.numAwkSpeed.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numJmpHeight.Value)));
            awkPrm.JumpHeightBuff = Convert.ToSingle(awkForm.numJmpHeight.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkAtkKnockback.Value)));
            awkPrm.KnockbackBuff = Convert.ToSingle(awkForm.numAwkAtkKnockback.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkCharSize.Value)));
            awkPrm.CharSize = Convert.ToSingle(awkForm.numAwkCharSize.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkHealing.Value)));
            awkPrm.HealingBuff = Convert.ToSingle(awkForm.numAwkHealing.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkAtkDefDamage.Value)));
            awkPrm.AttackDefenseDamage = Convert.ToSingle(awkForm.numAwkAtkDefDamage.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkCkrCharge.Value)));
            awkPrm.ChakraChargeBuff = Convert.ToSingle(awkForm.numAwkCkrCharge.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkAtkCkrDrain.Value)));
            awkPrm.AttackChakraDrain = Convert.ToSingle(awkForm.numAwkAtkCkrDrain.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkIniHP.Value)));
            awkPrm.InitialHP = Convert.ToSingle(awkForm.numAwkIniHP.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkFinHP.Value)));
            awkPrm.FinalHP = Convert.ToSingle(awkForm.numAwkFinHP.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkIniCkr.Value)));
            awkPrm.InitialChakra = Convert.ToSingle(awkForm.numAwkIniCkr.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkFinCkr.Value)));
            awkPrm.FinalChakra = Convert.ToSingle(awkForm.numAwkFinCkr.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkHPDrain.Value)));
            awkPrm.HPDrain = Convert.ToSingle(awkForm.numAwkHPDrain.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkHPDrainLimit.Value)));
            awkPrm.HPDrainLimit = Convert.ToSingle(awkForm.numAwkHPDrainLimit.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkCkrDrain.Value)));
            awkPrm.ChakraDrain = Convert.ToSingle(awkForm.numAwkCkrDrain.Value);
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.numAwkCkrDrainLimit.Value)));
            awkPrm.ChakraDrainLimit = Convert.ToSingle(awkForm.numAwkCkrDrainLimit.Value);
            string currentItemString = awkForm.cmbAwkCharColorEff.SelectedItem.ToString();
            awkPrm.ColorEffect = currentItemString != "(None)" ? awkPrm.AwkCharColorEffsDict[currentItemString] : 0;
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToUInt32(awkPrm.ColorEffect)));
            AwkData.AddRange(BitConverter.GetBytes(Convert.ToInt32(awkForm.cmbAwkAuraColor.SelectedIndex)));
            awkPrm.AuraColor = Convert.ToInt32(awkForm.cmbAwkAuraColor.SelectedIndex);

            int currentPLSoundIndex = awkForm.cmbPLSound.SelectedIndex;
            int currentPLSound = currentPLSoundIndex - 4;
            switch (currentPLSoundIndex)
            {
                case 0:
                    charAwkAct.AddRange(BitConverter.GetBytes((short)-4));
                    CharAwkActivationSound[charID] = -4;
                    break;
                case 1:
                    charAwkAct.AddRange(BitConverter.GetBytes((short)-3));
                    CharAwkActivationSound[charID] = -3;
                    break;
                case 2:
                    charAwkAct.AddRange(BitConverter.GetBytes((short)-2));
                    CharAwkActivationSound[charID] = -2;
                    break;
                case 3:
                    charAwkAct.AddRange(BitConverter.GetBytes((short)-1));
                    CharAwkActivationSound[charID] = -1;
                    break;
                default:
                    charAwkAct.AddRange(BitConverter.GetBytes((short)currentPLSound));
                    CharAwkActivationSound[charID] = (short)currentPLSound;
                    break;
            }
            int valor = 0;
            for (int i = 0; i < 7; i++)
            {
                if (awkForm.chkAwkActType.GetItemChecked(i))
                {
                    valor |= (1 << i);
                }
            }
            charAwkAct.AddRange(BitConverter.GetBytes((short)valor));
            CharAwkActivationType[charID] = valor;
            byte[] resultBytes = AwkData.ToArray();
            byte[] resultBytes2 = charAwkAct.ToArray();
            return (resultBytes, resultBytes2);
        }

        public static void UpdateP1AwkPrm(byte[] resultBytes, byte[] resultBytes2, int selectedAwk, int charID, int awkPos)
        {
            int skipAwks = selectedAwk * 0x64;
            int awkAreaOffset = GAME.isUN6 == true ? 0x94A000 + skipAwks : 0x5A8260 + skipAwks;
            int skipChars = charID * 8;
            int charAwkIDListOffset = GAME.isUN6 == true ? 0x962170 + skipChars : 0x5C91B0 + skipChars;
            byte[] awkID = BitConverter.GetBytes(Convert.ToInt16(selectedAwk));

            Util.WriteProcessMemoryBytes(awkAreaOffset, resultBytes);
            if (CharAwkIDList[charID].Count == 1)
            {
                byte[] count = BitConverter.GetBytes(1);
                Util.WriteProcessMemoryBytes(charAwkIDListOffset + 4, count);
                CharAwkIDList[charID][0] = selectedAwk;
                Util.WriteProcessMemoryBytes(charAwkIDListOffset, awkID);
            }
            else
            {
                int skipAwk = awkPos * 2;
                int charAwkArea = Util.ReadProcessMemoryInt32(charAwkIDListOffset);
                Util.WriteProcessMemoryBytes(charAwkArea + skipAwk, awkID);
                CharAwkIDList[charID][awkPos] = selectedAwk;
            }

            int skipActs = charID * 4;
            int actOffset = 0x5C8FD0 + skipActs;
            Util.WriteProcessMemoryBytes(actOffset, resultBytes2);
        }
        public static void WriteELFAwkPrm(byte[] resultBytes, byte[] resultBytes2, int selectedAwk, int charID, int awkPos)
        {
            if(GAME.isUN6 == true)
            {
                MessageBox.Show("Ultimate Ninja 6 is not yet supported for this.");
            }
            else
            {
                if (!File.Exists(GAME.elfPath))
                {
                    MessageBox.Show("Unable to save, check if the file has been deleted or moved.", string.Empty, MessageBoxButtons.OK);
                }
                else
                {
                    using (FileStream fs = new FileStream(GAME.elfPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        int skipAwks = selectedAwk * 0x64;
                        int awkAreaOffset = 0x5A8260 + skipAwks;
                        fs.Seek(awkAreaOffset + skipAwks, SeekOrigin.Begin);
                        int subValue = 0xFFE80;
                        awkAreaOffset = GAME.isUN6 == true ? 0xA000 : awkAreaOffset - subValue;

                        fs.Seek(awkAreaOffset, SeekOrigin.Begin);
                        fs.Write(resultBytes, 0, resultBytes.Length);

                        int skipChars = charID * 8;
                        int charAwkOffset = 0x5C91B0 + skipChars;
                        charAwkOffset = GAME.isUN6 == true ? 0x22170 : charAwkOffset - subValue;
                        fs.Seek(charAwkOffset + 4, SeekOrigin.Begin);
                        byte[] awkID = BitConverter.GetBytes(Convert.ToInt16(selectedAwk));

                        if (CharAwkIDList[charID].Count == 1)
                        {
                            byte[] count = BitConverter.GetBytes(1);
                            fs.Write(count, 0, count.Length);
                            fs.Seek(charAwkOffset, SeekOrigin.Begin);
                            fs.Write(awkID, 0, awkID.Length);
                        }
                        else
                        {
                            fs.Seek(charAwkOffset, SeekOrigin.Begin);
                            byte[] buffer = new byte[4];
                            fs.Read(buffer, 0, buffer.Length);
                            int skipAwk = awkPos * 2;
                            int charAwkArea = BitConverter.ToInt32(buffer, 0);
                            charAwkArea = charAwkArea - subValue;
                            fs.Seek(charAwkArea + skipAwk, SeekOrigin.Begin);
                            fs.Write(awkID, 0, awkID.Length);
                        }

                        int skipActs = charID * 4;
                        int actOffset = 0x5C8FD0 + skipActs;
                        actOffset = actOffset - subValue;
                        fs.Seek(actOffset, SeekOrigin.Begin);
                        fs.Write(resultBytes2, 0, resultBytes2.Length);

                        MessageBox.Show("The changes were saved successfully!", string.Empty, MessageBoxButtons.OK);
                    }
                }
            }
        }
    }
}
