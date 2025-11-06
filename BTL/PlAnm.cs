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
    internal class PlAnm
    {
        public static List<List<string>> PlAnmListName = new List<List<string>>();
        public static List<List<PlAnm>> PlAnmPrm = new List<List<PlAnm>>();
        public static List<List<PlAnm>> PlAnmPrmBkp = new List<List<PlAnm>>();

        #region Anm Attributes

        public float AnmCharXDistance, AnmCharYDistance, AnmHitBoxScale, AnmHitBoxXPosition, AnmHitBoxYPosition, AnmHitBoxScale2, AnmHitBoxXPosition2, AnmHitBoxYPosition2;

        public float AnmUnk5, AnmUnk6;

        public int AnmUnk, AnmUnk1, AnmUnk4, AnmUnk7, AnmUnk8;

        public uint AnmUnk2, AnmUnk3;

        public uint AnmSpeed, AtkFlagGroup2, AtkDefenseFlag, AtkFlagGroup4, AtkPos, AtkDpadFlag, AtkButtonFlag, AtkDamageEffect;

        public short AnmID, AnmStartHitFrame, AnmEndHitFrame, AnmStartHitFrame2, AnmEndHitFrame2;

        public byte[] AnmObjAtk, AnmObjAtk2;

        #endregion

        internal static PlAnm ReadPlAnmPrm(byte[] Input) => new PlAnm
        {
            AnmID = (short)Input.ReadUInt(0x0, 16),

            AnmUnk = (short)Input.ReadUInt(0x2, 16),
            AnmUnk1 = (short)Input.ReadUInt(0x4, 16),

            AnmSpeed = Input.ReadUInt(0x6, 16),

            AnmUnk2 = Input.ReadUInt(0x8, 8),
            AnmUnk3 = Input.ReadUInt(0x9, 8),
            AnmUnk4 = (short)Input.ReadUInt(0xA, 16),

            AnmCharXDistance = Input.ReadSingle(0xC),
            AnmCharYDistance = Input.ReadSingle(0x10),

            AnmUnk5 = Input.ReadSingle(0x14),
            AnmUnk6 = Input.ReadSingle(0x18),
            AnmUnk7 = (Int32)Input.ReadUInt(0x1C, 32),

            AnmStartHitFrame = (short)Input.ReadUInt(0x20, 16),
            AnmEndHitFrame = (short)Input.ReadUInt(0x22, 16),

            AnmHitBoxScale = Input.ReadSingle(0x24),
            AnmObjAtk = Input.ReadBytes(0x28, 4),
            AnmHitBoxXPosition = Input.ReadSingle(0x2C),
            AnmHitBoxYPosition = Input.ReadSingle(0x30),

            AnmUnk8 = (Int32)Input.ReadUInt(0x34, 32),
            AnmStartHitFrame2 = (short)Input.ReadUInt(0x38, 16),
            AnmEndHitFrame2 = (short)Input.ReadUInt(0x3A, 16),
            AnmHitBoxScale2 = Input.ReadSingle(0x3C),
            AnmObjAtk2 = Input.ReadBytes(0x40, 4),
            AnmHitBoxXPosition2 = Input.ReadSingle(0x44),
            AnmHitBoxYPosition2 = Input.ReadSingle(0x48),
        };

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public Dictionary<string, byte[]> CommonBonnesList = new Dictionary<string, byte[]>()
        {
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
        {"OBJ_2cmn00t0 l forearm", new byte[]{ 0x40, 0xC3, 0x41, 0x00 }},
        {"OBJ_2cmn00t0 l hand", new byte[]{ 0xB0, 0x90, 0x46, 0x00 }},
        {"OBJ_2cmn00t0 l finger0", new byte[]{ 0x80, 0xC4, 0x41, 0x00 }},
        {"OBJ_2cmn00t0 r clavicle", new byte[]{ 0xC0, 0x6F, 0x42, 0x00 }},
        {"OBJ_2cmn00t0 r upperarm", new byte[]{ 0xC0, 0x26, 0x4C, 0x00 }},
        {"OBJ_2cmn00t0 r hand", new byte[]{ 0x90, 0x90, 0x46, 0x00 }},
        {"OBJ_2cmn00t0 r finger0", new byte[]{ 0x80, 0xC3, 0x41, 0x00 }},
        {"OBJ_2cmn00t0 tail", new byte[]{ 0x80, 0x26, 0x4C, 0x00 }},
        {"OBJ_2cmn00t0 tail1", new byte[]{ 0xE0, 0x26, 0x4C, 0x00 }},
        {"OBJ_2cmn00t0 tail2", new byte[]{ 0x80, 0x1B, 0x4B, 0x00 }},
        {"OBJ_2cmn00t0 body", new byte[]{ 0x40, 0xB9, 0x42, 0x00 }},
        };

        public static PlAnm GetPlAnm(int currentCharID, int selectedIndex)
        {
            int anmCount = PlGen.CharGenPrm[currentCharID].AnmCount;

            while (PlAnmPrm.Count <= GAME.charCount)
            {
                PlAnmPrm.Add(new List<PlAnm>());
                PlAnmPrmBkp.Add(new List<PlAnm>());
            }
            if (PlAnmPrm[currentCharID].Count == 0)
            {
                IntPtr processHandle = PCSX2Process.OpenProcess(PCSX2Process.PROCESS_VM_READ, false, PCSX2Process.ID);

                byte[] anmListOffsetBytes = PlGen.CharGenPrm[currentCharID].AnmListOffset;
                int anmListPointer = BitConverter.ToInt32(anmListOffsetBytes, 0);

                List<PlAnm> ninjaCharsAnm = new List<PlAnm>();
                List<PlAnm> ninjaCharsAnmBkp = new List<PlAnm>();

                for (int i = 0; i != anmCount; i++)
                {
                    int skipsAnmBlocks = i * 0x4C;
                    int currentAnmListOffs = anmListPointer + skipsAnmBlocks;
                    byte[] currentAnmBlock = Util.ReadProcessMemoryBytes(currentAnmListOffs, 0x4C);

                    var ninja = ReadPlAnmPrm(currentAnmBlock);
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
                int anmNameCount = PlGen.CharGenPrm[CharIndex].AnmNameCount;
                int anmNameAreaPointer = BitConverter.ToInt32(PlGen.CharGenPrm[CharIndex].AnmNameListOffset, 0);
                byte[] anmNameAreaBuffer = Util.ReadProcessMemoryBytes(anmNameAreaPointer, anmNameCount * 0x4);

                List<string> anmNameList = new List<string>();
                for (int i = 0; i < anmNameCount; i++)
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
                byte[] anmNormalMemoryOffset = PlGen.CharGenPrm[charID].AnmListOffset;
                P1AnmPointer = BitConverter.ToInt32(anmNormalMemoryOffset, 0) + skipAnms;
                Util.WriteProcessMemoryBytes(P1AnmPointer, resultBytes);

                PCSX2Process.CloseHandle(processHandle);
            }
        }

        public static void SendTextAnm(MovesetParameters movForm, PlAnm charAnm)
        {
            int currentCharID = int.Parse(movForm.lblCharID2.Text);

            movForm.cmbPlayAnmID.Items.AddRange(movForm.cmbPlayAnmID.Items.Count == 0 ? PlAnmListName[currentCharID].ToArray() : new object[0]);
            movForm.cmbPlayAnmID.SelectedIndex = charAnm.AnmID;
            movForm.numAnmUnk1.Value = charAnm.AnmUnk1;
            movForm.numAnmSpeed.Value = charAnm.AnmSpeed;
            movForm.numAnmUnk2.Value = charAnm.AnmUnk2;
            movForm.numAnmUnk3.Value = charAnm.AnmUnk3;
            movForm.numAnmUnk4.Value = charAnm.AnmUnk4;
            movForm.txtCharXDistance.Text = Convert.ToString(charAnm.AnmCharXDistance);
            movForm.txtCharYDistance.Text = Convert.ToString(charAnm.AnmCharYDistance);
            movForm.txtAnmUnk5.Text = Convert.ToString(charAnm.AnmUnk5);
            movForm.txtAnmUnk6.Text = Convert.ToString(charAnm.AnmUnk6);

            movForm.numAnmUnk7.Value = charAnm.AnmUnk7;
            movForm.numAnmStartHitFrame.Value = charAnm.AnmStartHitFrame;
            movForm.numAnmEndHitFrame.Value = charAnm.AnmEndHitFrame;
            movForm.txtHitBoxScale.Text = Convert.ToString(charAnm.AnmHitBoxScale);
            byte[] anmObjectAtk = new byte[4];
            Array.Copy(charAnm.AnmObjAtk, 0x0, anmObjectAtk, 0, 4);
            int anmObjectAtkPointer = BitConverter.ToInt32(anmObjectAtk, 0);
            string anmObjectAtkString = Util.ReadStringWithOffset(anmObjectAtkPointer, false);
            var commonBonnesList = charAnm.CommonBonnesList;
            movForm.cmbAnmObjectAtk.Items.Clear();
            if (commonBonnesList.ContainsKey(anmObjectAtkString))
            {
                movForm.cmbAnmObjectAtk.Items.AddRange(commonBonnesList.Keys.ToArray());
                for (int i = 0; i < movForm.cmbAnmObjectAtk.Items.Count; i++)
                {
                    if (movForm.cmbAnmObjectAtk.Items[i].ToString() == anmObjectAtkString)
                    {
                        movForm.cmbAnmObjectAtk.SelectedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                movForm.cmbAnmObjectAtk.Items.AddRange(commonBonnesList.Keys.ToArray());
                movForm.cmbAnmObjectAtk.Items.Add(anmObjectAtkString == "" ? "(None)" : anmObjectAtkString);
                movForm.cmbAnmObjectAtk.SelectedIndex = movForm.cmbAnmObjectAtk.Items.Count - 1;
            }
            movForm.txtHitBoxXPos.Text = Convert.ToString(charAnm.AnmHitBoxXPosition);
            movForm.txtHitBoxYPos.Text = Convert.ToString(charAnm.AnmHitBoxYPosition);

            movForm.numAnmUnk8.Value = charAnm.AnmUnk8;
            movForm.numAnmStartHitFrame2.Value = charAnm.AnmStartHitFrame2;
            movForm.numAnmEndHitFrame2.Value = charAnm.AnmEndHitFrame2;
            movForm.txtAnmHitBoxScale2.Text = Convert.ToString(charAnm.AnmHitBoxScale2);
            byte[] anmObjectAtk2 = new byte[4];
            Array.Copy(charAnm.AnmObjAtk2, 0x0, anmObjectAtk2, 0, 4);
            int anmObjectAtkPointer2 = BitConverter.ToInt32(anmObjectAtk2, 0);
            string anmObjectAtkString2 = Util.ReadStringWithOffset(anmObjectAtkPointer2, false);
            movForm.txtAnmHitBoxXPos2.Text = Convert.ToString(charAnm.AnmHitBoxXPosition2);
            movForm.txtAnmHitBoxYPos2.Text = Convert.ToString(charAnm.AnmHitBoxYPosition2);

            movForm.cmbAnmObjectAtk2.Items.Clear();
            if (commonBonnesList.ContainsKey(anmObjectAtkString2))
            {
                movForm.cmbAnmObjectAtk2.Items.AddRange(commonBonnesList.Keys.ToArray());
                for (int i = 0; i < movForm.cmbAnmObjectAtk2.Items.Count; i++)
                {
                    if (movForm.cmbAnmObjectAtk2.Items[i].ToString() == anmObjectAtkString2)
                    {
                        movForm.cmbAnmObjectAtk2.SelectedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                movForm.cmbAnmObjectAtk2.Items.AddRange(commonBonnesList.Keys.ToArray());
                movForm.cmbAnmObjectAtk2.Items.Add(anmObjectAtkString2 == "" ? "(None)" : anmObjectAtkString2);
                movForm.cmbAnmObjectAtk2.SelectedIndex = movForm.cmbAnmObjectAtk2.Items.Count - 1;
            }
        }

        public static byte[] UpdateCharAnmPrm(MovesetParameters movForm, int charID)
        {
            int anmBlockID = int.Parse(movForm.listBox1.SelectedItem.ToString().Split(':')[0]);

            var ninjaCharsAnm = PlAnmPrm[charID][anmBlockID];

            List<byte> anmBlockBytes = new List<byte>();

            int anmID = movForm.cmbPlayAnmID.SelectedIndex;
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(anmID)));
            ninjaCharsAnm.AnmID = Convert.ToInt16(anmID);
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(ninjaCharsAnm.AnmUnk)));
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16((short)movForm.numAnmUnk1.Value)));
            ninjaCharsAnm.AnmUnk1 = (short)movForm.numAnmUnk1.Value;
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16(movForm.numAnmSpeed.Value)));
            ninjaCharsAnm.AnmSpeed = Convert.ToUInt16(movForm.numAnmSpeed.Value);
            anmBlockBytes.Add((byte)movForm.numAnmUnk2.Value);
            ninjaCharsAnm.AnmUnk2 = (byte)movForm.numAnmUnk2.Value;
            anmBlockBytes.Add((byte)movForm.numAnmUnk3.Value);
            ninjaCharsAnm.AnmUnk3 = (byte)movForm.numAnmUnk3.Value;
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16((short)movForm.numAnmUnk4.Value)));
            ninjaCharsAnm.AnmUnk4 = (short)movForm.numAnmUnk4.Value;
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.txtCharXDistance.Text)));
            ninjaCharsAnm.AnmCharXDistance = Convert.ToSingle(movForm.txtCharXDistance.Text);
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.txtCharYDistance.Text)));
            ninjaCharsAnm.AnmCharYDistance = Convert.ToSingle(movForm.txtCharYDistance.Text);
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.txtAnmUnk5.Text)));
            ninjaCharsAnm.AnmUnk5 = Convert.ToSingle(movForm.txtAnmUnk5.Text);
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.txtAnmUnk6.Text)));
            ninjaCharsAnm.AnmUnk6 = Convert.ToSingle(movForm.txtAnmUnk6.Text);
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(movForm.numAnmUnk7.Value)));
            ninjaCharsAnm.AnmUnk7 = Convert.ToInt32(movForm.numAnmUnk7.Value);
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(movForm.numAnmStartHitFrame.Value)));
            ninjaCharsAnm.AnmStartHitFrame = Convert.ToInt16(movForm.numAnmStartHitFrame.Value);
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(movForm.numAnmEndHitFrame.Value)));
            ninjaCharsAnm.AnmEndHitFrame = Convert.ToInt16(movForm.numAnmEndHitFrame.Value);
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.txtHitBoxScale.Text)));
            ninjaCharsAnm.AnmHitBoxScale = Convert.ToSingle(movForm.txtHitBoxScale.Text);
            int selectedIndexcmbAnmObjectAtk = movForm.cmbAnmObjectAtk.SelectedIndex;
            var commonBonnesList = ninjaCharsAnm.CommonBonnesList;
            if (commonBonnesList.TryGetValue(movForm.cmbAnmObjectAtk.Items[selectedIndexcmbAnmObjectAtk].ToString(), out byte[] anmObjAtkPointerBytes))
            {
                anmBlockBytes.AddRange(anmObjAtkPointerBytes);
                ninjaCharsAnm.AnmObjAtk = anmObjAtkPointerBytes;
            }
            else
            {
                anmBlockBytes.AddRange(ninjaCharsAnm.AnmObjAtk);
            }
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.txtHitBoxXPos.Text)));
            ninjaCharsAnm.AnmHitBoxXPosition = Convert.ToSingle(movForm.txtHitBoxXPos.Text);
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.txtHitBoxYPos.Text)));
            ninjaCharsAnm.AnmHitBoxYPosition = Convert.ToSingle(movForm.txtHitBoxYPos.Text);
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(movForm.numAnmUnk8.Value)));
            ninjaCharsAnm.AnmUnk8 = Convert.ToInt32(movForm.numAnmUnk8.Value);
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(movForm.numAnmStartHitFrame2.Value)));
            ninjaCharsAnm.AnmStartHitFrame2 = Convert.ToInt16(movForm.numAnmStartHitFrame2.Value);
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(movForm.numAnmEndHitFrame2.Value)));
            ninjaCharsAnm.AnmEndHitFrame2 = Convert.ToInt16(movForm.numAnmEndHitFrame2.Value);
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.txtAnmHitBoxScale2.Text)));
            ninjaCharsAnm.AnmHitBoxScale2 = Convert.ToSingle(movForm.txtAnmHitBoxScale2.Text);
            int selectedIndexCmbAnmObjectAtk2 = movForm.cmbAnmObjectAtk2.SelectedIndex;
            if (commonBonnesList.TryGetValue(movForm.cmbAnmObjectAtk2.Items[selectedIndexCmbAnmObjectAtk2].ToString(), out byte[] anmObjAtkPointerBytes2))
            {
                anmBlockBytes.AddRange(anmObjAtkPointerBytes2);
                ninjaCharsAnm.AnmObjAtk2 = anmObjAtkPointerBytes2;
            }
            else
            {
                anmBlockBytes.AddRange(ninjaCharsAnm.AnmObjAtk2);
            }
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.txtAnmHitBoxXPos2.Text)));
            ninjaCharsAnm.AnmHitBoxXPosition2 = Convert.ToSingle(movForm.txtAnmHitBoxXPos2.Text);
            anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(movForm.txtAnmHitBoxYPos2.Text)));
            ninjaCharsAnm.AnmHitBoxYPosition2 = Convert.ToSingle(movForm.txtAnmHitBoxYPos2.Text);

            byte[] resultBytes = anmBlockBytes.ToArray();
            return resultBytes;
        }

        public static byte[] UpdateAllCharAnmPrm(MovesetParameters movForm, int charID)
        {
            List<byte> anmBlockBytes = new List<byte>();

            for(int i = 0; i < PlGen.CharGenPrm[charID].AnmCount; i++)
            {
                var ninjaCharsAnm = PlAnmPrm[charID][i];

                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(ninjaCharsAnm.AnmID)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(ninjaCharsAnm.AnmUnk)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16((short)ninjaCharsAnm.AnmUnk1)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToUInt16((ushort)ninjaCharsAnm.AnmSpeed)));
                anmBlockBytes.Add((byte)ninjaCharsAnm.AnmUnk2);
                anmBlockBytes.Add((byte)ninjaCharsAnm.AnmUnk3);
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16((short)ninjaCharsAnm.AnmUnk4)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.AnmCharXDistance)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.AnmCharYDistance)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.AnmUnk5)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.AnmUnk6)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(ninjaCharsAnm.AnmUnk7)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(ninjaCharsAnm.AnmStartHitFrame)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(ninjaCharsAnm.AnmEndHitFrame)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.AnmHitBoxScale)));
                anmBlockBytes.AddRange(ninjaCharsAnm.AnmObjAtk);
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.AnmHitBoxXPosition)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.AnmHitBoxYPosition)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt32(ninjaCharsAnm.AnmUnk8)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(ninjaCharsAnm.AnmStartHitFrame2)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(ninjaCharsAnm.AnmEndHitFrame2)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.AnmHitBoxScale2)));
                anmBlockBytes.AddRange(ninjaCharsAnm.AnmObjAtk2);
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.AnmHitBoxXPosition2)));
                anmBlockBytes.AddRange(BitConverter.GetBytes(Convert.ToSingle(ninjaCharsAnm.AnmHitBoxYPosition2)));
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
                    byte[] charAnmAreaOffsetBytes = PlGen.CharGenPrm[charID].AnmListOffset;
                    charAnmAreaOffsetBytes[3] = 0x0;
                    int subValue = 0xFFE80;
                    int charAnmAreaOffset = BitConverter.ToInt32(charAnmAreaOffsetBytes, 0) - subValue;

                    fs.Seek(charAnmAreaOffset, SeekOrigin.Begin);

                    fs.Write(resultBytes, 0, resultBytes.Length);

                    MessageBox.Show("The changes were saved successfully!", string.Empty, MessageBoxButtons.OK);
                }
            }
        }
    }
}
