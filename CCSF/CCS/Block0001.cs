using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CCSFileExplorerWV
{
	// Token: 0x02000006 RID: 6
	public class Block0001 : Block
	{
		public static int fileInfo = 0;

        public override Block Clone()
        {
            var clone = (Block0001)base.Clone();
            clone.Name = string.Copy(this.Name);
            return clone;
        }

        public Block0001(Stream s)
		{
			Size = Block.ReadUInt32(s);
			ID = Block.ReadUInt32(s);
			uint size = Size - 1U;
			Data = new byte[size * 4U];
			s.Read(Data, 0, (int)(size * 4U));
            BinaryReader dStream = new BinaryReader(new MemoryStream(Data));
            Name = Block.ReadFixedLenString(dStream, 0x20, '\0');
			Unknown = new List<uint>();
			fileInfo = dStream.ReadInt32();
			int i = 0;
			while ((long)i < (long)((ulong)(size - 8U)))
			{
				Unknown.Add(BitConverter.ToUInt32(Data, i * 4 + 32));
				i++;
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002694 File Offset: 0x00000894
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

		// Token: 0x06000019 RID: 25 RVA: 0x00002700 File Offset: 0x00000900
		public override void WriteBlock(Stream s)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(memoryStream);
            bw.Write(Encoding.UTF8.GetBytes(Name));
			int count = 0x20 - Encoding.UTF8.GetBytes(Name).Length;
            for(int i = 0; i < count; i++)
			{
				bw.Write((byte)0);
			}
			for(int i = 0; i < Unknown.Count; i++)
			{
				bw.Write((UInt32)Unknown[i]);
			}
			Data = memoryStream.ToArray();
            Block.WriteUInt32(s, BlockID);
			Block.WriteUInt32(s, (uint)(Data.Length / 4 + 1));
			Block.WriteUInt32(s, ID);
			s.Write(Data, 0, Data.Length);
		}

		// Token: 0x0400000C RID: 12
		public string Name;

		// Token: 0x0400000D RID: 13
		public List<uint> Unknown;
	}
}
