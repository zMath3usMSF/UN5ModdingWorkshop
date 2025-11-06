namespace UN5ModdingWorkshop
{
    partial class UpdateMatch
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
            this.lblPlayerID = new System.Windows.Forms.Label();
            this.lblMapID = new System.Windows.Forms.Label();
            this.btnChange = new System.Windows.Forms.Button();
            this.cmbCharList = new System.Windows.Forms.ComboBox();
            this.cmbMapList = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblPlayerID
            // 
            this.lblPlayerID.AutoSize = true;
            this.lblPlayerID.Location = new System.Drawing.Point(12, 9);
            this.lblPlayerID.Name = "lblPlayerID";
            this.lblPlayerID.Size = new System.Drawing.Size(23, 13);
            this.lblPlayerID.TabIndex = 1;
            this.lblPlayerID.Text = "P1:";
            // 
            // lblMapID
            // 
            this.lblMapID.AutoSize = true;
            this.lblMapID.Location = new System.Drawing.Point(231, 9);
            this.lblMapID.Name = "lblMapID";
            this.lblMapID.Size = new System.Drawing.Size(31, 13);
            this.lblMapID.TabIndex = 3;
            this.lblMapID.Text = "Map:";
            // 
            // btnChange
            // 
            this.btnChange.Location = new System.Drawing.Point(387, 47);
            this.btnChange.Name = "btnChange";
            this.btnChange.Size = new System.Drawing.Size(75, 23);
            this.btnChange.TabIndex = 4;
            this.btnChange.Text = "Change!";
            this.btnChange.UseVisualStyleBackColor = true;
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // cmbCharList
            // 
            this.cmbCharList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCharList.FormattingEnabled = true;
            this.cmbCharList.Location = new System.Drawing.Point(41, 5);
            this.cmbCharList.Name = "cmbCharList";
            this.cmbCharList.Size = new System.Drawing.Size(184, 21);
            this.cmbCharList.TabIndex = 5;
            // 
            // cmbMapList
            // 
            this.cmbMapList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMapList.FormattingEnabled = true;
            this.cmbMapList.Location = new System.Drawing.Point(268, 5);
            this.cmbMapList.Name = "cmbMapList";
            this.cmbMapList.Size = new System.Drawing.Size(194, 21);
            this.cmbMapList.TabIndex = 6;
            // 
            // UpdateMatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 82);
            this.Controls.Add(this.cmbMapList);
            this.Controls.Add(this.cmbCharList);
            this.Controls.Add(this.btnChange);
            this.Controls.Add(this.lblMapID);
            this.Controls.Add(this.lblPlayerID);
            this.MaximizeBox = false;
            this.Name = "UpdateMatch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update Match";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnChange;
        public System.Windows.Forms.Label lblPlayerID;
        public System.Windows.Forms.Label lblMapID;
        private System.Windows.Forms.ComboBox cmbCharList;
        private System.Windows.Forms.ComboBox cmbMapList;
    }
}