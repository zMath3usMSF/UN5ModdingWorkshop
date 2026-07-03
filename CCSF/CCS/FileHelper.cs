using ICSharpCode.SharpZipLib.GZip;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CCSFileExplorerWV
{
	// Token: 0x02000013 RID: 19
	public static class FileHelper
	{
		// Token: 0x06000057 RID: 87 RVA: 0x00004579 File Offset: 0x00002779
		public static bool isGzipMagic(byte[] data, int start = 0)
		{
			return data[start++] == 31 && data[start++] == 139 && data[start++] == 8 && (data[start] == 8 || data[start] == 0);
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000045B0 File Offset: 0x000027B0
		public static byte[] unzipArray(byte[] data)
		{
            MemoryStream ms = new MemoryStream();
            GZipStream gzipStream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress);
            gzipStream.CopyTo(ms);
			return ms.ToArray();
		}

        // Token: 0x06000059 RID: 89 RVA: 0x000045E4 File Offset: 0x000027E4
        public static byte[] zipArray(byte[] data, string filename)
        {
            MemoryStream ms = new MemoryStream();
            GZipOutputStream gs = new GZipOutputStream(ms);
            gs.SetLevel(0);
            gs.FileName = filename + ".tmp";
            gs.Write(data, 0, data.Length);
            gs.Close();
            return ms.ToArray();
        }
    }
}
