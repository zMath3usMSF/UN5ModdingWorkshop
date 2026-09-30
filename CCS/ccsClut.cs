using System.Collections.Generic;
using System.IO;

namespace UN5ModdingWorkshop
{
    public class CCSColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }
        public CCSColor(byte r, byte g, byte b, byte a)
        {
            R = r; G = g; B = b; A = a;
        }
    }

    internal class ccsClut : ccsChunk
    {
        uint PathIndex;
        public uint Index;
        public uint BlitGroupIndex;
        public uint Unk1;

        /// <summary>Par de coordenada de swizzle (X, blockIndex-já-processado) ligado
        /// à mesma textura/BlitGroup — ver ccsSwizzleEntry / GsSwizzle.</summary>
        public ccsSwizzleEntry SwizzleRef = new ccsSwizzleEntry(0, 0);

        public List<CCSColor> Palette = new List<CCSColor>();

        public override void Read(BinaryReader br)
        {
            uint size = br.ReadUInt32(); // não usado após leitura, só validação futura se quiser
            Index = br.ReadUInt32();
            PathIndex = (ccs.chunks[1] as ccsIndexTable).PathParentIndexes[(int)Index];
            BlitGroupIndex = br.ReadUInt32();
            Unk1 = br.ReadUInt32();

            short swX = br.ReadInt16();
            short swY = br.ReadInt16();
            SwizzleRef = new ccsSwizzleEntry(swX, swY);

            uint colorCount = br.ReadUInt32();

            for (int i = 0; i < colorCount; i++)
            {
                byte r = br.ReadByte();
                byte g = br.ReadByte();
                byte b = br.ReadByte();
                byte a = br.ReadByte();
                Palette.Add(new CCSColor(r, g, b, a));
            }
        }

        public override void Write(BinaryWriter bw)
        {
            int bodyBytes =
                4 +  // Index
                4 +  // BlitGroupIndex
                4 +  // Unk1
                2 +  // swizzle X
                2 +  // swizzle Y
                4 +  // ColorCount
                (Palette.Count * 4); // RGBA por cor

            uint sizeField = (uint)(bodyBytes / 4);

            bw.Write(sizeField);
            bw.Write(Index);
            bw.Write(BlitGroupIndex);
            bw.Write(Unk1);
            bw.Write(SwizzleRef.X);
            bw.Write(SwizzleRef.Y);
            bw.Write((uint)Palette.Count);

            foreach (var color in Palette)
            {
                bw.Write(color.R);
                bw.Write(color.G);
                bw.Write(color.B);
                bw.Write(color.A);
            }
        }
    }
}