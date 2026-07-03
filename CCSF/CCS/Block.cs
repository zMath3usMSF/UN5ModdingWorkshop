using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CCSFileExplorerWV
{
	// Token: 0x02000005 RID: 5
	public abstract class Block
	{
        public virtual Block Clone()
        {
            return (Block)this.MemberwiseClone();
        }

        public virtual byte[] FullBlockData
		{
			get
			{
				MemoryStream memoryStream = new MemoryStream();
				memoryStream.Write(BitConverter.GetBytes(this.BlockID), 0, 4);
				memoryStream.Write(BitConverter.GetBytes(this.Size), 0, 4);
				memoryStream.Write(BitConverter.GetBytes(this.ID), 0, 4);
				memoryStream.Write(this.Data, 0, this.Data.Length);
				return memoryStream.ToArray();
			}
		}
        public static Dictionary<string, uint> blocksTypes = new Dictionary<string, uint>()
		{
			{ "Header", 0x0001 },
			{ "IndexTable", 0x0002 },
			{ "Setup", 0x0003 },
			{ "Stream", 0x0005 },
			{ "Object", 0x0100 },
			{ "ObjectFrame", 0x0101 },
			{ "ObjectController", 0x0102 },
			{ "NoteFrame", 0x0108 },
			{ "Material", 0x0200 },
			{ "MaterialFrame", 0x0201 },
			{ "MaterialController", 0x0202 },
			{ "Texture", 0x0300 },
			{ "Clut", 0x0400 },
			{ "Camera", 0x0500 },
			{ "CameraFrame", 0x0502 },
			{ "CameraController", 0x0503 },
			{ "Light", 0x0600 },
			{ "AmbientFrame", 0x0601 },
			{ "DistantLightFrame", 0x0602 },
			{ "DistantLightController", 0x0603 },
			{ "DirectLightFrame", 0x0604 },
			{ "DirectLightController", 0x0605 },
			{ "SpotLightFrame", 0x0606 },
			{ "SpotLightController", 0x0607 },
			{ "OmniLightFrame", 0x0608 },
			{ "OmniLightController", 0x0609 },
			{ "Animation", 0x0700 },
			{ "Model", 0x0800 },
			{ "ModelVertexFrame", 0x0802 },
			{ "ModelNormalFrame", 0x0803 },
			{ "Clump", 0x0900 },
			{ "ExternalObject", 0x0a00 },
			{ "HitModel", 0x0b00 },
			{ "BoundingBox", 0x0c00 },
			{ "Effect", 0x0e00 },
			{ "Particle", 0x0d00 },
			{ "ParticleAnmCtrl", 0x0d80 },
			{ "ParticleGenerator", 0x0d90 },
			{ "Blit_Group", 0x1000 },
			{ "FrameBuffer_Page", 0x1100 },
			{ "FrameBuffer_Rect", 0x1200 },
			{ "DummyPosition", 0x1300 },
			{ "DummyPositionRotation", 0x1400 },
			{ "Layer", 0x1700 },
			{ "Shadow", 0x1800 },
			{ "ShadowFrame", 0x1801 },
			{ "Morph", 0x1900 },
			{ "MorphFrame", 0x1901 },
			{ "MorphController", 0x1902 },
			{ "StreamOutlineParam", 0x1a00 },
			{ "OutlineFrame", 0x1a01 },
			{ "StreamCelShadeParam", 0x1b00 },
			{ "CelShadeFrame", 0x1b01 },
			{ "StreamToneShadeParam", 0x1c00 },
			{ "ToneShadeFrame", 0x1c01 },
			{ "StreamFBSBlurParam", 0x1d00 },
			{ "FBSBlurFrame", 0x1d01 },
			{ "Sprite2Tbl", 0x1f00 },
			{ "AnimationObject", 0x2000 },
			{ "PCM_Audio", 0x2200 },
			{ "PCMFrame", 0x2201 },
			{ "Dynamics", 0x2300 },
			{ "Binary_Blob", 0x2400 },
			{ "SPD", 0x2700 },
			{ "Frame", 0xff01 }
		};
		public static bool issValidBlockType(byte[] buffer)
		{
			bool isValidBlockType = false;
			uint type = BitConverter.ToUInt16(buffer, 0);
			uint magic = BitConverter.ToUInt16(buffer, 2);
			if(blocksTypes.ContainsValue(type) == true && magic == 0xCCCC)
			{
				isValidBlockType = true;
			}

			return isValidBlockType;
		}
        // Token: 0x0600000C RID: 12
        public abstract TreeNode ToNode();

		// Token: 0x0600000D RID: 13
		public abstract void WriteBlock(Stream s);

		// Token: 0x0600000E RID: 14 RVA: 0x00002418 File Offset: 0x00000618
		public static Block ReadBlock(Stream s)
		{
			uint type = Block.ReadUInt32(s) & 0xFFFF;
			Block result;
			switch (type)
			{
			case 0x0001:
				result = new Block0001(s);
				goto IL_6F;
			case 0x0002:
				result = new Block0002(s);
				goto IL_6F;
			case 0x0003:
			case 0x0004:
				break;
			case 0x0005:
				result = new Block0005(s);
				goto IL_6F;
            case 0x2400:
                result = new Block2400(s);
                goto IL_6F;
            default:
			if (type == 0x0300)
			{
				result = new Block0300(s);
				goto IL_6F;
			}
			if (type == 0x0800)
			{
				result = new Block0800(s);
				goto IL_6F;
			}
			break;
			}
			result = new BlockDefault(s);
			IL_6F:
			result.BlockID = (uint)((0xCCCC << 16) | type);
            return result;
		}
        public static Block ReadBlockFile(Stream s)
        {
            uint type = Block.ReadUInt32(s);
            Block result;
            switch (type)
            {
                case 3435921409U:
                    result = new Block0001(s);
                    goto IL_6F;
                case 3435921410U:
                    result = new Block0002(s);
                    goto IL_6F;
                case 3435923456U:
					result = new Block0800(s);
                    goto IL_6F;
                case 3435921411U:
                case 3435921412U:
                    break;
                case 3435921413U:
                    result = new Block0005(s);
                    goto IL_6F;
                default:
                    if (type == 3435922176U)
                    {
                        result = new Block0300(s);
                        goto IL_6F;
                    }
                    break;
            }
            result = new BlockDefault(s);
IL_6F:
			result.BlockID = type;
            return result;
        }
        // Token: 0x0600000F RID: 15 RVA: 0x0000249C File Offset: 0x0000069C
        public static uint ReadUInt32(Stream s)
		{
			byte[] buff = new byte[4];
			s.Read(buff, 0, 4);
			return BitConverter.ToUInt32(buff, 0);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000024C4 File Offset: 0x000006C4
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
            if(len >= 0) br.BaseStream.Position += len - fileStrBytes.Count;
            return Encoding.GetEncoding("shift-jis").GetString(fileStrBytes.ToArray());
		}

        // Token: 0x06000012 RID: 18 RVA: 0x00002532 File Offset: 0x00000732
        public static void WriteUInt32(Stream s, uint u)
		{
			s.Write(BitConverter.GetBytes(u), 0, 4);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002544 File Offset: 0x00000744
		public static void WriteString(Stream s, string t, int minsize = -1)
		{
			MemoryStream i = new MemoryStream();
			byte[] str = Encoding.GetEncoding("shift-jis").GetBytes(t);
            i.Write(str, 0, str.Length);

            if (str.Length != minsize)
			{
                i.WriteByte(0);
                if (minsize != -1)
                {
                    while (i.Length != (long)minsize)
                    {
                        i.WriteByte(0);
                    }
                }
            }
			s.Write(i.ToArray(), 0, (int)i.Length);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000025AC File Offset: 0x000007AC
		public static bool isValidBlockType(uint u)
		{
			foreach (uint vu in Block.validBlockTypes)
			{
				if (u == vu)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000004 RID: 4
		public uint BlockID;

		// Token: 0x04000005 RID: 5
		public uint Size;

		// Token: 0x04000006 RID: 6
		public uint ID;

		// Token: 0x04000007 RID: 7
		public byte[] Data;

		// Token: 0x04000008 RID: 8
		public CCSFile CCSFile;

		// Token: 0x04000009 RID: 9
		public FileEntry FileEntry;

		// Token: 0x0400000A RID: 10
		public ObjectEntry ObjectEntry;

		// Token: 0x0400000B RID: 11
		public static uint[] validBlockTypes = new uint[]
		{
			3435921409U,
			3435921410U,
			3435921413U,
			3435921664U,
			3435921666U,
			3435921672U,
			3435921920U,
			3435922176U,
			3435922432U,
			3435922688U,
			3435922690U,
			3435922944U,
			3435922945U,
			3435922947U,
			3435922953U,
			3435923200U,
			3435923456U,
			3435923712U,
			3435923968U,
			3435924224U,
			3435924480U,
			3435924992U,
			3435925760U,
			3435926016U,
			3435926272U,
			3435926528U,
			3435927808U,
			3435927809U,
			3435929600U,
			3435986689U,
			3435921922U
		};
	}
}
