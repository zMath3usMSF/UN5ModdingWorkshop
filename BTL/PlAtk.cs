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
    internal class PlAtk
    {
        public static List<List<PlAtk>> CharAtkPrm = new List<List<PlAtk>>();
        public static List<List<PlAtk>> CharAtkPrmBkp = new List<List<PlAtk>>();
        public static List<List<string>> comboNameList = new List<List<string>>();

        #region Atk Attributes

        public float AtkChakra, AtkDamage, AtkKnockBack, AtkSummonDistance1, AtkSummonDistance2, AtkKnockBackDirection;

        public uint AtkFlagGroup1, AtkFlagGroup2, AtkDefenseFlag, AtkFlagGroup4, AtkPos, AtkDpadFlag, AtkButtonFlag, AtkDamageEffect;

        public short AtkPrevious, AtkAnm;

        public short AtkDefenseEffect, AtkHitSpeed, AtkPlSound, AtkSound, AtkDamageParticle, AtkEnemySound, AtkDamageSound, AtkDefenseParticle, AtkDefenseSound;

        public uint AtkPos2;

        public UInt16 AtkHitCount, AtkHitEffect, AtkSoundDelay;

        public byte[] AtkUnk, AtkUnk1, AtkUnk2, AtkUnk3, AtkUnk4,
                      AtkUnk15, AtkUnk16;

        #endregion

        internal static PlAtk ReadCharAtkPrm(byte[] Input) => new PlAtk
        {
            AtkUnk = Input.ReadBytes(0x0, 4),
            AtkUnk1 = Input.ReadBytes(0x4, 4),
            AtkUnk2 = Input.ReadBytes(0x8, 4),
            AtkUnk3 = Input.ReadBytes(0xC, 4),
            AtkUnk4 = Input.ReadBytes(0x10, 4),

            AtkFlagGroup1 = Input.ReadUInt(0x14, 8),

            AtkFlagGroup2 = Input.ReadUInt(0x15, 8),

            AtkDefenseFlag = Input.ReadUInt(0x16, 8),

            AtkFlagGroup4 = Input.ReadUInt(0x17, 8),

            AtkPrevious = (short)Input.ReadUInt(0x18, 8),

            AtkPos = Input.ReadUInt(0x19, 8),

            AtkUnk15 = Input.ReadBytes(0x1A, 3),

            AtkDpadFlag = Input.ReadUInt(0x1D, 8),
            AtkButtonFlag = Input.ReadUInt(0x1E, 8),

            AtkUnk16 = Input.ReadBytes(0x1F, 1),

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

        public static void UpdateP1Atk(byte[] resultBytes, int selectedAtk, int charID) //resultbytes is divided into two parts because the 4 bytes of offset 0x1C
                                                                            //change when it goes into memory, which causes a bug if it is changed.
        {
            int P1AtkOffset = Util.BTL_GetPlayer1MemoryOffs() + 0xBC;
            int skipAtks = selectedAtk * 0x54;

            int P1AtkOffs = Util.ReadProcessMemoryInt32(P1AtkOffset) + skipAtks;
            byte[] resultBytesPart1 = new byte[0x1C];
            Array.Copy(resultBytes, 0, resultBytesPart1, 0, resultBytesPart1.Length);
            Util.WriteProcessMemoryBytes(P1AtkOffs, resultBytesPart1);

            byte[] resultBytesParte2 = new byte[0x30];
            Array.Copy(resultBytes, 0x20, resultBytesParte2, 0, resultBytesParte2.Length);
            int P1AtkOffs2 = Util.ReadProcessMemoryInt32(P1AtkOffset) + skipAtks + 0x20;
            Util.WriteProcessMemoryBytes(P1AtkOffs2, resultBytesParte2);

            //Write Normal in Memory
            byte[] atkNormalMemoryOffset = PlGen.CharGenPrm[charID].AtkListOffset;
            P1AtkOffs = BitConverter.ToInt32(atkNormalMemoryOffset, 0) + skipAtks;
            Array.Copy(resultBytes, 0, resultBytesPart1, 0, resultBytesPart1.Length);
            Util.WriteProcessMemoryBytes(P1AtkOffs, resultBytesPart1);

            Array.Copy(resultBytes, 0x20, resultBytesParte2, 0, resultBytesParte2.Length);
            P1AtkOffs2 = BitConverter.ToInt32(atkNormalMemoryOffset, 0) + skipAtks + 0x20;
            Util.WriteProcessMemoryBytes(P1AtkOffs2, resultBytesParte2);
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
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Up 1)");
                        break;
                    case 11:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Down 1)");
                        break;
                    case 12:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Sides 1)");
                        break;
                    case 13:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Up 2)");
                        break;
                    case 14:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Down 2)");
                        break;
                    case 15:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Sides 2)");
                        break;
                    case 16:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Up 3)");
                        break;
                    case 17:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Down 3)");
                        break;
                    case 18:
                        movForm.listBox1.Items.Add($"{i}: (Extra-Hit Sides 3)");
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

        public static void SendTextAtk(MovesetParameters movForm, PlAtk charAtkPrm)
        {
            if (movForm.cmbGP3FUndefendable.Items.Count == 0)
            {
                string[] yesNoOptions = { "No", "Yes" };
                foreach (var comboBox in new[] {movForm.cmbGP1FUnk1, movForm.cmbGP1FUnk2, movForm.cmbGP1FUnk3, movForm.cmbGP1FUnk4, movForm.cmbGP1FUnk5, movForm.cmbGP1FUnk6, movForm.cmbGP1FallingAfterHitting, movForm.cmbGP1FUnk8,
                                                movForm.cmbGP2FUnk1, movForm.cmbGP2FUnk2, movForm.cmbGP2FUnk3, movForm.cmbGP2AntiCounter, movForm.cmbGP2FUnk5, movForm.cmbGP2FUnk6, movForm.cmbGP2FUnk7, movForm.cmbGP2FUnk8,
                                                movForm.cmbGP3FBreakDef, movForm.cmbGP3FUndefendable, movForm.cmbGP3FUnk1, movForm.cmbGP3FHitFallen, movForm.cmbGP3FBounce, movForm.cmbGP3FWallKB, movForm.cmbGP3FUnk3, movForm.cmbGP3FUnk4,
                                                movForm.cmbGP4FUnk1, movForm.cmbGP4FUnk2, movForm.cmbGP4FUnk3, movForm.cmbGP4HitFainted, movForm.cmbGP4FUnk5, movForm.cmbGP4Backdash, movForm.cmbGP4DamageOnCounterattack, movForm.cmbGP4DamageOnDefense,})
                {
                    comboBox.Items.AddRange(yesNoOptions);
                }
            }
            VerifyFlagGroup1Bits(movForm, charAtkPrm.AtkFlagGroup1);
            VerifyFlagGroup2Bits(movForm, charAtkPrm.AtkFlagGroup2);
            VerifyDefenseFlagBits(movForm, charAtkPrm.AtkDefenseFlag);
            VerifyFlagGroup4Bits(movForm, charAtkPrm.AtkFlagGroup4);

            movForm.txtChakra.Text = ($"{charAtkPrm.AtkChakra}");
            movForm.txtDamage.Text = ($"{charAtkPrm.AtkDamage}");
            movForm.txtKnockBack.Text = ($"{charAtkPrm.AtkKnockBack}");

            movForm.cmbDmgEffect.Items.AddRange(movForm.cmbDmgEffect.Items.Count == 0 ? DamageEffectList.Values.ToArray() : new object[0]);
            int currentDmgEffect = (int)charAtkPrm.AtkDamageEffect;
            movForm.cmbDmgEffect.SelectedIndex = Math.Min(currentDmgEffect, 31);

            movForm.cmbDefenseEffect.Items.AddRange(movForm.cmbDefenseEffect.Items.Count == 0 ? PlAtk.DefenseEffectList.Values.ToArray() : new object[0]);
            int currentDefenseEffect = charAtkPrm.AtkDefenseEffect;
            movForm.cmbDefenseEffect.SelectedIndex = currentDefenseEffect == 255 ? 0 : currentDefenseEffect + 1;

            movForm.txtHitCount.Text = ($"{charAtkPrm.AtkHitCount}");
            movForm.txtHitSpeed.Text = ($"{charAtkPrm.AtkHitSpeed}");
            movForm.txtHitEffect.Text = ($"{charAtkPrm.AtkHitEffect}");
            movForm.txtSummonDistance1.Text = $"{charAtkPrm.AtkSummonDistance1}";
            movForm.txtSummonDistance2.Text = $"{charAtkPrm.AtkSummonDistance2}";
            movForm.txtKnockBackDirection.Text = $"{charAtkPrm.AtkKnockBackDirection}";
            movForm.txtAtkSound.Text = ($"{charAtkPrm.AtkSound}");

            movForm.cmbPLSound.Items.AddRange(movForm.cmbPLSound.Items.Count == 0 ? PLSoundList.Values.ToArray() : new object[0]);
            int currentPLSound = (int)charAtkPrm.AtkPlSound;
            movForm.cmbPLSound.SelectedIndex = currentPLSound == -4 ? 0 : currentPLSound == -3 ? 1 : currentPLSound == -2 ? 2 : currentPLSound == -1 ? 3 : currentPLSound > 34 ? 0 : currentPLSound + 4;
            movForm.txtSoundDelay.Text = ($"{charAtkPrm.AtkSoundDelay}");
            movForm.txtDmgSound.Text = ($"{charAtkPrm.AtkDamageSound}");
            movForm.cmbDmgParticle.Items.AddRange(movForm.cmbDmgParticle.Items.Count == 0 ? DamageParticleList.Values.ToArray() : new object[0]);
            int currentDmgParticle = (int)charAtkPrm.AtkDamageParticle;
            movForm.cmbDmgParticle.SelectedIndex = currentDmgParticle > 24 || currentDmgParticle == -1 ? 0 : currentDmgParticle + 1;
            movForm.txtDefenseSound.Text = ($"{charAtkPrm.AtkDefenseSound}");
            movForm.cmbDefenseParticle.Items.AddRange(movForm.cmbDefenseParticle.Items.Count == 0 ? DefenseParticleList.Values.ToArray() : new object[0]);
            int currentDefenseParticle = charAtkPrm.AtkDefenseParticle;
            movForm.cmbDefenseParticle.SelectedIndex = currentDefenseParticle < 0 ? currentDefenseParticle + 1 : 0;
            movForm.txtEnemySound.Text = ($"{charAtkPrm.AtkEnemySound}");
        }
        public static void VerifyFlagGroup1Bits(MovesetParameters movForm, uint AtkDefenseFlag)
        {
            bool[] bits = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                bits[i] = (AtkDefenseFlag & (1 << (7 - i))) != 0;
            }

            movForm.cmbGP1FUnk1.SelectedIndex = bits[0] ? 1 : 0;
            movForm.cmbGP1FUnk2.SelectedIndex = bits[1] ? 1 : 0;
            movForm.cmbGP1FUnk3.SelectedIndex = bits[2] ? 1 : 0;
            movForm.cmbGP1FUnk4.SelectedIndex = bits[3] ? 1 : 0;
            movForm.cmbGP1FUnk5.SelectedIndex = bits[4] ? 1 : 0;
            movForm.cmbGP1FUnk6.SelectedIndex = bits[5] ? 1 : 0;
            movForm.cmbGP1FallingAfterHitting.SelectedIndex = bits[6] ? 1 : 0;
            movForm.cmbGP1FUnk8.SelectedIndex = bits[7] ? 1 : 0;
        }

        public static void VerifyFlagGroup2Bits(MovesetParameters movForm, uint AtkDefenseFlag)
        {
            bool[] bits = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                bits[i] = (AtkDefenseFlag & (1 << (7 - i))) != 0;
            }

            movForm.cmbGP2FUnk1.SelectedIndex = bits[0] ? 1 : 0;
            movForm.cmbGP2FUnk2.SelectedIndex = bits[1] ? 1 : 0;
            movForm.cmbGP2FUnk3.SelectedIndex = bits[2] ? 1 : 0;
            movForm.cmbGP2AntiCounter.SelectedIndex = bits[3] ? 1 : 0;
            movForm.cmbGP2FUnk5.SelectedIndex = bits[4] ? 1 : 0;
            movForm.cmbGP2FUnk6.SelectedIndex = bits[5] ? 1 : 0;
            movForm.cmbGP2FUnk7.SelectedIndex = bits[6] ? 1 : 0;
            movForm.cmbGP2FUnk8.SelectedIndex = bits[7] ? 1 : 0;
        }

        public static void VerifyDefenseFlagBits(MovesetParameters movForm, uint AtkDefenseFlag)
        {
            bool[] bits = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                bits[i] = (AtkDefenseFlag & (1 << (7 - i))) != 0;
            }

            movForm.cmbGP3FBreakDef.SelectedIndex = bits[0] ? 1 : 0;
            movForm.cmbGP3FUndefendable.SelectedIndex = bits[1] ? 1 : 0;
            movForm.cmbGP3FHitFallen.SelectedIndex = bits[2] ? 1 : 0;
            movForm.cmbGP3FUnk1.SelectedIndex = bits[3] ? 1 : 0;
            movForm.cmbGP3FBounce.SelectedIndex = bits[4] ? 1 : 0;
            movForm.cmbGP3FWallKB.SelectedIndex = bits[5] ? 1 : 0;
            movForm.cmbGP3FUnk3.SelectedIndex = bits[6] ? 1 : 0;
            movForm.cmbGP3FUnk4.SelectedIndex = bits[7] ? 1 : 0;
        }

        public static void VerifyFlagGroup4Bits(MovesetParameters movForm, uint AtkDefenseFlag)
        {
            bool[] bits = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                bits[i] = (AtkDefenseFlag & (1 << (7 - i))) != 0;
            }

            movForm.cmbGP4FUnk1.SelectedIndex = bits[0] ? 1 : 0;
            movForm.cmbGP4FUnk2.SelectedIndex = bits[1] ? 1 : 0;
            movForm.cmbGP4FUnk3.SelectedIndex = bits[2] ? 1 : 0;
            movForm.cmbGP4HitFainted.SelectedIndex = bits[3] ? 1 : 0;
            movForm.cmbGP4FUnk5.SelectedIndex = bits[4] ? 1 : 0;
            movForm.cmbGP4Backdash.SelectedIndex = bits[5] ? 1 : 0;
            movForm.cmbGP4DamageOnCounterattack.SelectedIndex = bits[6] ? 1 : 0;
            movForm.cmbGP4DamageOnDefense.SelectedIndex = bits[7] ? 1 : 0;
        }

        public static byte[] UpdateCharAtkPrm(MovesetParameters movForm, int charID, int atkID)
        {
            var ninjaCharsAtk = CharAtkPrm[charID][atkID];

            List<byte> atkBlockBytes = new List<byte>();

            atkBlockBytes.AddRange(ninjaCharsAtk.AtkUnk);
            atkBlockBytes.AddRange(ninjaCharsAtk.AtkUnk1);
            atkBlockBytes.AddRange(ninjaCharsAtk.AtkUnk2);
            atkBlockBytes.AddRange(ninjaCharsAtk.AtkUnk3);
            atkBlockBytes.AddRange(ninjaCharsAtk.AtkUnk4);

            int[] bitsGroupFlag1 = new int[8];
            bitsGroupFlag1[7] = movForm.cmbGP1FUnk1.SelectedIndex;
            bitsGroupFlag1[6] = movForm.cmbGP1FUnk2.SelectedIndex;
            bitsGroupFlag1[5] = movForm.cmbGP1FUnk3.SelectedIndex;
            bitsGroupFlag1[4] = movForm.cmbGP1FUnk4.SelectedIndex;
            bitsGroupFlag1[3] = movForm.cmbGP1FUnk5.SelectedIndex;
            bitsGroupFlag1[2] = movForm.cmbGP1FUnk6.SelectedIndex;
            bitsGroupFlag1[1] = movForm.cmbGP1FallingAfterHitting.SelectedIndex;
            bitsGroupFlag1[0] = movForm.cmbGP1FUnk8.SelectedIndex;
            byte byteGroupFlag1 = Util.FormarByte(bitsGroupFlag1);
            atkBlockBytes.Add(byteGroupFlag1);
            ninjaCharsAtk.AtkFlagGroup1 = byteGroupFlag1;

            int[] bitsGroupFlag2 = new int[8];
            bitsGroupFlag2[7] = movForm.cmbGP2FUnk1.SelectedIndex;
            bitsGroupFlag2[6] = movForm.cmbGP2FUnk2.SelectedIndex;
            bitsGroupFlag2[5] = movForm.cmbGP2FUnk3.SelectedIndex;
            bitsGroupFlag2[4] = movForm.cmbGP2AntiCounter.SelectedIndex;
            bitsGroupFlag2[3] = movForm.cmbGP2FUnk5.SelectedIndex;
            bitsGroupFlag2[2] = movForm.cmbGP2FUnk6.SelectedIndex;
            bitsGroupFlag2[1] = movForm.cmbGP2FUnk7.SelectedIndex;
            bitsGroupFlag2[0] = movForm.cmbGP2FUnk8.SelectedIndex;
            byte byteGroupFlag2 = Util.FormarByte(bitsGroupFlag2);
            atkBlockBytes.Add(byteGroupFlag2);
            ninjaCharsAtk.AtkFlagGroup2 = byteGroupFlag2;

            int[] valoresSelecionados = new int[8];
            valoresSelecionados[7] = movForm.cmbGP3FBreakDef.SelectedIndex;
            valoresSelecionados[6] = movForm.cmbGP3FUndefendable.SelectedIndex;
            valoresSelecionados[5] = movForm.cmbGP3FHitFallen.SelectedIndex;
            valoresSelecionados[4] = movForm.cmbGP3FUnk1.SelectedIndex;
            valoresSelecionados[3] = movForm.cmbGP3FBounce.SelectedIndex;
            valoresSelecionados[2] = movForm.cmbGP3FWallKB.SelectedIndex;
            valoresSelecionados[1] = movForm.cmbGP3FUnk3.SelectedIndex;
            valoresSelecionados[0] = movForm.cmbGP3FUnk4.SelectedIndex;
            byte resultado = Util.FormarByte(valoresSelecionados);

            atkBlockBytes.Add(resultado);
            ninjaCharsAtk.AtkDefenseFlag = resultado;

            int[] bitsGroupFlag4 = new int[8];
            bitsGroupFlag4[7] = movForm.cmbGP4FUnk1.SelectedIndex;
            bitsGroupFlag4[6] = movForm.cmbGP4FUnk2.SelectedIndex;
            bitsGroupFlag4[5] = movForm.cmbGP4FUnk3.SelectedIndex;
            bitsGroupFlag4[4] = movForm.cmbGP4HitFainted.SelectedIndex;
            bitsGroupFlag4[3] = movForm.cmbGP4FUnk5.SelectedIndex;
            bitsGroupFlag4[2] = movForm.cmbGP4Backdash.SelectedIndex;
            bitsGroupFlag4[1] = movForm.cmbGP4DamageOnCounterattack.SelectedIndex;
            bitsGroupFlag4[0] = movForm.cmbGP4DamageOnDefense.SelectedIndex;
            byte byteGroupFlag4 = Util.FormarByte(bitsGroupFlag4);
            atkBlockBytes.Add(byteGroupFlag4);
            ninjaCharsAtk.AtkFlagGroup4 = byteGroupFlag4;

            byte atkPrevious = (byte)ninjaCharsAtk.AtkPrevious;
            atkBlockBytes.Add(atkPrevious);
            byte atkPos = (byte)ninjaCharsAtk.AtkPos;
            atkBlockBytes.Add(atkPos);
            ninjaCharsAtk.AtkPos = atkPos;
            atkBlockBytes.AddRange(ninjaCharsAtk.AtkUnk15);
            byte atkDpadFlag = (byte)ninjaCharsAtk.AtkDpadFlag;
            atkBlockBytes.Add(atkDpadFlag);
            byte atkButtonFlag = (byte)ninjaCharsAtk.AtkButtonFlag;
            atkBlockBytes.Add(atkButtonFlag);
            atkBlockBytes.AddRange(ninjaCharsAtk.AtkUnk16);
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

                atkBlockBytes.AddRange(ninjaCharsAtk.AtkUnk);
                atkBlockBytes.AddRange(ninjaCharsAtk.AtkUnk1);
                atkBlockBytes.AddRange(ninjaCharsAtk.AtkUnk2);
                atkBlockBytes.AddRange(ninjaCharsAtk.AtkUnk3);
                atkBlockBytes.AddRange(ninjaCharsAtk.AtkUnk4);

                atkBlockBytes.Add((byte)ninjaCharsAtk.AtkFlagGroup1);
                atkBlockBytes.Add((byte)ninjaCharsAtk.AtkFlagGroup2);
                atkBlockBytes.Add((byte)ninjaCharsAtk.AtkDefenseFlag);
                atkBlockBytes.Add((byte)ninjaCharsAtk.AtkFlagGroup4);
                atkBlockBytes.Add((byte)ninjaCharsAtk.AtkPrevious);
                atkBlockBytes.Add((byte)ninjaCharsAtk.AtkPos);
                atkBlockBytes.AddRange(ninjaCharsAtk.AtkUnk15);
                atkBlockBytes.Add((byte)ninjaCharsAtk.AtkDpadFlag);
                byte atkButtonFlag = (byte)ninjaCharsAtk.AtkButtonFlag;
                atkBlockBytes.Add(atkButtonFlag);
                atkBlockBytes.AddRange(ninjaCharsAtk.AtkUnk16);
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
