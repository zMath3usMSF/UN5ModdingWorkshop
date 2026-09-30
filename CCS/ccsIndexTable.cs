using System.Collections.Generic;
using System.IO;

namespace UN5ModdingWorkshop
{
    internal class ccsIndexTable : ccsChunk
    {
        public List<string> Paths = new List<string>();
        public List<string> Names = new List<string>();
        public List<uint> PathParentIndexes = new List<uint>();

        public override void Read(BinaryReader br)
        {
            uint size = br.ReadUInt32();
            uint pathsCount = br.ReadUInt32();
            uint namesCount = br.ReadUInt32();

            for (int i = 0; i < pathsCount; i++)
                Paths.Add(Util.ReadFixedLenString(br, 0x20, '\0'));

            for (int i = 0; i < namesCount; i++)
            {
                Names.Add(Util.ReadFixedLenString(br, 0x1E, '\0'));
                PathParentIndexes.Add(br.ReadUInt16());
            }
        }

        public override void Write(BinaryWriter bw)
        {
            int bodyBytes = 4 + 4; // PathsCount + NamesCount
            bodyBytes += Paths.Count * 0x20;
            bodyBytes += Names.Count * (0x1E + 2); // nome fixo + índice ushort

            uint sizeField = (uint)(bodyBytes / 4);

            bw.Write(sizeField);
            bw.Write((uint)Paths.Count);
            bw.Write((uint)Names.Count);

            foreach (var path in Paths)
                Util.WriteFixedLenString(bw, path, 0x20);

            for (int i = 0; i < Names.Count; i++)
            {
                Util.WriteFixedLenString(bw, Names[i], 0x1E);
                bw.Write((ushort)PathParentIndexes[i]);
            }
        }
    }
}