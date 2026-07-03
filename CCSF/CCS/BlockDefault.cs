using System;
using System.IO;
using System.Windows.Forms;

namespace CCSFileExplorerWV
{
	// Token: 0x0200000B RID: 11
	public class BlockDefault : Block
	{
		// Token: 0x06000030 RID: 48 RVA: 0x00002CDD File Offset: 0x00000EDD
		public BlockDefault(uint _type, uint _id, byte[] _data)
		{
			this.BlockID = _type;
			this.ID = _id;
			this.Data = _data;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003564 File Offset: 0x00001764
		public BlockDefault(Stream s)
		{
            this.Size = Block.ReadUInt32(s);
            uint size = this.Size - 1U;
            this.ID = Block.ReadUInt32(s);
            this.Data = new byte[size * 4U];
            s.Read(this.Data, 0, (int)(size * 4U));
        }

		// Token: 0x06000032 RID: 50 RVA: 0x000035B8 File Offset: 0x000017B8
		public override TreeNode ToNode()
		{
			return new TreeNode(string.Concat(new string[]
			{
				this.BlockID.ToString("X8"),
				"ID:0x",
				this.ID.ToString("X"),
				" Size: 0x",
				this.Data.Length.ToString("X")
			}));
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003624 File Offset: 0x00001824
		public override void WriteBlock(Stream s)
		{
			Block.WriteUInt32(s, this.BlockID);
			/*if ((BlockID & 0xFFFF) == 0x0400)
			{
				MemoryStream memoryStream = new MemoryStream(Data);
				byte[] initData = new byte[8];
				memoryStream.Read(initData, 0, 8);
				byte[] finalData = new byte[Data.Length - 0xC];
				memoryStream.Seek(0xC, SeekOrigin.Begin);
				memoryStream.Read(finalData, 0, (int)(memoryStream.Length - 0xC));
				MemoryStream bMS = new MemoryStream();
				BinaryWriter bw = new BinaryWriter(bMS);
				bw.Write(initData);
				bw.Write((ushort)0x3C0);
				bw.Write((ushort)0x1F0);
				bw.Write(finalData);
				Data = bMS.ToArray();
			}*/
            Block.WriteUInt32(s, (uint)(this.Data.Length / 4 + 1));
			Block.WriteUInt32(s, this.ID);
			s.Write(this.Data, 0, this.Data.Length);
		}
	}
}
