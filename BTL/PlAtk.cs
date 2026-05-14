using CCSFileExplorerWV;
using DiscUtils.Btrfs.Base.Items;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;
using static CCSFileExplorerWV.CCSFile;

namespace UN5ModdingWorkshop
{
    internal class PlAtk
    {
        public static List<List<PlAtk>> CharAtkPrm = new List<List<PlAtk>>();
        public static List<List<PlAtk>> CharAtkPrmBkp = new List<List<PlAtk>>();
        public static List<List<string>> comboNameList = new List<List<string>>();

        #region Atk Attributes

        uint NameOffs1 = 0x6105C0; //Unused
        uint NameOffs2 = 0x6105C0; //Unused

        uint Index = 0x0;

        uint ComboNameTableIdx = 0x0;
        uint UseComboNameTableFlag = 0x80;
        uint ComboNameTableIdx2 = 0x0;

        uint JutsuIdx = 0x1;

        uint SpecialFlag = 0x1;
        uint ThrowFlag = 0x0;
        uint JutsuFlag = 0x0;
        uint JanKenPonFlag = 0x0;

        uint DirectionFlag = 0x0;
        uint OtherFlag1 = 0x0;
        uint DefenseFlag = 0x0;
        uint OtherFlag2 = 0x0;

        uint PreviousIdx = 0x0;
        uint SequenceIdx = 0x0;
        int KawarimiDificulty = 0x0;

        uint Type = 0x0;
        uint DpadFlag = 0x0;
        uint ButtonFlag = 0x0;
        uint AutoExecuteFlag = 0x0;

        float Chakra = 0f;
        float Damage = 0.015f;
        float Knockback = 1f;

        uint DamageEffect;
        uint DefenseEffect;

        int HitCount = 1;
        int HitStop = 1;
        int HitSpeed = 0;

        float RangeValue1 = 0f;
        float RangeValue2 = 0f;

        float KnockbackDirection = 1.5f;

        int WhiffSound = 30;
        int PlayerSound = -4;
        uint SoundDelay = 0;
        int HitSound = 48;
        int DamageParticle = 1;
        int DefenseSound = 81;
        int DefenseParticle = -1;
        int EnemySound = -1;

        public uint AnimationIdx = 0;

        #endregion

        internal static PlAtk ReadCharAtkPrm(byte[] Input) => new PlAtk
        {
            NameOffs1 = Input.ReadUInt(0x0, 32),
            NameOffs2 = Input.ReadUInt(0x4, 32),

            Index = Input.ReadUInt(0x8, 16),

            ComboNameTableIdx = Input.ReadUInt(0xA, 8),
            UseComboNameTableFlag = Input.ReadUInt(0xB, 8),
            ComboNameTableIdx2 = Input.ReadUInt(0xC, 16),

            JutsuIdx = Input.ReadUInt(0xE, 16),

            SpecialFlag = Input.ReadUInt(0x10, 8),
            ThrowFlag = Input.ReadUInt(0x11, 8),
            JutsuFlag = Input.ReadUInt(0x12, 8),
            JanKenPonFlag = Input.ReadUInt(0x13, 8),

            DirectionFlag = Input.ReadUInt(0x14, 8),
            OtherFlag1 = Input.ReadUInt(0x15, 8),
            DefenseFlag = Input.ReadUInt(0x16, 8),
            OtherFlag2 = Input.ReadUInt(0x17, 8),

            PreviousIdx = Input.ReadUInt(0x18, 8),
            SequenceIdx = Input.ReadUInt(0x19, 8),
            KawarimiDificulty = Input.ReadInt(0x1A, 8),
            //Padding

            Type = Input.ReadUInt(0x1C, 8),
            DpadFlag = Input.ReadUInt(0x1D, 8),
            ButtonFlag = Input.ReadUInt(0x1E, 8),
            AutoExecuteFlag = Input.ReadUInt(0x1F, 8),

            Chakra = Input.ReadSingle(0x20),
            Damage = Input.ReadSingle(0x24),
            Knockback = Input.ReadSingle(0x28),

            DamageEffect = Input.ReadUInt(0x2C, 8),
            DefenseEffect = Input.ReadUInt(0x2D, 8),

            HitCount = Input.ReadInt(0x2E, 16),
            HitStop = Input.ReadInt(0x30, 16),
            HitSpeed = Input.ReadInt(0x32, 16),

            RangeValue1 = Input.ReadSingle(0x34),
            RangeValue2 = Input.ReadSingle(0x38),
            KnockbackDirection = Input.ReadSingle(0x3C),

            WhiffSound = Input.ReadInt(0x40, 16),
            PlayerSound = Input.ReadInt(0x42, 16),
            SoundDelay = Input.ReadUInt(0x44, 16),
            HitSound = Input.ReadInt(0x46, 16),
            DamageParticle = Input.ReadInt(0x48, 16),
            DefenseSound = Input.ReadInt(0x4A, 16),
            DefenseParticle = Input.ReadInt(0x4C, 16),
            EnemySound = Input.ReadInt(0x4E, 16),

            AnimationIdx = Input.ReadUInt(0x50, 32),
        };

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public static void UpdateP1Atk(byte[] atkData, int selectedAtk, int charID) //resultbytes is divided into two parts because the 4 bytes of offset 0x1C
                                                                            //change when it goes into memory, which causes a bug if it is changed.
        {
            int P1AtkOffset = Util.BTL_GetPlayer1MemoryOffs() + 0xBC;
            byte[] atkDataPart = new byte[0x50];
            int skipAtks = selectedAtk * 0x54;

            int P1AtkOffs = Util.ReadProcessMemoryInt32(P1AtkOffset) + skipAtks;
            Array.Copy(atkData, 0, atkDataPart, 0, atkDataPart.Length);
            Util.WriteProcessMemoryBytes(P1AtkOffs, atkDataPart);

            //Write Normal in Memory
            byte[] atkNormalMemoryOffset = PlGen.CharGenPrm[charID].AtkListOffset;

            P1AtkOffs = BitConverter.ToInt32(atkNormalMemoryOffset, 0) + skipAtks;
            Util.WriteProcessMemoryBytes(P1AtkOffs, atkDataPart);
        }
        
        public static PlAtk GetCharAtk(int charID, int atkID)
        {
            int atkCount = PlGen.CharGenPrm[charID].AtkCount;

            while (CharAtkPrm.Count <= GAME.charCount)
            {
                CharAtkPrm.Add(new List<PlAtk>());
                CharAtkPrmBkp.Add(new List<PlAtk>());
            }
            if (CharAtkPrm[charID].Count == 0)
            {
                byte[] atkListOffsetBytes = PlGen.CharGenPrm[charID].AtkListOffset;
                int atkListPointer = BitConverter.ToInt32(PlGen.CharGenPrm[charID].AtkListOffset, 0);

                List<PlAtk> charAtkPrm = new List<PlAtk>();
                List<PlAtk> charAtkPrmBkp = new List<PlAtk>();
                for (int i = 0; i != atkCount; i++)
                {
                    int skipsAtkBlocks = i * 0x54;
                    int currentAtkListPointer = atkListPointer + skipsAtkBlocks;
                    byte[] currentAtkBlock = Util.ReadProcessMemoryBytes(currentAtkListPointer, 0x54);

                    var ninja = ReadCharAtkPrm(currentAtkBlock);
                    var clone = (PlAtk)ninja.Clone();
                    charAtkPrm.Add(ninja);
                    charAtkPrmBkp.Add(clone);
                }
                CharAtkPrm[charID] = charAtkPrm;
                CharAtkPrmBkp[charID] = charAtkPrmBkp;
            }
            return CharAtkPrm[charID][atkID];
        }
        
        public static string GetCharComboName(int charID, int comboNameID)
        {
            if (comboNameList.Count == 0)
            {
                for (int i = 0; i < GAME.charCount; i++)
                {
                    List<string> comboName = new List<string>();
                    for (int j = 0; j <= PlGen.CharGenPrm[i].AtkCount; j++)
                    {
                        comboName.Add("");
                    }
                    comboNameList.Add(comboName);
                }
            }
            if (comboNameList[charID][1] == "")
            {
                int charAtkNameTblOffset = 0x5BA950;
                byte[] generalComboNameOffset = Util.ReadProcessMemoryBytes(charAtkNameTblOffset, 4);
                int comboOffset = BitConverter.ToInt32(generalComboNameOffset, 0);

                byte[] generalComboNameArea = Util.ReadProcessMemoryBytes(comboOffset, GAME.charCount * 4);
                byte[] charComboNameAreaOffsetBytes = new byte[4];
                Array.Copy(generalComboNameArea, charID * 4, charComboNameAreaOffsetBytes, 0, charComboNameAreaOffsetBytes.Length);
                int charComboNameAreaOffset = BitConverter.ToInt32(charComboNameAreaOffsetBytes, 0);

                byte[] charComboNameArea = Util.ReadProcessMemoryBytes(charComboNameAreaOffset, PlGen.CharGenPrm[charID].AtkCount * 4);
                List<string> charComboName = new List<string>();
                for (int j = 0; j < PlGen.CharGenPrm[charID].AtkCount; j++)
                {
                    byte[] charComboNameOffsetBytes = new byte[4];
                    Array.Copy(charComboNameArea, j * 4, charComboNameOffsetBytes, 0, charComboNameOffsetBytes.Length);
                    int charComboNameOffset = BitConverter.ToInt32(charComboNameOffsetBytes, 0);

                    charComboName.Add(Util.ReadStringWithOffset(charComboNameOffset, false));
                }
                comboNameList[charID] = charComboName;
            }
            return comboNameList[charID][comboNameID];
        }
        
        public static void AddCharComboList(MovesetParameters movForm, int charID, string txtCharNameForm)
        {
            movForm.lblCharName2.Text = txtCharNameForm;
            movForm.lblComboCount2.Text = PlGen.CharGenPrm[charID].AtkCount.ToString();
            for (int i = 1; i < 94; i++)
            {
                List<string> comboName = new List<string>();
                for (int j = 0; j <= 3; j++)
                {
                    string hm = GetCharComboName(i, j);
                }
            }
            List<string> jutsus = new List<string>();
            for (int i = 1; i < 94; i++)
            {
                jutsus.Add(comboNameList[i][1]);
                jutsus.Add(comboNameList[i][3]);
            }
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            File.WriteAllLines(Path.Combine(desktop, "jutsus.txt"), jutsus.ToArray());
            for (int i = 0; i < PlGen.CharGenPrm[charID].AtkCount; i++)
            {
                switch (i)
                {
                    case 4:
                        movForm.listBox1.Items.Add($"{i}: (Ultimate Jutsu 1)");
                        break;
                    case 5:
                        movForm.listBox1.Items.Add($"{i}: (Ultimate Jutsu 2)");
                        break;
                    case 6:
                        movForm.listBox1.Items.Add($"{i}: (Ultimate Jutsu 3)");
                        break;
                    case 7:
                        movForm.listBox1.Items.Add($"{i}: (Ultimate Jutsu 1 Buff)");
                        break;
                    case 8:
                        movForm.listBox1.Items.Add($"{i}: (Ultimate Jutsu 2 Buff)");
                        break;
                    case 9:
                        movForm.listBox1.Items.Add($"{i}: (Ultimate Jutsu 3 Buff)");
                        break;
                    case 10:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Side 1)");
                        break;
                    case 11:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Up 1)");
                        break;
                    case 12:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Down 1)");
                        break;
                    case 13:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Side 2)");
                        break;
                    case 14:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Up 2)");
                        break;
                    case 15:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Down 2)");
                        break;
                    case 16:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Side 3)");
                        break;
                    case 17:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Up 3)");
                        break;
                    case 18:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Down 3)");
                        break;
                    case 19:
                        movForm.listBox1.Items.Add($"{i}: (Dash)");
                        break;
                    case 20:
                        movForm.listBox1.Items.Add($"{i}: (JanKenPon)");
                        break;
                    default:
                        if (GetCharComboName(charID, i) == "")
                        {
                            for (int i2 = 0; i2 < comboNameList[charID].Count; i2++)
                            {
                                int value = i2 + 1;
                                if (i >= 20 && movForm.listBox1.Items[i - value].ToString().Contains(" (JanKenPon)"))
                                {
                                    movForm.listBox1.Items.Add($"{i}: (Base Combo)");
                                    for (int i3 = 0; i3 < comboNameList[charID].Count; i3++)
                                    {
                                        if (comboNameList[charID][i + 1] == "")
                                        {
                                            movForm.listBox1.Items.Add($"{i + 1}: (Base Combo)");
                                            i++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    break;
                                }
                                if (comboNameList[charID][i + i2] != "")
                                {
                                    movForm.listBox1.Items.Add($"{i}: " + comboNameList[charID][i + i2]);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            movForm.listBox1.Items.Add($"{i}: " + GetCharComboName(charID, i));
                        }
                        break;
                }
            }

            movForm.btnEditAnmParameters.Visible = true;
            movForm.btnEditAtkParameters.Visible = false;
        }

        public static void SendTextAtk(int charID, MovesetParameters movForm, PlAtk Atk)
        {
            if(Atk.Index < 0x15)
            {
                movForm.cmbDpad.Enabled = false;
            }
            else
            {
                movForm.cmbDpad.Enabled = true;
            }
            // Header
            movForm.numKawarimi.Value = Convert.ToInt16(Atk.KawarimiDificulty);
            movForm.txtComboName.Text = movForm.listBox1.SelectedItem.ToString().Split(':')[1].Trim();
            movForm.lblInfo.Text = GetAttackLabel(Atk);

            // Type dropdown
            movForm.cmbType.SelectedIndex = Atk.Type switch
            {
                0x11 => 0,
                0x12 => 1,
                0x14 => 2,
                _ => movForm.cmbType.SelectedIndex
            };

            // Flags
            movForm.clbFlags.Items.Clear();
            var flags = new (string name, uint source, int bit)[]
            {
                ("Ground",                    Atk.DirectionFlag, 0),
                ("Air",                       Atk.DirectionFlag, 1),
                ("Follow Up",                 Atk.DirectionFlag, 2),
                ("Up",                        Atk.DirectionFlag, 3),
                ("Down",                      Atk.DirectionFlag, 4),
                ("Front",                     Atk.DirectionFlag, 5),
                ("Back",                      Atk.DirectionFlag, 6),
                ("Side Extra-Hit",            Atk.DirectionFlag, 7),
                ("Down Extra-Hit",            Atk.OtherFlag1,    0),
                ("Up Extra-Hit",              Atk.OtherFlag1,    1),
                (null,                        Atk.OtherFlag1,    2),
                (null,                        Atk.OtherFlag1,    3),
                ("Anti Counter",              Atk.OtherFlag1,    4),
                (null,                        Atk.OtherFlag1,    5),
                (null,                        Atk.OtherFlag1,    6),
                (null,                        Atk.OtherFlag1,    7),
                (null,                        Atk.DefenseFlag,   0),
                (null,                        Atk.DefenseFlag,   1),
                ("Without Wall KB",           Atk.DefenseFlag,   2),
                ("Don't Bounce",              Atk.DefenseFlag,   3),
                (null,                        Atk.DefenseFlag,   4),
                ("Hit Fallen",                Atk.DefenseFlag,   5),
                ("Undefendable",              Atk.DefenseFlag,   6),
                ("Break Defense",             Atk.DefenseFlag,   7),
                ("Damage on Defense",         Atk.OtherFlag2,    0),
                ("Damage on Counter Attack",  Atk.OtherFlag2,    1),
                ("Backdash",                  Atk.OtherFlag2,    2),
                (null,                        Atk.OtherFlag2,    3),
                ("Hit Fainted",               Atk.OtherFlag2,    4),
                (null,                        Atk.OtherFlag2,    5),
                (null,                        Atk.OtherFlag2,    6),
                (null,                        Atk.OtherFlag2,    7),
            };

            int unkCount = 1;
            foreach (var (name, source, bit) in flags)
            {
                bool isSet = ((source >> bit) & 1) == 1;
                movForm.clbFlags.Items.Add(name ?? $"Unk{unkCount}", isSet);
                if (name == null) unkCount++;
            }

            // Numeric fields
            movForm.numChakra.Value = (decimal)Atk.Chakra;
            movForm.numDamage.Value = (decimal)Atk.Damage;
            movForm.numKnockback.Value = (decimal)Atk.Knockback;
            movForm.numRangeValue1.Value = Atk.RangeValue1 < -100f ? 0 : (decimal)Atk.RangeValue1;
            movForm.numRangeValue2.Value = Atk.RangeValue2 < -100f ? 0 : (decimal)Atk.RangeValue2;
            movForm.numKnockbackDirection.Value = (decimal)Atk.KnockbackDirection;
            movForm.numSoundDelay.Value = Atk.SoundDelay;
            movForm.numHitSound.Value = Atk.HitSound;
            movForm.numWhiffSound.Value = Atk.WhiffSound;
            movForm.numDefenseSound.Value = Atk.DefenseSound;
            movForm.numEnemySound.Value = Atk.EnemySound;

            // Hit fields com flag 0x7FFF
            SetHitField(movForm.numHitCount, movForm.chkHitCount, Atk.HitCount);
            SetHitField(movForm.numHitStop, movForm.chkHitStop, Atk.HitStop);
            SetHitField(movForm.numHitSpeed, movForm.chkHitSpeed, Atk.HitSpeed);

            // Combos
            movForm.cmbDmgEffect.SelectedIndex = Math.Min((int)Atk.DamageEffect, 31);
            movForm.cmbDefenseEffect.SelectedIndex = Atk.DefenseEffect == 255 ? 0 : (int)Atk.DefenseEffect + 1;
            movForm.cmbDmgParticle.SelectedIndex = Atk.DamageParticle > 24 || Atk.DamageParticle == -1 ? 0 : Atk.DamageParticle + 1;
            movForm.cmbDefenseParticle.SelectedIndex = Atk.DefenseParticle < 0 ? 0 : Atk.DefenseParticle + 1;

            int plSound = (int)Atk.PlayerSound;
            movForm.cmbPLSound.SelectedIndex = plSound switch
            {
                -4 => 0,
                -3 => 1,
                -2 => 2,
                -1 => 3,
                _ when plSound > 34 => 0,
                _ => plSound + 4
            };

            SetDpadFlagGroupToCmbBox((int)Atk.DpadFlag, movForm.cmbDpad);
            DrawCommandSequence(movForm.picCommand, Atk, charID);
        }

        private static string GetAttackLabel(PlAtk Atk)
        {
            bool isThrow = Atk.ThrowFlag == 1 || Atk.ThrowFlag == 2;
            if (isThrow && ((Atk.Type >> 1) & 1) == 1) return "Ground Throw";
            if (isThrow && ((Atk.Type >> 2) & 1) == 1) return "Aerial Throw";
            if (Atk.SpecialFlag == 0x4) return "Combo";
            if (Atk.SpecialFlag == 0x8 || Atk.SpecialFlag == 0x10) return "Charge";
            if (((Atk.Type >> 2) & 1) == 1 && Atk.SpecialFlag == 1) return "While Jumping";
            if (Atk.Index <= 0x3) return "Jutsu";
            if (Atk.Index <= 0x9) return "Ultimate Jutsu";
            if (Atk.Index <= 0x12) return "Extra-Hit";
            if (Atk.Index == 0x13) return "Dash";
            if (Atk.Index == 0x14) return "Jan-Ken-Pon";
            return "";
        }

        private static void SetHitField(NumericUpDown num, CheckBox chk, int value)
        {
            bool isFlag = value == 0x7FFF;
            num.Value = isFlag ? 0 : value;
            num.Enabled = !isFlag;
            chk.Checked = isFlag;
        }

        private static void InitCombo(ComboBox cmb, object[] items)
        {
            if (cmb.Items.Count == 0)
                cmb.Items.AddRange(items);
        }

        public static void DrawCommandSequence(PictureBox pic, PlAtk Atk, int charID)
        {
            int currentAtkPrevious = (int)Atk.PreviousIdx;
            List<uint> buttons = new List<uint>();
            List<uint> dpads = new List<uint>();

            if (Atk.SpecialFlag == 0x1 || Atk.SpecialFlag == 0x4 || Atk.ThrowFlag == 0x1 || Atk.ThrowFlag == 0x2)
            {
                // Golpe atual primeiro
                if ((Atk.AutoExecuteFlag >> 3) == 0)
                {
                    dpads.Add(Atk.DpadFlag);
                    buttons.Add(Atk.ButtonFlag);
                }

                // Percorre a cadeia até -1
                while (currentAtkPrevious != 255)
                {
                    PlAtk currentAtk = GetCharAtk(charID, currentAtkPrevious);
                    dpads.Add(currentAtk.DpadFlag);
                    buttons.Add(currentAtk.ButtonFlag);
                    currentAtkPrevious = (int)currentAtk.PreviousIdx;
                }
            }
            else
            {
                // Golpe simples sem cadeia
                dpads.Add(Atk.DpadFlag);
                buttons.Add(Atk.ButtonFlag);
            }
            buttons.Reverse();
            dpads.Reverse();

            if (Atk.Index == 0x0 || Atk.Index == 0x1)
            {
                const int iconSize = 24;
                const int padding = 2;

                // Up Up + Ball: 3 ícones fixos
                int baseWidth = Math.Max(padding + 3 * (iconSize + padding), 10);
                int baseHeight = iconSize + padding * 2;

                float scale = Math.Min((float)pic.Width / baseWidth, (float)pic.Height / baseHeight);
                int scaledIcon = (int)(iconSize * scale);
                int scaledPad = (int)(padding * scale);

                Bitmap bmp = new Bitmap(pic.Width, pic.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                    int[] icons = { 0, 0, 4 }; // DpadCima, DpadCima, Bola
                    int x = scaledPad;
                    foreach (int idx in icons)
                    {
                        g.DrawImage(CharSel.CommandIcons[idx],
                            new Rectangle(x, scaledPad, scaledIcon, scaledIcon));
                        x += scaledIcon + scaledPad;
                    }
                }

                pic.Image?.Dispose();
                pic.Image = bmp;
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else if (Atk.Index == 0x2 || Atk.Index == 0x3)
            {
                const int iconSize = 24;
                const int padding = 2;

                // Up Up + Ball: 3 ícones fixos
                int baseWidth = Math.Max(padding + 3 * (iconSize + padding), 10);
                int baseHeight = iconSize + padding * 2;

                float scale = Math.Min((float)pic.Width / baseWidth, (float)pic.Height / baseHeight);
                int scaledIcon = (int)(iconSize * scale);
                int scaledPad = (int)(padding * scale);

                Bitmap bmp = new Bitmap(pic.Width, pic.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                    int[] icons = { 1, 1, 4 }; // DpadCima, DpadCima, Bola
                    int x = scaledPad;
                    foreach (int idx in icons)
                    {
                        g.DrawImage(CharSel.CommandIcons[idx],
                            new Rectangle(x, scaledPad, scaledIcon, scaledIcon));
                        x += scaledIcon + scaledPad;
                    }
                }

                pic.Image?.Dispose();
                pic.Image = bmp;
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else if (Atk.Index >= 0x4 && Atk.Index <= 0x9)
            {
                const int iconSize = 24;
                const int padding = 2;

                // Up Up + Ball: 3 ícones fixos
                int baseWidth = Math.Max(padding + 3 * (iconSize + padding), 10);
                int baseHeight = iconSize + padding * 2;

                float scale = Math.Min((float)pic.Width / baseWidth, (float)pic.Height / baseHeight);
                int scaledIcon = (int)(iconSize * scale);
                int scaledPad = (int)(padding * scale);

                Bitmap bmp = new Bitmap(pic.Width, pic.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                    int[] icons = { 5, 4 };
                    int x = scaledPad;
                    foreach (int idx in icons)
                    {
                        g.DrawImage(CharSel.CommandIcons[idx],
                            new Rectangle(x, scaledPad, scaledIcon, scaledIcon));
                        x += scaledIcon + scaledPad;
                    }
                }

                pic.Image?.Dispose();
                pic.Image = bmp;
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else if (Atk.Index == 0x13)
            {
                const int iconSize = 24;
                const int padding = 2;

                // Up Up + Ball: 3 ícones fixos
                int baseWidth = Math.Max(padding + 3 * (iconSize + padding), 10);
                int baseHeight = iconSize + padding * 2;

                float scale = Math.Min((float)pic.Width / baseWidth, (float)pic.Height / baseHeight);
                int scaledIcon = (int)(iconSize * scale);
                int scaledPad = (int)(padding * scale);

                Bitmap bmp = new Bitmap(pic.Width, pic.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                    int[] icons = { 7, 7, 7 };
                    int x = scaledPad;
                    foreach (int idx in icons)
                    {
                        g.DrawImage(CharSel.CommandIcons[idx],
                            new Rectangle(x, scaledPad, scaledIcon, scaledIcon));
                        x += scaledIcon + scaledPad;
                    }
                }

                pic.Image?.Dispose();
                pic.Image = bmp;
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else if (Atk.Index == 0x14)
            {
                const int iconSize = 24;
                const int padding = 2;

                int baseWidth = Math.Max(padding + 3 * (iconSize + padding), 10);
                int baseHeight = iconSize + padding * 2;

                float scale = Math.Min((float)pic.Width / baseWidth, (float)pic.Height / baseHeight);
                int scaledIcon = (int)(iconSize * scale);
                int scaledPad = (int)(padding * scale);

                Bitmap bmp = new Bitmap(pic.Width, pic.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                    int[] icons = { 6, 5, 4 };
                    int x = scaledPad;
                    foreach (int idx in icons)
                    {
                        g.DrawImage(CharSel.CommandIcons[idx],
                            new Rectangle(x, scaledPad, scaledIcon, scaledIcon));
                        x += scaledIcon + scaledPad;
                    }
                }

                pic.Image?.Dispose();
                pic.Image = bmp;
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else if ((Atk.SpecialFlag != 0 || Atk.ThrowFlag != 0x0) && Atk.SpecialFlag != 0x2)
            {
                const int iconSize = 24;
                const int padding = 2;
                int count = dpads.Count;

                // Conta ícones visíveis: dpad (se >= 2) + botão por entrada
                int totalIcons = count + dpads.Count(d => d >= 2);
                int baseWidth = Math.Max(padding + totalIcons * (iconSize + padding), 10);
                int baseHeight = iconSize + padding * 2;

                float scale = Math.Min((float)pic.Width / baseWidth, (float)pic.Height / baseHeight);
                int scaledIcon = (int)(iconSize * scale);
                int scaledPad = (int)(padding * scale);

                Bitmap bmp = new Bitmap(pic.Width, pic.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                    int x = scaledPad;
                    for (int i = 0; i < count; i++)
                    {
                        if (dpads[i] >= 2)
                        {
                            g.DrawImage(CharSel.CommandIcons[GetDpadIcon((int)dpads[i])],
                                new Rectangle(x, scaledPad, scaledIcon, scaledIcon));
                            x += scaledIcon + scaledPad;
                        }

                        g.DrawImage(CharSel.CommandIcons[4],
                            new Rectangle(x, scaledPad, scaledIcon, scaledIcon));
                        x += scaledIcon + scaledPad;
                    }
                }

                pic.Image?.Dispose();
                pic.Image = bmp;
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        public static void SetDpadFlagGroupToCmbBox(int dpad, ComboBox cmbDpad)
        {
            if (dpad == 0)
            {
                cmbDpad.SelectedIndex = 0;
            } //None
            else if ((dpad & 0x01) != 0)
            {
                cmbDpad.SelectedIndex = 1;
            } //None
            else if ((dpad & 0x02) != 0)
            {
                cmbDpad.SelectedIndex = 2;
            } //Up
            else if ((dpad & 0x04) != 0)
            {
                cmbDpad.SelectedIndex = 3;
            } //Down
            else if ((dpad & 0x10) != 0)
            {
                cmbDpad.SelectedIndex = 4;
            } //Right (Solo)
            else if ((dpad & 0x40) != 0)
            {
                cmbDpad.SelectedIndex = 5;
            } //Right
            else if ((dpad & 0x20) != 0)
            {
                cmbDpad.SelectedIndex = 6;
            } //Left (Solo)
            else if ((dpad & 0x80) != 0)
            {
                cmbDpad.SelectedIndex = 7;
            } //Left

        }

        public static int GetDpadFlagGroupFromCmbBox(ComboBox cmbDpad)
        {
            int dpadFlagGroup = 0;
            int cmbBoxSelectedIndex = cmbDpad.SelectedIndex;
            
            if(cmbBoxSelectedIndex == 1)
            {
                dpadFlagGroup = dpadFlagGroup | 0x1;
            }
            else if(cmbBoxSelectedIndex == 2)
            {
                dpadFlagGroup = dpadFlagGroup | 0x2;
            }
            else if(cmbBoxSelectedIndex == 3)
            {
                dpadFlagGroup = dpadFlagGroup | 0x4;
            }
            else if(cmbBoxSelectedIndex == 4)
            {
                dpadFlagGroup = dpadFlagGroup | 0x10;
            }
            else if(cmbBoxSelectedIndex == 5)
            {
                dpadFlagGroup = dpadFlagGroup | 0x40;
            }
            else if(cmbBoxSelectedIndex == 6)
            {
                dpadFlagGroup = dpadFlagGroup | 0x20;
            }
            else if(cmbBoxSelectedIndex == 7)
            {
                dpadFlagGroup = dpadFlagGroup | 0x80;
            }

            return dpadFlagGroup;
        }

        public static int GetDpadIcon(int dpad)
        {
            int dpadIconIdx = 0;
            if((dpad & 0x02) != 0)
            {
                dpadIconIdx = 0; //Up
            }
            else if((dpad & 0x04) != 0)
            {
                dpadIconIdx = 1; //Right
            }
            else if((dpad & 0x50) != 0)
            {
                dpadIconIdx = 2; //Down
            }
            else if((dpad & 0xA0) != 0)
            {
                dpadIconIdx = 3; //Left
            }
            return dpadIconIdx;
        }

        public static byte[] UpdateCharAtkPrm(MovesetParameters movForm, int charID, int atkID)
        {
            var Atk = CharAtkPrm[charID][atkID];

            List<byte> AtkData = new List<byte>();

            AtkData.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Atk.NameOffs1)));
            AtkData.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Atk.NameOffs1)));

            AtkData.AddRange(BitConverter.GetBytes(Convert.ToUInt16(Atk.Index)));

            AtkData.Add(Convert.ToByte(Atk.ComboNameTableIdx));
            AtkData.Add(Convert.ToByte(Atk.UseComboNameTableFlag));
            AtkData.AddRange(BitConverter.GetBytes(Convert.ToUInt16(Atk.ComboNameTableIdx2)));

            AtkData.AddRange(BitConverter.GetBytes(Convert.ToUInt16(Atk.JutsuIdx)));

            AtkData.Add(Convert.ToByte(Atk.SpecialFlag));
            AtkData.Add(Convert.ToByte(Atk.ThrowFlag));
            AtkData.Add(Convert.ToByte(Atk.JutsuFlag));
            AtkData.Add(Convert.ToByte(Atk.JanKenPonFlag));

            int flagsCount = 0;
            int[] bitsGroupFlag1 = new int[8];
            for(int i = 0; i < 8; i++)
            {
                bitsGroupFlag1[i] = movForm.clbFlags.GetItemChecked(flagsCount) ? 1 : 0;
                flagsCount++;
            }
            byte byteGroupFlag1 = Util.FormarByte(bitsGroupFlag1);
            AtkData.Add(byteGroupFlag1);
            Atk.DirectionFlag = byteGroupFlag1;

            int[] bitsGroupFlag2 = new int[8];
            for (int i = 0; i < 8; i++)
            {
                bitsGroupFlag2[i] = movForm.clbFlags.GetItemChecked(flagsCount) ? 1 : 0;
                flagsCount++;
            }
            byte byteGroupFlag2 = Util.FormarByte(bitsGroupFlag2);
            AtkData.Add(byteGroupFlag2);
            Atk.OtherFlag1 = byteGroupFlag2;

            int[] bitsGroupFlag3 = new int[8];
            for (int i = 0; i < 8; i++)
            {
                bitsGroupFlag3[i] = movForm.clbFlags.GetItemChecked(flagsCount) ? 1 : 0;
                flagsCount++;
            }
            byte resultado = Util.FormarByte(bitsGroupFlag3);

            AtkData.Add(resultado);
            Atk.DefenseFlag = resultado;

            int[] bitsGroupFlag4 = new int[8];
            for (int i = 0; i < 8; i++)
            {
                bitsGroupFlag4[i] = movForm.clbFlags.GetItemChecked(flagsCount) ? 1 : 0;
                flagsCount++;
            }
            byte byteGroupFlag4 = Util.FormarByte(bitsGroupFlag4);
            AtkData.Add(byteGroupFlag4);
            Atk.OtherFlag2 = byteGroupFlag4;

            AtkData.Add((byte)Atk.PreviousIdx);
            AtkData.Add((byte)Atk.SequenceIdx);
            Atk.KawarimiDificulty = (sbyte)(movForm.numKawarimi.Value);
            AtkData.Add((byte)Atk.KawarimiDificulty);
            AtkData.Add((byte)0);

            byte AtkTypeValue = (byte)movForm.cmbType.SelectedIndex;
            if (AtkTypeValue == 0)
            {
                AtkTypeValue = 0x11;
            }
            else if (AtkTypeValue == 1)
            {
                AtkTypeValue = 0x12;
            }
            else if (AtkTypeValue == 2)
            {
                AtkTypeValue = 0x14;
            }
            Atk.Type = AtkTypeValue;
            AtkData.Add(AtkTypeValue);

            byte atkDpadFlag = (byte)GetDpadFlagGroupFromCmbBox(movForm.cmbDpad);
            Atk.DpadFlag = atkDpadFlag;
            AtkData.Add(atkDpadFlag);
            byte atkButtonFlag = (byte)Atk.ButtonFlag;
            AtkData.Add(atkButtonFlag);
            AtkData.Add((byte)Atk.AutoExecuteFlag);
            AtkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.numChakra.Value)));
            Atk.Chakra = Convert.ToSingle(movForm.numChakra.Value);
            AtkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.numDamage.Value)));
            Atk.Damage = Convert.ToSingle(movForm.numDamage.Value);
            AtkData.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.numKnockback.Value)));
            Atk.Knockback = Convert.ToSingle(movForm.numKnockback.Value);
            byte currentDmgEffect = (byte)movForm.cmbDmgEffect.SelectedIndex;
            AtkData.Add(currentDmgEffect);
            Atk.DamageEffect = currentDmgEffect;
            byte currentDefenseEffect = (byte)(movForm.cmbDefenseEffect.SelectedIndex - 1);
            AtkData.Add(currentDefenseEffect);
            Atk.DefenseEffect = currentDefenseEffect;

            Atk.HitCount = movForm.chkHitCount.Checked ? 0x7FFF : (int)movForm.numHitCount.Value;
            AtkData.AddRange(BitConverter.GetBytes(Convert.ToInt16(Atk.HitCount)));
            Atk.HitStop = movForm.chkHitStop.Checked ? 0x7FFF : (int)movForm.numHitStop.Value;
            AtkData.AddRange(BitConverter.GetBytes(Convert.ToInt16(Atk.HitStop)));
            Atk.HitSpeed = movForm.chkHitSpeed.Checked ? 0x7FFF : (int)movForm.numHitSpeed.Value;
            AtkData.AddRange(BitConverter.GetBytes(Convert.ToInt16(Atk.HitSpeed)));

            Atk.RangeValue1 = (float)movForm.numRangeValue1.Value;
            AtkData.AddRange(BitConverter.GetBytes(Atk.RangeValue1));

            Atk.RangeValue2 = (float)movForm.numRangeValue2.Value;
            AtkData.AddRange(BitConverter.GetBytes(Atk.RangeValue2));

            Atk.KnockbackDirection = (float)movForm.numKnockbackDirection.Value;
            AtkData.AddRange(BitConverter.GetBytes(Atk.KnockbackDirection));           

            Atk.WhiffSound = (int)movForm.numWhiffSound.Value;
            AtkData.AddRange(BitConverter.GetBytes(Convert.ToInt16(Atk.WhiffSound)));

            int currentPLSoundIndex = movForm.cmbPLSound.SelectedIndex;
            int currentPLSound = currentPLSoundIndex - 4;
            switch (currentPLSoundIndex)
            {
                case 0:
                    AtkData.AddRange(BitConverter.GetBytes(Convert.ToInt16(-4)));
                    Atk.PlayerSound = -4;
                    break;
                case 1:
                    AtkData.AddRange(BitConverter.GetBytes(Convert.ToInt16(-3)));
                    Atk.PlayerSound = -3;
                    break;
                case 2:
                    AtkData.AddRange(BitConverter.GetBytes(Convert.ToInt16(-2)));
                    Atk.PlayerSound = -2;
                    break;
                case 3:
                    AtkData.AddRange(BitConverter.GetBytes(Convert.ToInt16(-1)));
                    Atk.PlayerSound = -1;
                    break;
                default:
                    AtkData.AddRange(BitConverter.GetBytes(Convert.ToInt16(currentPLSound)));
                    Atk.PlayerSound = (short)currentPLSound;
                    break;
            }
            
            Atk.SoundDelay = (uint)movForm.numSoundDelay.Value;
            AtkData.AddRange(BitConverter.GetBytes(Convert.ToUInt16(Atk.SoundDelay)));

            Atk.HitSound = (int)movForm.numHitSound.Value;
            AtkData.AddRange(BitConverter.GetBytes(Convert.ToInt16(Atk.HitSound)));
            int currentDmgParticleIndex = movForm.cmbDmgParticle.SelectedIndex;
            int currentDmgParticle = currentDmgParticleIndex - 1;
            if (currentDmgParticleIndex == 0)
            {
                AtkData.AddRange(BitConverter.GetBytes(Convert.ToInt16(-1)));
                Atk.DamageParticle = -1;
            }
            else
            {
                AtkData.AddRange(BitConverter.GetBytes(Convert.ToInt16(currentDmgParticle)));
                Atk.DamageParticle = (short)currentDmgParticle;
            }
            Atk.DefenseSound = (int)movForm.numDefenseSound.Value;
            AtkData.AddRange(BitConverter.GetBytes(Convert.ToInt16(Atk.DefenseSound)));

            Atk.DefenseParticle = (int)(movForm.cmbDefenseParticle.SelectedIndex - 1);
            AtkData.AddRange(BitConverter.GetBytes(Convert.ToInt16(Atk.DefenseParticle)));

            Atk.EnemySound = (int)movForm.numEnemySound.Value;
            AtkData.AddRange(BitConverter.GetBytes(Convert.ToInt16(Atk.EnemySound)));

            byte[] resultBytes = AtkData.ToArray();
            return resultBytes;
        }

        public static byte[] UpdateAllCharAtkPrm(MovesetParameters movForm, int charID)
        {
            List<byte> AtkData = new List<byte>();

            for (int i = 0; i < PlGen.CharGenPrm[charID].AtkCount; i++)
            {
                var Atk = CharAtkPrm[charID][i];

                AtkData.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Atk.NameOffs1)));
                AtkData.AddRange(BitConverter.GetBytes(Convert.ToUInt32(Atk.NameOffs1)));

                AtkData.AddRange(BitConverter.GetBytes(Convert.ToUInt16(Atk.Index)));

                AtkData.Add(Convert.ToByte(Atk.ComboNameTableIdx));
                AtkData.Add(Convert.ToByte(Atk.UseComboNameTableFlag));
                AtkData.AddRange(BitConverter.GetBytes(Convert.ToUInt16(Atk.ComboNameTableIdx2)));

                AtkData.AddRange(BitConverter.GetBytes(Convert.ToUInt16(Atk.JutsuIdx)));

                AtkData.Add(Convert.ToByte(Atk.SpecialFlag));
                AtkData.Add(Convert.ToByte(Atk.ThrowFlag));
                AtkData.Add(Convert.ToByte(Atk.JutsuFlag));
                AtkData.Add(Convert.ToByte(Atk.JanKenPonFlag));

                AtkData.AddRange(BitConverter.GetBytes((short)Atk.ComboNameTableIdx));
                AtkData.AddRange(BitConverter.GetBytes((byte)Atk.UseComboNameTableFlag));
                AtkData.AddRange(BitConverter.GetBytes((short)Atk.ComboNameTableIdx2));

                AtkData.AddRange(BitConverter.GetBytes((short)Atk.JutsuIdx));

                AtkData.Add((byte)Atk.DirectionFlag);
                AtkData.Add((byte)Atk.OtherFlag1);
                AtkData.Add((byte)Atk.DefenseFlag);
                AtkData.Add((byte)Atk.OtherFlag2);

                AtkData.Add((byte)Atk.PreviousIdx);
                AtkData.Add((byte)(movForm.numKawarimi.Value));
                AtkData.Add((byte)Atk.KawarimiDificulty);
                AtkData.Add((byte)0);

                AtkData.Add((byte)Atk.Type);
                AtkData.Add((byte)Atk.DpadFlag);
                byte atkButtonFlag = (byte)Atk.ButtonFlag;
                AtkData.Add(atkButtonFlag);
                AtkData.Add((byte)Atk.AutoExecuteFlag);
                AtkData.AddRange(BitConverter.GetBytes(Atk.Chakra));
                AtkData.AddRange(BitConverter.GetBytes(Atk.Damage));
                AtkData.AddRange(BitConverter.GetBytes(Atk.Knockback));
                AtkData.Add((byte)Atk.DamageEffect);
                AtkData.Add((byte)Atk.DefenseEffect);
                AtkData.AddRange(BitConverter.GetBytes(Atk.HitCount));
                AtkData.AddRange(BitConverter.GetBytes(Atk.HitSpeed));
                AtkData.AddRange(BitConverter.GetBytes(Atk.HitStop));
                AtkData.AddRange(BitConverter.GetBytes(Atk.RangeValue1));
                AtkData.AddRange(BitConverter.GetBytes(Atk.RangeValue2));
                AtkData.AddRange(BitConverter.GetBytes(Atk.KnockbackDirection));
                AtkData.AddRange(BitConverter.GetBytes(Atk.WhiffSound));
                AtkData.AddRange(BitConverter.GetBytes(Atk.PlayerSound));
                AtkData.AddRange(BitConverter.GetBytes(Atk.SoundDelay));
                AtkData.AddRange(BitConverter.GetBytes(Atk.HitSound));
                AtkData.AddRange(BitConverter.GetBytes(Atk.DamageParticle));
                AtkData.AddRange(BitConverter.GetBytes(Atk.DefenseSound));
                AtkData.AddRange(BitConverter.GetBytes(Atk.DefenseParticle));
                AtkData.AddRange(BitConverter.GetBytes(Atk.EnemySound));
                AtkData.AddRange(BitConverter.GetBytes(Convert.ToInt32(Atk.AnimationIdx)));
            }
            byte[] resultBytes = AtkData.ToArray();
            return resultBytes;
        }
        public static void WriteELFCharAtk(byte[] resultBytes, int charID)
        {
            if (!File.Exists(GAME.caminhoELF))
            {
                MessageBox.Show("Unable to save, check if the file has been deleted or moved.", string.Empty, MessageBoxButtons.OK);
            }
            else
            {
                using (FileStream fs = new FileStream(GAME.caminhoELF, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    byte[] charAtkAreaOffsetBytes = PlGen.CharGenPrm[charID].AtkListOffset;
                    charAtkAreaOffsetBytes[3] = 0x0;
                    int subValue = 0xFFE80;
                    int charAtkAreaOffset = BitConverter.ToInt32(charAtkAreaOffsetBytes, 0) - subValue;

                    fs.Seek(charAtkAreaOffset, SeekOrigin.Begin);

                    fs.Write(resultBytes, 0, resultBytes.Length);

                    MessageBox.Show("The changes were saved successfully!", string.Empty, MessageBoxButtons.OK);
                }
            }
        }
    }
}
