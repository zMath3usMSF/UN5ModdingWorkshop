using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WindowsFormsApp1;

namespace UN5ModdingWorkshop
{
    public static class Util
    {
        public static byte[] ReadProcessMemoryBytes(int address, int length)
        {
            byte[] buffer = new byte[length];

            PCSX2Process.ReadProcessMemory(PCSX2Process.processHandle, ToPointer(address), buffer, buffer.Length, out var none);
            return buffer;
        }
        public static int ReadProcessMemoryInt8(int address)
        {
            byte[] buffer = new byte[1];

            PCSX2Process.ReadProcessMemory(PCSX2Process.processHandle, ToPointer(address), buffer, buffer.Length, out var none);
            return buffer[0];
        }
        public static int ReadProcessMemoryInt16(int address)
        {
            byte[] buffer = new byte[2];

            PCSX2Process.ReadProcessMemory(PCSX2Process.processHandle, ToPointer(address), buffer, 2, out var none);
            return BitConverter.ToInt16(buffer, 0);
        }
        public static int ReadProcessMemoryInt32(int address)
        {
            byte[] buffer = new byte[4];

            PCSX2Process.ReadProcessMemory(PCSX2Process.processHandle, ToPointer(address), buffer, 4, out var none);
            return BitConverter.ToInt32(buffer, 0);
        }
        public static float ReadProcessMemoryFloat(int address)
        {
            byte[] buffer = new byte[4];

            PCSX2Process.ReadProcessMemory(PCSX2Process.processHandle, ToPointer(address), buffer, 4, out var none);
            return BitConverter.ToSingle(buffer, 0);
        }
        public static void WriteProcessMemoryInt32(int address, int value)
        {
            byte[] buffer = BitConverter.GetBytes(Convert.ToInt32(value));

            PCSX2Process.WriteProcessMemory(PCSX2Process.processHandle, ToPointer(address), buffer, 4, out var none);
        }
        public static void WriteProcessMemoryInt16(int address, int value)
        {
            byte[] buffer = BitConverter.GetBytes(Convert.ToInt16(value));

            PCSX2Process.WriteProcessMemory(PCSX2Process.processHandle, ToPointer(address), buffer, 2, out var none);
        }

        public static void WriteProcessMemoryInt8(int address, int value)
        {
            byte[] buffer = BitConverter.GetBytes(Convert.ToByte(value));

            PCSX2Process.WriteProcessMemory(PCSX2Process.processHandle, ToPointer(address), buffer, 1, out var none);
        }

        public static void WriteProcessMemoryBytes(int address, byte[] value)
        {
            PCSX2Process.WriteProcessMemory(PCSX2Process.processHandle, ToPointer(address), value, (uint)value.Length, out var none);
        }
        public static IntPtr ToPointer(int value)
        {
            return (IntPtr)(GAME.eeAddress + (ulong)value);
        }
        public static string ReadStringWithOffset(int basePointer, bool encShift)
        {
            List<byte> stringBytes = new List<byte>();

            while (true)
            {
                int currentByte = ReadProcessMemoryInt8(basePointer);
                if (currentByte == 0) break;
                stringBytes.Add((byte)currentByte);
                basePointer += 1;
            }
            string decodedString = "";
            if(encShift == true)
            {
                decodedString = Encoding.GetEncoding("shift-jis").GetString(stringBytes.ToArray());
            }
            else
            {
                decodedString = Encoding.GetEncoding("iso-8859-1").GetString(stringBytes.ToArray());
            }
            return decodedString;
        }
        public static string ReadFixedLenString(BinaryReader br, int len, char t)
        {
            List<byte> fileStrBytes = new List<byte>();
            while (fileStrBytes.Count != len)
            {
                byte b = br.ReadByte();
                if (b == t || b == 0)
                {
                    br.BaseStream.Position--;
                    break;
                }
                fileStrBytes.Add(b);
            }
            if (len >= 0) br.BaseStream.Position += len - fileStrBytes.Count;
            return Encoding.GetEncoding("shift-jis").GetString(fileStrBytes.ToArray());
        }

        public static void WriteFixedLenString(BinaryWriter bw, string value, int len)
        {
            byte[] bytes = Encoding.GetEncoding("shift-jis").GetBytes(value ?? string.Empty);
            if (bytes.Length > len)
                Array.Resize(ref bytes, len);

            bw.Write(bytes);
            for (int i = bytes.Length; i < len; i++)
                bw.Write((byte)0);
        }
        public static string ReadMemoryFixedLenString(int basePointer, int len, char t)
        {
            List<byte> fileStrBytes = new List<byte>();
            while (fileStrBytes.Count != len)
            {
                byte b = (byte)(ReadProcessMemoryInt8(basePointer));
                basePointer += 1;
                if (b == t || b == 0)
                {
                    break;
                }
                fileStrBytes.Add(b);
            }
            return Encoding.GetEncoding("iso-8859-1").GetString(fileStrBytes.ToArray());
        }

        public static void WriteMemoryFixedLenString(int basePointer, string value, int len)
        {
            byte[] bytes = Encoding.GetEncoding("shift-jis").GetBytes(value ?? string.Empty);
            if (bytes.Length > len)
                Array.Resize(ref bytes, len);

            for (int i = 0; i < len; i++)
            {
                WriteProcessMemoryInt8(basePointer + i, bytes[i]);
            }
        }

        public static void ReadStringList(List<string> list, int offset, int stringLength)
        {
            for (int i = 0; ; i++)
            {
                string value = Util.ReadMemoryFixedLenString(
                    offset + (i * stringLength),
                    stringLength,
                    '\0'
                );

                if (value == "")
                    break;

                list.Add(value);
            }
        }

        public static void AlignBinaryWriter(BinaryWriter bw, int value)
        {
            while(bw.BaseStream.Position % value != 0)
            {
                bw.Write((byte)0);
            }
        }

        public static void VerifyCurrentPlayersIDs()
        {
            int P1Offset = ReadProcessMemoryInt32(GAME.Global_Pointer - 0x1F0) + 0x4C;
            BTL.P1ID = ReadProcessMemoryInt32(P1Offset);
        }
        public static byte FormarByte(int[] bits)
        {
            byte resultado = 0;
            for (int i = 0; i < 8; i++)
            {
                //Definindo o bit na posição i de acordo com o valor na posição i do array bits
                resultado |= (byte)(bits[i] << i);
            }
            return resultado;
        }
        public static int BTL_GetPlayer1MemoryOffs()
        {
            int btlManagerOffs = ReadProcessMemoryInt32(GAME.Global_Pointer - 0x1F0);
            int Player1MemoryOffset = ReadProcessMemoryInt32(btlManagerOffs + 0xDE4);
            return Player1MemoryOffset;
        }

        public static string GetCurrentGameMode()
        {
            int btlManagerOffs = ReadProcessMemoryInt32(GAME.Global_Pointer - 0x1F0);
            int currentGameModeID = ReadProcessMemoryInt32(btlManagerOffs + 0xC);
            switch(currentGameModeID)
            {
                case 1:
                    return "Minigame";
                case 2:
                    return "Free Battle";
                case 3:
                    return "Pratice";
                case 4:
                    return "Master";
                case 5:
                    return "Shop";
                case 6:
                    return "Collection";
                case 7:
                    return "Options";
                default:
                    return "None";
            }
        }

        public static bool IsStartedBattle()
        {
            int LastLoadingTimeLog = Util.ReadProcessMemoryInt32(Util.ReadProcessMemoryInt32(GAME.Global_Pointer - 0x1F0) + 0x60);
            if(Util.ReadProcessMemoryInt32(Util.ReadProcessMemoryInt32(GAME.Global_Pointer - 0x1D0)) == 0xF)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
