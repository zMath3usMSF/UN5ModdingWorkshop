namespace WindowsFormsApp1
{
    partial class Main
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pCSX2MemoryProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openELFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeCharacterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeP1CharacterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeP2CharacterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewCharacterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoADVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractCVMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeGzlistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblProgress = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnEditJutsusParameters = new System.Windows.Forms.Button();
            this.btnEditAwekeningParameters = new System.Windows.Forms.Button();
            this.btnEditMovesetParameters = new System.Windows.Forms.Button();
            this.btnEditGeneralParameters = new System.Windows.Forms.Button();
            this.picArrowRight = new System.Windows.Forms.PictureBox();
            this.picArrowLeft = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnSelectGamePath = new System.Windows.Forms.Button();
            this.txtGamePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.picBackground = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picArrowRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picArrowLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.tabControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.optionsToolStripMenuItem1,
            this.gameToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(863, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pCSX2MemoryProcessToolStripMenuItem,
            this.openELFToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // pCSX2MemoryProcessToolStripMenuItem
            // 
            this.pCSX2MemoryProcessToolStripMenuItem.Name = "pCSX2MemoryProcessToolStripMenuItem";
            this.pCSX2MemoryProcessToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pCSX2MemoryProcessToolStripMenuItem.Text = "PCSX2 Process";
            this.pCSX2MemoryProcessToolStripMenuItem.Click += new System.EventHandler(this.pCSX2MemoryProcessToolStripMenuItem_Click);
            // 
            // openELFToolStripMenuItem
            // 
            this.openELFToolStripMenuItem.Name = "openELFToolStripMenuItem";
            this.openELFToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openELFToolStripMenuItem.Text = "ELF";
            this.openELFToolStripMenuItem.Click += new System.EventHandler(this.openELFToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeCharacterToolStripMenuItem,
            this.addNewCharacterToolStripMenuItem,
            this.infoADVToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.optionsToolStripMenuItem.Text = "Util";
            // 
            // changeCharacterToolStripMenuItem
            // 
            this.changeCharacterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeP1CharacterToolStripMenuItem,
            this.changeP2CharacterToolStripMenuItem});
            this.changeCharacterToolStripMenuItem.Name = "changeCharacterToolStripMenuItem";
            this.changeCharacterToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.changeCharacterToolStripMenuItem.Text = "Change Character";
            // 
            // changeP1CharacterToolStripMenuItem
            // 
            this.changeP1CharacterToolStripMenuItem.Name = "changeP1CharacterToolStripMenuItem";
            this.changeP1CharacterToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.changeP1CharacterToolStripMenuItem.Text = "Change P1 Character";
            this.changeP1CharacterToolStripMenuItem.Click += new System.EventHandler(this.changeP1CharacterToolStripMenuItem_Click);
            // 
            // changeP2CharacterToolStripMenuItem
            // 
            this.changeP2CharacterToolStripMenuItem.Name = "changeP2CharacterToolStripMenuItem";
            this.changeP2CharacterToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.changeP2CharacterToolStripMenuItem.Text = "Change P2 Character";
            this.changeP2CharacterToolStripMenuItem.Click += new System.EventHandler(this.changeP2CharacterToolStripMenuItem_Click);
            // 
            // addNewCharacterToolStripMenuItem
            // 
            this.addNewCharacterToolStripMenuItem.Name = "addNewCharacterToolStripMenuItem";
            this.addNewCharacterToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.addNewCharacterToolStripMenuItem.Text = "Add New Character";
            this.addNewCharacterToolStripMenuItem.Click += new System.EventHandler(this.addNewCharacterToolStripMenuItem_Click);
            // 
            // infoADVToolStripMenuItem
            // 
            this.infoADVToolStripMenuItem.Name = "infoADVToolStripMenuItem";
            this.infoADVToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.infoADVToolStripMenuItem.Text = "Info ADV";
            this.infoADVToolStripMenuItem.Click += new System.EventHandler(this.infoADVToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem1
            // 
            this.optionsToolStripMenuItem1.Name = "optionsToolStripMenuItem1";
            this.optionsToolStripMenuItem1.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem1.Text = "Options";
            this.optionsToolStripMenuItem1.Visible = false;
            // 
            // gameToolStripMenuItem
            // 
            this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractCVMToolStripMenuItem,
            this.buildGameToolStripMenuItem,
            this.makeGzlistToolStripMenuItem});
            this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            this.gameToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.gameToolStripMenuItem.Text = "Game";
            this.gameToolStripMenuItem.Click += new System.EventHandler(this.gameToolStripMenuItem_Click);
            // 
            // extractCVMToolStripMenuItem
            // 
            this.extractCVMToolStripMenuItem.Name = "extractCVMToolStripMenuItem";
            this.extractCVMToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.extractCVMToolStripMenuItem.Text = "Extract Game";
            this.extractCVMToolStripMenuItem.Click += new System.EventHandler(this.extractCVMToolStripMenuItem_Click_1);
            // 
            // buildGameToolStripMenuItem
            // 
            this.buildGameToolStripMenuItem.Name = "buildGameToolStripMenuItem";
            this.buildGameToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.buildGameToolStripMenuItem.Text = "Build Game";
            // 
            // makeGzlistToolStripMenuItem
            // 
            this.makeGzlistToolStripMenuItem.Name = "makeGzlistToolStripMenuItem";
            this.makeGzlistToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.makeGzlistToolStripMenuItem.Text = "MakeGzlist";
            this.makeGzlistToolStripMenuItem.Visible = false;
            this.makeGzlistToolStripMenuItem.Click += new System.EventHandler(this.makeGzlistToolStripMenuItem_Click_1);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblProgress);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 423);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(863, 15);
            this.panel1.TabIndex = 17;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblProgress.Location = new System.Drawing.Point(853, 0);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(10, 13);
            this.lblProgress.TabIndex = 0;
            this.lblProgress.Text = ".";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnEditJutsusParameters);
            this.tabPage1.Controls.Add(this.btnEditAwekeningParameters);
            this.tabPage1.Controls.Add(this.btnEditMovesetParameters);
            this.tabPage1.Controls.Add(this.btnEditGeneralParameters);
            this.tabPage1.Controls.Add(this.picArrowRight);
            this.tabPage1.Controls.Add(this.picArrowLeft);
            this.tabPage1.Controls.Add(this.pictureBox2);
            this.tabPage1.Controls.Add(this.btnSelectGamePath);
            this.tabPage1.Controls.Add(this.txtGamePath);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.pictureBox3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(855, 376);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Character";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnEditJutsusParameters
            // 
            this.btnEditJutsusParameters.Location = new System.Drawing.Point(83, 121);
            this.btnEditJutsusParameters.Name = "btnEditJutsusParameters";
            this.btnEditJutsusParameters.Size = new System.Drawing.Size(151, 23);
            this.btnEditJutsusParameters.TabIndex = 19;
            this.btnEditJutsusParameters.Text = "Edit Jutsu";
            this.btnEditJutsusParameters.UseVisualStyleBackColor = true;
            this.btnEditJutsusParameters.Visible = false;
            // 
            // btnEditAwekeningParameters
            // 
            this.btnEditAwekeningParameters.Location = new System.Drawing.Point(83, 92);
            this.btnEditAwekeningParameters.Name = "btnEditAwekeningParameters";
            this.btnEditAwekeningParameters.Size = new System.Drawing.Size(151, 23);
            this.btnEditAwekeningParameters.TabIndex = 18;
            this.btnEditAwekeningParameters.Text = "Edit Awekeninng";
            this.btnEditAwekeningParameters.UseVisualStyleBackColor = true;
            this.btnEditAwekeningParameters.Visible = false;
            this.btnEditAwekeningParameters.Click += new System.EventHandler(this.btnEditAwekeningParameters_Click);
            // 
            // btnEditMovesetParameters
            // 
            this.btnEditMovesetParameters.Location = new System.Drawing.Point(83, 63);
            this.btnEditMovesetParameters.Name = "btnEditMovesetParameters";
            this.btnEditMovesetParameters.Size = new System.Drawing.Size(151, 23);
            this.btnEditMovesetParameters.TabIndex = 17;
            this.btnEditMovesetParameters.Text = "Edit Moveset";
            this.btnEditMovesetParameters.UseVisualStyleBackColor = true;
            this.btnEditMovesetParameters.Visible = false;
            this.btnEditMovesetParameters.Click += new System.EventHandler(this.btnEditMovesetParameters_Click);
            // 
            // btnEditGeneralParameters
            // 
            this.btnEditGeneralParameters.Location = new System.Drawing.Point(83, 34);
            this.btnEditGeneralParameters.Name = "btnEditGeneralParameters";
            this.btnEditGeneralParameters.Size = new System.Drawing.Size(151, 23);
            this.btnEditGeneralParameters.TabIndex = 16;
            this.btnEditGeneralParameters.Text = "Edit Atributtes";
            this.btnEditGeneralParameters.UseVisualStyleBackColor = true;
            this.btnEditGeneralParameters.Visible = false;
            this.btnEditGeneralParameters.Click += new System.EventHandler(this.btnEditGeneralParameters_Click);
            // 
            // picArrowRight
            // 
            this.picArrowRight.Location = new System.Drawing.Point(847, 311);
            this.picArrowRight.Name = "picArrowRight";
            this.picArrowRight.Size = new System.Drawing.Size(10, 14);
            this.picArrowRight.TabIndex = 6;
            this.picArrowRight.TabStop = false;
            // 
            // picArrowLeft
            // 
            this.picArrowLeft.Location = new System.Drawing.Point(-2, 311);
            this.picArrowLeft.Name = "picArrowLeft";
            this.picArrowLeft.Size = new System.Drawing.Size(10, 14);
            this.picArrowLeft.TabIndex = 5;
            this.picArrowLeft.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(9, 322);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(38, 46);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // btnSelectGamePath
            // 
            this.btnSelectGamePath.Location = new System.Drawing.Point(825, 6);
            this.btnSelectGamePath.Name = "btnSelectGamePath";
            this.btnSelectGamePath.Size = new System.Drawing.Size(28, 23);
            this.btnSelectGamePath.TabIndex = 2;
            this.btnSelectGamePath.Text = "...";
            this.btnSelectGamePath.UseVisualStyleBackColor = true;
            this.btnSelectGamePath.Click += new System.EventHandler(this.btnSelectGamePath_Click);
            // 
            // txtGamePath
            // 
            this.txtGamePath.Location = new System.Drawing.Point(456, 8);
            this.txtGamePath.Name = "txtGamePath";
            this.txtGamePath.ReadOnly = true;
            this.txtGamePath.Size = new System.Drawing.Size(363, 20);
            this.txtGamePath.TabIndex = 1;
            this.txtGamePath.TextChanged += new System.EventHandler(this.txtGamePath_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(387, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Game Path:";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(319, 34);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(252, 243);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 4;
            this.pictureBox3.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(0, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(863, 402);
            this.tabControl1.TabIndex = 16;
            this.tabControl1.Visible = false;
            // 
            // picBackground
            // 
            this.picBackground.Image = global::UN5ModdingWorkshop.Properties.Resources.BackgroundImage;
            this.picBackground.Location = new System.Drawing.Point(0, 22227);
            this.picBackground.Name = "picBackground";
            this.picBackground.Size = new System.Drawing.Size(863, 398);
            this.picBackground.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBackground.TabIndex = 20;
            this.picBackground.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(863, 438);
            this.Controls.Add(this.picBackground);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "Ultimate Ninja 5: Modding Workshop";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picArrowRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picArrowLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.tabControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pCSX2MemoryProcessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openELFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeCharacterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeP1CharacterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeP2CharacterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem1;
        public System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addNewCharacterToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ToolStripMenuItem infoADVToolStripMenuItem;
        public System.Windows.Forms.TabPage tabPage1;
        public System.Windows.Forms.PictureBox picArrowRight;
        public System.Windows.Forms.PictureBox picArrowLeft;
        public System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button btnSelectGamePath;
        public System.Windows.Forms.TextBox txtGamePath;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.PictureBox pictureBox3;
        public System.Windows.Forms.TabControl tabControl1;
        public System.Windows.Forms.Button btnEditGeneralParameters;
        public System.Windows.Forms.Button btnEditMovesetParameters;
        public System.Windows.Forms.Button btnEditJutsusParameters;
        public System.Windows.Forms.Button btnEditAwekeningParameters;
        private System.Windows.Forms.ToolStripMenuItem gameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractCVMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeGzlistToolStripMenuItem;
        public System.Windows.Forms.PictureBox picBackground;
    }
}

