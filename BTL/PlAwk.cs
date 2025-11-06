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
    internal class PlAwk
    {
        public static List<PlAwk> CharAwkPrm = new List<PlAwk>();
        public static List<PlAwk> CharAwkPrmBkp = new List<PlAwk>();
        public static List<List<int>> CharAwkIDList = new List<List<int>>();
        public static List<int> CharAwkActivationType = new List<int>();
        public static List<int> CharAwkActivationSound = new List<int>();
        public static int awkCount = 0;

        public float AwkDamageBuff, AwkDefenseBuff, AwkSpeedBuff, AwkJmpHeightBuff, AwkKnockbackBuff, AwkCharSize, AwkHealingBuff, AwkAtkDefDamage,
                     AwkCkrChargeBuff, AwkAtkCkrDrain, AwkIniHP, AwkFinHP, AwkIniChakra, AwkFinChakra, AwkHPDrain, AwkHPDrainLimit, AwkCkrDrain,
                     AwkCkrDrainLimit;
        public int AwkID, AwkDuration, AwkInfCkrFlag, AwkAuraColor;
        public byte[] Unk, AwkExtPrg, Unk2, AwkColorEffect;

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
        public Dictionary<string, byte[]> AwkCharColorEffsDict = new Dictionary<string, byte[]>()
        {
        {"Yellow", new byte[]{0x50, 0x81, 0x5A, 00 }},
        {"Yellow 2", new byte[]{0x60, 0x81, 0x5A, 00 }},
        {"Red", new byte[] { 0x70, 0x81, 0x5A, 00 }},
        {"White", new byte[] { 0x80, 0x81, 0x5A, 00 }},
        {"Dark Blue", new byte[] { 0x90, 0x81, 0x5A, 00 }},
        {"Light Blue", new byte[]{0xA0, 0x81, 0x5A, 00 }},
        {"Dark Blue 2", new byte[]{0xB0, 0x81, 0x5A, 00 }},
        {"Purple", new byte[]{0xC0, 0x81, 0x5A, 00 }},
        {"Black", new byte[]{0xD0, 0x81, 0x5A, 00 }},
        {"White 2", new byte[]{0xE0, 0x81, 0x5A, 00 }},
        {"White 3", new byte[]{0xF0, 0x81, 0x5A, 00 }},
        {"White 4", new byte[]{0x00, 0x82, 0x5A, 00 }},
        {"White 5", new byte[]{0x10, 0x82, 0x5A, 00 }},
        {"Black 5", new byte[]{0x20, 0x82, 0x5A, 00 }},
        {"Black 6", new byte[]{0x30, 0x82, 0x5A, 00 }},
        {"Black 7", new byte[]{0x40, 0x82, 0x5A, 00 }},
        {"Black 8", new byte[]{0x50, 0x82, 0x5A, 00 }},

        };
        public Dictionary<string, byte[]> NA2AwkCharColorEffsDict = new Dictionary<string, byte[]>()
        {
        {"Yellow", new byte[]{ 0x90, 0xE1, 0x59, 00 }},
        {"Yellow 2", new byte[]{ 0xA0, 0xE1, 0x59, 00 }},
        {"Red", new byte[] { 0xB0, 0xE1, 0x59, 00 }},
        {"White", new byte[] { 0xC0, 0xE1, 0x59, 00 }},
        {"Dark Blue", new byte[] { 0xD0, 0xE1, 0x59, 00 }},
        {"Light Blue", new byte[]{ 0xE0, 0xE1, 0x59, 00 }},
        {"Dark Blue 2", new byte[]{ 0xF0, 0xE1, 0x59, 00 }},
        {"Purple", new byte[]{ 0x00, 0xE2, 0x59, 00 }},
        {"Black", new byte[]{ 0x10, 0xE2, 0x59, 00 }},
        {"White 2", new byte[]{ 0x20, 0xE2, 0x59, 00 }},
        {"White 3", new byte[]{ 0x30, 0xE2, 0x59, 00 }},
        {"White 4", new byte[]{ 0x40, 0xE2, 0x59, 00 }},
        {"White 5", new byte[]{ 0x50, 0xE2, 0x59, 00 }},
        {"Black 5", new byte[]{ 0x60, 0xE2, 0x59, 00 }},
        {"Black 6", new byte[]{ 0x70, 0xE2, 0x59, 00 }},
        {"Black 7", new byte[]{ 0x80, 0xE2, 0x59, 00 }},
        {"Black 8", new byte[]{ 0x90, 0xE2, 0x59, 00 }},

        };
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        internal static PlAwk ReadAwkGenPrm(byte[] Input) => new PlAwk
        {
            Unk = Input.ReadBytes(0x0, 4),
            AwkExtPrg = Input.ReadBytes(0x4, 4),
            AwkID = (int)Input.ReadUInt(0x8, 32),
            AwkDuration = (int)Input.ReadUInt(0xC, 32),
            AwkInfCkrFlag = (int)Input.ReadUInt(0x10, 32),
            AwkDamageBuff = Input.ReadSingle(0x14),
            AwkDefenseBuff = Input.ReadSingle(0x18),
            AwkSpeedBuff = Input.ReadSingle(0x1C),
            AwkJmpHeightBuff = Input.ReadSingle(0x20),
            AwkKnockbackBuff = Input.ReadSingle(0x24),
            AwkCharSize = Input.ReadSingle(0x28),
            AwkHealingBuff = Input.ReadSingle(0x2C),
            AwkAtkDefDamage = Input.ReadSingle(0x30),
            AwkCkrChargeBuff = Input.ReadSingle(0x34),
            AwkAtkCkrDrain = Input.ReadSingle(0x38),
            AwkIniHP = Input.ReadSingle(0x3C),
            AwkFinHP = Input.ReadSingle(0x40),
            AwkIniChakra = Input.ReadSingle(0x44),
            AwkFinChakra = Input.ReadSingle(0x48),
            AwkHPDrain = Input.ReadSingle(0x4C),
            AwkHPDrainLimit = Input.ReadSingle(0x50),
            AwkCkrDrain = Input.ReadSingle(0x54),
            AwkCkrDrainLimit = Input.ReadSingle(0x58),
            AwkColorEffect = Input.ReadBytes(0x5C, 4),
            AwkAuraColor = (int)Input.ReadUInt(0x60, 32),
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
        public static void SendTextAwk(AwakeningParameters awkForm, PlAwk charAwkPrm, int selectedIndex, int charID)
        {
            if(awkForm.cmbSwitchToAwakening.Items.Count == 0)
            {
                for(int i = 0; i < awkCount; i++)
                {
                    awkForm.cmbSwitchToAwakening.Items.Add($"{i}");
                }
            }
            awkForm.cmbSwitchToAwakening.SelectedIndex = selectedIndex;
            awkForm.numAwkDuration.Value = charAwkPrm.AwkDuration;
            if(awkForm.cmbInfCkrFlag.Items.Count == 0)
            {
                string[] yesnoOption = { "No", "Yes" };
                awkForm.cmbInfCkrFlag.Items.AddRange(yesnoOption);
            }
            awkForm.cmbInfCkrFlag.SelectedIndex = charAwkPrm.AwkInfCkrFlag == 0x82 ? 1 : 0;
            awkForm.txtAwkDamage.Text = Convert.ToString(charAwkPrm.AwkDamageBuff);
            awkForm.txtAwkDefense.Text = Convert.ToString(charAwkPrm.AwkDefenseBuff);
            awkForm.txtAwkSpeed.Text = Convert.ToString(charAwkPrm.AwkSpeedBuff);
            awkForm.txtJmpHeight.Text = Convert.ToString(charAwkPrm.AwkJmpHeightBuff);
            awkForm.txtAwkAtkKnockback.Text = Convert.ToString(charAwkPrm.AwkKnockbackBuff);
            awkForm.txtAwkCharSize.Text = Convert.ToString(charAwkPrm.AwkCharSize);
            awkForm.txtAwkHealing.Text = Convert.ToString(charAwkPrm.AwkHealingBuff);
            awkForm.txtAwkAtkDefDamage.Text = Convert.ToString(charAwkPrm.AwkAtkDefDamage);
            awkForm.txtAwkCkrCharge.Text = Convert.ToString(charAwkPrm.AwkCkrChargeBuff);
            awkForm.txtAwkAtkCkrDrain.Text = Convert.ToString(charAwkPrm.AwkAtkCkrDrain);           
            awkForm.txtAwkIniHP.Text = Convert.ToString(charAwkPrm.AwkIniHP);
            awkForm.txtAwkFinHP.Text = Convert.ToString(charAwkPrm.AwkFinHP);
            awkForm.txtAwkIniCkr.Text = Convert.ToString(charAwkPrm.AwkIniChakra);
            awkForm.txtAwkFinCkr.Text = Convert.ToString(charAwkPrm.AwkFinChakra);
            awkForm.txtAwkHPDrain.Text = Convert.ToString(charAwkPrm.AwkHPDrain);
            awkForm.txtAwkHPDrainLimit.Text = Convert.ToString(charAwkPrm.AwkHPDrainLimit);
            awkForm.txtAwkCkrDrain.Text = Convert.ToString(charAwkPrm.AwkCkrDrain);
            awkForm.txtAwkCkrDrainLimit.Text = Convert.ToString(charAwkPrm.AwkCkrDrainLimit);
            var AwkColorDictionary = charAwkPrm.NA2AwkCharColorEffsDict;
            if (awkForm.cmbAwkCharColorEff.Items.Count == 0)
            {
                awkForm.cmbAwkCharColorEff.Items.AddRange(AwkColorDictionary.Keys.ToArray());
                awkForm.cmbAwkCharColorEff.Items.Add("(None)");
            }
            List<byte[]> dicToList = AwkColorDictionary.Values.ToList();
            for(int i = 0; i < AwkColorDictionary.Count; i++)
            {
                int one = BitConverter.ToInt32(charAwkPrm.AwkColorEffect, 0);
                int two = BitConverter.ToInt32(dicToList[i], 0);
                if (one == two)
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
            awkForm.cmbAwkAuraColor.SelectedIndex = charAwkPrm.AwkAuraColor;

            if(awkForm.cmbAwkActType.Items.Count == 0)
            {
                awkForm.cmbAwkActType.Items.AddRange(charAwkPrm.AwkActTypesDict.Keys.ToArray());
            }
            string type = Convert.ToString(CharAwkActivationType[charID]);
            foreach (var item in awkForm.cmbAwkActType.Items)
            {
                if (item.ToString().Contains(type))
                {
                    awkForm.cmbAwkActType.SelectedItem = item;
                    break;
                }
            }
            if (awkForm.cmbPLSound.Items.Count == 0)
            {
                awkForm.cmbPLSound.Items.AddRange(PlAtk.PLSoundList.Values.ToArray());
            }
            int currentPLSound = CharAwkActivationSound[charID];
            awkForm.cmbPLSound.SelectedIndex = currentPLSound == -4 ? 0 : currentPLSound == -3 ? 1 : currentPLSound == -2 ? 2 : currentPLSound == -1 ? 3 : currentPLSound > 34 ? 0 : currentPLSound + 4;
        }
        public static (byte[] charAwkPrmBlock, byte[] charAwkAct)UpdateCharAwkPrm(AwakeningParameters awkForm, int selectedAwk, int charID, bool reset)
        {
            List<byte> charAwkPrmBlock = new List<byte>();
            List<byte> charAwkAct = new List<byte>();
            var awkPrm = reset == true ? CharAwkPrmBkp[selectedAwk] : CharAwkPrm[selectedAwk];

            charAwkPrmBlock.AddRange(awkPrm.Unk);
            charAwkPrmBlock.AddRange(awkPrm.AwkExtPrg);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToInt32(awkPrm.AwkID)));
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToInt32(awkForm.numAwkDuration.Value)));
            awkPrm.AwkDuration = Convert.ToInt32(awkForm.numAwkDuration.Value);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(awkForm.cmbInfCkrFlag.SelectedIndex == 0 ? 0x02 : 0x82));
            awkPrm.AwkInfCkrFlag = awkForm.cmbInfCkrFlag.SelectedIndex == 0 ? 0x02 : 0x82;
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkDamage.Text)));
            awkPrm.AwkDamageBuff = Convert.ToSingle(awkForm.txtAwkDamage.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkDefense.Text)));
            awkPrm.AwkDefenseBuff = Convert.ToSingle(awkForm.txtAwkDefense.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkSpeed.Text)));
            awkPrm.AwkSpeedBuff = Convert.ToSingle(awkForm.txtAwkSpeed.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtJmpHeight.Text)));
            awkPrm.AwkJmpHeightBuff = Convert.ToSingle(awkForm.txtJmpHeight.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkAtkKnockback.Text)));
            awkPrm.AwkKnockbackBuff = Convert.ToSingle(awkForm.txtAwkAtkKnockback.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkCharSize.Text)));
            awkPrm.AwkCharSize = Convert.ToSingle(awkForm.txtAwkCharSize.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkHealing.Text)));
            awkPrm.AwkHealingBuff = Convert.ToSingle(awkForm.txtAwkHealing.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkAtkDefDamage.Text)));
            awkPrm.AwkAtkDefDamage = Convert.ToSingle(awkForm.txtAwkAtkDefDamage.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkCkrCharge.Text)));
            awkPrm.AwkCkrChargeBuff = Convert.ToSingle(awkForm.txtAwkCkrCharge.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkAtkCkrDrain.Text)));
            awkPrm.AwkAtkCkrDrain = Convert.ToSingle(awkForm.txtAwkAtkCkrDrain.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkIniHP.Text)));
            awkPrm.AwkIniHP = Convert.ToSingle(awkForm.txtAwkIniHP.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkFinHP.Text)));
            awkPrm.AwkFinHP = Convert.ToSingle(awkForm.txtAwkFinHP.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkIniCkr.Text)));
            awkPrm.AwkIniChakra = Convert.ToSingle(awkForm.txtAwkIniCkr.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkFinCkr.Text)));
            awkPrm.AwkFinChakra = Convert.ToSingle(awkForm.txtAwkFinCkr.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkHPDrain.Text)));
            awkPrm.AwkHPDrain = Convert.ToSingle(awkForm.txtAwkHPDrain.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkHPDrainLimit.Text)));
            awkPrm.AwkHPDrainLimit = Convert.ToSingle(awkForm.txtAwkHPDrainLimit.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkCkrDrain.Text)));
            awkPrm.AwkCkrDrain = Convert.ToSingle(awkForm.txtAwkCkrDrain.Text);
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToSingle(awkForm.txtAwkCkrDrainLimit.Text)));
            awkPrm.AwkCkrDrainLimit = Convert.ToSingle(awkForm.txtAwkCkrDrainLimit.Text);
            string currentItemString = awkForm.cmbAwkCharColorEff.SelectedItem.ToString();
            byte[] awkCharColorEffOffset = currentItemString != "(None)" ? awkPrm.AwkCharColorEffsDict[currentItemString] : new byte[]{00, 00, 00, 00};
            charAwkPrmBlock.AddRange(awkCharColorEffOffset);
            awkPrm.AwkColorEffect = awkCharColorEffOffset;
            charAwkPrmBlock.AddRange(BitConverter.GetBytes(Convert.ToInt32(awkForm.cmbAwkAuraColor.SelectedIndex)));
            awkPrm.AwkAuraColor = Convert.ToInt32(awkForm.cmbAwkAuraColor.SelectedIndex);

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
            int selectedAwkType = Convert.ToInt32(awkForm.cmbAwkActType.SelectedItem.ToString().Split(':')[0]);
            charAwkAct.AddRange(BitConverter.GetBytes((short)selectedAwkType));
            CharAwkActivationType[charID] = selectedAwkType;
            byte[] resultBytes = charAwkPrmBlock.ToArray();
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
                if (!File.Exists(GAME.caminhoELF))
                {
                    MessageBox.Show("Unable to save, check if the file has been deleted or moved.", string.Empty, MessageBoxButtons.OK);
                }
                else
                {
                    using (FileStream fs = new FileStream(GAME.caminhoELF, FileMode.OpenOrCreate, FileAccess.ReadWrite))
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
