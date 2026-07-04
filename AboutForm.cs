using System;
using System.Drawing;
using System.Windows.Forms;

namespace UN5ModdingWorkshop
{
    public class AboutForm : Form
    {
        public AboutForm()
        {
            Text = "About";
            Size = new Size(460, 640);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                AutoScroll = false,
                Padding = new Padding(20)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            layout.Controls.Add(CreateTitle("Ultimate Ninja 5: Modding Workshop", "v2.0"));
            layout.Controls.Add(CreateSpacer());
            layout.Controls.Add(CreateLabel("Made by zMath3usMSF", bold: true, size: 10f));
            layout.Controls.Add(CreateSpacer());

            layout.Controls.Add(CreateSectionTitle("God (Deus)"));
            layout.Controls.Add(CreateLabel("\"So whether you eat or drink, or whatever you do, do it all for the glory of God.\"", size: 9f));
            layout.Controls.Add(CreateLabel("1 Corinthians 10:31 | ESV", bold: true, size: 8.5f));
            layout.Controls.Add(CreateSpacer());
            layout.Controls.Add(CreateLabel("\"Portanto, quer comais quer bebais, ou façais qualquer outra coisa, fazei tudo para glória de Deus.\"", size: 9f));
            layout.Controls.Add(CreateLabel("1 Coríntios 10:31 | ACF", bold: true, size: 8.5f));

            layout.Controls.Add(CreateSectionTitle("Credits (CCSF)"));
            layout.Controls.Add(CreateLabel("WarrantyVoider"));
            layout.Controls.Add(CreateLabel("NCDyson"));
            layout.Controls.Add(CreateLabel("Bit.Raiden"));
            layout.Controls.Add(CreateLabel("HydraBlazeZ"));
            layout.Controls.Add(CreateLabel("zMath3usMSF"));
            layout.Controls.Add(CreateSpacer());

            layout.Controls.Add(CreateSectionTitle("Cheat Credits"));
            layout.Controls.Add(CreateLabel("NTSCMode — Bit.Raiden, zMath3usMSF"));
            layout.Controls.Add(CreateLabel("NoLinkedCharacter — zMath3usMSF"));
            layout.Controls.Add(CreateLabel("NoExtraHit — Shalashaska, zMath3usMSF"));
            layout.Controls.Add(CreateLabel("NoJankenpon — Shalashaska"));

            layout.Controls.Add(CreateSectionTitle("Input/Output Code and Other Helps"));
            layout.Controls.Add(CreateLabel("I/O Code and Helps — Bit.Raiden"));
            layout.Controls.Add(CreateSpacer());

            Controls.Add(layout);
        }

        private Control CreateTitle(string main, string sub)
        {
            var panel = new Panel
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                Width = 360
            };

            var mainLabel = new Label
            {
                Text = main,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                AutoSize = false,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 40
            };

            var subLabel = new Label
            {
                Text = sub,
                Font = new Font("Segoe UI", 10f, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = false,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 20
            };

            panel.Controls.Add(subLabel);
            panel.Controls.Add(mainLabel);

            return panel;
        }

        private Control CreateSectionTitle(string text)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 90, 200),
                AutoSize = true,
                MaximumSize = new Size(360, 0),
                Margin = new Padding(0, 8, 0, 4)
            };
        }

        private Control CreateLabel(string text, bool bold = false, float size = 9.5f)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
                AutoSize = true,
                MaximumSize = new Size(360, 0),
                Margin = new Padding(4, 1, 0, 1)
            };
        }

        private Control CreateSpacer()
        {
            return new Label { Height = 6, AutoSize = false };
        }
    }
}