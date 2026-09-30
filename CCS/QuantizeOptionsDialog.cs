using System;
using System.Drawing;
using System.Windows.Forms;

namespace UN5ModdingWorkshop
{
    public enum QuantizationPriority
    {
        Color,
        Opacity
    }

    public class QuantizeOptionsDialog : Form
    {
        public int SelectedColorCount { get; private set; } = 256;
        public QuantizationPriority SelectedPriority { get; private set; } = QuantizationPriority.Color;

        private RadioButton rb16;
        private RadioButton rb256;
        private RadioButton rbColor;
        private RadioButton rbOpacity;

        public QuantizeOptionsDialog()
        {
            Text = "Opções de importação de textura";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(340, 260);

            var grpColors = new GroupBox { Text = "Quantidade de cores", Location = new Point(15, 15), Size = new Size(300, 80) };
            rb16 = new RadioButton { Text = "16 cores (4bpp / PSMT4)", Location = new Point(15, 25), AutoSize = true };
            rb256 = new RadioButton { Text = "256 cores (8bpp / PSMT8)", Location = new Point(15, 50), AutoSize = true, Checked = true };
            grpColors.Controls.Add(rb16);
            grpColors.Controls.Add(rb256);

            var grpPriority = new GroupBox { Text = "Prioridade de redução", Location = new Point(15, 105), Size = new Size(300, 80) };
            rbColor = new RadioButton { Text = "Priorizar cores (melhor gradiente de cor)", Location = new Point(15, 25), AutoSize = true, Checked = true };
            rbOpacity = new RadioButton { Text = "Priorizar opacidade (melhor transparência)", Location = new Point(15, 50), AutoSize = true };
            grpPriority.Controls.Add(rbColor);
            grpPriority.Controls.Add(rbOpacity);

            var btnOk = new Button { Text = "OK", Location = new Point(160, 195), Size = new Size(75, 25), DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Cancelar", Location = new Point(245, 195), Size = new Size(75, 25), DialogResult = DialogResult.Cancel };

            btnOk.Click += (s, e) =>
            {
                SelectedColorCount = rb16.Checked ? 16 : 256;
                SelectedPriority = rbColor.Checked ? QuantizationPriority.Color : QuantizationPriority.Opacity;
            };

            Controls.AddRange(new Control[] { grpColors, grpPriority, btnOk, btnCancel });
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }
    }
}