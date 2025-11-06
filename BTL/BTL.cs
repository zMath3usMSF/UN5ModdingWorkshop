using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1;

namespace UN5ModdingWorkshop
{
    public class BTL
    {
        public static int P1ID { get; set; }
        public static List<int> charMainAreaOffsets = new List<int>();
        public static List<byte[]> charMainAreaList = new List<byte[]>();
        public static List<string> charNameList = new List<string>();
        public static List<string> charCCSList = new List<string>();
        public static List<string> mapNameList = new List<string>();

        public static void ReadCharProgDataTbl(IntPtr processHandle, int charProgDataTblOffs)
        {

            for (int i = 0; i < 0x5E; i++)
            {
                #region Read Character MIPS Offset and Character Data
                int charDataOffs = Util.ReadProcessMemoryInt32(charProgDataTblOffs + (i * 8) + 4);
                charMainAreaOffsets.Add(charDataOffs);

                byte[] charMainAreaBuffer = Util.ReadProcessMemoryBytes(charDataOffs, 0x120);
                charMainAreaList.Add(charMainAreaBuffer);
                #endregion

                #region Read General Char Parameters
                var ninja = PlGen.ReadCharGenPrm(charMainAreaBuffer);
                var clone = (PlGen)ninja.Clone();
                PlGen.CharGenPrm.Add(ninja);
                PlGen.CharGenPrmBkp.Add(clone);
                #endregion
            }
            PlAwk.ReadCharAwkIDList();
            if (GAME.isUN6 == true)
            {
                for (int i = 0; i < GAME.charCount - 0x5D; i++)
                {
                    #region Read Character MIPS Offset and Character Data
                    int charDataOffs = Util.ReadProcessMemoryInt32(0x956100 + (i * 8) + 4);
                    charMainAreaOffsets.Add(charDataOffs);

                    byte[] charMainAreaBuffer = Util.ReadProcessMemoryBytes(charDataOffs, 0x120);
                    charMainAreaList.Add(charMainAreaBuffer);
                    #endregion

                    #region Read General Char Parameters
                    var ninja = PlGen.ReadCharGenPrm(charMainAreaBuffer);
                    var clone = (PlGen)ninja.Clone();
                    PlGen.CharGenPrm.Add(ninja);
                    PlGen.CharGenPrmBkp.Add(clone);
                    #endregion
                }
                PlAwk.ReadCharAwkIDList();
            }
        }

        public static void Clear()
        {
            BTL.charMainAreaOffsets.Clear();
            BTL.charMainAreaList.Clear();
            BTL.charNameList.Clear();
            BTL.charCCSList.Clear();
            BTL.mapNameList.Clear();

            PlGen.CharGenPrm.Clear();
            PlGen.CharGenPrmBkp.Clear();

            PlAtk.CharAtkPrm.Clear();
            PlAtk.CharAtkPrmBkp.Clear();
            PlAtk.comboNameList.Clear();

            PlAnm.PlAnmPrm.Clear();
            PlAnm.PlAnmPrmBkp.Clear();
            PlAnm.PlAnmListName.Clear();

            PlAwk.CharAwkPrm.Clear();
            PlAwk.CharAwkPrmBkp.Clear();
            PlAwk.CharAwkIDList.Clear();
            PlAwk.CharAwkActivationType.Clear();
            PlAwk.CharAwkActivationSound.Clear();

        }


        public static string GetCharCCSName(int selectedIndex)
        {
            while (BTL.charCCSList.Count <= GAME.charCount)
            {
                charCCSList.Add("");
            }
            if (charCCSList[selectedIndex] == "")
            {
                byte[] ccsOffsetBytes = PlGen.CharGenPrm[selectedIndex].CCSOffset;
                int ccsPointer = BitConverter.ToInt32(ccsOffsetBytes, 0);

                charCCSList[selectedIndex] = Util.ReadStringWithOffset(ccsPointer, false);
            }

            return charCCSList[selectedIndex];
        }

        public static string GetMapName(int mapIndex)
        {
            while (mapNameList.Count <= 24)
            {
                mapNameList.Add("");
            }
            if (mapNameList[mapIndex] == "")
            {
                IntPtr processHandle = PCSX2Process.OpenProcess(PCSX2Process.PROCESS_VM_READ, false, PCSX2Process.ID);
                int mapNameAreaPointer = 0x5C7970;

                int mapNameOffs = Util.ReadProcessMemoryInt32(mapNameAreaPointer + (mapIndex * 4));
                string decodedMapName = Util.ReadStringWithOffset(mapNameOffs, false);
                mapNameList[mapIndex] = decodedMapName;
            }

            return mapNameList[mapIndex];
        }

        public static void UpdateMatch(bool isP1, int PlayerID, int MapID)
        {
            int LastLoadingTimeLog = Util.ReadProcessMemoryInt32(Util.ReadProcessMemoryInt32(GAME.Global_Pointer - 0x1F0) + 0x60);
            if (Util.ReadProcessMemoryInt32(Util.ReadProcessMemoryInt32(GAME.Global_Pointer - 0x1D0)) == 0xF &&
                LastLoadingTimeLog != 0 &&
                PlayerID != 0 &&
                PlayerID != GAME.lastSelectedID)
            {
                GAME.lastSelectedID = PlayerID;
                Util.WriteProcessMemoryInt32(isP1 == true ? Util.ReadProcessMemoryInt32(GAME.Global_Pointer - 0x1F0) + 0x50 :
                                                            Util.ReadProcessMemoryInt32(GAME.Global_Pointer - 0x1F0) + 0x78, PlayerID);
                //Update Player

                int oldStageID = Util.ReadProcessMemoryInt8(Util.ReadProcessMemoryInt32(GAME.Global_Pointer - 0x1F0) + 0x98);
                Util.WriteProcessMemoryInt8(Util.ReadProcessMemoryInt32(GAME.Global_Pointer - 0x1F0) + 0x9A, oldStageID);
                //Keep the Current Stage

                Util.WriteProcessMemoryInt32(Util.ReadProcessMemoryInt32(GAME.Global_Pointer - 0x1F0) + 0x60, 0);
                //Reset Loading Time Log

                Util.WriteProcessMemoryInt32(Util.ReadProcessMemoryInt32(GAME.Global_Pointer - 0x1D0), 0x18);
                //Restart Battle
            }
        }

        public static void ReadCharNameTbl(IntPtr processHandle, int charStringTblOffset)
        {
            int charNameTblOffs = Util.ReadProcessMemoryInt32(charStringTblOffset);
            for (int i = 0; i < GAME.charCount; i++)
            {
                int charNameOffs = Util.ReadProcessMemoryInt32(charNameTblOffs + i * 8);
                string charName = Util.ReadStringWithOffset(charNameOffs, false);

                int charFullNameOffs = Util.ReadProcessMemoryInt32(charNameTblOffs + i * 8 + 4);
                string charFullName = Util.ReadStringWithOffset(charFullNameOffs, false);
                charNameList.Add(charFullName);

                if (!GAME.charInvalid.Contains(i))
                {
                    if (i <= 93)
                    {
                        string charValue = i.ToString();

                        charFullName = charValue + ": " + charFullName;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}
