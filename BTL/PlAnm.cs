using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace UN5ModdingWorkshop
{
    internal class PlAnm
    {
        public static List<List<string>> PlAnmListName = new List<List<string>>();
        public static List<List<PlAnm>> PlAnmPrm = new List<List<PlAnm>>();
        public static List<List<PlAnm>> PlAnmPrmBkp = new List<List<PlAnm>>();

        #region Anm Attributes

        public short AnmID = 0;
        public int Duration = -0x10;
        public short StartFrame = 0;
        public uint Speed = 256;
        public uint UnkFlag1 = 2;
        public uint UnkFlag2 = 2;

        public short CharMoveFrame = 0x7FFF;
        public float CharXDistance = 0f;
        public float CharYDistance = 0f;
        public float CharXSlide = 0f;
        public float CharYSlide = 0f;

        public int FlagsGroup1 = 0x12;
        public short StartHitFrame1 = -0x7FFF;
        public short EndHitFrame1 = -0x7FFF;
        public float HitBoxScale = 0f;
        public byte[] ObjAtk = { 0xC0, 0x05, 0x61, 0x00 };
        public float HitBoxXPosition = 0f;
        public float HitBoxYPosition = 0f;

        public int FlagsGroup2 = 0x12;
        public short StartHitFrame2 = -0x7FFF;
        public short EndHitFrame2 = -0x7FFF;
        public float HitBoxScale2 = 0f;
        public byte[] ObjAtk2 = { 0xC0, 0x05, 0x61, 0x00 };
        public float HitBoxXPosition2 = 0f;
        public float HitBoxYPosition2 = 0f;

        #endregion

        internal static PlAnm Read(byte[] Input) => new PlAnm
        {
            AnmID = (short)Input.ReadUInt(0x0, 16),

            Duration = (short)Input.ReadUInt(0x2, 16),
            StartFrame = (short)Input.ReadUInt(0x4, 16),

            Speed = Input.ReadUInt(0x6, 16),

            UnkFlag1 = Input.ReadUInt(0x8, 8),
            UnkFlag2 = Input.ReadUInt(0x9, 8),
            CharMoveFrame = (short)Input.ReadUInt(0xA, 16),

            CharXDistance = Input.ReadSingle(0xC),
            CharYDistance = Input.ReadSingle(0x10),

            CharXSlide = Input.ReadSingle(0x14),
            CharYSlide = Input.ReadSingle(0x18),
            FlagsGroup1 = (Int32)Input.ReadUInt(0x1C, 32),

            StartHitFrame1 = (short)Input.ReadUInt(0x20, 16),
            EndHitFrame1 = (short)Input.ReadUInt(0x22, 16),

            HitBoxScale = Input.ReadSingle(0x24),
            ObjAtk = Input.ReadBytes(0x28, 4),
            HitBoxXPosition = Input.ReadSingle(0x2C),
            HitBoxYPosition = Input.ReadSingle(0x30),

            FlagsGroup2 = (Int32)Input.ReadUInt(0x34, 32),
            StartHitFrame2 = (short)Input.ReadUInt(0x38, 16),
            EndHitFrame2 = (short)Input.ReadUInt(0x3A, 16),
            HitBoxScale2 = Input.ReadSingle(0x3C),
            ObjAtk2 = Input.ReadBytes(0x40, 4),
            HitBoxXPosition2 = Input.ReadSingle(0x44),
            HitBoxYPosition2 = Input.ReadSingle(0x48),
        };

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public Dictionary<string, byte[]> CommonBonnesList = new Dictionary<string, byte[]>()
        {
        {"", new byte[]{ 0xC0, 0x05, 0x61, 0x00 }},
        {"OBJ_2cmn00t0 pelvis", new byte[]{ 0xA0, 0xC3, 0x41, 0x00 }},
        {"OBJ_2cmn00t0 spine", new byte[]{ 0xC0, 0xC3, 0x41, 0x00 }},
        {"OBJ_2cmn00t0 l thigh", new byte[]{ 0xA0, 0xF6, 0x46, 0x00 }},
        {"OBJ_2cmn00t0 l calf", new byte[]{ 0x00, 0xC5, 0x41, 0x00 }},
        {"OBJ_2cmn00t0 l foot", new byte[]{ 0x20, 0xC3, 0x41, 0x00 }},
        {"OBJ_2cmn00t0 r thigh", new byte[]{ 0x60, 0x88, 0x49, 0x00 }},
        {"OBJ_2cmn00t0 r calf", new byte[]{ 0xE0, 0x6F, 0x42, 0x00 }},
        {"OBJ_2cmn00t0 r foot", new byte[]{ 0xA0, 0xC4, 0x41, 0x00 }},
        {"OBJ_2cmn00t0 spine1", new byte[]{ 0x70, 0xB9, 0x4C, 0x00 }},
        {"OBJ_2cmn00t0 neck", new byte[]{ 0x40, 0x88, 0x49, 0x00 }},
        {"OBJ_2cmn00t0 head", new byte[]{ 0x20, 0xB9, 0x42, 0x00 }},
        {"OBJ_2cmn00t0 l clavicle", new byte[]{ 0x60, 0xC3, 0x41, 0x00 }},
        {"OBJ_2cmn00t0 l upperarm", new byte[]{ 0x30, 0x48, 0x46, 0x00 }},
        {"OBJ_2cmn00t0 l forearm", new byte[]{ 0x40, 0xC3, 0x41, 0x00 }},
        {"OBJ_2cmn00t0 l hand", new byte[]{ 0xB0, 0x90, 0x46, 0x00 }},
        {"OBJ_2cmn00t0 l finger0", new byte[]{ 0x80, 0xC4, 0x41, 0x00 }},
        {"OBJ_2cmn00t0 r clavicle", new byte[]{ 0xC0, 0x6F, 0x42, 0x00 }},
        {"OBJ_2cmn00t0 r upperarm", new byte[]{ 0xC0, 0x26, 0x4C, 0x00 }},
        {"OBJ_2cmn00t0 r forearm", new byte[]{ 0x60, 0x93, 0x45, 0x00 }},
        {"OBJ_2cmn00t0 r hand", new byte[]{ 0x90, 0x90, 0x46, 0x00 }},
        {"OBJ_2cmn00t0 r finger0", new byte[]{ 0x80, 0xC3, 0x41, 0x00 }},
        {"OBJ_2cmn00t0 tail", new byte[]{ 0x80, 0x26, 0x4C, 0x00 }},
        {"OBJ_2cmn00t0 tail1", new byte[]{ 0xE0, 0x26, 0x4C, 0x00 }},
        {"OBJ_2cmn00t0 tail2", new byte[]{ 0x80, 0x1B, 0x4B, 0x00 }},
        {"OBJ_2cmn00t0 body", new byte[]{ 0x40, 0xB9, 0x42, 0x00 }},
        };

        public static PlAnm Get(int currentCharID, int selectedIndex)
        {
            while (PlAnmPrm.Count <= GAME.charCount)
            {
                PlAnmPrm.Add(new List<PlAnm>());
                PlAnmPrmBkp.Add(new List<PlAnm>());
            }
            if (PlAnmPrm[currentCharID].Count == 0)
            {
                List<PlAnm> ninjaCharsAnm = new List<PlAnm>();
                List<PlAnm> ninjaCharsAnmBkp = new List<PlAnm>();

                for (int i = 0; i != PlGen.List[currentCharID].AnmCount; i++)
                {
                    int skipsAnmBlocks = i * 0x4C;
                    int currentAnmListOffs = (int)PlGen.List[currentCharID].AnmListOffset + skipsAnmBlocks;
                    byte[] currentAnmBlock = Util.ReadProcessMemoryBytes(currentAnmListOffs, 0x4C);

                    var ninja = Read(currentAnmBlock);
                    var clone = (PlAnm)ninja.Clone();
                    ninjaCharsAnm.Add(ninja);
                    ninjaCharsAnmBkp.Add(clone);
                }
                PlAnmPrm[currentCharID] = ninjaCharsAnm;
                PlAnmPrmBkp[currentCharID] = ninjaCharsAnmBkp;
            }

            return PlAnmPrm[currentCharID][selectedIndex];
        }
        public static string GetPlAnmName(int CharIndex, int AnmIndex)
        {
            while (PlAnmListName.Count <= GAME.charCount)
            {
                PlAnmListName.Add(new List<string>());
            }
            if (PlAnmListName[CharIndex].Count == 0)
            {
                byte[] anmNameAreaBuffer = Util.ReadProcessMemoryBytes((int)PlGen.List[CharIndex].AnmNameListOffset, 
                                                                        (int)PlGen.List[CharIndex].AnmNameCount * 0x4);

                List<string> anmNameList = new List<string>();
                for (int i = 0; i < PlGen.List[CharIndex].AnmNameCount; i++)
                {
                    int anmNamePointer = BitConverter.ToInt32(anmNameAreaBuffer, i * 0x4);
                    string docodedAnmName = Util.ReadStringWithOffset(anmNamePointer, false);
                    anmNameList.Add(docodedAnmName);
                }
                PlAnmListName[CharIndex] = anmNameList;
            }
            return PlAnmListName[CharIndex][AnmIndex];
        }

        public static void UpdateP1Anm(byte[] resultBytes, int selectedAnm, int charID)
        {
            IntPtr processHandle = PCSX2Process.OpenProcess(PCSX2Process.PROCESS_ALL_ACCESS, false, PCSX2Process.ID);
            if (processHandle != IntPtr.Zero)
            {
                int P1Offset = Util.BTL_GetPlayer1MemoryOffs() + 0xCC;

                int skipAnms = selectedAnm * 0x4C;
                int P1AnmPointer = Util.ReadProcessMemoryInt32(P1Offset) + skipAnms;
                Util.WriteProcessMemoryBytes(P1AnmPointer, resultBytes);

                //Write Normal in Memory
                Util.WriteProcessMemoryBytes((int)PlGen.List[charID].AnmListOffset + skipAnms, resultBytes);

                PCSX2Process.CloseHandle(processHandle);
            }
        }

        public static void SendTextAnm(MovesetParameters movForm, PlAnm Anm)
        {
            int currentCharID = int.Parse(movForm.lblCharID2.Text);

            movForm.cmbPlayAnmID.Items.AddRange(movForm.cmbPlayAnmID.Items.Count == 0 ? PlAnmListName[currentCharID].ToArray() : new object[0]);
            movForm.cmbPlayAnmID.SelectedIndex = Anm.AnmID;
            if(Anm.Duration >= 0)
            {
                movForm.cmbDuration.SelectedIndex = 0;
                movForm.numDuration.Value = Anm.Duration;
                movForm.numDuration.Enabled = true;
            }
            else
            {
                movForm.cmbDuration.SelectedIndex = Anm.Duration + 0x16;
                movForm.numDuration.Enabled = false;
            }

            movForm.numStartFrame.Value = Anm.StartFrame;
            movForm.numAnmSpeed.Value = (decimal)(Anm.Speed / 256f);
            movForm.numAnmUnk2.Value = Anm.UnkFlag1;
            movForm.numAnmUnk3.Value = Anm.UnkFlag2;
            if(Anm.CharMoveFrame == 0x7FFF)
            {
                movForm.numCharStartMoveFrame.Enabled = false;
                movForm.numCharStartMoveFrame.Value = 0;
                movForm.chkNoneCharStartMoveFrame.Checked = true;
            }
            else
            {
                movForm.numCharStartMoveFrame.Enabled = true;
                movForm.numCharStartMoveFrame.Value = Anm.CharMoveFrame;
                movForm.chkNoneCharStartMoveFrame.Checked = false;
            }
            movForm.numCharXDistance.Value = (decimal)Anm.CharXDistance;
            movForm.numCharYDistance.Value = (decimal)Anm.CharYDistance;
            movForm.numCharXSlide.Value = (decimal)Anm.CharXSlide;
            movForm.numCharYSlide.Value = (decimal)Anm.CharYSlide;

            if(Anm.StartHitFrame1 == -0x7FFF)
            {
                movForm.numAnmStartHitFrame1.Enabled = false;
                movForm.numAnmStartHitFrame1.Value = 0;
                movForm.chkNoneStartHitFrame1.Checked = true;
            }
            else
            {
                movForm.numAnmStartHitFrame1.Enabled = true;
                movForm.numAnmStartHitFrame1.Value = Anm.StartHitFrame1;
                movForm.chkNoneStartHitFrame1.Checked = false;
            }
            if(Anm.EndHitFrame1 == 0x7FFF)
            {
                movForm.numAnmEndHitFrame1.Enabled = false;
                movForm.numAnmEndHitFrame1.Value = 0;
                movForm.chkLastEndHitFrame1.Checked = true;
                movForm.chkNoneEndHitFrame1.Checked = false;
            }
            else if(Anm.EndHitFrame1 == -0x7FFF)
            {
                movForm.numAnmEndHitFrame1.Enabled = false;
                movForm.numAnmEndHitFrame1.Value = 0;
                movForm.chkNoneEndHitFrame1.Checked = true;
                movForm.chkLastEndHitFrame1.Checked = false;
            }
            else
            {
                 movForm.numAnmEndHitFrame1.Enabled = true;
                 movForm.numAnmEndHitFrame1.Value = Anm.EndHitFrame1;
                 movForm.chkNoneEndHitFrame1.Checked = false;
                 movForm.chkLastEndHitFrame1.Checked = false;
            }

            movForm.numHitBoxScale1.Value = Convert.ToDecimal(Anm.HitBoxScale);
            byte[] anmObjectAtk = new byte[4];
            Array.Copy(Anm.ObjAtk, 0x0, anmObjectAtk, 0, 4);
            int anmObjectAtkPointer = BitConverter.ToInt32(anmObjectAtk, 0);
            string hitBoxBasePos = Util.ReadStringWithOffset(anmObjectAtkPointer, false);
            if(hitBoxBasePos == "")
            {
                movForm.txtHitBone1.Text = "";
                movForm.txtHitBone1.Enabled = false;
                movForm.chkHitBoxCharPos1.Checked = true;
            }
            else
            {
                movForm.txtHitBone1.Text = hitBoxBasePos;
                movForm.txtHitBone1.Enabled = true;
                movForm.chkHitBoxCharPos1.Checked = false;
            }
            movForm.numHitBoxXPos1.Value = Convert.ToDecimal(Anm.HitBoxXPosition);
            movForm.numHitBoxYPos1.Value = Convert.ToDecimal(Anm.HitBoxYPosition);

            if (Anm.StartHitFrame2 == -0x7FFF)
            {
                movForm.numAnmStartHitFrame2.Enabled = false;
                movForm.numAnmStartHitFrame2.Value = 0;
                movForm.chkNoneStartHitFrame2.Checked = true;
            }
            else
            {
                movForm.numAnmStartHitFrame2.Enabled = true;
                movForm.numAnmStartHitFrame2.Value = Anm.StartHitFrame2;
                movForm.chkNoneStartHitFrame2.Checked = false;
            }
            if (Anm.EndHitFrame2 == 0x7FFF)
            {
                movForm.numAnmEndHitFrame2.Enabled = false;
                movForm.numAnmEndHitFrame2.Value = 0;
                movForm.chkLastEndHitFrame2.Checked = true;
                movForm.chkNoneEndHitFrame2.Checked = false;
            }
            else if (Anm.EndHitFrame2 == -0x7FFF)
            {
                movForm.numAnmEndHitFrame2.Enabled = false;
                movForm.numAnmEndHitFrame2.Value = 0;
                movForm.chkNoneEndHitFrame2.Checked = true;
                movForm.chkLastEndHitFrame2.Checked = false;
            }
            else
            {
                movForm.numAnmEndHitFrame2.Enabled = true;
                movForm.numAnmEndHitFrame2.Value = Anm.EndHitFrame2;
                movForm.chkNoneEndHitFrame2.Checked = false;
                movForm.chkLastEndHitFrame2.Checked = false;
            }

            movForm.numAnmHitBoxScale2.Value = Convert.ToDecimal(Anm.HitBoxScale2);
            byte[] anmObjectAtk2 = new byte[4];
            Array.Copy(Anm.ObjAtk2, 0x0, anmObjectAtk2, 0, 4);
            int anmObjectAtkPointer2 = BitConverter.ToInt32(anmObjectAtk2, 0);
            string hitBoxBasePos2 = Util.ReadStringWithOffset(anmObjectAtkPointer2, false);
            if (hitBoxBasePos2 == "")
            {
                movForm.txtHitBone2.Text = "";
                movForm.txtHitBone2.Enabled = false;
                movForm.chkHitBoxCharPos2.Checked = true;
            }
            else
            {
                movForm.txtHitBone2.Text = hitBoxBasePos2;
                movForm.txtHitBone2.Enabled = true;
                movForm.chkHitBoxCharPos2.Checked = false;
            }
            movForm.numHitBoxXPos2.Value = Convert.ToDecimal(Anm.HitBoxXPosition2);
            movForm.numHitBoxYPos2.Value = Convert.ToDecimal(Anm.HitBoxYPosition2);


            for(int i = 0; i < 8; i++)
            {
                bool currentFlag = ((Anm.FlagsGroup1 >> i) & 1) == 1;
                movForm.chkAnmFlags1.SetItemChecked(i, currentFlag);
            }
            for (int i = 0; i < 8; i++)
            {
                bool currentFlag = ((Anm.FlagsGroup2 >> i) & 1) == 1;
                movForm.chkAnmFlags2.SetItemChecked(i, currentFlag);
            }
        }

        private static void ChkHitBoxCharPos1_CheckedChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public static byte[] UpdateCharAnmPrm(MovesetParameters movForm, int charID)
        {
            int anmBlockID = int.Parse(movForm.listBox1.SelectedItem.ToString().Split(':')[0]);

            var Anm = PlAnmPrm[charID][anmBlockID];

            List<byte> AnmData = new List<byte>();

            int anmID = movForm.cmbPlayAnmID.SelectedIndex;
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToInt16(anmID)));
            Anm.AnmID = Convert.ToInt16(anmID);
            if(movForm.cmbDuration.SelectedIndex == 0)
            {
                Anm.Duration = (int)movForm.numDuration.Value;
            }
            else
            {
                Anm.Duration = movForm.cmbDuration.SelectedIndex - 0x16;
            }
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToInt16(Anm.Duration)));
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToInt16((short)movForm.numStartFrame.Value)));
            Anm.StartFrame = (short)movForm.numStartFrame.Value;
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToUInt16(movForm.numAnmSpeed.Value * 256)));
            Anm.Speed = Convert.ToUInt16(movForm.numAnmSpeed.Value * 256);
            AnmData.Add((byte)movForm.numAnmUnk2.Value);
            Anm.UnkFlag1 = (byte)movForm.numAnmUnk2.Value;
            AnmData.Add((byte)movForm.numAnmUnk3.Value);
            Anm.UnkFlag2 = (byte)movForm.numAnmUnk3.Value;
            Anm.CharMoveFrame = movForm.chkNoneCharStartMoveFrame.Checked == true ? (short)0x7FFF : (short)movForm.numCharStartMoveFrame.Value;
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToInt16((short)Anm.CharMoveFrame)));
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.numCharXDistance.Value)));
            Anm.CharXDistance = Convert.ToSingle(movForm.numCharXDistance.Value);
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.numCharYDistance.Value)));
            Anm.CharYDistance = Convert.ToSingle(movForm.numCharYDistance.Value);
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.numCharXSlide.Value)));
            Anm.CharXSlide = Convert.ToSingle(movForm.numCharXSlide.Value);
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.numCharYSlide.Value)));
            Anm.CharYSlide = Convert.ToSingle(movForm.numCharYSlide.Value);
            var anmFlagsGroup1Bits = new int[8];
            for (int i = 0; i < 8; i++)
            {
                anmFlagsGroup1Bits[i] = movForm.chkAnmFlags1.GetItemChecked(i) ? 1 : 0;
            }
            Anm.FlagsGroup1 = Util.FormarByte(anmFlagsGroup1Bits);

            AnmData.AddRange(BitConverter.GetBytes(Convert.ToInt32(Anm.FlagsGroup1)));

            if (movForm.chkNoneStartHitFrame1.Checked)
            {
                Anm.StartHitFrame1 = -0x7FFF;
            }
            else
            {
                Anm.StartHitFrame1 = Convert.ToInt16(movForm.numAnmStartHitFrame1.Value);
            }
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToInt16(Anm.StartHitFrame1)));

            if (movForm.chkLastEndHitFrame1.Checked)
            {
                Anm.EndHitFrame1 = 0x7FFF;
            }
            else if (movForm.chkNoneEndHitFrame1.Checked)
            {
                Anm.EndHitFrame1 = -0x7FFF;
            }
            else
            {
                Anm.EndHitFrame1 = Convert.ToInt16(movForm.numAnmEndHitFrame1.Value);
            }
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToInt16(Anm.EndHitFrame1)));

            AnmData.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.numHitBoxScale1.Value)));
            Anm.HitBoxScale = Convert.ToSingle(movForm.numHitBoxScale1.Value);
            var commonBonnesList = Anm.CommonBonnesList;
            if (commonBonnesList.TryGetValue(movForm.txtHitBone1.Text, out byte[] anmObjAtkPointerBytes))
            {
                AnmData.AddRange(anmObjAtkPointerBytes);
                Anm.ObjAtk = anmObjAtkPointerBytes;
            }
            else
            {
                AnmData.AddRange(Anm.ObjAtk);
            }
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.numHitBoxXPos1.Value)));
            Anm.HitBoxXPosition = Convert.ToSingle(movForm.numHitBoxXPos1.Value);
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.numHitBoxYPos1.Value)));
            Anm.HitBoxYPosition = Convert.ToSingle(movForm.numHitBoxYPos1.Value);
            var anmFlagsGroup2Bits = new int[8];
            for (int i = 0; i < 8; i++)
            {
                anmFlagsGroup2Bits[i] = movForm.chkAnmFlags2.GetItemChecked(i) ? 1 : 0;
            }
            Anm.FlagsGroup2 = Util.FormarByte(anmFlagsGroup2Bits);            
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToInt32(Anm.FlagsGroup2)));

            if (movForm.chkNoneStartHitFrame2.Checked)
            {
                Anm.StartHitFrame2 = -0x7FFF;
            }
            else
            {
                Anm.StartHitFrame2 = Convert.ToInt16(movForm.numAnmStartHitFrame2.Value);
            }
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToInt16(Anm.StartHitFrame2)));

            if (movForm.chkLastEndHitFrame2.Checked)
            {
                Anm.EndHitFrame2 = 0x7FFF;
            }
            else if (movForm.chkNoneEndHitFrame2.Checked)
            {
                Anm.EndHitFrame2 = -0x7FFF;
            }
            else
            {
                Anm.EndHitFrame2 = Convert.ToInt16(movForm.numAnmEndHitFrame2.Value);
            }
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToInt16(Anm.EndHitFrame2)));
            
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.numAnmHitBoxScale2.Value)));
            Anm.HitBoxScale2 = Convert.ToSingle(movForm.numAnmHitBoxScale2.Value);
            if (commonBonnesList.TryGetValue(movForm.txtHitBone2.Text, out byte[] anmObjAtkPointerBytes2))
            {
                AnmData.AddRange(anmObjAtkPointerBytes2);
                Anm.ObjAtk2 = anmObjAtkPointerBytes2;
            }
            else
            {
                AnmData.AddRange(Anm.ObjAtk2);
            }
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.numHitBoxXPos2.Value)));
            Anm.HitBoxXPosition2 = Convert.ToSingle(movForm.numHitBoxXPos2.Value);
            AnmData.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.numHitBoxYPos2.Value)));
            Anm.HitBoxYPosition2 = Convert.ToSingle(movForm.numHitBoxYPos2.Value);

            byte[] resultBytes = AnmData.ToArray();
            return resultBytes;
        }

        public static byte[] UpdateAllCharAnmPrm(MovesetParameters movForm, int charID)
        {
            List<byte> anmBlockBytes = new List<byte>();

            for(int i = 0; i < PlGen.List[charID].AnmCount; i++)
            {
                var ninjaCharsAnm = PlAnmPrm[charID][i];

                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(ninjaCharsAnm.AnmID)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(ninjaCharsAnm.Duration)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16((short)ninjaCharsAnm.StartFrame)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16((ushort)ninjaCharsAnm.Speed)));
                anmBlockBytes.Add((byte)ninjaCharsAnm.UnkFlag1);
                anmBlockBytes.Add((byte)ninjaCharsAnm.UnkFlag2);
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16((short)ninjaCharsAnm.CharMoveFrame)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.CharXDistance)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.CharYDistance)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.CharXSlide)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.CharYSlide)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(ninjaCharsAnm.FlagsGroup1)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(ninjaCharsAnm.StartHitFrame1)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(ninjaCharsAnm.EndHitFrame1)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.HitBoxScale)));
                anmBlockBytes.AddRange(ninjaCharsAnm.ObjAtk);
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.HitBoxXPosition)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.HitBoxYPosition)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(ninjaCharsAnm.FlagsGroup2)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(ninjaCharsAnm.StartHitFrame2)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(ninjaCharsAnm.EndHitFrame2)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.HitBoxScale2)));
                anmBlockBytes.AddRange(ninjaCharsAnm.ObjAtk2);
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.HitBoxXPosition2)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.HitBoxYPosition2)));
            }
            byte[] resultBytes = anmBlockBytes.ToArray();
            return resultBytes;
        }
        public static void WriteELFCharAnm(byte[] resultBytes, int charID)
        {
            if (!File.Exists(GAME.caminhoELF))
            {
                MessageBox.Show("Unable to save, check if the file has been deleted or moved.", string.Empty, MessageBoxButtons.OK);
            }
            else
            {
                using (FileStream fs = new FileStream(GAME.caminhoELF, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    int subValue = 0xFFE80;
                    int charAnmAreaOffset = (int)PlGen.List[charID].AnmListOffset - subValue;

                    fs.Seek(charAnmAreaOffset, SeekOrigin.Begin);

                    fs.Write(resultBytes, 0, resultBytes.Length);

                    MessageBox.Show("The changes were saved successfully!", string.Empty, MessageBoxButtons.OK);
                }
            }
        }
    }
}
