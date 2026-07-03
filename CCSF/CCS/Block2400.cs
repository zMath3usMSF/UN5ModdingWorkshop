using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CCSFileExplorerWV
{
    public class Block2400 : Block
    {
        public string scriptType = "";
        public List<string> funcs = new List<string>();
        public byte[] textData;

        public Block2400(uint _type, uint _id, byte[] _data)
        {
            BlockID = _type;
            ID = _id;
            Data = _data;
        }

        public Block2400(Stream s)
        {
            Size = Block.ReadUInt32(s);
            uint size = Size - 1U;
            ID = Block.ReadUInt32(s);
            Data = new byte[size * 4U];
            s.Read(Data, 0, (int)(size * 4U));
        }

        public override TreeNode ToNode()
        {
            return new TreeNode(string.Concat(new string[]
            {
                BlockID.ToString("X8"),
                "ID:0x",
                ID.ToString("X"),
                " Size: 0x",
                Data.Length.ToString("X")
            }));
        }

        public override void WriteBlock(Stream s)
        {
            Block.WriteUInt32(s, BlockID);
            Block.WriteUInt32(s, (uint)(Data.Length / 4 + 1));
            Block.WriteUInt32(s, ID);
            s.Write(Data, 0, Data.Length);
        }

        public string CheckScriptType(byte[] blockData)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(blockData));
            string scriptMagic = ReadFixedLenString(br, 4, '\0');
            string scriptType;
            switch (scriptMagic)
            {
                case "EVNA":
                    ReadPuppet(blockData);
                    scriptType = "puppet";
                    break;
                case "EOLD":
                    scriptType = "unknown";
                    break;
                default:
                    scriptType = "unknown";
                    break;
            }
            return scriptType;
        }

        public void ReadPuppet(byte[] blockData)
        {
            if (ReadFixedLenString(new BinaryReader(new MemoryStream(blockData)), 0x20, ';') == "EVNAME")
            {
                scriptType = "PUPPET";
                BinaryReader br = new BinaryReader(new MemoryStream(blockData));
                StringBuilder sb = new StringBuilder();
                while (br.BaseStream.Position != br.BaseStream.Length)
                {
                    string currentLine = ReadFixedLenString(br, -1, ',');
                    funcs.Add(currentLine);
                    sb.Insert(sb.Length, currentLine + "\n");
                }
            }
        }

        public byte[] WritePuppet(string[] funcsLines)
        {
            MemoryStream msPpt = new MemoryStream();
            BinaryWriter bwPpt = new BinaryWriter(msPpt);
            MemoryStream msTxt = new MemoryStream();
            BinaryWriter bwTxt = new BinaryWriter(msTxt);
            int txtCount = funcsLines.Count(s => s.Contains("DrawSubMsgButtom"));
            bwTxt.Write(Encoding.GetEncoding("shift-jis").GetBytes("TEXT"));
            bwTxt.Write((UInt32)txtCount);
            bwTxt.Write((UInt64)0);
            for (int i = 0; i < funcsLines.Length; i++)
            {
                if (funcsLines[i] != "")
                {
                    bwPpt.Write(Encoding.GetEncoding("shift-jis").GetBytes(funcsLines[i] + ','));
                    if (funcsLines[i].Contains("DrawSubMsgButtom") || funcsLines[i].Contains("DrawCenterMsgButtom"))
                    {
                        string[] parts = funcsLines[i].Split(';');
                        bwTxt.Write(Encoding.GetEncoding("ISO-8859-1").GetBytes(parts[1] + '\0'));
                    }
                }
            }
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path = Path.Combine(path, "ppt.bin");
            while (bwPpt.BaseStream.Length % 4 != 0)
            {
                bwPpt.Write((byte)0);
            }
            while (bwTxt.BaseStream.Length % 4 != 0)
            {
                bwTxt.Write((byte)0);
            }
            File.WriteAllBytes(path, msTxt.ToArray());
            textData = msTxt.ToArray();
            return msPpt.ToArray();
        }
    }
}
