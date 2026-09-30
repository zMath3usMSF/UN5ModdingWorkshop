using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UN5ModdingWorkshop
{
    public partial class CCSF_Editor : Form
    {
        public CCS ccs;
        private string currentCcsPath; // <-- novo

        private Dictionary<int, ccsTexture> texturesByPathIndex = new Dictionary<int, ccsTexture>();
        private Dictionary<int, ccsClut> clutsByIndex = new Dictionary<int, ccsClut>();

        public CCSF_Editor()
        {
            InitializeComponent();
            Filelist.LoadDirectory(GAME.gamePath, treeViewFilelist);
            treeViewFilelist.AfterSelect += TreeViewFilelist_AfterSelect;
            treeViewCCS.AfterSelect += TreeViewCCS_AfterSelect;
        }

        private void TreeViewCCS_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int selectedNodeIndex = treeViewCCS.SelectedNode.Index;
            var indexTable = ccs.chunks[1] as ccsIndexTable;
            string pathName = indexTable.Paths[selectedNodeIndex].ToUpper();

            if (!pathName.Contains(".BMP") || pathName.Contains("#"))
                return;

            if (!texturesByPathIndex.TryGetValue(selectedNodeIndex, out ccsTexture texture))
            {
                MessageBox.Show($"Textura não encontrada para o índice {selectedNodeIndex}.");
                return;
            }

            if (!clutsByIndex.TryGetValue((int)texture.ClutIndex, out ccsClut clut))
            {
                MessageBox.Show($"CLUT não encontrada (índice {texture.ClutIndex}).");
                return;
            }

            var oldImage = picCCSImage.Image;
            picCCSImage.Image = ccsTexture.ConvertToBitmap(texture, clut);
            oldImage?.Dispose();
        }

        private void TreeViewFilelist_AfterSelect(object sender, TreeViewEventArgs e)
        {
            treeViewCCS.Nodes.Clear();
            texturesByPathIndex.Clear();
            clutsByIndex.Clear();

            string selectedFile = e.Node.Tag.ToString();
            string fileExtension = Path.GetExtension(selectedFile).ToUpper();

            if (fileExtension != ".CCS")
                return;

            currentCcsPath = selectedFile; // <-- guarda pra reescrever depois

            ccs = new CCS();
            ccs.Read(selectedFile);

            foreach (var chunk in ccs.chunks)
            {
                if (chunk is ccsTexture tex)
                {
                    texturesByPathIndex[(int)tex.PathIndex] = tex;
                }
                else if (chunk is ccsClut clt)
                {
                    clutsByIndex[(int)clt.Index] = clt;
                }
            }

            if (!(ccs.chunks[1] is ccsIndexTable indexTable))
                return;

            TreeNode firstBmpNode = null;

            foreach (var path in indexTable.Paths)
            {
                string nodeText = Path.GetFileName(path).ToUpper();
                TreeNode node = new TreeNode(nodeText);

                if (path.Contains("#"))
                {
                    node.ForeColor = Color.Red;
                }

                treeViewCCS.Nodes.Add(node);

                string upperPath = path.ToUpper();
                if (firstBmpNode == null && upperPath.Contains(".BMP") && !upperPath.Contains("#"))
                {
                    firstBmpNode = node;
                }
            }

            if (firstBmpNode != null)
            {
                treeViewCCS.SelectedNode = firstBmpNode;
            }
        }

        private void btnReplaceTexture_Click(object sender, EventArgs e)
        {
            if (treeViewCCS.SelectedNode == null)
            {
                MessageBox.Show("Selecione uma textura na árvore antes de importar.");
                return;
            }

            int selectedNodeIndex = treeViewCCS.SelectedNode.Index;

            if (!texturesByPathIndex.TryGetValue(selectedNodeIndex, out ccsTexture texture))
            {
                MessageBox.Show("A textura selecionada não foi encontrada.");
                return;
            }

            if (!clutsByIndex.TryGetValue((int)texture.ClutIndex, out ccsClut clut))
            {
                MessageBox.Show($"CLUT não encontrada (índice {texture.ClutIndex}).");
                return;
            }

            // --- 1. Pede o PNG ao usuário ---
            using var openDialog = new OpenFileDialog
            {
                Title = "Selecione a imagem para importar",
                Filter = "Imagens PNG (*.png)|*.png|Todos os arquivos (*.*)|*.*"
            };

            if (openDialog.ShowDialog() != DialogResult.OK)
                return;

            Bitmap sourceImage;
            try
            {
                using var loaded = new Bitmap(openDialog.FileName);
                // Clona pra garantir Format32bppArgb e liberar o arquivo em disco.
                sourceImage = loaded.Clone(new Rectangle(0, 0, loaded.Width, loaded.Height), PixelFormat.Format32bppArgb);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Não foi possível abrir a imagem: {ex.Message}");
                return;
            }

            // --- Validação obrigatória: potência de 2 e no máximo 512x512 ---
            // O formato do jogo grava largura/altura como expoente (1 << byte), então só
            // aceita potências de 2. Sem essa checagem, DimensionToExponent lançaria uma
            // exceção lá na hora do Write() — melhor barrar aqui, antes de gastar tempo
            // com quantização, e sem deixar a textura corrompida a meio caminho.
            if (!IsPowerOfTwo(sourceImage.Width) || !IsPowerOfTwo(sourceImage.Height) ||
                sourceImage.Width > 512 || sourceImage.Height > 512)
            {
                MessageBox.Show(
                    $"A imagem tem {sourceImage.Width}x{sourceImage.Height}, mas o formato do jogo só aceita " +
                    "dimensões em potência de 2 (ex: 64, 128, 256, 512) e no máximo 512x512.\n\n" +
                    "Importação cancelada — ajuste a resolução da imagem e tente novamente.",
                    "Resolução inválida", MessageBoxButtons.OK, MessageBoxIcon.Error);

                sourceImage.Dispose();
                return;
            }

            // Aviso (não bloqueante) se a resolução não for potência de 2 — o formato original
            // guarda largura/altura como expoente (1 << byte), então dimensões que não sejam
            // potência de 2 não são representáveis fielmente.
            if (!IsPowerOfTwo(sourceImage.Width) || !IsPowerOfTwo(sourceImage.Height))
            {
                var result = MessageBox.Show(
                    $"A imagem tem {sourceImage.Width}x{sourceImage.Height}, que não é potência de 2 " +
                    "(ex: 64, 128, 256, 512...). O formato original do jogo só representa dimensões " +
                    "em potência de 2 corretamente. Deseja continuar mesmo assim?",
                    "Resolução não é potência de 2", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    sourceImage.Dispose();
                    return;
                }
            }

            // --- 2. Pede as opções de quantização ---
            using var optionsDialog = new QuantizeOptionsDialog();
            if (optionsDialog.ShowDialog() != DialogResult.OK)
            {
                sourceImage.Dispose();
                return;
            }

            int maxColors = optionsDialog.SelectedColorCount;
            QuantizationPriority priority = optionsDialog.SelectedPriority;

            // --- 3. Roda o quantizador ---
            List<CCSColor> newPalette;
            byte[] pixelIndices;
            try
            {
                (newPalette, pixelIndices) = ColorQuantizer.Quantize(sourceImage, maxColors, priority);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao reduzir as cores da imagem: {ex.Message}");
                sourceImage.Dispose();
                return;
            }

            // --- 4. Monta os RawData no formato indexado (4bpp ou 8bpp) esperado pelo jogo ---
            bool is4Bpp = maxColors == 16;
            byte newTextureType = is4Bpp ? GsPsm.PSMT4 : GsPsm.PSMT8;

            List<byte> rawData = is4Bpp
                ? PackIndices4Bpp(pixelIndices)
                : new List<byte>(pixelIndices);

            // TextureDataSize no header é gravado em unidades de 4 bytes (ver ccsTexture.Read: << 2).
            int textureDataSize = (rawData.Count + 3) / 4;

            // --- 5. Atualiza os objetos em memória (Write() em disco ainda é outra etapa) ---
            texture.SetCustomData(sourceImage.Width, sourceImage.Height, newTextureType, textureDataSize, rawData);

            if (texture.BlitGroupIndex == 0)
            {
                var (tbw, swizzledX, swizzledY) = texture.RecalculateForCustomResolution(sourceImage.Width, sourceImage.Height);
                // tbw, swizzledX, swizzledY calculados, mas ainda não persistidos — Write() vai usar isso.
            }
            else
            {
                MessageBox.Show(
                    "Esta textura pertence a um BlitGroup compartilhado (mapa/HUD). O recálculo automático " +
                    "de posição para esse caso ainda não é suportado — a textura foi atualizada, mas a posição " +
                    "de VRAM pode ficar incorreta no jogo.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            clut.Palette = newPalette;

            // --- 6. Atualiza a pré-visualização ---
            var oldImage = picCCSImage.Image;
            picCCSImage.Image = ccsTexture.ConvertToBitmap(texture, clut);
            oldImage?.Dispose();
            sourceImage.Dispose();

            // --- 7. Regrava o .ccs inteiro no disco, já com a textura/CLUT atualizadas ---
            if (string.IsNullOrEmpty(currentCcsPath))
            {
                MessageBox.Show(
                    "Textura importada em memória, mas não foi possível salvar: caminho do arquivo .ccs desconhecido.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ccs.Write(currentCcsPath);
                MessageBox.Show("Textura importada e arquivo .ccs salvo com sucesso.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"A textura foi importada em memória, mas houve um erro ao salvar o arquivo .ccs:\n{ex.Message}",
                    "Erro ao salvar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool IsPowerOfTwo(int n) => n > 0 && (n & (n - 1)) == 0;

        /// <summary>Empacota índices de paleta 0-15 em nibbles (2 pixels por byte), igual ao formato PSMT4.</summary>
        private static List<byte> PackIndices4Bpp(byte[] indices)
        {
            var packed = new List<byte>((indices.Length + 1) / 2);
            for (int i = 0; i < indices.Length; i += 2)
            {
                byte low = (byte)(indices[i] & 0x0F);
                byte high = (i + 1 < indices.Length) ? (byte)(indices[i + 1] & 0x0F) : (byte)0;
                packed.Add((byte)(low | (high << 4)));
            }
            return packed;
        }

        private void btnExportTexture_Click(object sender, EventArgs e)
        {
            if (treeViewCCS.SelectedNode == null)
            {
                MessageBox.Show("Selecione uma textura na árvore antes de exportar.");
                return;
            }

            int selectedNodeIndex = treeViewCCS.SelectedNode.Index;

            if (!texturesByPathIndex.TryGetValue(selectedNodeIndex, out ccsTexture texture))
            {
                MessageBox.Show("A textura selecionada não foi encontrada.");
                return;
            }

            if (!clutsByIndex.TryGetValue((int)texture.ClutIndex, out ccsClut clut))
            {
                MessageBox.Show($"CLUT não encontrada (índice {texture.ClutIndex}).");
                return;
            }

            string imageName = Path.GetFileNameWithoutExtension(treeViewCCS.SelectedNode.Text);

            using var saveFileDialog = new SaveFileDialog
            {
                Title = "Selecione o local para salvar a imagem exportada",
                Filter = "Imagens PNG (*.png)|*.png|Todos os arquivos (*.*)|*.*",
                FileName = imageName + ".PNG"
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            Bitmap image = picCCSImage.Image as Bitmap;

            if (image == null)
            {
                MessageBox.Show("Não há imagem para exportar.");
                return;
            }

            image.Save(saveFileDialog.FileName, ImageFormat.Png);
        }

        private void testeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ccs == null)
            {
                MessageBox.Show("Carregue um arquivo .ccs primeiro.");
                return;
            }

            string report = VramLayoutAnalyzer.Analyze(ccs);

            using var saveDialog = new SaveFileDialog
            {
                Title = "Salvar análise de layout de VRAM",
                Filter = "Arquivo de texto (*.txt)|*.txt",
                FileName = "analise_vram.txt"
            };

            if (saveDialog.ShowDialog() != DialogResult.OK)
                return;

            File.WriteAllText(saveDialog.FileName, report, Encoding.UTF8);
            MessageBox.Show($"Análise salva em:\n{saveDialog.FileName}");
        }
    }
}