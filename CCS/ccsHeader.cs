using System.IO;

namespace UN5ModdingWorkshop
{
    internal class ccsHeader : ccsChunk
    {
        public string Magic;
        public string Name;
        public uint Version;
        public uint TotalChunkCount;
        public uint Unk1;

        public override void Read(BinaryReader br)
        {
            uint size = br.ReadUInt32();
            Magic = Util.ReadFixedLenString(br, 0x04, '\0');
            Name = Util.ReadFixedLenString(br, 0x20, '\0');
            Version = br.ReadUInt32();
            TotalChunkCount = br.ReadUInt32();
            Unk1 = br.ReadUInt32();
            br.BaseStream.Seek(4, SeekOrigin.Current); // 4 bytes finais ainda não identificados
        }

        public override void Write(BinaryWriter bw)
        {
            const int bodyBytes = 0x04 + 0x20 + 4 + 4 + 4 + 4; // Magic+Name+Version+Count+Unk1+trailing
            uint sizeField = (uint)(bodyBytes / 4);

            bw.Write(sizeField);
            Util.WriteFixedLenString(bw, Magic, 0x04);
            Util.WriteFixedLenString(bw, Name, 0x20);
            bw.Write(Version);
            bw.Write(TotalChunkCount);
            bw.Write(Unk1);
            bw.Write((uint)0); // espelha os 4 bytes pulados no Read (ainda não sabemos o que são)
        }
    }
}