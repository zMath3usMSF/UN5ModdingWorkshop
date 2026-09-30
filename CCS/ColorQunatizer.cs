using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace UN5ModdingWorkshop
{
    public static class ColorQuantizer
    {
        private struct Pixel
        {
            public byte R, G, B, A;
            public int Count; // quantos pixels da imagem têm exatamente essa cor
        }

        /// <summary>
        /// Reduz uma imagem para no máximo <paramref name="maxColors"/> cores (16 ou 256),
        /// retornando a paleta resultante e o índice de paleta de cada pixel (ordem linear,
        /// esquerda->direita, topo->baixo).
        /// </summary>
        public static (List<CCSColor> palette, byte[] indices) Quantize(
            Bitmap source, int maxColors, QuantizationPriority priority)
        {
            int width = source.Width;
            int height = source.Height;

            // --- 1. Extrai todos os pixels e agrupa cores idênticas (acelera muito) ---
            var colorCounts = new Dictionary<int, int>();
            var rawPixels = new int[width * height];

            BitmapData bmpData = source.LockBits(
                new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                int stride = bmpData.Stride;
                byte[] buffer = new byte[stride * height];
                Marshal.Copy(bmpData.Scan0, buffer, 0, buffer.Length);

                for (int y = 0; y < height; y++)
                {
                    int rowOffset = y * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int off = rowOffset + x * 4;
                        byte b = buffer[off + 0];
                        byte g = buffer[off + 1];
                        byte r = buffer[off + 2];
                        byte a = buffer[off + 3];

                        int argb = (a << 24) | (r << 16) | (g << 8) | b;
                        rawPixels[y * width + x] = argb;

                        colorCounts.TryGetValue(argb, out int c);
                        colorCounts[argb] = c + 1;
                    }
                }
            }
            finally
            {
                source.UnlockBits(bmpData);
            }

            var uniqueColors = colorCounts.Select(kv => new Pixel
            {
                A = (byte)((kv.Key >> 24) & 0xFF),
                R = (byte)((kv.Key >> 16) & 0xFF),
                G = (byte)((kv.Key >> 8) & 0xFF),
                B = (byte)(kv.Key & 0xFF),
                Count = kv.Value
            }).ToList();

            // --- 2. Median cut: divide recursivamente até ter maxColors buckets ---
            var buckets = new List<List<Pixel>> { uniqueColors };

            while (buckets.Count < maxColors)
            {
                int splitIndex = FindLargestBucket(buckets, priority);
                if (splitIndex < 0) break; // não dá mais pra dividir (todas as buckets têm 1 cor só)

                var bucket = buckets[splitIndex];
                if (bucket.Count <= 1) break;

                var (left, right) = SplitBucket(bucket, priority);
                buckets.RemoveAt(splitIndex);
                buckets.Add(left);
                buckets.Add(right);
            }

            // --- 3. Calcula a cor média (ponderada pelo Count) de cada bucket -> paleta final ---
            var palette = new List<CCSColor>();
            var bucketAverages = new List<(double r, double g, double b, double a)>();

            foreach (var bucket in buckets)
            {
                long totalCount = bucket.Sum(p => (long)p.Count);
                double r = bucket.Sum(p => p.R * (double)p.Count) / totalCount;
                double g = bucket.Sum(p => p.G * (double)p.Count) / totalCount;
                double b = bucket.Sum(p => p.B * (double)p.Count) / totalCount;
                double a = bucket.Sum(p => p.A * (double)p.Count) / totalCount;

                bucketAverages.Add((r, g, b, a));

                // O formato do jogo guarda alpha "pela metade" (ConvertToBitmap faz A*2 na
                // hora de exibir), então gravamos alpha/2 aqui, limitado a 128 (0x80).
                byte storedAlpha = (byte)Math.Min(Math.Round(a / 2.0), 128);

                palette.Add(new CCSColor((byte)Math.Round(r), (byte)Math.Round(g), (byte)Math.Round(b), storedAlpha));
            }

            // Preenche o resto da paleta (caso maxColors seja 16/256 mas tenham saído menos
            // buckets, ex: imagem com poucas cores únicas) com preto transparente, pra manter
            // o tamanho fixo esperado pelo formato do jogo.
            while (palette.Count < maxColors)
                palette.Add(new CCSColor(0, 0, 0, 0));

            // --- 4. Mapeia cada pixel original pro índice de paleta mais próximo ---
            var nearestIndexCache = new Dictionary<int, byte>();
            byte[] indices = new byte[width * height];

            for (int i = 0; i < rawPixels.Length; i++)
            {
                int argb = rawPixels[i];
                if (!nearestIndexCache.TryGetValue(argb, out byte idx))
                {
                    byte a = (byte)((argb >> 24) & 0xFF);
                    byte r = (byte)((argb >> 16) & 0xFF);
                    byte g = (byte)((argb >> 8) & 0xFF);
                    byte b = (byte)(argb & 0xFF);

                    idx = FindNearestPaletteIndex(r, g, b, a, bucketAverages, priority);
                    nearestIndexCache[argb] = idx;
                }
                indices[i] = idx;
            }

            // O formato do jogo armazena os dados de baixo pra cima (ConvertToBitmap inverte a
            // linha ao exibir, pra compensar essa convenção). Como extraímos os pixels do PNG
            // de cima pra baixo (ordem normal), precisamos inverter a ordem das LINHAS aqui
            // antes de devolver, senão a textura fica de cabeça para baixo ao ser exibida.
            byte[] flippedIndices = new byte[width * height];
            for (int y = 0; y < height; y++)
            {
                int srcRow = y;
                int dstRow = height - 1 - y;
                Array.Copy(indices, srcRow * width, flippedIndices, dstRow * width, width);
            }

            return (palette, flippedIndices);
        }

        // --- Pesos de distância conforme a prioridade escolhida ---
        private static (double wr, double wg, double wb, double wa) GetWeights(QuantizationPriority priority)
        {
            // Pesos maiores = canal considerado "mais importante" na hora de agrupar/comparar cores.
            return priority == QuantizationPriority.Opacity
                ? (0.6, 0.6, 0.6, 4.0)   // opacidade pesa muito mais que a cor em si
                : (1.4, 1.6, 1.2, 0.5);  // cor pesa mais (G um pouco mais, olho humano é mais sensível a verde)
        }

        private static int FindLargestBucket(List<List<Pixel>> buckets, QuantizationPriority priority)
        {
            var weights = GetWeights(priority);
            int bestIndex = -1;
            double bestRange = 0;

            for (int i = 0; i < buckets.Count; i++)
            {
                if (buckets[i].Count <= 1) continue;

                double range = GetWeightedRange(buckets[i], weights);
                if (range > bestRange)
                {
                    bestRange = range;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private static double GetWeightedRange(List<Pixel> bucket, (double wr, double wg, double wb, double wa) w)
        {
            byte rMin = 255, rMax = 0, gMin = 255, gMax = 0, bMin = 255, bMax = 0, aMin = 255, aMax = 0;
            foreach (var p in bucket)
            {
                if (p.R < rMin) rMin = p.R; if (p.R > rMax) rMax = p.R;
                if (p.G < gMin) gMin = p.G; if (p.G > gMax) gMax = p.G;
                if (p.B < bMin) bMin = p.B; if (p.B > bMax) bMax = p.B;
                if (p.A < aMin) aMin = p.A; if (p.A > aMax) aMax = p.A;
            }

            return (rMax - rMin) * w.wr + (gMax - gMin) * w.wg + (bMax - bMin) * w.wb + (aMax - aMin) * w.wa;
        }

        private static (List<Pixel> left, List<Pixel> right) SplitBucket(List<Pixel> bucket, QuantizationPriority priority)
        {
            var w = GetWeights(priority);

            byte rMin = 255, rMax = 0, gMin = 255, gMax = 0, bMin = 255, bMax = 0, aMin = 255, aMax = 0;
            foreach (var p in bucket)
            {
                if (p.R < rMin) rMin = p.R; if (p.R > rMax) rMax = p.R;
                if (p.G < gMin) gMin = p.G; if (p.G > gMax) gMax = p.G;
                if (p.B < bMin) bMin = p.B; if (p.B > bMax) bMax = p.B;
                if (p.A < aMin) aMin = p.A; if (p.A > aMax) aMax = p.A;
            }

            double rangeR = (rMax - rMin) * w.wr;
            double rangeG = (gMax - gMin) * w.wg;
            double rangeB = (bMax - bMin) * w.wb;
            double rangeA = (aMax - aMin) * w.wa;

            double maxRange = Math.Max(Math.Max(rangeR, rangeG), Math.Max(rangeB, rangeA));

            List<Pixel> sorted;
            if (maxRange == rangeR) sorted = bucket.OrderBy(p => p.R).ToList();
            else if (maxRange == rangeG) sorted = bucket.OrderBy(p => p.G).ToList();
            else if (maxRange == rangeB) sorted = bucket.OrderBy(p => p.B).ToList();
            else sorted = bucket.OrderBy(p => p.A).ToList();

            // Corta pela mediana ponderada pelo Count (não pelo índice bruto da lista),
            // pra dividir o "peso" de pixels igualmente entre os dois lados.
            long totalCount = sorted.Sum(p => (long)p.Count);
            long half = totalCount / 2;
            long running = 0;
            int cutIndex = sorted.Count / 2; // fallback

            for (int i = 0; i < sorted.Count; i++)
            {
                running += sorted[i].Count;
                if (running >= half)
                {
                    cutIndex = i + 1;
                    break;
                }
            }
            cutIndex = Math.Max(1, Math.Min(cutIndex, sorted.Count - 1));

            return (sorted.Take(cutIndex).ToList(), sorted.Skip(cutIndex).ToList());
        }

        private static byte FindNearestPaletteIndex(
            byte r, byte g, byte b, byte a,
            List<(double r, double g, double b, double a)> palette,
            QuantizationPriority priority)
        {
            var w = GetWeights(priority);
            double bestDist = double.MaxValue;
            int bestIndex = 0;

            for (int i = 0; i < palette.Count; i++)
            {
                double dr = r - palette[i].r;
                double dg = g - palette[i].g;
                double db = b - palette[i].b;
                double da = a - palette[i].a;

                double dist = dr * dr * w.wr + dg * dg * w.wg + db * db * w.wb + da * da * w.wa;
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestIndex = i;
                }
            }

            return (byte)bestIndex;
        }
    }
}