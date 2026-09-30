using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;

namespace UN5ModdingWorkshop
{
    public class CCS
    {
        public List<ccsChunk> chunks = new List<ccsChunk>();

        public void Read(string ccsPath)
        {
            using var br = new BinaryReader(File.OpenRead(ccsPath));
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                uint rawType = br.ReadUInt32();
                uint typeKey = rawType & 0xFFFF;

                // Antes, tipos desconhecidos eram apenas pulados (dados perdidos ao
                // regravar). Agora usamos ccsChunk (a base) como fallback: ela lê e
                // grava o corpo cru, então nada se perde mesmo sem sabermos decodificar.
                Type type = ccsTypes.TryGetValue(typeKey, out Type t) ? t : typeof(ccsChunk);

                ccsChunk chunk = (ccsChunk)Activator.CreateInstance(type);
                chunk.ccs = this;
                chunk.ChunkType = rawType; // preserva os 32 bits originais, não só os 16 usados no lookup

                chunk.Read(br);

                chunks.Add(chunk);
            }
        }

        public void Write(string ccsPath)
        {
            using var bw = new BinaryWriter(File.Create(ccsPath));
            Write(bw);
        }

        public void Write(BinaryWriter bw)
        {
            // Mantém TotalChunkCount do header sincronizado com a lista real de chunks.
            var header = chunks.OfType<ccsHeader>().FirstOrDefault();
            if (header != null)
                header.TotalChunkCount = (uint)chunks.Count;

            foreach (var chunk in chunks)
            {
                bw.Write(chunk.ChunkType);
                chunk.Write(bw);
            }
        }

        public static Dictionary<uint, Type> ccsTypes = new Dictionary<uint, Type>()
        {
            { 0x0001, typeof(ccsHeader) },
            { 0x0002, typeof(ccsIndexTable) },
            { 0x0003, typeof(ccsSetup) },
            { 0x0005, typeof(ccsStream) },
            { 0x0300, typeof(ccsTexture) },
            { 0x0400, typeof(ccsClut) },
            { 0x0800, typeof(ccsModel) },
        };

        public Bitmap GetCCSImage(string imageName)
        {
            foreach (var chunk in chunks)
            {
                if (chunk is ccsTexture textureChunk)
                {
                    if (Path.GetFileName(textureChunk.PathName).ToUpper() == imageName)
                    {
                        ccsClut clutChunk = new ccsClut();
                        foreach (var clut in chunks)
                        {
                            if (clut is ccsClut clutChunkTemp && clutChunkTemp.Index == textureChunk.ClutIndex)
                            {
                                clutChunk = clutChunkTemp;
                                break;
                            }
                        }
                        return ccsTexture.ConvertToBitmap(textureChunk, clutChunk);
                    }
                }
            }
            return null;
        }
    }
}