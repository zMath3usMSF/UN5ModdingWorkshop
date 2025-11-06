using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace UN5ModdingWorkshop
{
    internal class CharSkl
    {
        public static List<List<CharSkl>> CharSklPrm = new List<List<CharSkl>>();
        public static List<List<CharSkl>> CharSklPrmBkp = new List<List<CharSkl>>();
        public static List<string> charJutsuNameList = new List<string>();
        public static List<int> charSelectedJutsuList = new List<int>();

        #region Jutsus Parameters
        uint isUseCamera, isPlayerUse1CMN, isEnemyUse1CMN;
        int IniDamageEffect, FinDamageEff, HitCount, HitStop, HitSpeed, DamageSound,
            DamageParticle, DefenseSound, DefenseParticle, EnemySound;
        float Knockback, Damage;
        byte[] JutsuName;

        uint Unk2;
        int FUnk1, FUnk2, FUnk3, FUnk4, FUnk5, FUnk6, FUnk7, FUnk8, FUnk9;
        int Unk, Unk1, DefEffect, Unk4, Unk5, Unk6, Unk7, Unk8, Unk9;
        #endregion
        internal static CharSkl ReadCharSklPrm(byte[] Input) => new CharSkl
        {
            JutsuName = Input.ReadBytes(0x0, 32),
            isUseCamera = Input.ReadUInt(0x4, 8),
            isPlayerUse1CMN = Input.ReadUInt(0x5, 8),
            isEnemyUse1CMN = Input.ReadUInt(0x6, 8),
            FUnk1 = (int)Input.ReadUInt(0x8, 8),
            FUnk2 = (int)Input.ReadUInt(0x9, 8),
            FUnk3 = (int)Input.ReadUInt(0xA, 8),
            FUnk4 = (int)Input.ReadUInt(0xB, 8),
            FUnk5 = (int)Input.ReadUInt(0xC, 8),
            FUnk6 = (int)Input.ReadUInt(0xD, 8),
            FUnk7 = (int)Input.ReadUInt(0xE, 8),
            FUnk8 = (int)Input.ReadUInt(0xF, 8),
            FUnk9 = (int)Input.ReadUInt(0x10, 16),
            IniDamageEffect = (short)Input.ReadUInt(0x12, 16),
            FinDamageEff = (short)Input.ReadUInt(0x14, 16),
            DefEffect = (short)Input.ReadUInt(0x16, 16),
            Knockback = Input.ReadSingle(0x18),
            Unk4 = (int)Input.ReadUInt(0x1C, 32),
            Unk5 = (int)Input.ReadUInt(0x20, 32),
            Unk6 = (int)Input.ReadUInt(0x24, 32),
            Damage = Input.ReadSingle(0x28),
            HitCount = (short)Input.ReadUInt(0x2C, 16),
            HitStop = (short)Input.ReadUInt(0x2E, 16),
            HitSpeed = (short)Input.ReadUInt(0x30, 16),
            Unk7 = (short)Input.ReadUInt(0x32, 16),
            Unk8 = (short)Input.ReadUInt(0x34, 16),
            Unk9 = (short)Input.ReadUInt(0x36, 16),
            DamageSound = (short)Input.ReadUInt(0x38, 16),
            DamageParticle = (short)Input.ReadUInt(0x3A, 16),
            DefenseSound = (short)Input.ReadUInt(0x3C, 16),
            DefenseParticle = (short)Input.ReadUInt(0x3E, 16),
            EnemySound = (short)Input.ReadUInt(0x40, 16),
        };
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public static void ReadAllJutsuName()
        {
            byte[] jutsuNameAreaBuffer = new byte[0xC5 * 0x4];
            PCSX2Process.ReadProcessMemory(PCSX2Process.processHandle, (IntPtr)0x208C2AD0, jutsuNameAreaBuffer, jutsuNameAreaBuffer.Length, out var none2);
            for (int i = 0; i < 0xC5; i++)
            {
                int currentPointer = BitConverter.ToInt32(jutsuNameAreaBuffer, i * 4);
                string docodedAnmName = Util.ReadStringWithOffset(currentPointer, false);
                charJutsuNameList.Add(docodedAnmName);
            }
        }
        public static (int selectedJutsu1, int selectedJutsu2) GetCharSelectedJutsu(int charID)
        {
            int charID2 = charID * 2;
            int charCount = 94 * 2;
            while (charSelectedJutsuList.Count <= charCount)
            {
                charSelectedJutsuList.Add(254);
            }
            if (charSelectedJutsuList[charID2] == 254 && charSelectedJutsuList[charID2 + 1] == 254)
            {
                int anmNameAreaPointer = 0x208E2AD0;

                IntPtr anmNameAreaOffset = (IntPtr)anmNameAreaPointer;

                byte[] anmNameAreaBuffer = new byte[charCount * 0x4];
                PCSX2Process.ReadProcessMemory(PCSX2Process.processHandle, anmNameAreaOffset, anmNameAreaBuffer, anmNameAreaBuffer.Length, out var none2);

                for (int i = 0; i < 2; i++)
                {
                    int skip = i * 0x4;
                    byte[] anmNamePointerBytes = new byte[4];
                    Array.Copy(anmNameAreaBuffer, charID2 * 4 + skip, anmNamePointerBytes, 0, 4);

                    int currentSelectedJutsu = BitConverter.ToInt32(anmNamePointerBytes, 0);

                    charSelectedJutsuList[charID2 + i] = currentSelectedJutsu;
                }
            }
            return (charSelectedJutsuList[charID2], charSelectedJutsuList[charID2 + 1]);
        }
        public static List<CharSkl> GetCharSklPrm(int JutsuIndex)
        {
            while (CharSklPrm.Count <= 197)
            {
                CharSklPrm.Add(new List<CharSkl>());
                CharSklPrmBkp.Add(new List<CharSkl>());
            }
            if (CharSklPrm[JutsuIndex].Count == 0)
            {
                int jutsuConfigAreaPointer = 0x208C8910;
                IntPtr jutsuConfigAreaOffset = (IntPtr)jutsuConfigAreaPointer;

                byte[] jutsuConfigArea = new byte[197 * 0x8];
                PCSX2Process.ReadProcessMemory(PCSX2Process.processHandle, jutsuConfigAreaOffset, jutsuConfigArea, jutsuConfigArea.Length, out var none2);

                int selectedJutsuConfigCount = BitConverter.ToInt32(jutsuConfigArea, JutsuIndex * 0x8);
                int selectedJutsuConfigPointer = BitConverter.ToInt32(jutsuConfigArea, JutsuIndex * 0x8 + 0x4) + 0x20000000;
                for (int i = 0; i < selectedJutsuConfigCount; i++)
                {
                    byte[] jutsuConfigBlock = new byte[0x44];
                    PCSX2Process.ReadProcessMemory(PCSX2Process.processHandle, (IntPtr)selectedJutsuConfigPointer + i * 0x44, jutsuConfigBlock, jutsuConfigBlock.Length, out var none3);
                    CharSklPrm[JutsuIndex].Add(ReadCharSklPrm(jutsuConfigBlock));
                }
            }
            return CharSklPrm[JutsuIndex];
        }
        public static void SendTextSkl(JutsuParameters sklForm, CharSkl charSklPrm, int selectedGroup, int charID)
        {
            CheckState GetCheckState(uint value) => value == 0 ? CheckState.Unchecked : CheckState.Checked;

            sklForm.checkedListBox1.SetItemCheckState(0, GetCheckState(charSklPrm.isUseCamera));
            sklForm.checkedListBox1.SetItemCheckState(1, GetCheckState(charSklPrm.isPlayerUse1CMN));
            sklForm.checkedListBox1.SetItemCheckState(2, GetCheckState(charSklPrm.isEnemyUse1CMN));
            sklForm.checkedListBox1.SetItemCheckState(3, GetCheckState(GetBit(charSklPrm.FUnk1, 0)));
            sklForm.checkedListBox1.SetItemCheckState(4, GetCheckState(GetBit(charSklPrm.FUnk1, 1)));
            sklForm.checkedListBox1.SetItemCheckState(5, GetCheckState(GetBit(charSklPrm.FUnk1, 2)));
            sklForm.checkedListBox1.SetItemCheckState(6, GetCheckState(GetBit(charSklPrm.FUnk1, 3)));
            sklForm.checkedListBox1.SetItemCheckState(7, GetCheckState(GetBit(charSklPrm.FUnk1, 4)));
            sklForm.checkedListBox1.SetItemCheckState(8, GetCheckState(GetBit(charSklPrm.FUnk1, 5)));
            sklForm.checkedListBox1.SetItemCheckState(9, GetCheckState(GetBit(charSklPrm.FUnk1, 6)));
            sklForm.checkedListBox1.SetItemCheckState(10, GetCheckState(GetBit(charSklPrm.FUnk1, 7)));
            sklForm.checkedListBox1.SetItemCheckState(11, GetCheckState(GetBit(charSklPrm.FUnk2, 0)));
            sklForm.checkedListBox1.SetItemCheckState(12, GetCheckState(GetBit(charSklPrm.FUnk2, 1)));
            sklForm.checkedListBox1.SetItemCheckState(13, GetCheckState(GetBit(charSklPrm.FUnk2, 2)));
            sklForm.checkedListBox1.SetItemCheckState(14, GetCheckState(GetBit(charSklPrm.FUnk2, 3)));
            sklForm.checkedListBox1.SetItemCheckState(15, GetCheckState(GetBit(charSklPrm.FUnk2, 4)));
            sklForm.checkedListBox1.SetItemCheckState(16, GetCheckState(GetBit(charSklPrm.FUnk2, 5)));
            sklForm.checkedListBox1.SetItemCheckState(17, GetCheckState(GetBit(charSklPrm.FUnk2, 6)));
            sklForm.checkedListBox1.SetItemCheckState(18, GetCheckState(GetBit(charSklPrm.FUnk2, 7)));
            sklForm.checkedListBox1.SetItemCheckState(19, GetCheckState(GetBit(charSklPrm.FUnk3, 0)));
            sklForm.checkedListBox1.SetItemCheckState(20, GetCheckState(GetBit(charSklPrm.FUnk3, 1)));
            sklForm.checkedListBox1.SetItemCheckState(21, GetCheckState(GetBit(charSklPrm.FUnk3, 2)));
            sklForm.checkedListBox1.SetItemCheckState(22, GetCheckState(GetBit(charSklPrm.FUnk3, 3)));
            sklForm.checkedListBox1.SetItemCheckState(23, GetCheckState(GetBit(charSklPrm.FUnk3, 4)));
            sklForm.checkedListBox1.SetItemCheckState(24, GetCheckState(GetBit(charSklPrm.FUnk3, 5)));
            sklForm.checkedListBox1.SetItemCheckState(25, GetCheckState(GetBit(charSklPrm.FUnk3, 6)));
            sklForm.checkedListBox1.SetItemCheckState(26, GetCheckState(GetBit(charSklPrm.FUnk3, 7)));
            sklForm.checkedListBox1.SetItemCheckState(27, GetCheckState(GetBit(charSklPrm.FUnk4, 0)));
            sklForm.checkedListBox1.SetItemCheckState(28, GetCheckState(GetBit(charSklPrm.FUnk4, 1)));
            sklForm.checkedListBox1.SetItemCheckState(29, GetCheckState(GetBit(charSklPrm.FUnk4, 2)));
            sklForm.checkedListBox1.SetItemCheckState(30, GetCheckState(GetBit(charSklPrm.FUnk4, 3)));
            sklForm.checkedListBox1.SetItemCheckState(31, GetCheckState(GetBit(charSklPrm.FUnk4, 4)));
            sklForm.checkedListBox1.SetItemCheckState(32, GetCheckState(GetBit(charSklPrm.FUnk4, 5)));
            sklForm.checkedListBox1.SetItemCheckState(33, GetCheckState(GetBit(charSklPrm.FUnk4, 6)));
            sklForm.checkedListBox1.SetItemCheckState(34, GetCheckState(GetBit(charSklPrm.FUnk4, 7)));
            sklForm.checkedListBox1.SetItemCheckState(35, GetCheckState(GetBit(charSklPrm.FUnk5, 0)));
            sklForm.checkedListBox1.SetItemCheckState(36, GetCheckState(GetBit(charSklPrm.FUnk5, 1)));
            sklForm.checkedListBox1.SetItemCheckState(37, GetCheckState(GetBit(charSklPrm.FUnk5, 2)));
            sklForm.checkedListBox1.SetItemCheckState(38, GetCheckState(GetBit(charSklPrm.FUnk5, 3)));
            sklForm.checkedListBox1.SetItemCheckState(39, GetCheckState(GetBit(charSklPrm.FUnk5, 4)));
            sklForm.checkedListBox1.SetItemCheckState(40, GetCheckState(GetBit(charSklPrm.FUnk5, 5)));
            sklForm.checkedListBox1.SetItemCheckState(41, GetCheckState(GetBit(charSklPrm.FUnk5, 6)));
            sklForm.checkedListBox1.SetItemCheckState(42, GetCheckState(GetBit(charSklPrm.FUnk5, 7)));
            sklForm.checkedListBox1.SetItemCheckState(43, GetCheckState(GetBit(charSklPrm.FUnk6, 0)));
            sklForm.checkedListBox1.SetItemCheckState(44, GetCheckState(GetBit(charSklPrm.FUnk6, 1)));
            sklForm.checkedListBox1.SetItemCheckState(45, GetCheckState(GetBit(charSklPrm.FUnk6, 2)));
            sklForm.checkedListBox1.SetItemCheckState(46, GetCheckState(GetBit(charSklPrm.FUnk6, 3)));
            sklForm.checkedListBox1.SetItemCheckState(47, GetCheckState(GetBit(charSklPrm.FUnk6, 4)));
            sklForm.checkedListBox1.SetItemCheckState(48, GetCheckState(GetBit(charSklPrm.FUnk6, 5)));
            sklForm.checkedListBox1.SetItemCheckState(49, GetCheckState(GetBit(charSklPrm.FUnk6, 6)));
            sklForm.checkedListBox1.SetItemCheckState(50, GetCheckState(GetBit(charSklPrm.FUnk6, 7)));
            sklForm.checkedListBox1.SetItemCheckState(51, GetCheckState(GetBit(charSklPrm.FUnk7, 0)));
            sklForm.checkedListBox1.SetItemCheckState(52, GetCheckState(GetBit(charSklPrm.FUnk7, 1)));
            sklForm.checkedListBox1.SetItemCheckState(53, GetCheckState(GetBit(charSklPrm.FUnk7, 2)));
            sklForm.checkedListBox1.SetItemCheckState(54, GetCheckState(GetBit(charSklPrm.FUnk7, 3)));
            sklForm.checkedListBox1.SetItemCheckState(55, GetCheckState(GetBit(charSklPrm.FUnk7, 4)));
            sklForm.checkedListBox1.SetItemCheckState(56, GetCheckState(GetBit(charSklPrm.FUnk7, 5)));
            sklForm.checkedListBox1.SetItemCheckState(57, GetCheckState(GetBit(charSklPrm.FUnk7, 6)));
            sklForm.checkedListBox1.SetItemCheckState(58, GetCheckState(GetBit(charSklPrm.FUnk7, 7)));
            sklForm.checkedListBox1.SetItemCheckState(59, GetCheckState(GetBit(charSklPrm.FUnk8, 0)));
            sklForm.checkedListBox1.SetItemCheckState(60, GetCheckState(GetBit(charSklPrm.FUnk8, 1)));
            sklForm.checkedListBox1.SetItemCheckState(61, GetCheckState(GetBit(charSklPrm.FUnk8, 2)));
            sklForm.checkedListBox1.SetItemCheckState(62, GetCheckState(GetBit(charSklPrm.FUnk8, 3)));
            sklForm.checkedListBox1.SetItemCheckState(63, GetCheckState(GetBit(charSklPrm.FUnk8, 4)));
            sklForm.checkedListBox1.SetItemCheckState(64, GetCheckState(GetBit(charSklPrm.FUnk8, 5)));
            sklForm.checkedListBox1.SetItemCheckState(65, GetCheckState(GetBit(charSklPrm.FUnk8, 6)));
            sklForm.checkedListBox1.SetItemCheckState(66, GetCheckState(GetBit(charSklPrm.FUnk8, 7)));
            sklForm.checkedListBox1.SetItemCheckState(67, GetCheckState((uint)charSklPrm.FUnk9));
            sklForm.cmbFirstDmgEffect.SelectedIndex = charSklPrm.IniDamageEffect;
            sklForm.cmbLastDmgEffect.SelectedIndex = charSklPrm.FinDamageEff;

            sklForm.cmbDefenseEffect.SelectedIndex = charSklPrm.DefEffect;

            sklForm.numKnockback.Value = Convert.ToDecimal(charSklPrm.Knockback);

            sklForm.numUnk4.Value = charSklPrm.Unk4;
            sklForm.numUnk5.Value = charSklPrm.Unk5;
            sklForm.numUnk6.Value = charSklPrm.Unk6;

            sklForm.numDamage.Value = Convert.ToDecimal(charSklPrm.Damage);
            sklForm.numHitCount.Value = Convert.ToDecimal(charSklPrm.HitCount);
            sklForm.numHitStop.Value = Convert.ToDecimal(charSklPrm.HitStop);
            sklForm.numHitSpeed.Value = Convert.ToDecimal(charSklPrm.HitSpeed);

            sklForm.numUnk7.Value = charSklPrm.Unk7;
            sklForm.numUnk8.Value = charSklPrm.Unk8;
            sklForm.numUnk9.Value = charSklPrm.Unk9;

            sklForm.numDmgSound.Value = Convert.ToDecimal(charSklPrm.DamageSound);
            sklForm.cmbDmgParticle.Items.AddRange(sklForm.cmbDmgParticle.Items.Count == 0 ? PlAtk.DamageParticleList.Values.ToArray() : new object[0]);
            int currentDmgParticle = charSklPrm.DamageParticle;
            sklForm.cmbDmgParticle.SelectedIndex = currentDmgParticle > 24 || currentDmgParticle == -1 ? 0 : currentDmgParticle + 1;
            sklForm.numDefSound.Value = Convert.ToDecimal(charSklPrm.DefenseSound);
            sklForm.cmbDefenseParticle.Items.AddRange(sklForm.cmbDefenseParticle.Items.Count == 0 ? PlAtk.DefenseParticleList.Values.ToArray() : new object[0]);
            int currentDefenseParticle = charSklPrm.DefenseParticle;
            sklForm.cmbDefenseParticle.SelectedIndex = currentDefenseParticle == -1 ? 0 : currentDefenseParticle + 1;
            sklForm.numEnemySound.Value = Convert.ToDecimal(charSklPrm.EnemySound);
        }
        public static uint GetBit(int value, int pos)
        {
            int mask = 1 << pos;
            return (uint)((value & mask) == 0 ? 0 : 1);
        }
        public static int SetBit(int value, int pos, int bitvalue)
        {
            if (bitvalue == 1)
            {
                // Define o bit na posição para 1 usando OR e um bit mask.
                return (value | (1 << pos));
            }
            else
            {
                // Define o bit na posição para 0 usando AND com um bit mask invertido.
                return (value & ~(1 << pos));
            }
        }

        public static byte[] UpdateCharSklPrm(JutsuParameters sklForm, CharSkl charSklPrm)
        {
            int GetCheckState(CheckState value) => value == CheckState.Unchecked ? 0 : 1;

            List<byte> sklBlockBytes = new List<byte>();
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(0x612118)));
            sklBlockBytes.Add((byte)GetCheckState(sklForm.checkedListBox1.GetItemCheckState(0)));
            sklBlockBytes.Add((byte)GetCheckState(sklForm.checkedListBox1.GetItemCheckState(1)));
            sklBlockBytes.Add((byte)GetCheckState(sklForm.checkedListBox1.GetItemCheckState(2)));
            sklBlockBytes.Add(0);
            int FUnk1 = 0;
            FUnk1 = SetBit(FUnk1, 0, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(3)));
            FUnk1 = SetBit(FUnk1, 1, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(4)));
            FUnk1 = SetBit(FUnk1, 2, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(5)));
            FUnk1 = SetBit(FUnk1, 3, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(6)));
            FUnk1 = SetBit(FUnk1, 4, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(7)));
            FUnk1 = SetBit(FUnk1, 5, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(8)));
            FUnk1 = SetBit(FUnk1, 6, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(9)));
            FUnk1 = SetBit(FUnk1, 7, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(10)));
            sklBlockBytes.Add((byte)FUnk1);
            int FUnk2 = 0;
            FUnk2 = SetBit(FUnk2, 0, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(11)));
            FUnk2 = SetBit(FUnk2, 1, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(12)));
            FUnk2 = SetBit(FUnk2, 2, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(13)));
            FUnk2 = SetBit(FUnk2, 3, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(14)));
            FUnk2 = SetBit(FUnk2, 4, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(15)));
            FUnk2 = SetBit(FUnk2, 5, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(16)));
            FUnk2 = SetBit(FUnk2, 6, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(17)));
            FUnk2 = SetBit(FUnk2, 7, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(18)));
            sklBlockBytes.Add((byte)FUnk2);
            int FUnk3 = 0;
            FUnk3 = SetBit(FUnk3, 0, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(19)));
            FUnk3 = SetBit(FUnk3, 1, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(20)));
            FUnk3 = SetBit(FUnk3, 2, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(21)));
            FUnk3 = SetBit(FUnk3, 3, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(22)));
            FUnk3 = SetBit(FUnk3, 4, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(23)));
            FUnk3 = SetBit(FUnk3, 5, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(24)));
            FUnk3 = SetBit(FUnk3, 6, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(25)));
            FUnk3 = SetBit(FUnk3, 7, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(26)));
            sklBlockBytes.Add((byte)FUnk3);
            int FUnk4 = 0;
            FUnk4 = SetBit(FUnk4, 0, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(27)));
            FUnk4 = SetBit(FUnk4, 1, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(28)));
            FUnk4 = SetBit(FUnk4, 2, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(29)));
            FUnk4 = SetBit(FUnk4, 3, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(30)));
            FUnk4 = SetBit(FUnk4, 4, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(31)));
            FUnk4 = SetBit(FUnk4, 5, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(32)));
            FUnk4 = SetBit(FUnk4, 6, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(33)));
            FUnk4 = SetBit(FUnk4, 7, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(34)));
            sklBlockBytes.Add((byte)FUnk4);
            int FUnk5 = 0;
            FUnk5 = SetBit(FUnk5, 0, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(35)));
            FUnk5 = SetBit(FUnk5, 1, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(36)));
            FUnk5 = SetBit(FUnk5, 2, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(37)));
            FUnk5 = SetBit(FUnk5, 3, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(38)));
            FUnk5 = SetBit(FUnk5, 4, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(39)));
            FUnk5 = SetBit(FUnk5, 5, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(40)));
            FUnk5 = SetBit(FUnk5, 6, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(41)));
            FUnk5 = SetBit(FUnk5, 7, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(42)));
            sklBlockBytes.Add((byte)FUnk5);
            int FUnk6 = 0;
            FUnk6 = SetBit(FUnk6, 0, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(43)));
            FUnk6 = SetBit(FUnk6, 1, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(44)));
            FUnk6 = SetBit(FUnk6, 2, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(45)));
            FUnk6 = SetBit(FUnk6, 3, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(46)));
            FUnk6 = SetBit(FUnk6, 4, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(47)));
            FUnk6 = SetBit(FUnk6, 5, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(48)));
            FUnk6 = SetBit(FUnk6, 6, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(49)));
            FUnk6 = SetBit(FUnk6, 7, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(50)));
            sklBlockBytes.Add((byte)FUnk6);
            int FUnk7 = 0;
            FUnk7 = SetBit(FUnk7, 0, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(51)));
            FUnk7 = SetBit(FUnk7, 1, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(52)));
            FUnk7 = SetBit(FUnk7, 2, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(53)));
            FUnk7 = SetBit(FUnk7, 3, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(54)));
            FUnk7 = SetBit(FUnk7, 4, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(55)));
            FUnk7 =  SetBit(FUnk7, 5, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(56)));
            FUnk7 = SetBit(FUnk7, 6, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(57)));
            FUnk7 = SetBit(FUnk7, 7, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(58)));
            sklBlockBytes.Add((byte)FUnk7);
            int FUnk8 = 0;
            FUnk8 = SetBit(FUnk8, 0, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(59)));
            FUnk8 = SetBit(FUnk8, 1, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(60)));
            FUnk8 = SetBit(FUnk8, 2, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(61)));
            FUnk8 = SetBit(FUnk8, 3, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(62)));
            FUnk8 = SetBit(FUnk8, 4, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(63)));
            FUnk8 = SetBit(FUnk8, 5, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(64)));
            FUnk8 = SetBit(FUnk8, 6, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(65)));
            FUnk8 = SetBit(FUnk8, 7, GetCheckState(sklForm.checkedListBox1.GetItemCheckState(66)));
            sklBlockBytes.Add((byte)FUnk8);
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(GetCheckState(sklForm.checkedListBox1.GetItemCheckState(67)))));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(sklForm.cmbFirstDmgEffect.SelectedIndex)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(sklForm.cmbLastDmgEffect.SelectedIndex)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(sklForm.cmbDefenseEffect.SelectedIndex)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(sklForm.numKnockback.Value)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(sklForm.numUnk4.Value)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(sklForm.numUnk5.Value)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(sklForm.numUnk6.Value)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(sklForm.numDamage.Value)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(sklForm.numHitCount.Value)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(sklForm.numHitStop.Value)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(sklForm.numHitSpeed.Value)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(sklForm.numUnk7.Value)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(sklForm.numUnk8.Value)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(sklForm.numUnk9.Value)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(sklForm.numDmgSound.Value)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(sklForm.cmbDmgParticle.SelectedIndex - 1)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(sklForm.numDefSound.Value)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(sklForm.cmbDefenseParticle.SelectedIndex - 1)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(sklForm.numEnemySound.Value)));
            sklBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(0)));
            return sklBlockBytes.ToArray();
        }

        public static void WriteCharSkl(byte[] sklBlockBytes, int sklID)
        {
            IntPtr jutsuConfigAreaOffset = (IntPtr)(sklID * 8) + 0x208C8914;

            byte[] jutsuBytes = new byte[4];
            PCSX2Process.ReadProcessMemory(PCSX2Process.processHandle, jutsuConfigAreaOffset, jutsuBytes, 4, out var none2);


            IntPtr jutsuOffset = (IntPtr)BitConverter.ToInt32(jutsuBytes, 0) + 0x20000000;
            PCSX2Process.WriteProcessMemory(PCSX2Process.processHandle, (IntPtr)jutsuOffset, sklBlockBytes, (uint)sklBlockBytes.Length, out var wtf);
        }
    }
}
