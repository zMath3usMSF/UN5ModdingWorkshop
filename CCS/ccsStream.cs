using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UN5ModdingWorkshop
{
    internal class ccsStream : ccsChunk
    {
        public override void Read(BinaryReader br)
        {
            uint size = br.ReadUInt32();
            rawBody = br.ReadBytes((int)(size * 4));
        }

        public override void Write(BinaryWriter bw)
        {
            uint size = (uint)((rawBody?.Length ?? 0) / 4);
            bw.Write(size);
            if (rawBody != null)
                bw.Write(rawBody);
        }
    }
}
