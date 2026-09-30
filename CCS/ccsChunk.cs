using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UN5ModdingWorkshop
{
    public class ccsChunk
    {
        public CCS ccs;

        /// <summary>Tipo do chunk (os 4 bytes lidos antes do Size), preservado
        /// exatamente como veio no arquivo — inclusive os 16 bits altos, que o
        /// lookup de tipo ignora mas que precisam ser regravados fielmente.</summary>
        public uint ChunkType;

        /// <summary>Corpo cru do chunk (tudo após o campo Size), guardado como
        /// fallback para chunks sem Read/Write próprios — garante que chunks
        /// ainda não decodificados sejam regravados sem perda de dados.</summary>
        protected byte[] rawBody;

        public virtual void Read(BinaryReader br)
        {
            uint size = br.ReadUInt32();
            rawBody = br.ReadBytes((int)(size * 4));
        }

        public virtual void Write(BinaryWriter bw)
        {
            uint size = (uint)((rawBody?.Length ?? 0) / 4);
            bw.Write(size);
            if (rawBody != null)
                bw.Write(rawBody);
        }
    }
}