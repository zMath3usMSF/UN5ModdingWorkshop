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


namespace UN5ModdingWorkshop
{
    public class CharSel
    {
        public static int SelectedID = 0;
        public static List<int> CharSelID = new List<int>();
        static List<Bitmap> CharIcons = new List<Bitmap>();
        static List<Bitmap> charPicturePicBoxes = new List<Bitmap>();
        public static List<Bitmap> CommandIcons = new List<Bitmap>();
        static Bitmap selIconImage;
        static PictureBox selIcon = new PictureBox();
        static CCS charselFile = new CCS();
        static Main mainF;
        static PictureBox charIcon = null;
        private static int selectedCharIndex = 1;

        public static void Create(Main main, string gamePath)
        {
            mainF = main;
            charselFile = new CCS();
            charselFile.Read(Path.Combine(gamePath, "DATA\\ROFS\\CHARSEL1.CCS"));
            Bitmap charselTexture = charselFile.GetCCSImage("PURECHARSEL10.BMP");
            ReadAllCharIcon(gamePath);
            CharSelID = ReadAllCharSelID(gamePath);
            Bitmap purecharsel01 = charselFile.GetCCSImage("PURECHARSEL01.BMP");
            Bitmap nrtImage = purecharsel01.Clone(new Rectangle(0, 0, 168, 168), purecharsel01.PixelFormat);
            main.pictureBox3.Image = nrtImage;
            ReadAllCharRender(gamePath);
            selIconImage = charselTexture.Clone(new Rectangle(202, 468, 36, 40), charselTexture.PixelFormat);
            selIcon.SizeMode = PictureBoxSizeMode.AutoSize;
            selIcon.BackColor = Color.Transparent;
            mainF.panel2.Controls.Add(selIcon);
            selIcon.BringToFront();

            Bitmap charsel01 = charselFile.GetCCSImage("CHARSEL01.BMP");
            Bitmap arrowImage = charsel01.Clone(new Rectangle(259, 185, 10, 14), charsel01.PixelFormat);
            main.picArrowRight.Image = (Bitmap)arrowImage.Clone();
            arrowImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
            main.picArrowLeft.Image = arrowImage;
            main.picArrowRight.Click += PicArrowRight_Click;
            main.picArrowRight.DoubleClick += PicArrowRight_Click;
            main.picArrowLeft.Click += PicArrowLeft_Click;
            main.picArrowLeft.DoubleClick += PicArrowLeft_Click;

            CreateCharPicBoxes();
            ArrangeCharPicBoxes();
            CreateCommandImages();

            CCS gaugeFile = new CCS();
            gaugeFile.Read(Path.Combine(GAME.gamePath, "DATA\\ROFS\\CMN\\GAUGE.CCS"));
            Bitmap xCommandTexture = gaugeFile.GetCCSImage("XCOMMAND02.BMP");

            //L1/L2
            for (int i = 0; i < 2; i++)
            {
                var icon = new Bitmap(32, 20);
                var g = Graphics.FromImage(icon);
                var srcRect = new Rectangle(0, i * 20, 32, 20);
                g.DrawImage(xCommandTexture, 0, 0, srcRect, GraphicsUnit.Pixel);
                CommandIcons.Add(icon);
                g.Dispose();
            }

            //R1/R2
            for (int i = 0; i < 2; i++)
            {
                var icon = new Bitmap(32, 20);
                var g = Graphics.FromImage(icon);
                var srcRect = new Rectangle(32, i * 20, 32, 20);
                g.DrawImage(xCommandTexture, 0, 0, srcRect, GraphicsUnit.Pixel);
                CommandIcons.Add(icon);
                g.Dispose();
            }

            main.picR1.Image = CommandIcons[11];
            main.picR1.Click += PicR1_Click;
            main.picR1.DoubleClick += PicR1_Click;
            mainF.panel2.Resize += Panel2_Resize;
        }

        static Dictionary<int, int> charTransformation = new Dictionary<int, int>()
        {
            { 0x1, 0x2F },
            { 0x2, 0x30 },
            { 0x3, 0x31 },
            { 0x4, 0x32 },
            { 0xE, 0x33 },
            { 0x22, 0x34 },
            { 0x23, 0x35 },
            { 0x24, 0x36 },
            { 0x25, 0x37 },
            { 0x26, 0x38 },
            { 0x39, 0x49 },
            { 0x3F, 0x4B }
        };

        private static void Panel2_Resize(object sender, EventArgs e)
        {
            ArrangeCharPicBoxes();
        }

        private static void PicR1_Click(object sender, EventArgs e)
        {
            if (charTransformation.TryGetValue(CharSelID[SelectedID], out int transformedID))
            {
                CharSelID[SelectedID] = transformedID;
                CharSelect(charIcon);

                int originalKey = CharSelID[SelectedID] == transformedID //Invert Value
                    ? charTransformation.First(x => x.Value == transformedID).Key
                    : transformedID;

                charTransformation.Remove(originalKey); //Remove
                charTransformation[transformedID] = originalKey; //Add New Key and Value
                mainF.picR1.Visible = true;
            }
        }

        private static void CreateCharPicBoxes()
        {
            for (int i = 0; i < 44; i++)
            {
                PictureBox pic = Clone(mainF.pictureBox2);

                pic.Image = CharIcons[CharSelID[i]];
                pic.MouseClick += Pic_Click;
                pic.Tag = $"Char_{i}";

                mainF.panel2.Controls.Add(pic);

                if (i == 1)
                    CharSelect(pic);
            }

            ArrangeCharPicBoxes();
        }

        private static void ArrangeCharPicBoxes()
        {
            const int rows = 2;
            const int spacingX = 38;
            const int spacingY = 46;
            const int offsetX = 10;

            int iconWidth = mainF.pictureBox2.Width;
            int availableWidth = mainF.panel2.ClientSize.Width - offsetX;

            // Quantas colunas cabem de fato na largura atual
            int maxColumns = Math.Max(1, (availableWidth - iconWidth) / spacingX + 1);
            int maxVisibleItems = maxColumns * rows;

            int offsetY = mainF.panel2.ClientSize.Height - 5;

            int index = 0;

            foreach (Control control in mainF.panel2.Controls)
            {
                if (control is PictureBox pic &&
                    pic != selIcon &&
                    pic.Tag?.ToString().StartsWith("Char_") == true)
                {
                    if (index < maxVisibleItems)
                    {
                        int col = index / rows;
                        int row = index % rows;

                        int xPos = offsetX + col * spacingX;
                        int yPos = offsetY - pic.Height - (row * spacingY);

                        pic.Location = new Point(xPos, yPos);
                        pic.Visible = true;
                    }
                    else
                    {
                        pic.Visible = false; // não cabe, esconde
                    }

                    index++;
                }
            }

            string currentTag = $"Char_{SelectedID}";
            foreach (Control control in mainF.panel2.Controls)
            {
                if (control is PictureBox pic && pic != selIcon && pic.Tag?.ToString() == currentTag)
                {
                    selIcon.Location = pic.Location;
                    selIcon.Size = pic.Size;
                    selIcon.Visible = pic.Visible;
                    selIcon.BringToFront();
                    break;
                }
            }
        }


        private static void CreateCommandImages()
        {
            CCS gaugeFile = new CCS();
            gaugeFile.Read(Path.Combine(GAME.gamePath, "DATA\\ROFS\\CMN\\GAUGE.CCS"));
            Bitmap xCommandTexture = gaugeFile.GetCCSImage("XCOMMAND.BMP");

            //D-Pad
            for (int i = 0; i < 4; i++)
            {
                var icon = new Bitmap(32, 32);
                var g = Graphics.FromImage(icon);
                var srcRect = new Rectangle(i * 32, 0, 32, 32);
                g.DrawImage(xCommandTexture, 0, 0, srcRect, GraphicsUnit.Pixel);
                CommandIcons.Add(icon);
                g.Dispose();
            }

            //Buttons
            for (int i = 0; i < 5; i++)
            {
                var icon = new Bitmap(24, 32);
                var g = Graphics.FromImage(icon);
                var srcRect = new Rectangle(i * 24, 32, 24, 32);
                g.DrawImage(xCommandTexture, 0, 0, srcRect, GraphicsUnit.Pixel);
                CommandIcons.Add(icon);
                g.Dispose();
            }
        }

        private static void PicArrowLeft_Click(object sender, EventArgs e) => ArrowRightLeftClick(true);

        private static void PicArrowRight_Click(object sender, EventArgs e) => ArrowRightLeftClick(false);

        private static void ArrowRightLeftClick(bool direction)
        {
            selIcon.Visible = false;

            int selTagID = int.Parse(
                selIcon.Tag.ToString().Split('_')[1]
            );

            selIcon.Tag = $"Char_{(direction
                ? (selTagID + 2)
                : selTagID - 2 + (GAME.charSelCount * 2))
                % (GAME.charSelCount * 2)}";

            foreach (Control control in mainF.panel2.Controls)
            {
                PictureBox pic = control as PictureBox;

                if (pic != null && pic.Tag != null)
                {
                    string tagString = pic.Tag.ToString();

                    if (tagString.StartsWith("Char_"))
                    {
                        string[] partes = tagString.Split('_');

                        if (partes.Length == 2 &&
                            int.TryParse(partes[1], out int charID))
                        {
                            int newIndex = direction
                                ? (charID - 2 + (GAME.charSelCount * 2)) % (GAME.charSelCount * 2)
                                : (charID + 2) % (GAME.charSelCount * 2);

                            pic.Tag = $"Char_{newIndex}";

                            if (selIcon.Tag != null &&
                                pic.Tag.ToString() == selIcon.Tag.ToString() &&
                                selIcon.Location != pic.Location)
                            {
                                selIcon.Location = new Point(
                                    pic.Location.X,
                                    pic.Location.Y
                                );

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
            if (e.Button == MouseButtons.Right)
            {
                BTL.UpdateMatch(true, CharSelID[SelectedID], 0);
            }
        }
        static void CharSelect(PictureBox pictureBox)
        {
            charIcon = pictureBox;
            if (charIcon != null)
            {
                mainF.pictureBox3.Image = charPicturePicBoxes[CharSelID[Convert.ToInt32(charIcon.Tag.ToString().Split('_')[1])]];
                charIcon.Image = CharIcons[CharSelID[Convert.ToInt32(charIcon.Tag.ToString().Split('_')[1])]];
                SelectedID = Convert.ToInt32(charIcon.Tag.ToString().Split('_')[1]);
                Bitmap teste = MesclarBitmaps(new Bitmap(charIcon.Image), new Bitmap(selIconImage));
                selIcon.Image = teste;
                selIcon.Visible = true;
                selIcon.Size = charIcon.Size;
                selIcon.Location = new Point(charIcon.Location.X, charIcon.Location.Y);
                selIcon.Tag = charIcon.Tag;
                selIcon.BringToFront();

                mainF.picR1.Visible = charTransformation.ContainsKey(CharSelID[SelectedID]) == true ? true : false;
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

            return clonePic;
        }

        public static void ReadAllCharIcon(string gamePath)
        {
            Bitmap charselTexture = charselFile.GetCCSImage("PURECHARSEL10.BMP");
            byte[] modData = GAME.isUN6 != true ? 
                             File.ReadAllBytes(GAME.GetELFPathInSystemCNF(gamePath)) : 
                             File.ReadAllBytes(gamePath + "\\PRG\\DLC.BIN");

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
                 File.ReadAllBytes(GAME.GetELFPathInSystemCNF(gamePath)) :
                 File.ReadAllBytes(gamePath + "\\PRG\\DLC.BIN");

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
                        purecharsel = charselFile.GetCCSImage($"PURECHARSEL{pureIndex:00}.BMP");
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
            GAME.elfPath = GAME.GetELFPathInSystemCNF(gamePath);
            byte[] modData = File.ReadAllBytes(GAME.elfPath);
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
