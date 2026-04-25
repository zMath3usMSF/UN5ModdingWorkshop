using CCSFileExplorerWV;
using DiscUtils.Btrfs.Base.Items;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        public float AtkChakra, AtkDamage, AtkKnockBack, AtkSummonDistance1, AtkSummonDistance2, AtkKnockBackDirection;

        public uint AtkPos, AtkDpadFlag, AtkButtonFlag, AtkDamageEffect;

        public short AtkPrevious, AtkAnm;

        public short AtkDefenseEffect, AtkHitSpeed, AtkPlSound, AtkSound, AtkDamageParticle, AtkEnemySound, AtkDamageSound, AtkDefenseParticle, AtkDefenseSound;

        public uint AtkPos2, AtkUnk16;

        public UInt16 AtkHitCount, AtkHitEffect, AtkSoundDelay;

        public byte[] AtkUnk2, AtkUnk3, AtkUnk15;

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

            AtkPrevious = (short)Input.ReadUInt(0x18, 8),

            AtkPos = Input.ReadUInt(0x19, 8),

            AtkUnk15 = Input.ReadBytes(0x1A, 3),

            AtkDpadFlag = Input.ReadUInt(0x1D, 8),
            AtkButtonFlag = Input.ReadUInt(0x1E, 8),

            AtkUnk16 = Input.ReadUInt(0x1F, 8),

            AtkChakra = Input.ReadSingle(0x20),
            AtkDamage = Input.ReadSingle(0x24),
            AtkKnockBack = Input.ReadSingle(0x28),
            AtkDamageEffect = Input.ReadUInt(0x2C, 8),

            AtkDefenseEffect = (short)Input.ReadUInt(0x2D, 8),

            AtkHitCount = (UInt16)Input.ReadUInt(0x2E, 16),
            AtkHitSpeed = (short)Input.ReadUInt(0x30, 16),
            AtkHitEffect = (UInt16)Input.ReadUInt(0x32, 16),

            AtkSummonDistance1 = Input.ReadSingle(0x34),
            AtkSummonDistance2 = Input.ReadSingle(0x38),
            AtkKnockBackDirection = Input.ReadSingle(0x3C),

            AtkSound = (short)Input.ReadUInt(0x40, 16),
            AtkPlSound = (short)Input.ReadUInt(0x42, 16),
            AtkSoundDelay = (UInt16)Input.ReadUInt(0x44, 16),
            AtkDamageSound = (short)Input.ReadUInt(0x46, 16),
            AtkDamageParticle = (short)Input.ReadUInt(0x48, 16),

            AtkDefenseSound = (short)Input.ReadUInt(0x4A, 16),
            AtkDefenseParticle = (short)Input.ReadUInt(0x4C, 16),

            AtkEnemySound = (short)Input.ReadUInt(0x4E, 16),

            AtkAnm = (short)Input.ReadUInt(0x50, 32),
        };

        public static Dictionary<int, string> DamageEffectList = new Dictionary<int, string>()
        {
          {0, "Normal"},
          {1, "Normal 1"},
          {2, "Normal 2"},
          {3, "Normal 3"},
          {4, "Normal 4"},
          {5, "Normal 5"},
          {6, "Normal 6"},
          {7, "Normal (Aerial)"},
          {8, "Normal (Aerial) 1"},
          {9, "Throw to Diagonal"},
          {10, "Throw to Diagonal 1"},
          {11, "Throw to Up"},
          {12, "Throw to Up (Recovery)"},
          {13, "Throw to Diagonal (Delay Eff)"},
          {14, "Throw to Front (Recovery)"},
          {15, "Throw to Front (Faint)"},
          {16, "Throw to Diagonal (Faint)"},
          {17, "Throw to Front (Faint) 1"},
          {18, "Throw to Front (Faint) 2"},
          {19, "Super-Throw to Front"},
          {20, "Throw to Up (Faint)"},
          {21, "Throw to Down (Faint)"},
          {22, "Throw to Down (Faint) 2"},
          {23, "???"},
          {24, "???"},
          {25, "???"},
          {26, "???"},
          {27, "???"},
          {28, "???"},
          {29, "Faint"},
          {30, "Faint (Flames Eff)"},
          {31, "Faint (Blue Flames Eff)"}
        };

        public static Dictionary<int, string> PLSoundList = new Dictionary<int, string>()
        {
        {0, "ATK_cmn_vS"},
        {1, "ATK_cmn_vM"},
        {2, "ATK_cmn_vL"},
        {3, "null"},
        {4, "ATK_cmn_vS_rv0"},
        {5, "ATK_cmn_vS_rv1"},
        {6, "ATK_cmn_vS_rv2"},
        {7, "ATK_cmn_vM_rv0"},
        {8, "ATK_cmn_vM_rv1"},
        {9, "ATK_cmn_vM_rv2"},
        {10, "ATK_cmn_vL_rv0"},
        {11, "ATK_cmn_vL_rv1"},
        {12, "ATK_cmn_vL_rv2"},
        {13, "DMG_cmn_vS_rv0"},
        {14, "DMG_cmn_vS_rv1"},
        {15, "DMG_cmn_vS_rv2"},
        {16, "DMG_cmn_vM_rv0"},
        {17, "DMG_cmn_vM_rv1"},
        {18, "DMG_cmn_vM_rv2"},
        {19, "DMG_cmn_vL_rv0"},
        {20, "DMG_cmn_vL_rv1"},
        {21, "DMG_cmn_vL_rv2"},
        {22, "ATK_death_vL"},
        {23, "jump"},
        {24, "jump_double"},
        {25, "UNK"},
        {26, "ITM_take"},
        {27, "null"},
        {28, "ITM_hpRecover"},
        {29, "null"},
        {30, "substitution_rv0"},
        {31, "substitution_rv1"},
        {32, "substitution_rv2"},
        {33, "jump_double"},
        {34, "provocation"},
        {35, "ckrCharge"}
        };

        public static Dictionary<int, string> DamageParticleList = new Dictionary<int, string>()
        {
        {0, "Normal Middle"},
        {1, "Without"},
        {2, "Normal Small"},
        {3, "Normal Middle"},
        {4, "Normal Large"},
        {5, "Normal Large 1"},
        {6, "Normal Large 2"},
        {7, "Normal Small 1"},
        {8, "Normal Large 3"},
        {9, "Cut Blue Small"},
        {10, "Cut Blue Small 1"},
        {11, "Cut Blue Middle"},
        {12, "Cut Blue Large"},
        {13, "Cut Purple Small"},
        {14, "Cut Purple Small 1"},
        {15, "Cut Purple Middle"},
        {16, "Cut Purple Large"},
        {17, "Normal Small"},
        {18, "Normal Middle"},
        {19, "Normal Large"},
        {20, "Normal Large 1"},
        {21, "Normal Large 2"},
        {22, "Normal Small (Red Kanji)"},
        {23, "null"},
        {24, "Explosion Small"},
        {25, "Without (Kanji)"},
        };

        public Dictionary<int, string> DefenseFlagList = new Dictionary<int, string>()
        {
        {0, "Without"},
        {1, "???"},
        {2, "??? 2"},
        {3, "Idefensible"},
        {4, "Defense Break"}
        };

        public static Dictionary<int, string> DefenseEffectList = new Dictionary<int, string>()
        {
        {0, "???"},
        {1, "Normal"},
        {2, "Normal 1"},
        {3, "Normal 2"},
        {4, "Normal 3"},
        {5, "Normal 4"},
        {6, "Diagonal"},
        {7, "Diagonal 1"},
        {8, "Diagonal 2"},
        {9, "Diagonal 3"},
        {10, "Diagonal 4"}
        };

        public static Dictionary<int, string> DefenseParticleList = new Dictionary<int, string>()
        {
        {0, "Normal"},
        {1, "Without"},
        {2, "Normal"},
        {3, "Normal 1"},
        {4, "(Hit, Red Kanji)"},
        {5, "Without"},
        {6, "Explosion"}
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
            int atkType2 = Atk.AtkUnk15[2];
            movForm.lblNamePanel.Text = movForm.listBox1.SelectedItem.ToString().Split(':')[1].Trim();
            if ((Atk.ThrowFlag == 1 || Atk.ThrowFlag == 2) && ((atkType2 >> 1) & 1) == 1)
            {
                movForm.lblInfo.Text = "Ground Throw";
            }
            else if ((Atk.ThrowFlag == 1 || Atk.ThrowFlag == 2) && ((atkType2 >> 2) & 1) == 1)
            {
                movForm.lblInfo.Text = "Aerial Throw";
            }
            else if (Atk.SpecialFlag == 0x4)
            {
                movForm.lblInfo.Text = "Combo";
            }
            else if (Atk.SpecialFlag == 0x8 ||Atk.SpecialFlag == 0x10)
            {
                movForm.lblInfo.Text = "Charge";
            }
            else if (((atkType2 >> 2) & 1) == 1 && Atk.SpecialFlag == 1)
            {
                movForm.lblInfo.Text = "While Jumping";
            }
            else if (Atk.Index >= 0x0 && Atk.Index <= 0x3)
            {
                movForm.lblInfo.Text = "Jutsu";
            }
            else if(Atk.Index >= 0x4 && Atk.Index <= 0x9)
            {
                movForm.lblInfo.Text = "Ultimate Jutsu";
            }
            else if (Atk.Index >= 0xA && Atk.Index <= 0x12)
            {
                movForm.lblInfo.Text = "Extra-Hit";
            }
            else if (Atk.Index == 0x13)
            {
                movForm.lblInfo.Text = "Dash";
            }
            else if (Atk.Index == 0x14)
            {
                movForm.lblInfo.Text = "Jan-Ken-Pon";
            }
            else
            {
                movForm.lblInfo.Text = "";
            }
            movForm.clbFlags.Items.Clear();
            int flagsCount = 1;
            for (int i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 0:
                        movForm.clbFlags.Items.Add("Ground", ((Atk.DirectionFlag >> i) & 1) == 1);
                        break;
                    case 1:
                        movForm.clbFlags.Items.Add("Air", ((Atk.DirectionFlag >> i) & 1) == 1);
                        break;
                    case 2:
                        movForm.clbFlags.Items.Add("Follow Up", ((Atk.DirectionFlag >> i) & 1) == 1);
                        break;
                    case 3:
                        movForm.clbFlags.Items.Add("Up", ((Atk.DirectionFlag >> i) & 1) == 1);
                        break;
                    case 4:
                        movForm.clbFlags.Items.Add("Down", ((Atk.DirectionFlag >> i) & 1) == 1);
                        break;
                    case 5:
                        movForm.clbFlags.Items.Add("Front", ((Atk.DirectionFlag >> i) & 1) == 1);
                        break;
                    case 6:
                        movForm.clbFlags.Items.Add("Back", ((Atk.DirectionFlag >> i) & 1) == 1);
                        break;
                    case 7:
                        movForm.clbFlags.Items.Add("Side Extra-Hit", ((Atk.DirectionFlag >> i) & 1) == 1);
                        break;
                    default:
                        movForm.clbFlags.Items.Add($"Unk{flagsCount}", ((Atk.DirectionFlag >> i) & 1) == 1);
                        break;
                }
                flagsCount++;
            }
            for (int i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 0:
                        movForm.clbFlags.Items.Add("Down Extra-Hit", ((Atk.OtherFlag1 >> i) & 1) == 1);
                        break;
                    case 1:
                        movForm.clbFlags.Items.Add("Up Extra-Hit", ((Atk.OtherFlag1 >> i) & 1) == 1);
                        break;
                    case 4:
                        movForm.clbFlags.Items.Add("Anti Counter", ((Atk.OtherFlag1 >> i) & 1) == 1);
                        break;
                    default:
                        movForm.clbFlags.Items.Add($"Unk{flagsCount}", ((Atk.OtherFlag1 >> i) & 1) == 1);
                        break;
                }
                flagsCount++;
            }
            for (int i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 2:
                        movForm.clbFlags.Items.Add("Without Wall KB", ((Atk.DefenseFlag >> i) & 1) == 1);
                        break;
                    case 3:
                        movForm.clbFlags.Items.Add("Don't Bounce", ((Atk.DefenseFlag >> i) & 1) == 1);
                        break;
                    case 5:
                        movForm.clbFlags.Items.Add("Hit Fallen", ((Atk.DefenseFlag >> i) & 1) == 1);
                        break;
                    case 6:
                        movForm.clbFlags.Items.Add("Undefendable", ((Atk.DefenseFlag >> i) & 1) == 1);
                        break;
                    case 7:
                        movForm.clbFlags.Items.Add("Break Defense", ((Atk.DefenseFlag >> i) & 1) == 1);
                        break;
                    default:
                        movForm.clbFlags.Items.Add($"Unk{flagsCount}", ((Atk.DefenseFlag >> i) & 1) == 1);
                        break;
                }
                flagsCount++;
            }
            for (int i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 4:
                        movForm.clbFlags.Items.Add("Hit Fainted", ((Atk.OtherFlag2 >> i) & 1) == 1);
                        break;
                    case 2:
                        movForm.clbFlags.Items.Add("Backdash", ((Atk.OtherFlag2 >> i) & 1) == 1);
                        break;
                    case 1:
                        movForm.clbFlags.Items.Add("Damage on Counter Attack", ((Atk.OtherFlag2 >> i) & 1) == 1);
                        break;
                    case 0:
                        movForm.clbFlags.Items.Add("Damage on Defense", ((Atk.OtherFlag2 >> i) & 1) == 1);
                        break;
                    default:
                        movForm.clbFlags.Items.Add($"Unk{flagsCount}", ((Atk.OtherFlag2 >> i) & 1) == 1);
                        break;
                }
                flagsCount++;
            }

            movForm.txtChakra.Text = ($"{Atk.AtkChakra}");
            movForm.txtDamage.Text = ($"{Atk.AtkDamage}");
            movForm.txtKnockBack.Text = ($"{Atk.AtkKnockBack}");

            movForm.cmbDmgEffect.Items.AddRange(movForm.cmbDmgEffect.Items.Count == 0 ? DamageEffectList.Values.ToArray() : new object[0]);
            int currentDmgEffect = (int)Atk.AtkDamageEffect;
            movForm.cmbDmgEffect.SelectedIndex = Math.Min(currentDmgEffect, 31);

            movForm.cmbDefenseEffect.Items.AddRange(movForm.cmbDefenseEffect.Items.Count == 0 ? PlAtk.DefenseEffectList.Values.ToArray() : new object[0]);
            int currentDefenseEffect = Atk.AtkDefenseEffect;
            movForm.cmbDefenseEffect.SelectedIndex = currentDefenseEffect == 255 ? 0 : currentDefenseEffect + 1;

            movForm.txtHitCount.Text = ($"{Atk.AtkHitCount}");
            movForm.txtHitSpeed.Text = ($"{Atk.AtkHitSpeed}");
            movForm.txtHitEffect.Text = ($"{Atk.AtkHitEffect}");
            movForm.txtSummonDistance1.Text = $"{Atk.AtkSummonDistance1}";
            movForm.txtSummonDistance2.Text = $"{Atk.AtkSummonDistance2}";
            movForm.txtKnockBackDirection.Text = $"{Atk.AtkKnockBackDirection}";
            movForm.txtAtkSound.Text = ($"{Atk.AtkSound}");

            movForm.cmbPLSound.Items.AddRange(movForm.cmbPLSound.Items.Count == 0 ? PLSoundList.Values.ToArray() : new object[0]);
            int currentPLSound = (int)Atk.AtkPlSound;
            movForm.cmbPLSound.SelectedIndex = currentPLSound == -4 ? 0 : currentPLSound == -3 ? 1 : currentPLSound == -2 ? 2 : currentPLSound == -1 ? 3 : currentPLSound > 34 ? 0 : currentPLSound + 4;
            movForm.txtSoundDelay.Text = ($"{Atk.AtkSoundDelay}");
            movForm.txtDmgSound.Text = ($"{Atk.AtkDamageSound}");
            movForm.cmbDmgParticle.Items.AddRange(movForm.cmbDmgParticle.Items.Count == 0 ? DamageParticleList.Values.ToArray() : new object[0]);
            int currentDmgParticle = (int)Atk.AtkDamageParticle;
            movForm.cmbDmgParticle.SelectedIndex = currentDmgParticle > 24 || currentDmgParticle == -1 ? 0 : currentDmgParticle + 1;
            movForm.txtDefenseSound.Text = ($"{Atk.AtkDefenseSound}");
            movForm.cmbDefenseParticle.Items.AddRange(movForm.cmbDefenseParticle.Items.Count == 0 ? DefenseParticleList.Values.ToArray() : new object[0]);
            int currentDefenseParticle = Atk.AtkDefenseParticle;
            movForm.cmbDefenseParticle.SelectedIndex = currentDefenseParticle < 0 ? currentDefenseParticle + 1 : 0;
            movForm.txtEnemySound.Text = ($"{Atk.AtkEnemySound}");

            SetDpadFlagGroupToCmbBox((int)Atk.AtkDpadFlag, movForm.cmbDpad);

            DrawCommandSequence(movForm.picCommand, Atk, charID);
        }

        private static readonly Dictionary<uint, int> ButtonIconMap = new Dictionary<uint, int>()
        {
            { 0x20, 4 }, // Circle
            { 0x10, 5 }, // Triangle
            { 0x80, 6 }, // Square
            { 0x40, 7 }, // Cross
            { 0x08, 8 }, // Plus
        };

        public static void DrawCommandSequence(PictureBox pic, PlAtk Atk, int charID)
        {
            int currentAtkPrevious = Atk.AtkPrevious;
            List<uint> buttons = new List<uint>();
            List<uint> dpads = new List<uint>();

            if (Atk.SpecialFlag == 0x1 || Atk.SpecialFlag == 0x4 || Atk.ThrowFlag == 0x1 || Atk.ThrowFlag == 0x2)
            {
                // Golpe atual primeiro
                if ((Atk.AtkUnk16 >> 3) == 0)
                {
                    dpads.Add(Atk.AtkDpadFlag);
                    buttons.Add(Atk.AtkButtonFlag);
                }

                // Percorre a cadeia até -1
                while (currentAtkPrevious != 255)
                {
                    PlAtk currentAtk = GetCharAtk(charID, currentAtkPrevious);
                    dpads.Add(currentAtk.AtkDpadFlag);
                    buttons.Add(currentAtk.AtkButtonFlag);
                    currentAtkPrevious = currentAtk.AtkPrevious;
                }
            }
            else
            {
                // Golpe simples sem cadeia
                dpads.Add(Atk.AtkDpadFlag);
                buttons.Add(Atk.AtkButtonFlag);
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
            var ninjaCharsAtk = CharAtkPrm[charID][atkID];

            List<byte> atkBlockBytes = new List<byte>();

            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(ninjaCharsAtk.NameOffs1)));
            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(ninjaCharsAtk.NameOffs1)));

            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(ninjaCharsAtk.Index)));

            atkBlockBytes.Add(Convert.ToByte(ninjaCharsAtk.ComboNameTableIdx));
            atkBlockBytes.Add(Convert.ToByte(ninjaCharsAtk.UseComboNameTableFlag));
            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(ninjaCharsAtk.ComboNameTableIdx2)));

            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(ninjaCharsAtk.JutsuIdx)));

            atkBlockBytes.Add(Convert.ToByte(ninjaCharsAtk.SpecialFlag));
            atkBlockBytes.Add(Convert.ToByte(ninjaCharsAtk.ThrowFlag));
            atkBlockBytes.Add(Convert.ToByte(ninjaCharsAtk.JutsuFlag));
            atkBlockBytes.Add(Convert.ToByte(ninjaCharsAtk.JanKenPonFlag));

            int flagsCount = 0;
            int[] bitsGroupFlag1 = new int[8];
            for(int i = 0; i < 8; i++)
            {
                bitsGroupFlag1[i] = movForm.clbFlags.GetItemChecked(flagsCount) ? 1 : 0;
                flagsCount++;
            }
            byte byteGroupFlag1 = Util.FormarByte(bitsGroupFlag1);
            atkBlockBytes.Add(byteGroupFlag1);
            ninjaCharsAtk.DirectionFlag = byteGroupFlag1;

            int[] bitsGroupFlag2 = new int[8];
            for (int i = 0; i < 8; i++)
            {
                bitsGroupFlag2[i] = movForm.clbFlags.GetItemChecked(flagsCount) ? 1 : 0;
                flagsCount++;
            }
            byte byteGroupFlag2 = Util.FormarByte(bitsGroupFlag2);
            atkBlockBytes.Add(byteGroupFlag2);
            ninjaCharsAtk.OtherFlag1 = byteGroupFlag2;

            int[] bitsGroupFlag3 = new int[8];
            for (int i = 0; i < 8; i++)
            {
                bitsGroupFlag3[i] = movForm.clbFlags.GetItemChecked(flagsCount) ? 1 : 0;
                flagsCount++;
            }
            byte resultado = Util.FormarByte(bitsGroupFlag3);

            atkBlockBytes.Add(resultado);
            ninjaCharsAtk.DefenseFlag = resultado;

            int[] bitsGroupFlag4 = new int[8];
            for (int i = 0; i < 8; i++)
            {
                bitsGroupFlag4[i] = movForm.clbFlags.GetItemChecked(flagsCount) ? 1 : 0;
                flagsCount++;
            }
            byte byteGroupFlag4 = Util.FormarByte(bitsGroupFlag4);
            atkBlockBytes.Add(byteGroupFlag4);
            ninjaCharsAtk.OtherFlag2 = byteGroupFlag4;

            byte atkPrevious = (byte)ninjaCharsAtk.AtkPrevious;
            atkBlockBytes.Add(atkPrevious);
            byte atkPos = (byte)ninjaCharsAtk.AtkPos;
            atkBlockBytes.Add(atkPos);
            ninjaCharsAtk.AtkPos = atkPos;
            atkBlockBytes.AddRange(ninjaCharsAtk.AtkUnk15);
            byte atkDpadFlag = (byte)GetDpadFlagGroupFromCmbBox(movForm.cmbDpad);
            ninjaCharsAtk.AtkDpadFlag = atkDpadFlag;
            atkBlockBytes.Add(atkDpadFlag);
            byte atkButtonFlag = (byte)ninjaCharsAtk.AtkButtonFlag;
            atkBlockBytes.Add(atkButtonFlag);
            atkBlockBytes.Add((byte)ninjaCharsAtk.AtkUnk16);
            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.txtChakra.Text)));
            ninjaCharsAtk.AtkChakra = Convert.ToSingle(movForm.txtChakra.Text);
            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.txtDamage.Text)));
            ninjaCharsAtk.AtkDamage = Convert.ToSingle(movForm.txtDamage.Text);
            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.txtKnockBack.Text)));
            ninjaCharsAtk.AtkKnockBack = Convert.ToSingle(movForm.txtKnockBack.Text);
            byte currentDmgEffect = (byte)movForm.cmbDmgEffect.SelectedIndex;
            atkBlockBytes.Add(currentDmgEffect);
            ninjaCharsAtk.AtkDamageEffect = currentDmgEffect;
            byte currentDefenseEffect = (byte)(movForm.cmbDefenseEffect.SelectedIndex - 1);
            atkBlockBytes.Add(currentDefenseEffect);
            ninjaCharsAtk.AtkDefenseEffect = currentDefenseEffect;
            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(movForm.txtHitCount.Text)));
            ninjaCharsAtk.AtkHitCount = Convert.ToUInt16(movForm.txtHitCount.Text);
            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(movForm.txtHitSpeed.Text)));
            ninjaCharsAtk.AtkHitSpeed = Convert.ToInt16(movForm.txtHitSpeed.Text);
            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(movForm.txtHitEffect.Text)));
            ninjaCharsAtk.AtkHitEffect = Convert.ToUInt16(movForm.txtHitEffect.Text);
            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.txtSummonDistance1.Text)));
            ninjaCharsAtk.AtkSummonDistance1 = Convert.ToSingle(movForm.txtSummonDistance1.Text);
            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.txtSummonDistance2.Text)));
            ninjaCharsAtk.AtkSummonDistance2 = Convert.ToSingle(movForm.txtSummonDistance2.Text);
            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.txtKnockBackDirection.Text)));
            ninjaCharsAtk.AtkKnockBackDirection = Convert.ToSingle(movForm.txtKnockBackDirection.Text);
            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(movForm.txtAtkSound.Text)));
            ninjaCharsAtk.AtkSound = Convert.ToInt16(movForm.txtAtkSound.Text);
            int currentPLSoundIndex = movForm.cmbPLSound.SelectedIndex;
            int currentPLSound = currentPLSoundIndex - 4;
            switch (currentPLSoundIndex)
            {
                case 0:
                    atkBlockBytes.AddRange(BitConverter.GetBytes((short)-4));
                    ninjaCharsAtk.AtkPlSound = -4;
                    break;
                case 1:
                    atkBlockBytes.AddRange(BitConverter.GetBytes((short)-3));
                    ninjaCharsAtk.AtkPlSound = -3;
                    break;
                case 2:
                    atkBlockBytes.AddRange(BitConverter.GetBytes((short)-2));
                    ninjaCharsAtk.AtkPlSound = -2;
                    break;
                case 3:
                    atkBlockBytes.AddRange(BitConverter.GetBytes((short)-1));
                    ninjaCharsAtk.AtkPlSound = -1;
                    break;
                default:
                    atkBlockBytes.AddRange(BitConverter.GetBytes((short)currentPLSound));
                    ninjaCharsAtk.AtkPlSound = (short)currentPLSound;
                    break;
            }
            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(movForm.txtSoundDelay.Text)));
            ninjaCharsAtk.AtkSoundDelay = Convert.ToUInt16(movForm.txtSoundDelay.Text);
            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(movForm.txtDmgSound.Text)));
            ninjaCharsAtk.AtkDamageSound = Convert.ToInt16(movForm.txtDmgSound.Text);
            int currentDmgParticleIndex = movForm.cmbDmgParticle.SelectedIndex;
            int currentDmgParticle = currentDmgParticleIndex - 1;
            if (currentDmgParticleIndex == 0)
            {
                atkBlockBytes.AddRange(BitConverter.GetBytes((short)-1));
                ninjaCharsAtk.AtkDamageParticle = -1;
            }
            else
            {
                atkBlockBytes.AddRange(BitConverter.GetBytes((short)currentDmgParticle));
                ninjaCharsAtk.AtkDamageParticle = (short)currentDmgParticle;
            }
            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(movForm.txtDefenseSound.Text)));
            ninjaCharsAtk.AtkDefenseSound = Convert.ToInt16(movForm.txtDefenseSound.Text);

            int currentDefenseParticle = movForm.cmbDefenseParticle.SelectedIndex;
            switch (currentDefenseParticle)
            {
                case 0:
                    atkBlockBytes.AddRange(BitConverter.GetBytes((short)-1));
                    ninjaCharsAtk.AtkDefenseParticle = -1;
                    break;
                case 1:
                    atkBlockBytes.AddRange(BitConverter.GetBytes((short)0));
                    ninjaCharsAtk.AtkDefenseParticle = 0;
                    break;
                case 2:
                    atkBlockBytes.AddRange(BitConverter.GetBytes((short)1));
                    ninjaCharsAtk.AtkDefenseParticle = 1;
                    break;
                case 3:
                    atkBlockBytes.AddRange(BitConverter.GetBytes((short)2));
                    ninjaCharsAtk.AtkDefenseParticle = 2;
                    break;
                case 4:
                    atkBlockBytes.AddRange(BitConverter.GetBytes((short)3));
                    ninjaCharsAtk.AtkDefenseParticle = 3;
                    break;
                case 5:
                    atkBlockBytes.AddRange(BitConverter.GetBytes((short)4));
                    ninjaCharsAtk.AtkDefenseParticle = 4;
                    break;
                case 6:
                    atkBlockBytes.AddRange(BitConverter.GetBytes((short)5));
                    ninjaCharsAtk.AtkDefenseParticle = 5;
                    break;
                default:
                    break;
            }

            atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(movForm.txtEnemySound.Text)));
            ninjaCharsAtk.AtkEnemySound = Convert.ToInt16(movForm.txtEnemySound.Text);


            byte[] resultBytes = atkBlockBytes.ToArray();
            return resultBytes;
        }

        public static byte[] UpdateAllCharAtkPrm(MovesetParameters movForm, int charID)
        {
            List<byte> atkBlockBytes = new List<byte>();

            for (int i = 0; i < PlGen.CharGenPrm[charID].AtkCount; i++)
            {
                var ninjaCharsAtk = CharAtkPrm[charID][i];

                atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(ninjaCharsAtk.NameOffs1)));
                atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt32(ninjaCharsAtk.NameOffs1)));

                atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(ninjaCharsAtk.Index)));

                atkBlockBytes.Add(Convert.ToByte(ninjaCharsAtk.ComboNameTableIdx));
                atkBlockBytes.Add(Convert.ToByte(ninjaCharsAtk.UseComboNameTableFlag));
                atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(ninjaCharsAtk.ComboNameTableIdx2)));

                atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(ninjaCharsAtk.JutsuIdx)));

                atkBlockBytes.Add(Convert.ToByte(ninjaCharsAtk.SpecialFlag));
                atkBlockBytes.Add(Convert.ToByte(ninjaCharsAtk.ThrowFlag));
                atkBlockBytes.Add(Convert.ToByte(ninjaCharsAtk.JutsuFlag));
                atkBlockBytes.Add(Convert.ToByte(ninjaCharsAtk.JanKenPonFlag));

                atkBlockBytes.AddRange(BitConverter.GetBytes((short)ninjaCharsAtk.ComboNameTableIdx));
                atkBlockBytes.AddRange(BitConverter.GetBytes((byte)ninjaCharsAtk.UseComboNameTableFlag));
                atkBlockBytes.AddRange(BitConverter.GetBytes((short)ninjaCharsAtk.ComboNameTableIdx2));

                atkBlockBytes.AddRange(BitConverter.GetBytes((short)ninjaCharsAtk.JutsuIdx));

                atkBlockBytes.Add((byte)ninjaCharsAtk.DirectionFlag);
                atkBlockBytes.Add((byte)ninjaCharsAtk.OtherFlag1);
                atkBlockBytes.Add((byte)ninjaCharsAtk.DefenseFlag);
                atkBlockBytes.Add((byte)ninjaCharsAtk.OtherFlag2);
                atkBlockBytes.Add((byte)ninjaCharsAtk.AtkPrevious);
                atkBlockBytes.Add((byte)ninjaCharsAtk.AtkPos);
                atkBlockBytes.AddRange(ninjaCharsAtk.AtkUnk15);
                atkBlockBytes.Add((byte)ninjaCharsAtk.AtkDpadFlag);
                byte atkButtonFlag = (byte)ninjaCharsAtk.AtkButtonFlag;
                atkBlockBytes.Add(atkButtonFlag);
                atkBlockBytes.Add((byte)ninjaCharsAtk.AtkUnk16);
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkChakra));
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkDamage));
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkKnockBack));
                atkBlockBytes.Add((byte)ninjaCharsAtk.AtkDamageEffect);
                atkBlockBytes.Add((byte)ninjaCharsAtk.AtkDefenseEffect);
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkHitCount));
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkHitSpeed));
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkHitEffect));
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkSummonDistance1));
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkSummonDistance2));
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkKnockBackDirection));
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkSound));
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkPlSound));
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkSoundDelay));
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkDamageSound));
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkDamageParticle));
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkDefenseSound));
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkDefenseParticle));
                atkBlockBytes.AddRange(BitConverter.GetBytes(ninjaCharsAtk.AtkEnemySound));
                atkBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(ninjaCharsAtk.AtkAnm)));
            }
            byte[] resultBytes = atkBlockBytes.ToArray();
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
