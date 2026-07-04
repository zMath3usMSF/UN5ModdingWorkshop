namespace UN5ModdingWorkshop
{
    partial class CheatsMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkNTSCMode = new System.Windows.Forms.CheckBox();
            this.chkNoLinkedCharacter = new System.Windows.Forms.CheckBox();
            this.chkNoExtraHit = new System.Windows.Forms.CheckBox();
            this.chkNoJankenpon = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkNTSCMode
            // 
            this.chkNTSCMode.AutoSize = true;
            this.chkNTSCMode.Location = new System.Drawing.Point(12, 12);
            this.chkNTSCMode.Name = "chkNTSCMode";
            this.chkNTSCMode.Size = new System.Drawing.Size(159, 17);
            this.chkNTSCMode.TabIndex = 0;
            this.chkNTSCMode.Text = "NTSC Mode (30 FPS Mode)";
            this.chkNTSCMode.UseVisualStyleBackColor = true;
            // 
            // chkNoLinkedCharacter
            // 
            this.chkNoLinkedCharacter.AutoSize = true;
            this.chkNoLinkedCharacter.Location = new System.Drawing.Point(12, 35);
            this.chkNoLinkedCharacter.Name = "chkNoLinkedCharacter";
            this.chkNoLinkedCharacter.Size = new System.Drawing.Size(158, 17);
            this.chkNoLinkedCharacter.TabIndex = 1;
            this.chkNoLinkedCharacter.Text = "No Linked Character Option";
            this.chkNoLinkedCharacter.UseVisualStyleBackColor = true;
            // 
            // chkNoExtraHit
            // 
            this.chkNoExtraHit.AutoSize = true;
            this.chkNoExtraHit.Location = new System.Drawing.Point(12, 58);
            this.chkNoExtraHit.Name = "chkNoExtraHit";
            this.chkNoExtraHit.Size = new System.Drawing.Size(83, 17);
            this.chkNoExtraHit.TabIndex = 2;
            this.chkNoExtraHit.Text = "No Extra-Hit";
            this.chkNoExtraHit.UseVisualStyleBackColor = true;
            // 
            // chkNoJankenpon
            // 
            this.chkNoJankenpon.AutoSize = true;
            this.chkNoJankenpon.Location = new System.Drawing.Point(12, 81);
            this.chkNoJankenpon.Name = "chkNoJankenpon";
            this.chkNoJankenpon.Size = new System.Drawing.Size(96, 17);
            this.chkNoJankenpon.TabIndex = 3;
            this.chkNoJankenpon.Text = "No Jankenpon";
            this.chkNoJankenpon.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(274, 110);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save to ELF";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // CheatsMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 145);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkNoJankenpon);
            this.Controls.Add(this.chkNoExtraHit);
            this.Controls.Add(this.chkNoLinkedCharacter);
            this.Controls.Add(this.chkNTSCMode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "CheatsMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cheats";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnSave;
        public System.Windows.Forms.CheckBox chkNTSCMode;
        public System.Windows.Forms.CheckBox chkNoLinkedCharacter;
        public System.Windows.Forms.CheckBox chkNoExtraHit;
        public System.Windows.Forms.CheckBox chkNoJankenpon;
    }
}