using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CCSFileExplorerWV.CCSF;

namespace CCSFileExplorerWV
{
	// Token: 0x0200000A RID: 10
	public class Block0800 : Block
	{
		// Token: 0x06000025 RID: 37 RVA: 0x00002CDD File Offset: 0x00000EDD
		public Block0800(uint _type, uint _id, byte[] _data)
		{
			BlockID = _type;
			ID = _id;
			Data = _data;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002CFC File Offset: 0x00000EFC
		public Block0800(Stream s)
		{
			Size = Block.ReadUInt32(s);
			ID = Block.ReadUInt32(s);
			uint size = Size - 1U;
			long safe = s.Position;
			s.Seek(4, SeekOrigin.Current);
			int type = s.ReadByte();
			int unkFlag = s.ReadByte();
			if(type == 4)
			{
				uint meshCount = (uint)s.ReadByte();
				uint sub = meshCount * 0x3C;
				uint finalSize = size * 4U;
                s.Seek(safe, SeekOrigin.Begin);
				s.Seek(finalSize, SeekOrigin.Current);
                ///
                /// Checks if the Block was already calculated incorrectly in previous versions of CCS File Explorer, if so, it corrects it
                /// by getting the data size correctly and also fixing the block size.
                ///
                byte[] nextBlockID = new byte[4];
				s.Read(nextBlockID, 0, 4);
                s.Seek(safe, SeekOrigin.Begin);
                if (isValidBlockType(BitConverter.ToUInt32(nextBlockID, 0)))
                {
                    Data = new byte[size * 4U];
                    s.Read(Data, 0, (int)(size * 4U));
					Size = (finalSize - sub + 4) / 4;
                }
				else
				{
                    s.Seek(safe, SeekOrigin.Begin);
                    Data = new byte[finalSize - sub];
                    s.Read(Data, 0, (int)(finalSize - sub));
                }
            }
            else
			{
                s.Seek(safe, SeekOrigin.Begin);
                Data = new byte[size * 4U];
                s.Read(Data, 0, (int)(size * 4U));
            }
		}

        // Token: 0x06000027 RID: 39 RVA: 0x00002D78 File Offset: 0x00000F78
        public override TreeNode ToNode()
		{
			return new TreeNode(string.Concat(new string[]
			{
				BlockID.ToString("X8"),
				"ID:0x",
				ID.ToString("X"),
				" Size(bytes): 0x",
				Data.Length.ToString("X")
			}));
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002DE4 File Offset: 0x00000FE4
		public override void WriteBlock(Stream s)
		{
			Block.WriteUInt32(s, BlockID);
            int type = Data[4];
            int unk = Data[5];
			if (type == 4 && Block0001.fileInfo == 0x123)
            {
                uint meshCount = Data[6];
                uint sub = meshCount * 0x3C;
                uint size = (uint)Data.Length + sub;
				byte[] nextBlockID = new byte[4];
				s.Read(nextBlockID, 0, 4);
                Block.WriteUInt32(s, size / 4 + 1);
            }
            else
            {
                uint size = (uint)Data.Length;
                Block.WriteUInt32(s, size / 4 + 1);
            }
            Block.WriteUInt32(s, ID);
            s.Write(Data, 0, Data.Length);
        }

        // Token: 0x06000029 RID: 41 RVA: 0x00002E30 File Offset: 0x00001030
        public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Found " + models.Count.ToString() + " models:");
			int count = 1;
			foreach (Block0800.ModelData mdl in models)
			{
				sb.AppendLine(string.Concat(new string[]
				{
					" Model ",
					count++.ToString(),
					" has ",
					mdl.VertexCount.ToString(),
					" vertices"
				}));
			}
			return sb.ToString();
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002F00 File Offset: 0x00001100
		public void ProcessData()
		{
			models = new List<Block0800.ModelData>();
			MemoryStream i = new MemoryStream();
			Unknown2 = BitConverter.ToUInt32(Data, 0);
			Unknown3 = BitConverter.ToUInt32(Data, 4);
			Unknown3W1 = BitConverter.ToUInt16(Data, 4);
			ModelCount = BitConverter.ToUInt16(Data, 6);
			Unknown4 = BitConverter.ToUInt32(Data, 8);
			Unknown5 = BitConverter.ToUInt32(Data, 12);
			if (Unknown2 != 0U && Unknown3 != 0U)
			{
				if (CCSFile.FileVersion == CCSFile.FileVersionEnum.HACK_GU)
				{
					i.Write(Data, 24, Data.Length - 24);
				}
				else if (CCSFile.FileVersion == CCSFile.FileVersionEnum.IMOQF)
				{
					i.Write(Data, 16, Data.Length - 16);
				}
				i.Seek(0L, SeekOrigin.Begin);
				while (i.Position < i.Length)
				{
					models.Add(new Block0800.ModelData(i, this));
				}
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x0000301C File Offset: 0x0000121C
		public void CopyToScene(int index)
		{
			List<float[]> fl = new List<float[]>();
			foreach (Tristrip t in models[index].Tristrips)
			{
				float[] f = new float[5];
				f[0] = t.V1.X;
				f[1] = t.V1.Y;
				f[2] = t.V1.Z;
				fl.Add(f);
				f = new float[5];
				f[0] = t.V2.X;
				f[1] = t.V2.Y;
				f[2] = t.V2.Z;
				fl.Add(f);
				f = new float[5];
				f[0] = t.V3.X;
				f[1] = t.V3.Y;
				f[2] = t.V3.Z;
				fl.Add(f);
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00003130 File Offset: 0x00001330
		public string SaveModel(int n)
		{
			List<Block0800.ModelData> mdls;
			if (n < 0 || n >= models.Count)
			{
				mdls = models;
			}
			else
			{
				mdls = new Block0800.ModelData[]
				{
					models[n]
				}.ToList<Block0800.ModelData>();
			}
			StringBuilder sb = new StringBuilder();
			sb.Append("#Exported by CCSFileExplorerWV(updated by klokt.valg)\r\n");
			foreach (Block0800.ModelData mdl in mdls)
			{
				sb.AppendFormat("o Object_{0}\r\n", mdl.ModelID);
				sb.AppendFormat("\r\n# {0} vertices\r\n\r\n", mdl.Vertex.Count);
				foreach (Vector3f v in mdl.Vertex)
				{
					sb.AppendFormat("{0}\r\n", v.ToString());
				}
				sb.AppendFormat("\r\n# {0} texture UVs\r\n\r\n", mdl.UVs.Count);
				foreach (Vector2f v2 in mdl.UVs)
				{
					sb.AppendFormat("{0}\r\n", v2.ToString());
				}
				sb.AppendFormat("\r\n# {0} faces\r\n\r\n", mdl.Tristrips.Count);
				foreach (Tristrip v3 in mdl.Tristrips)
				{
					sb.AppendFormat("{0}\r\n", v3.ToString());
				}
			}
			return sb.ToString();
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00003358 File Offset: 0x00001558
		public void SaveModel(int n, string filename)
		{
			File.WriteAllText(filename, SaveModel(n));
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003368 File Offset: 0x00001568
		public List<float[]> ModelToFloats(Block0800.ModelData mdl)
		{
			List<float[]> result = new List<float[]>();
			List<int> strip = new List<int>();
			int pos = 0;
			bool isStart = true;
			while (pos < mdl.TristripsBytes.Count - 1)
			{
				if (isStart)
				{
					strip.Add(pos);
					strip.Add(pos + 1);
					isStart = false;
					pos += 2;
				}
				else if ((byte)(mdl.TristripsBytes[pos] >> 24) == 1)
				{
					isStart = true;
					result.AddRange(stripToList(strip, mdl.VertexBytes));
					strip = new List<int>();
				}
				else
				{
					strip.Add(pos++);
				}
			}
			if (strip.Count != 0)
			{
				result.AddRange(stripToList(strip, mdl.VertexBytes));
			}
			return result;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000340C File Offset: 0x0000160C
		public List<float[]> stripToList(List<int> strip, List<byte[]> vertices)
		{
			List<float[]> result = new List<float[]>();
			for (int i = 0; i < strip.Count - 2; i++)
			{
				int i2;
				int i3;
				if (i % 2 == 0)
				{
					i2 = i + 1;
					i3 = i + 2;
				}
				else
				{
					i2 = i + 2;
					i3 = i + 1;
				}
				float[] v = new float[5];
				for (int j = 0; j < 3; j++)
				{
					v[j] = (float)((int)((sbyte)vertices[strip[i]][j * 2]) & 255) / 256f + (float)((sbyte)vertices[strip[i]][j * 2 + 1]);
				}
				result.Add(v);
				v = new float[5];
				for (int k = 0; k < 3; k++)
				{
					v[k] = (float)((int)((sbyte)vertices[strip[i2]][k * 2]) & 255) / 256f + (float)((sbyte)vertices[strip[i2]][k * 2 + 1]);
				}
				result.Add(v);
				v = new float[5];
				for (int l = 0; l < 3; l++)
				{
					v[l] = (float)((int)((sbyte)vertices[strip[i3]][l * 2]) & 255) / 256f + (float)((sbyte)vertices[strip[i3]][l * 2 + 1]);
				}
				result.Add(v);
			}
			return result;
		}

		// Token: 0x04000013 RID: 19
		public List<Block0800.ModelData> models;

		// Token: 0x04000014 RID: 20
		public uint Unknown2;

		// Token: 0x04000015 RID: 21
		public uint Unknown3;

		// Token: 0x04000016 RID: 22
		public ushort Unknown3W1;

		// Token: 0x04000017 RID: 23
		public ushort ModelCount;

		// Token: 0x04000018 RID: 24
		public uint Unknown4;

		// Token: 0x04000019 RID: 25
		public uint Unknown5;

		// Token: 0x0400001A RID: 26
		public uint Unknown6;

		// Token: 0x0400001B RID: 27
		public uint Unknown7;

		// Token: 0x0200003E RID: 62
		public class ModelData
		{
			// Token: 0x0600011E RID: 286 RVA: 0x0000BC74 File Offset: 0x00009E74
			public ModelData(Stream s, Block0800 OwnerBlock)
			{
				int FormatVariant = 0;
				OwnerBlock = OwnerBlock;
				ModelID = Block.ReadUInt32(s);
				Unk2 = Block.ReadUInt32(s);
				VertexCount = Block.ReadUInt32(s);
				if (OwnerBlock.ModelCount > 1)
				{
					if (OwnerBlock.Unknown3W1 > 0)
					{
						if (VertexCount == 0U)
						{
							FormatVariant = 1;
							VertexCount = Unk2;
							Unk2 = Block.ReadUInt32(s);
						}
						else if (OwnerBlock.Unknown3W1 > 255)
						{
							FormatVariant = 4;
						}
						else
						{
							FormatVariant = 2;
						}
					}
					else
					{
						FormatVariant = 3;
					}
				}
				VertexBytes = new List<byte[]>();
				Vertex = new List<Vector3f>();
				long len = s.Length;
				int i = 0;
				while ((long)i < (long)((ulong)VertexCount))
				{
					byte[] buff = new byte[6];
					s.Read(buff, 0, 6);
					VertexBytes.Add(buff);
					Vertex.Add(new Vector3f(buff, i + 1));
					if (s.Position >= len)
					{
						VertexCount = 0U;
						VertexBytes = new List<byte[]>();
						TristripsBytes = new List<uint>();
						UnknownData = new List<uint>();
						UVBytes = new List<byte[]>();
						Vertex = new List<Vector3f>();
						UVs = new List<Vector2f>();
						Tristrips = new List<Tristrip>();
						return;
					}
					if (FormatVariant == 2)
					{
						buff = new byte[2];
						s.Read(buff, 0, 2);
					}
					i++;
				}
				if (6U * VertexCount % 4U > 0U)
				{
					s.Seek((long)((ulong)(6U * VertexCount % 4U)), SeekOrigin.Current);
				}
				TristripsBytes = new List<uint>();
				Tristrips = new List<Tristrip>();
				List<VertexConnection> ft = new List<VertexConnection>();
				int j = 0;
				while ((long)j < (long)((ulong)VertexCount))
				{
					uint v = Block.ReadUInt32(s);
					TristripsBytes.Add(v);
					ft.Add(new VertexConnection(v));
					j++;
				}
				bool started = false;
				int k = 0;
				int size = 0;
				while (k < ft.Count)
				{
					if (started && ft[k].Connect == 0)
					{
						size++;
					}
					else if (started && ft[k].Connect != 0)
					{
						started = false;
						resolveTristrip(k - size, size, (int)ft[k - size].Connect);
						size = 0;
					}
					if (ft[k].Connect != 0 && !started)
					{
						started = true;
						size += 2;
						k++;
					}
					if (k == ft.Count - 1)
					{
						resolveTristrip(k - size + 1, size, (int)ft[k - size + 1].Connect);
					}
					k++;
				}
				if (FormatVariant == 0 || FormatVariant == 3)
				{
					UnknownData = new List<uint>();
					int l = 0;
					while ((long)l < (long)((ulong)VertexCount))
					{
						UnknownData.Add(Block.ReadUInt32(s));
						l++;
					}
				}
				UVBytes = new List<byte[]>();
				UVs = new List<Vector2f>();
				uint UVCount = (FormatVariant == 2) ? Unk2 : VertexCount;
				int m = 0;
				while ((long)m < (long)((ulong)UVCount))
				{
					byte[] buff = new byte[4];
					s.Read(buff, 0, 4);
					UVBytes.Add(buff);
					UVs.Add(new Vector2f(buff, m + 1));
					m++;
				}
			}

			// Token: 0x0600011F RID: 287 RVA: 0x0000BFC0 File Offset: 0x0000A1C0
			private void resolveTristrip(int start, int size, int type)
			{
				for (int i = 0; i < size - 2; i++)
				{
					if (type != 1)
					{
						if (type != 2)
						{
							return;
						}
						if (i % 2 == 1)
						{
							Tristrips.Add(new Tristrip(Vertex[start + i], Vertex[start + i + 1], Vertex[start + i + 2]));
						}
						else
						{
							Tristrips.Add(new Tristrip(Vertex[start + i + 1], Vertex[start + i], Vertex[start + i + 2]));
						}
					}
					else if (i % 2 == 1)
					{
						Tristrips.Add(new Tristrip(Vertex[start + i + 1], Vertex[start + i], Vertex[start + i + 2]));
					}
					else
					{
						Tristrips.Add(new Tristrip(Vertex[start + i], Vertex[start + i + 1], Vertex[start + i + 2]));
					}
				}
			}

			// Token: 0x040000D2 RID: 210
			public Block0800 OwnerBlock;

			// Token: 0x040000D3 RID: 211
			public uint ModelID;

			// Token: 0x040000D4 RID: 212
			public uint Unk2;

			// Token: 0x040000D5 RID: 213
			public uint VertexCount;

			// Token: 0x040000D6 RID: 214
			public List<byte[]> VertexBytes;

			// Token: 0x040000D7 RID: 215
			public List<uint> TristripsBytes;

			// Token: 0x040000D8 RID: 216
			public List<uint> UnknownData;

			// Token: 0x040000D9 RID: 217
			public List<byte[]> UVBytes;

			// Token: 0x040000DA RID: 218
			public List<Vector3f> Vertex;

			// Token: 0x040000DB RID: 219
			public List<Vector2f> UVs;

			// Token: 0x040000DC RID: 220
			public List<Tristrip> Tristrips;
		}
	}
}
