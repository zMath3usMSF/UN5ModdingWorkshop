using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UN5ModdingWorkshop
{
    internal class ccsModel : ccsChunk
    {
        uint Size;
        uint Index;
        public override void Read(BinaryReader br)
        {
            Size = br.ReadUInt32();
            long initialPosition = br.BaseStream.Position;
            Index = br.ReadUInt32();
            br.BaseStream.Seek(4, SeekOrigin.Current);
            uint modelType = br.ReadByte();
            uint unk = br.ReadByte();
            uint meshCount = br.ReadByte();
            br.BaseStream.Seek(initialPosition, SeekOrigin.Begin);

            if (modelType == 4)
            {
                rawBody = br.ReadBytes((int)(Size * 4 - (meshCount * 0x3C)));
            }
            else
            {
                rawBody = br.ReadBytes((int)(Size * 4));
            }
        }

        public override void Write(BinaryWriter bw)
        {
            bw.Write(Size);
            if (rawBody != null)
                bw.Write(rawBody);
        }
    }
}
