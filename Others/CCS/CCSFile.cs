using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace CCSFileExplorerWV
{
	// Token: 0x0200000E RID: 14
	public class CCSFile
	{
        // Token: 0x06000040 RID: 64 RVA: 0x0000377D File Offset: 0x0000197D
        public CCSFile Clone()
        {
            var clone = new CCSFile(new byte[0], FileVersion = FileVersionEnum.HACK_GU)
            {
                raw = (byte[])this.raw.Clone(),
                FileVersion = this.FileVersion,
                isGzip = this.isGzip,
                header = (Block0001)this.header.Clone(),
                toc = (Block0002)this.toc.Clone(),
                blocks = this.blocks.Select(b => b.Clone()).ToList()
            };

            // Você pode precisar reconstruir `files` dependendo do uso
            // mas `Reload()` pode resolver isso:
            clone.Rebuild(); // opcional
            clone.Reload();  // para reanalisar os blocos e popular `files`
            return clone;
        }

        public CCSFile(byte[] rawBuffer, CCSFile.FileVersionEnum FileVersion)
		{
			raw = rawBuffer;
			FileVersion = FileVersion;
			Reload();
		}

		// Token: 0x06000041 RID: 65 RVA: 0x0000379C File Offset: 0x0000199C
		public void Reload()
		{
			isvalid = false;
			MemoryStream i = new MemoryStream(raw);
			i.Seek(0L, SeekOrigin.Begin);
			blocks = new List<Block>();
			while (i.Position < (long)raw.Length)
			{
				Block b = Block.ReadBlock(i);
				b.CCSFile = this;
				blocks.Add(b);
			}
			if (File.Exists("blocks.txt"))
			{
				File.Delete("blocks.txt");
			}
			StreamWriter sw = new StreamWriter("blocks.txt");
			foreach (Block b2 in blocks)
			{
				sw.WriteLine(b2.BlockID.ToString("X2") + "\n");
			}
			sw.Close();
			if (blocks.Count == 0)
			{
				return;
			}
			isvalid = true;
			header = (Block0001)blocks[0];
			toc = (Block0002)blocks[1];
			files = new List<FileEntry>();
			int j = 0;
			while ((long)j < (long)((ulong)toc.FileCount))
			{
				FileEntry entry = new FileEntry(toc.filenames[j]);
				int k = 0;
				while ((long)k < (long)((ulong)toc.ObjCount))
				{
					if ((int)(toc.indexes[k]) == j)
					{
						ObjectEntry obj = new ObjectEntry(toc.objnames[k]);
						for (int l = 0; l < blocks.Count; l++)
						{
							if ((ulong)(blocks[l].ID) == (ulong)((long)k))
							{
								obj.blocks.Add(blocks[l]);
								blocks[l].ObjectEntry = obj;
								blocks[l].FileEntry = entry;
							}
						}
						entry.objects.Add(obj);
					}
					k++;
				}
				files.Add(entry);
				j++;
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000039F8 File Offset: 0x00001BF8
		public void Rebuild()
		{
			MemoryStream i = new MemoryStream();
			foreach (Block block in blocks)
			{
				block.WriteBlock(i);
			}
			raw = isGzip == true ? FileHelper.zipArray(i.ToArray(), header.Name) : i.ToArray();
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003A5C File Offset: 0x00001C5C
		public void Save(string filename)
		{
			Rebuild();
			File.WriteAllBytes(filename, raw);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00003A70 File Offset: 0x00001C70
		public string Info()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Arquivo Válido : " + isvalid.ToString());
			if (isvalid)
			{
				sb.AppendLine(files.Count.ToString() + " files loaded");
			}
			return sb.ToString();
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003AD4 File Offset: 0x00001CD4
		public static Bitmap CreateImage(byte[] palette, byte[] data)
		{
			List<Color> pal = new List<Color>();
			int pos;
			for (pos = 16; pos < palette.Length; pos += 4)
			{
				byte r = palette[pos];
				byte g = palette[pos + 1];
				byte b = palette[pos + 2];
				byte a = palette[pos + 3];
				if (a <= 128)
				{
					a = (byte)(a * byte.MaxValue / 128);
				}
				pal.Add(Color.FromArgb((int)a, (int)r, (int)g, (int)b));
			}
			int sizeX = (int)Math.Pow(2.0, (double)data[12]);
			int sizeY = (int)Math.Pow(2.0, (double)data[13]);
			pos = 24;
			int dataSize = data.Length - pos;
			Bitmap result = new Bitmap(sizeX, sizeY);
			if (dataSize * 2 == sizeX * sizeY)
			{
				for (int y = 0; y < sizeY; y++)
				{
					for (int x = 0; x < sizeX / 2; x++)
					{
						result.SetPixel(x * 2, sizeY - y - 1, pal[(int)(data[pos] % 16)]);
						result.SetPixel(x * 2 + 1, sizeY - y - 1, pal[(int)(data[pos] / 16)]);
						pos++;
					}
				}
			}
			else if (dataSize >= sizeX * sizeY)
			{
				for (int y2 = 0; y2 < sizeY; y2++)
				{
					for (int x2 = 0; x2 < sizeX; x2++)
					{
						result.SetPixel(x2, sizeY - y2 - 1, pal[(int)data[pos++]]);
					}
				}
			}
			return result;
		}

        public static Bitmap GetCCSImage(CCSFile ccs, string fileName)
        {
            byte[] pallete = new byte[0];
            byte[] texture = new byte[0];
            for (int i = 0; i < ccs.files.Count; i++)
            {
                if (ccs.files[i].name.Contains(fileName) == true)
                {
                    for (int j = 0; j < ccs.files[i].objects.Count; j++)
                    {
                        if (ccs.files[i].objects[j].blocks[0].BlockID == 0xCCCC0400)
                        {
                            pallete = ccs.files[i].objects[j].blocks[0].Data;
                        }
                        if (ccs.files[i].objects[j].blocks[0].BlockID == 0xCCCC0300)
                        {
                            texture = ccs.files[i].objects[j].blocks[0].Data;
                        }
                    }
                }
            }
            return CCSFile.CreateImage(pallete, texture);
        }

        // Token: 0x04000021 RID: 33
        public CCSFile.FileVersionEnum FileVersion;

		// Token: 0x04000022 RID: 34
		public byte[] raw;

		// Token: 0x04000023 RID: 35
		public bool isvalid;

        public bool isGzip;

        // Token: 0x04000024 RID: 36
        public Block0001 header;

		// Token: 0x04000025 RID: 37
		public Block0002 toc;

		// Token: 0x04000026 RID: 38
		public List<FileEntry> files;

		// Token: 0x04000027 RID: 39
		public List<Block> blocks;

		// Token: 0x0200003F RID: 63
		public enum FileVersionEnum
		{
			// Token: 0x040000DE RID: 222
			HACK_GU,
			// Token: 0x040000DF RID: 223
			IMOQF
		}
	}
}
