namespace UN5ModdingWorkshop
{
    partial class CCSF_Editor
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
            this.treeViewFilelist = new System.Windows.Forms.TreeView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeViewCCS = new System.Windows.Forms.TreeView();
            this.picCCSImage = new System.Windows.Forms.PictureBox();
            this.btnImportTexture = new System.Windows.Forms.Button();
            this.btnExportTexture = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.exportAllCCSImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importAllCCSImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCCSImage)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewFilelist
            // 
            this.treeViewFilelist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeViewFilelist.Location = new System.Drawing.Point(12, 27);
            this.treeViewFilelist.Name = "treeViewFilelist";
            this.treeViewFilelist.Size = new System.Drawing.Size(419, 517);
            this.treeViewFilelist.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.testeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1140, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportAllCCSImagesToolStripMenuItem,
            this.importAllCCSImagesToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // treeViewCCS
            // 
            this.treeViewCCS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeViewCCS.Location = new System.Drawing.Point(437, 27);
            this.treeViewCCS.Name = "treeViewCCS";
            this.treeViewCCS.Size = new System.Drawing.Size(376, 517);
            this.treeViewCCS.TabIndex = 3;
            // 
            // picCCSImage
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.picCCSImage, 2);
            this.picCCSImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picCCSImage.Location = new System.Drawing.Point(3, 43);
            this.picCCSImage.Name = "picCCSImage";
            this.picCCSImage.Size = new System.Drawing.Size(303, 471);
            this.picCCSImage.TabIndex = 4;
            this.picCCSImage.TabStop = false;
            // 
            // btnImportTexture
            // 
            this.btnImportTexture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImportTexture.Location = new System.Drawing.Point(157, 3);
            this.btnImportTexture.Name = "btnImportTexture";
            this.btnImportTexture.Size = new System.Drawing.Size(149, 34);
            this.btnImportTexture.TabIndex = 5;
            this.btnImportTexture.Text = "Import";
            this.btnImportTexture.UseVisualStyleBackColor = true;
            this.btnImportTexture.Click += new System.EventHandler(this.btnReplaceTexture_Click);
            // 
            // btnExportTexture
            // 
            this.btnExportTexture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExportTexture.Location = new System.Drawing.Point(3, 3);
            this.btnExportTexture.Name = "btnExportTexture";
            this.btnExportTexture.Size = new System.Drawing.Size(148, 34);
            this.btnExportTexture.TabIndex = 6;
            this.btnExportTexture.Text = "Export";
            this.btnExportTexture.UseVisualStyleBackColor = true;
            this.btnExportTexture.Click += new System.EventHandler(this.btnExportTexture_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.picCCSImage, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnExportTexture, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnImportTexture, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(819, 27);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(309, 517);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // exportAllCCSImagesToolStripMenuItem
            // 
            this.exportAllCCSImagesToolStripMenuItem.Name = "exportAllCCSImagesToolStripMenuItem";
            this.exportAllCCSImagesToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.exportAllCCSImagesToolStripMenuItem.Text = "Export All CCS Images";
            // 
            // importAllCCSImagesToolStripMenuItem
            // 
            this.importAllCCSImagesToolStripMenuItem.Name = "importAllCCSImagesToolStripMenuItem";
            this.importAllCCSImagesToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.importAllCCSImagesToolStripMenuItem.Text = "Import All CCS Images";
            // 
            // testeToolStripMenuItem
            // 
            this.testeToolStripMenuItem.Name = "testeToolStripMenuItem";
            this.testeToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.testeToolStripMenuItem.Text = "Teste";
            this.testeToolStripMenuItem.Click += new System.EventHandler(this.testeToolStripMenuItem_Click);
            // 
            // CCSF_Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1140, 556);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.treeViewCCS);
            this.Controls.Add(this.treeViewFilelist);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "CCSF_Editor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CCS Editor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCCSImage)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TreeView treeViewFilelist;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        public System.Windows.Forms.TreeView treeViewCCS;
        public System.Windows.Forms.PictureBox picCCSImage;
        public System.Windows.Forms.Button btnImportTexture;
        public System.Windows.Forms.Button btnExportTexture;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStripMenuItem exportAllCCSImagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importAllCCSImagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testeToolStripMenuItem;
    }
}