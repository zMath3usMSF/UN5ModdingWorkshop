using CCSFileExplorerWV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;
using static CCSFileExplorerWV.CCSFile;

namespace UN5ModdingWorkshop
{
    public class CharSel
    {
        public static int SelectedID = 0;
        public static List<int> CharSelID = new List<int>();
        static List<Bitmap> CharIcons = new List<Bitmap>();
        static List<Bitmap> charPicturePicBoxes = new List<Bitmap>();
        public static List<Bitmap> xCommandIcons = new List<Bitmap>();
        static Bitmap selIconImage;
        static PictureBox selIcon = new PictureBox();
        static CCSFile charselFile = new CCSFile(new byte[0], FileVersionEnum.HACK_GU);
        static Main mainF;

        public static void Create(Main main, string gamePath)
        {
            mainF = main;
            charselFile = new CCSFile(File.ReadAllBytes(Path.Combine(gamePath, "DATA\\ROFS\\CHARSEL1.CCS")), FileVersionEnum.HACK_GU);
            Bitmap charselTexture = GetCCSImage(charselFile, "purecharsel10.bmp");
            ReadAllCharIcon(gamePath);
            CharSelID = ReadAllCharSelID(gamePath);
            Bitmap purecharsel01 = GetCCSImage(charselFile, "purecharsel01.bmp");
            Bitmap nrtImage = purecharsel01.Clone(new Rectangle(0, 0, 168, 168), purecharsel01.PixelFormat);
            main.pictureBox3.Image = nrtImage;
            main.tabPage1.Controls.Add(selIcon);
            ReadAllCharRender(gamePath);
            selIconImage = charselTexture.Clone(new Rectangle(202, 468, 36, 40), charselTexture.PixelFormat);
            selIcon.SizeMode = PictureBoxSizeMode.CenterImage;
            selIcon.Image = selIconImage;
            selIcon.Visible = false;

            Bitmap charsel01 = GetCCSImage(charselFile, "charsel01.bmp");
            Bitmap arrowImage = charsel01.Clone(new Rectangle(259, 185, 10, 14), charsel01.PixelFormat);
            main.picArrowRight.Image = (Bitmap)arrowImage.Clone();
            arrowImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
            main.picArrowLeft.Image = arrowImage;
            main.picArrowRight.Click += PicArrowRight_Click;
            main.picArrowRight.DoubleClick += PicArrowRight_Click;
            main.picArrowLeft.Click += PicArrowLeft_Click;
            main.picArrowLeft.DoubleClick += PicArrowLeft_Click;

            CreateCharPicBoxes();
            CreateCommandImages();
        }
        private static void CreateCharPicBoxes()
        {
            for (int i = 0; i < 44; i++)
            {
                PictureBox pic = Clone(mainF.pictureBox2);
                pic.Image = CharIcons[CharSelID[i]];
                int rows = 2;  // two rows
                int spacingX = 38;  // horizontal spacing
                int spacingY = 46;  // image height + vertical spacing

                int offsetX = pic.Location.X;  // base horizontal position
                int offsetY = pic.Location.Y;  // base vertical position (bottom row)

                int col = i / rows;      // column increases every pair of images
                int row = i % rows;      // row 0 or 1, alternating

                // Calculates Y so the top row stays above the bottom row
                int yPos = (row == 0) ? offsetY : offsetY - spacingY;

                pic.Location = new Point(
                    offsetX + col * spacingX,
                    yPos
                );

                // Add event, character ID, and add to TabPage
                pic.MouseClick += Pic_Click;
                pic.Tag = $"Char_{i}";
                mainF.tabPage1.Controls.Add(pic);

                // If i == Naruto TS, select the character by default
                if (i == 1) CharSelect(pic);
            }
        }

        private static void CreateCommandImages()
        {
            CCSFile gaugeFile = new CCSFile(File.ReadAllBytes(Path.Combine(GAME.gamePath, "DATA\\ROFS\\CMN\\GAUGE.CCS")), FileVersionEnum.HACK_GU);
            Bitmap xCommandTexture = GetCCSImage(gaugeFile, "xcommand.bmp");

            //D-Pad
            for (int i = 0; i < 4; i++)
            {
                var icon = new Bitmap(32, 32);
                var g = Graphics.FromImage(icon);
                var srcRect = new Rectangle(i * 32, 0, 32, 32);
                g.DrawImage(xCommandTexture, 0, 0, srcRect, GraphicsUnit.Pixel);
                xCommandIcons.Add(icon);
                g.Dispose();
            }

            //Buttons
            for (int i = 0; i < 5; i++)
            {
                var icon = new Bitmap(24, 32);
                var g = Graphics.FromImage(icon);
                var srcRect = new Rectangle(i * 24, 32, 24, 32);
                g.DrawImage(xCommandTexture, 0, 0, srcRect, GraphicsUnit.Pixel);
                xCommandIcons.Add(icon);
                g.Dispose();
            }
        }

        private static void PicArrowLeft_Click(object sender, EventArgs e) => ArrowRightLeftClick(true);

        private static void PicArrowRight_Click(object sender, EventArgs e) => ArrowRightLeftClick(false);

        private static void ArrowRightLeftClick(bool direction)
        {
            selIcon.Visible = false;
            int selTagID = int.Parse(selIcon.Tag.ToString().Split('_')[1]);
            selIcon.Tag = $"Char_{(direction == true ? (selTagID + 2) : selTagID - 2 + (GAME.charSelCount * 2)) % (GAME.charSelCount * 2)}";
            foreach (Control control in mainF.tabPage1.Controls)
            {
                PictureBox pic = control as PictureBox;
                if (pic != null && pic.Tag != null)
                {
                    string tagString = pic.Tag.ToString();
                    if (tagString.StartsWith("Char_"))
                    {
                        string[] partes = tagString.Split('_');
                        if (partes.Length == 2 && int.TryParse(partes[1], out int charID))
                        {
                            int newIndex = direction == true ? (charID - 2 + (GAME.charSelCount * 2)) % (GAME.charSelCount * 2) : (charID + 2) % (GAME.charSelCount * 2);
                            pic.Tag = $"Char_{newIndex}";
                            if (selIcon.Tag != null && pic.Tag.ToString() == selIcon.Tag.ToString() && selIcon.Location != pic.Location)
                            {
                                selIcon.Location = new Point(pic.Location.X, pic.Location.Y);
                                CharSelect(selIcon);
                            }
                            pic.Image = CharIcons[CharSelID[newIndex]];
                        }
                    }
                }
            }

        }

        public static void Pic_Click(object sender, MouseEventArgs e)
        {
            CharSelect(sender as PictureBox);
            if (e.Button == MouseButtons.Right) BTL.UpdateMatch(true, CharSelID[SelectedID], 0);
        }
        static void CharSelect(PictureBox pictureBox)
        {
            PictureBox charIcon = pictureBox;
            if (charIcon != null)
            {
                mainF.pictureBox3.Image = charPicturePicBoxes[CharSelID[Convert.ToInt32(charIcon.Tag.ToString().Split('_')[1])]];
                SelectedID = Convert.ToInt32(charIcon.Tag.ToString().Split('_')[1]);
                Bitmap teste = MesclarBitmaps(new Bitmap(charIcon.Image), new Bitmap(selIconImage));
                selIcon.Image = teste;
                selIcon.Visible = true;
                selIcon.Size = charIcon.Size;
                selIcon.Location = new Point(charIcon.Location.X, charIcon.Location.Y);
                selIcon.Tag = charIcon.Tag;
                selIcon.BringToFront();
            }
        }
        static Bitmap MesclarBitmaps(Bitmap background, Bitmap foreground)
        {
            if (background == null)
                throw new ArgumentNullException(nameof(background));
            if (foreground == null)
                throw new ArgumentNullException(nameof(foreground));

            Bitmap result = new Bitmap(background.Width, background.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.Clear(Color.Transparent);
                // Desenha a imagem de fundo
                g.DrawImage(background, 0, 0, background.Width, background.Height);

                // Calcula posição para centralizar a imagem foreground
                int x = (background.Width - foreground.Width) / 2;
                int y = (background.Height - foreground.Height) / 2;

                // Desenha a imagem de primeiro plano (foreground) centralizada
                g.DrawImage(foreground, x, y, foreground.Width, foreground.Height);
            }

            return result;
        }
        private static PictureBox Clone(PictureBox pic)
        {
            PictureBox clonePic = new PictureBox();
            clonePic.Size = pic.Size;
            clonePic.SizeMode = pic.SizeMode;
            clonePic.Location = new Point(pic.Location.X, pic.Location.Y);
            return clonePic;
        }
        public static void ReadAllCharIcon(string gamePath)
        {
            Bitmap charselTexture = GetCCSImage(charselFile, "purecharsel10.bmp");
            byte[] modData = GAME.isUN6 != true ? 
                             File.ReadAllBytes(gamePath + "\\UN5.ELF") : 
                             File.ReadAllBytes(gamePath + "\\PRG\\MOD.BIN");

            BinaryReader br = new BinaryReader(new MemoryStream(modData));
            br.BaseStream.Position = GAME.isUN6 != true ?
                                     0x4DC120 :
                                     0x196A0;

            for (int i = 0; i < GAME.charCount; i++)
            {
                int x = br.ReadUInt16();
                int y = br.ReadUInt16();
                int width = br.ReadUInt16();
                int height = br.ReadUInt16();
                if (width == 0 && height == 0)
                {
                    width = 1;
                    height = 1;
                }
                CharIcons.Add(charselTexture.Clone(new Rectangle(x, y, width, height), charselTexture.PixelFormat));
            }
        }
        private static void ReadAllCharRender(string gamePath)
        {
            byte[] modData = GAME.isUN6 != true ?
                 File.ReadAllBytes(gamePath + "\\UN5.ELF") :
                 File.ReadAllBytes(gamePath + "\\PRG\\MOD.BIN");

            using (BinaryReader br = new BinaryReader(new MemoryStream(modData)))
            {
                br.BaseStream.Position = GAME.isUN6 != true ?
                         0x4DBCA0 :
                         0x18AA0;

                Dictionary<int, Bitmap> imageCache = new Dictionary<int, Bitmap>();
                for (int i = 0; i < GAME.charCount; i++)
                {
                    int pureIndex = br.ReadInt32() + 1;
                    Bitmap purecharsel;
                    if (!imageCache.TryGetValue(pureIndex, out purecharsel))
                    {
                        purecharsel = GetCCSImage(charselFile, $"purecharsel{pureIndex:00}.bmp");
                        imageCache[pureIndex] = purecharsel;
                    }

                    int x = br.ReadUInt16();
                    int y = br.ReadUInt16();
                    int width = br.ReadUInt16();
                    int height = br.ReadUInt16();

                    if (width == 0 && height == 0)
                    {
                        width = 1;
                        height = 1;
                    }

                    Rectangle cropRect = new Rectangle(x, y, width, height);
                    if (x + width <= purecharsel.Width && y + height <= purecharsel.Height)
                    {
                        Bitmap cropped = purecharsel.Clone(cropRect, purecharsel.PixelFormat);
                        charPicturePicBoxes.Add(cropped);
                    }
                    else
                    {
                        charPicturePicBoxes.Add(new Bitmap(1, 1));
                    }
                }
            }
        }
        public static List<int> ReadAllCharSelID(string gamePath)
        {
            List<int> listCharselID = new List<int>();
            byte[] modData = File.ReadAllBytes(GAME.GetELFPathInSystemCNF());
            BinaryReader br = new BinaryReader(new MemoryStream(modData));
            br.BaseStream.Position = 0x4DD790;
            for (int i = 0; i < GAME.charSelCount * 2; i++)
            {
                listCharselID.Add(GAME.isUN6 != true ?
                                  br.ReadInt32() :
                                  br.ReadByte());
            }
            return listCharselID;
        }
    }
}
