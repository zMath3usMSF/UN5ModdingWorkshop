using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace UN5ModdingWorkshop
{
    // ==========================================================================
    // Constantes e matemática da GS (Graphics Synthesizer) do PS2, reconstruídas
    // via engenharia reversa de FUN_00112ce0, FUN_001a18d0 e FUN_00113040.
    // ==========================================================================
    public static class GsPsm
    {
        public const byte PSMCT32 = 0x00;
        public const byte PSMCT24 = 0x01;
        public const byte PSMCT16 = 0x02;
        public const byte PSMCT16S = 0x0A;
        public const byte PSMT8 = 0x13;
        public const byte PSMT4 = 0x14;
        public const byte PSMT8H = 0x1B;
        public const byte PSMT4HL = 0x24;
        public const byte PSMT4HH = 0x2C;
        public const byte PSMZ32 = 0x30;
        public const byte PSMZ24 = 0x31;
        public const byte PSMZ16 = 0x32;
        public const byte PSMZ16S = 0x3A;

        public static bool IsIndexed(byte psm) => psm == PSMT4 || psm == PSMT8 || psm == PSMT8H || psm == PSMT4HL || psm == PSMT4HH;
        public static bool Is4Bpp(byte psm) => psm == PSMT4 || psm == PSMT4HL || psm == PSMT4HH;
        public static bool Is8Bpp(byte psm) => psm == PSMT8 || psm == PSMT8H;
    }

    public static class GsSwizzle
    {
        static readonly byte[] Table_Default = {
            0x00,0x01,0x04,0x05,0x10,0x11,0x14,0x15,0x02,0x03,0x06,0x07,0x12,0x13,0x16,0x17,
            0x08,0x09,0x0C,0x0D,0x18,0x19,0x1C,0x1D,0x0A,0x0B,0x0E,0x0F,0x1A,0x1B,0x1E,0x1F
        };
        static readonly byte[] Table_PSMT4_CT16 = {
            0x00,0x02,0x08,0x0A,0x01,0x03,0x09,0x0B,0x04,0x06,0x0C,0x0E,0x05,0x07,0x0D,0x0F,
            0x10,0x12,0x18,0x1A,0x11,0x13,0x19,0x1B,0x14,0x16,0x1C,0x1E,0x15,0x17,0x1D,0x1F
        };
        static readonly byte[] Table_CT16S = {
            0x00,0x02,0x10,0x12,0x01,0x03,0x11,0x13,0x08,0x0A,0x18,0x1A,0x09,0x0B,0x19,0x1B,
            0x04,0x06,0x14,0x16,0x05,0x07,0x15,0x17,0x0C,0x0E,0x1C,0x1E,0x0D,0x0F,0x1D,0x1F
        };
        static readonly byte[] Table_Z32_Z24 = {
            0x18,0x19,0x1C,0x1D,0x08,0x09,0x0C,0x0D,0x1A,0x1B,0x1E,0x1F,0x0A,0x0B,0x0E,0x0F,
            0x10,0x11,0x14,0x15,0x00,0x01,0x04,0x05,0x13,0x13,0x16,0x17,0x02,0x03,0x06,0x07
        };
        static readonly byte[] Table_Z16 = {
            0x10,0x12,0x18,0x1A,0x11,0x13,0x19,0x1B,0x14,0x16,0x1C,0x1E,0x15,0x17,0x1D,0x1F,
            0x00,0x02,0x08,0x0A,0x01,0x03,0x09,0x0B,0x04,0x06,0x0C,0x0E,0x05,0x07,0x0D,0x0F
        };
        static readonly byte[] Table_Z16S = {
            0x10,0x12,0x00,0x02,0x11,0x13,0x01,0x03,0x18,0x1A,0x08,0x0A,0x19,0x1B,0x09,0x0B,
            0x14,0x16,0x04,0x06,0x15,0x17,0x05,0x07,0x1C,0x1E,0x0C,0x0E,0x1D,0x1F,0x0D,0x0F
        };

        static (int blockW, int blockH, byte[] table) GetFormatInfo(byte psm)
        {
            switch (psm)
            {
                case GsPsm.PSMCT16:
                case GsPsm.PSMT4:
                    return (16, 4, Table_PSMT4_CT16);
                case GsPsm.PSMCT16S:
                    return (16, 4, Table_CT16S);
                case GsPsm.PSMZ16:
                    return (16, 4, Table_Z16);
                case GsPsm.PSMZ16S:
                    return (16, 4, Table_Z16S);
                case GsPsm.PSMZ32:
                case GsPsm.PSMZ24:
                    return (8, 8, Table_Z32_Z24);
                default:
                    return (8, 8, Table_Default);
            }
        }

        public static short GetSwizzledAddress(byte psm, short x, short y)
        {
            var (blockW, blockH, table) = GetFormatInfo(psm);

            int blockIndex = (y / 32) + (x / 64) * 32;
            int colIdx = (64 / blockW) * ((y % 32) / blockH) + ((x % 64) / blockW);

            byte tableVal = table[colIdx & 0x1F];
            int address = (blockIndex << 5) + tableVal;
            return (short)address;
        }

        public static int GetTBW(byte psm, ushort width)
        {
            if (psm == GsPsm.PSMT4 || psm == GsPsm.PSMT8)
            {
                int blocks128 = (width + 0x7F) >> 7;
                return blocks128 << 1;
            }
            return (width + 0x3F) >> 6;
        }
    }

    // ==========================================================================
    // Dados de textura (payload) — separado do header/chunk container.
    // ==========================================================================
    internal class ccsTextureData
    {
        public int Width { get; set; }
        public int Height { get; set; }

        /// <summary>PSM (Pixel Storage Mode) da GS — ver constantes em GsPsm.</summary>
        public byte TextureType { get; set; }

        public uint MipmapsCount { get; set; }
        public int DataSize { get; set; }
        public List<byte> RawData { get; set; } = new List<byte>();

        public bool Is4Bpp => GsPsm.Is4Bpp(TextureType);
        public bool Is8Bpp => GsPsm.Is8Bpp(TextureType);

        public ccsTextureData() { }

        public ccsTextureData(int width, int height, byte textureType, uint mipmapsCount, int dataSize, List<byte> rawData)
        {
            Width = width;
            Height = height;
            TextureType = textureType;
            MipmapsCount = mipmapsCount;
            DataSize = dataSize;
            RawData = rawData ?? new List<byte>();
        }

        public int GetPaletteIndex(int pixelIndex, bool is4Bpp)
        {
            if (is4Bpp)
            {
                int byteIndex = pixelIndex >> 1;
                byte value = RawData[byteIndex];
                return (pixelIndex & 1) == 0
                    ? value & 0x0F
                    : (value >> 4) & 0x0F;
            }
            else
            {
                return RawData[pixelIndex];
            }
        }

        /// <summary>Calcula o expoente (log2) usado no header para uma dimensão em
        /// potência de 2. Lança exceção se não for potência de 2 exata.</summary>
        public static byte DimensionToExponent(int dimension)
        {
            if (dimension <= 0 || (dimension & (dimension - 1)) != 0)
                throw new ArgumentException($"Dimensão {dimension} não é potência de 2.");

            byte exponent = 0;
            int value = dimension;
            while (value > 1)
            {
                value >>= 1;
                exponent++;
            }
            return exponent;
        }
    }

    // ==========================================================================
    // Um nível de mip / entrada de swizzle dentro do BlitGroup.
    // ==========================================================================
    internal class ccsSwizzleEntry
    {
        public short X;
        public short Y;

        public ccsSwizzleEntry() { }
        public ccsSwizzleEntry(short x, short y) { X = x; Y = y; }

        public short GetSwizzledAddress(byte psm) => GsSwizzle.GetSwizzledAddress(psm, X, Y);
    }

    // ==========================================================================
    // Chunk de textura (header + payload).
    // ==========================================================================
    internal class ccsTexture : ccsChunk
    {
        public uint PathIndex;
        public string PathName;
        public uint Size;
        public uint Index;
        public uint ClutIndex;
        public uint BlitGroupIndex;
        public uint TexturesFlag;

        /// <summary>Byte em offset 0x39 do struct em runtime — usado como "flag"/PSM
        /// em várias chamadas (FUN_00112ce0, FUN_00113040). Espelha Data.TextureType
        /// no momento da leitura; mantido só para referência/depuração.</summary>
        public uint Unk1;

        /// <summary>Marca de memória não inicializada (0xCDCD) observada nas amostras.
        /// Mantido apenas para referência; não é usado ao gravar (gravamos sempre 0).</summary>
        public uint Unk2;

        /// <summary>
        /// Lista de pares (X,Y) de referência do swizzle, um por nível de mipmap.
        /// O primeiro par corresponde aos antigos campos "Unk3"/"Unk4".
        /// </summary>
        public List<ccsSwizzleEntry> SwizzleEntries = new List<ccsSwizzleEntry>();

        public ccsTextureData Data { get; private set; }

        public override void Read(BinaryReader br)
        {
            Size = br.ReadUInt32();
            Index = br.ReadUInt32();
            PathIndex = (ccs.chunks[1] as ccsIndexTable).PathParentIndexes[(int)Index];
            PathName = (ccs.chunks[1] as ccsIndexTable).Paths[(int)PathIndex];
            ClutIndex = br.ReadUInt32();
            BlitGroupIndex = br.ReadUInt32();
            TexturesFlag = br.ReadByte();
            byte textureType = br.ReadByte();
            uint mipmapsCount = br.ReadByte();
            Unk1 = br.ReadByte();
            int width = br.ReadByte();
            int height = br.ReadByte();
            Unk2 = br.ReadUInt16();

            short swX = br.ReadInt16();
            short swY = br.ReadInt16();
            SwizzleEntries.Add(new ccsSwizzleEntry(swX, swY));

            int textureDataSize = br.ReadInt32();

            width = 1 << width;
            height = 1 << height;

            var rawData = new List<byte>(br.ReadBytes(textureDataSize << 2));

            Data = new ccsTextureData(width, height, textureType, mipmapsCount, textureDataSize, rawData);
        }

        public override void Write(BinaryWriter bw)
        {
            byte widthExponent = ccsTextureData.DimensionToExponent(Data.Width);
            byte heightExponent = ccsTextureData.DimensionToExponent(Data.Height);

            int rawByteCount = Data.RawData.Count;
            int textureDataSizeField = (rawByteCount + 3) / 4;
            int padding = (textureDataSizeField * 4) - rawByteCount;

            if (SwizzleEntries.Count == 0)
                SwizzleEntries.Add(new ccsSwizzleEntry(0, 0));
            var primary = SwizzleEntries[0];

            int bodyBytes =
                4 + 4 + 4 +          // Index, ClutIndex, BlitGroupIndex
                1 + 1 + 1 + 1 + 1 + 1 + // TexturesFlag, TextureType, MipmapsCount, Unk1, width, height
                2 +                   // Unk2
                2 + 2 +               // swizzle X, Y
                4 +                   // TextureDataSize field
                (textureDataSizeField * 4);

            // Confirmado empiricamente em 4 amostras (PSMT4/PSMT8, BlitGroup 0 e != 0):
            // Size = (bodyBytes / 4) + 50. A origem exata desses 50 "blocos" extras ainda
            // não foi identificada no disassembly, mas a constante se mostrou estável.
            const uint SizeFieldConstantOffset = 50;
            uint sizeField = (uint)(bodyBytes / 4) + SizeFieldConstantOffset;

            bw.Write(sizeField);
            bw.Write(Index);
            bw.Write(ClutIndex);
            bw.Write(BlitGroupIndex);
            bw.Write((byte)TexturesFlag);
            bw.Write(Data.TextureType);
            bw.Write((byte)Data.MipmapsCount);
            bw.Write((byte)Unk1);
            bw.Write(widthExponent);
            bw.Write(heightExponent);
            bw.Write((ushort)Unk2);
            bw.Write(primary.X);
            bw.Write(primary.Y);
            bw.Write(textureDataSizeField);

            bw.Write(Data.RawData.ToArray());
            for (int i = 0; i < padding; i++)
                bw.Write((byte)0);
        }

        /// <summary>
        /// Substitui os dados de imagem por uma nova textura customizada (já
        /// quantizada/indexada). Não recalcula swizzle/TBW sozinho — chame
        /// RecalculateForCustomResolution em seguida.
        /// </summary>
        public void SetCustomData(int newWidth, int newHeight, byte newTextureType, int newDataSize, List<byte> newRawData)
        {
            Data = new ccsTextureData(newWidth, newHeight, newTextureType, 0, newDataSize, newRawData);
        }

        /// <summary>
        /// Recalcula TBW e a posição de swizzle (X,Y) corretos para uma nova largura/altura
        /// de textura customizada, preservando o PSM atual. Só suporta texturas com
        /// BlitGroupIndex == 0 (página isolada); para BlitGroups compartilhados (mapa/HUD),
        /// lança exceção porque a posição exigiria simular o bin-packing do jogo.
        /// </summary>
        public (int tbw, short swizzledX, short swizzledY) RecalculateForCustomResolution(int newWidth, int newHeight)
        {
            byte psm = Data.TextureType;
            int tbw = GsSwizzle.GetTBW(psm, (ushort)newWidth);

            if (BlitGroupIndex != 0)
            {
                throw new NotSupportedException(
                    "Recalcular posição para texturas de BlitGroup compartilhado (mapa/HUD) " +
                    "ainda não é suportado — requer simular o bin-packing do jogo."
                );
            }

            SwizzleEntries.Clear();
            SwizzleEntries.Add(new ccsSwizzleEntry(0, 0));

            return (tbw, SwizzleEntries[0].X, SwizzleEntries[0].Y);
        }

        public static Bitmap ConvertToBitmap(ccsTexture texture, ccsClut clut)
        {
            int width = texture.Data.Width;
            int height = texture.Data.Height;

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            bool is4bpp = texture.Data.Is4Bpp;
            bool is8bpp = texture.Data.Is8Bpp;

            if (!is4bpp && !is8bpp)
                throw new Exception($"Formato de textura (PSM 0x{texture.Data.TextureType:X2}) não suportado para conversão indexada.");

            int expectedPaletteCount = is4bpp ? 16 : 256;
            if (clut.Palette.Count != expectedPaletteCount)
            {
                throw new Exception(
                    $"CLUT com {clut.Palette.Count} cores não bate com o PSM 0x{texture.Data.TextureType:X2} " +
                    $"(esperado {expectedPaletteCount} cores)."
                );
            }

            int paletteCount = clut.Palette.Count;
            uint[] lut = new uint[paletteCount];
            for (int i = 0; i < paletteCount; i++)
            {
                CCSColor color = clut.Palette[i];
                byte alpha = (byte)Math.Min(color.A * 2, 255);
                lut[i] = (uint)(color.B | (color.G << 8) | (color.R << 16) | (alpha << 24));
            }

            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);

            try
            {
                int stride = bmpData.Stride;
                int bufferSize = stride * height;
                byte[] buffer = new byte[bufferSize];
                int pixelIndex = 0;

                for (int y = 0; y < height; y++)
                {
                    int bitmapY = height - 1 - y;
                    int rowOffset = bitmapY * stride;

                    for (int x = 0; x < width; x++)
                    {
                        int paletteIndex = texture.Data.GetPaletteIndex(pixelIndex, is4bpp);
                        uint pixel = lut[paletteIndex];
                        int offset = rowOffset + x * 4;

                        buffer[offset + 0] = (byte)(pixel & 0xFF);
                        buffer[offset + 1] = (byte)((pixel >> 8) & 0xFF);
                        buffer[offset + 2] = (byte)((pixel >> 16) & 0xFF);
                        buffer[offset + 3] = (byte)((pixel >> 24) & 0xFF);

                        pixelIndex++;
                    }
                }

                Marshal.Copy(buffer, 0, bmpData.Scan0, bufferSize);
            }
            finally
            {
                bitmap.UnlockBits(bmpData);
            }

            return bitmap;
        }
    }
}