using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UN5ModdingWorkshop
{
    public static class VramLayoutAnalyzer
    {
        public class VramEntry
        {
            public string Kind;      // "TEX" ou "CLUT"
            public string Label;     // PathName ou "Clut Index N"
            public int ChunkOrder;
            public byte Psm;
            public int WidthPx;
            public int HeightPx;
            public short SwX;
            public short SwY;

            /// <summary>Endereço final calculado por GsSwizzle.GetSwizzledAddress
            /// a partir de (Psm, SwX, SwY) — é isso que decide a posição real
            /// em VRAM, não o par (X,Y) bruto.</summary>
            public short SwizzledAddress;

            // Tamanho real ocupado pela textura/CLUT em pixels/texels (usado só
            // para a checagem de sobreposição por retângulo bruto, informativa).
            public int BlockWidth;
            public int BlockHeight;
        }

        public static string Analyze(CCS ccs)
        {
            var entries = new List<VramEntry>();

            for (int i = 0; i < ccs.chunks.Count; i++)
            {
                var chunk = ccs.chunks[i];

                if (chunk is ccsTexture tex)
                {
                    short addr = GsSwizzle.GetSwizzledAddress(
                        tex.Data.TextureType, tex.SwizzleEntries[0].X, tex.SwizzleEntries[0].Y);

                    entries.Add(new VramEntry
                    {
                        Kind = "TEX",
                        Label = tex.PathName?.Trim(),
                        ChunkOrder = i,
                        Psm = tex.Data.TextureType,
                        WidthPx = tex.Data.Width,
                        HeightPx = tex.Data.Height,
                        SwX = tex.SwizzleEntries[0].X,
                        SwY = tex.SwizzleEntries[0].Y,
                        SwizzledAddress = addr,
                        BlockWidth = tex.Data.Width,   // dimensão real, não TBW*64
                        BlockHeight = tex.Data.Height
                    });
                }
                else if (chunk is ccsClut clt)
                {
                    bool is256 = clt.Palette.Count == 256;
                    byte psm = is256 ? GsPsm.PSMT8 : GsPsm.PSMT4;

                    short addr = GsSwizzle.GetSwizzledAddress(psm, clt.SwizzleRef.X, clt.SwizzleRef.Y);

                    entries.Add(new VramEntry
                    {
                        Kind = "CLUT",
                        Label = $"Clut Index {clt.Index} ({clt.Palette.Count} cores)",
                        ChunkOrder = i,
                        Psm = psm,
                        WidthPx = is256 ? 16 : 8,
                        HeightPx = is256 ? 16 : 2,
                        SwX = clt.SwizzleRef.X,
                        SwY = clt.SwizzleRef.Y,
                        SwizzledAddress = addr,
                        BlockWidth = is256 ? 16 : 8,
                        BlockHeight = is256 ? 16 : 2
                    });
                }
            }

            var sb = new StringBuilder();

            // --- 1. Agrupa por PSM, ordena por SwX/SwY (posição lógica bruta) ---
            sb.AppendLine("=== TODAS AS ENTRADAS, AGRUPADAS POR PSM, ORDENADAS POR (X,Y) ===");
            foreach (var group in entries.GroupBy(e => e.Psm).OrderBy(g => g.Key))
            {
                sb.AppendLine($"\n--- PSM 0x{group.Key:X2} ---");
                foreach (var e in group.OrderBy(e => e.SwX).ThenBy(e => e.SwY))
                {
                    sb.AppendLine(
                        $"[{e.Kind}] Order={e.ChunkOrder,4} X={e.SwX,5} Y={e.SwY,5} Addr={e.SwizzledAddress,6} " +
                        $"Size={e.WidthPx}x{e.HeightPx} {e.Label}");
                }
            }

            // --- 2. Agrupa por PSM, ordena pelo ENDEREÇO SWIZZLED FINAL ---
            // Essa é a comparação que realmente importa: é o endereço final que
            // decide se duas entradas colidem em VRAM, não o par (X,Y) bruto.
            sb.AppendLine("\n\n=== ENDEREÇO SWIZZLED FINAL, POR PSM (ordenado por endereço) ===");
            foreach (var group in entries.GroupBy(e => e.Psm).OrderBy(g => g.Key))
            {
                sb.AppendLine($"\n--- PSM 0x{group.Key:X2} ---");
                foreach (var e in group.OrderBy(e => e.SwizzledAddress))
                {
                    sb.AppendLine(
                        $"[{e.Kind}] Order={e.ChunkOrder,4} Addr={e.SwizzledAddress,6}  (X={e.SwX},Y={e.SwY})  " +
                        $"Size={e.WidthPx}x{e.HeightPx} {e.Label}");
                }
            }

            // --- 3. Sobreposição por retângulo bruto (X,Y,W,H), só dentro do MESMO PSM ---
            // Comparar PSMs diferentes com a mesma régua (X,Y) não faz sentido físico
            // (4bpp e 8bpp endereçam texels de forma diferente), então filtramos por PSM.
            sb.AppendLine("\n\n=== SOBREPOSIÇÕES DETECTADAS (retângulo X,Y,W,H, mesmo PSM) ===");
            bool anyOverlap = false;
            foreach (var group in entries.GroupBy(e => e.Psm))
            {
                var list = group.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    for (int j = i + 1; j < list.Count; j++)
                    {
                        var a = list[i];
                        var b = list[j];

                        bool overlapX = a.SwX < b.SwX + b.BlockWidth && b.SwX < a.SwX + a.BlockWidth;
                        bool overlapY = a.SwY < b.SwY + b.BlockHeight && b.SwY < a.SwY + a.BlockHeight;

                        if (overlapX && overlapY)
                        {
                            anyOverlap = true;
                            sb.AppendLine(
                                $"[PSM 0x{a.Psm:X2}] [{a.Kind}] Order={a.ChunkOrder} ({a.SwX},{a.SwY} {a.BlockWidth}x{a.BlockHeight}) {a.Label}  " +
                                $"<-->  [{b.Kind}] Order={b.ChunkOrder} ({b.SwX},{b.SwY} {b.BlockWidth}x{b.BlockHeight}) {b.Label}");
                        }
                    }
                }
            }
            if (!anyOverlap)
                sb.AppendLine("(nenhuma sobreposição encontrada)");

            // --- 4. Sobreposição por ENDEREÇO SWIZZLED (faixa [addr, addr+blocos)) ---
            // Aproximação: assume que cada unidade de altura consome 32 endereços
            // (um "blockIndex" inteiro, ver GsSwizzle.GetSwizzledAddress: blockIndex<<5),
            // e usa isso para estimar quantos endereços a textura consome no total.
            sb.AppendLine("\n\n=== SOBREPOSIÇÕES POR ENDEREÇO SWIZZLED (mesmo PSM) ===");
            bool anyAddrOverlap = false;
            foreach (var group in entries.GroupBy(e => e.Psm))
            {
                var list = group.OrderBy(e => e.SwizzledAddress).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    for (int j = i + 1; j < list.Count; j++)
                    {
                        var a = list[i];
                        var b = list[j];
                        if (a.SwizzledAddress == b.SwizzledAddress)
                        {
                            anyAddrOverlap = true;
                            sb.AppendLine(
                                $"[PSM 0x{a.Psm:X2}] MESMO ENDEREÇO ({a.SwizzledAddress}): " +
                                $"[{a.Kind}] Order={a.ChunkOrder} {a.Label}  <-->  [{b.Kind}] Order={b.ChunkOrder} {b.Label}");
                        }
                    }
                }
            }
            if (!anyAddrOverlap)
                sb.AppendLine("(nenhum endereço inicial idêntico encontrado)");

            // --- 5. Grupos com posição bruta (X,Y) idêntica ---
            sb.AppendLine("\n\n=== GRUPOS COM MESMA POSIÇÃO BRUTA (X,Y) EXATA ===");
            foreach (var group in entries.GroupBy(e => (e.SwX, e.SwY)).Where(g => g.Count() > 1))
            {
                sb.AppendLine($"\nPosição ({group.Key.Item1},{group.Key.Item2}) — {group.Count()} entradas:");
                foreach (var e in group.OrderBy(e => e.ChunkOrder))
                    sb.AppendLine($"  [{e.Kind}] Order={e.ChunkOrder} PSM=0x{e.Psm:X2} Addr={e.SwizzledAddress} {e.Label}");
            }

            return sb.ToString();
        }
    }
}