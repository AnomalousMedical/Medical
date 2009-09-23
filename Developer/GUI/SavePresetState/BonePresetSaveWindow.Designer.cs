namespace Medical.GUI
{
    partial class BonePresetSaveWindow
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
            this.nameText = new System.Windows.Forms.TextBox();
            this.categoryText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.previewPicture = new System.Windows.Forms.PictureBox();
            this.typeCombo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.refreshImageButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rightSideSource = new System.Windows.Forms.RadioButton();
            this.leftSideSource = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.outputDirectoryText = new System.Windows.Forms.TextBox();
            this.outDirBrowseButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.previewPicture)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameText
            // 
            this.nameText.Location = new System.Drawing.Point(76, 53);
            this.nameText.Name = "nameText";
            this.nameText.Size = new System.Drawing.Size(180, 20);
            this.nameText.TabIndex = 0;
            // 
            // categoryText
            // 
            this.categoryText.Location = new System.Drawing.Point(76, 80);
            this.categoryText.Name = "categoryText";
            this.categoryText.Size = new System.Drawing.Size(180, 20);
            this.categoryText.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Category";
            // 
            // previewPicture
            // 
            this.previewPicture.Location = new System.Drawing.Point(3, 168);
            this.previewPicture.Name = "previewPicture";
            this.previewPicture.Size = new System.Drawing.Size(250, 250);
            this.previewPicture.TabIndex = 4;
            this.previewPicture.TabStop = false;
            // 
            // typeCombo
            // 
            this.typeCombo.FormattingEnabled = true;
            this.typeCombo.Items.AddRange(new object[] {
            "Growth Defect",
            "Degeneration"});
            this.typeCombo.Location = new System.Drawing.Point(76, 27);
            this.typeCombo.Name = "typeCombo";
            this.typeCombo.Size = new System.Drawing.Size(180, 21);
            this.typeCombo.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Type";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 152);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Image Preview";
            // 
            // refreshImageButton
            // 
            this.refreshImageButton.Location = new System.Drawing.Point(3, 424);
            this.refreshImageButton.Name = "refreshImageButton";
            this.refreshImageButton.Size = new System.Drawing.Size(89, 23);
            this.refreshImageButton.TabIndex = 10;
            this.refreshImageButton.Text = "Refresh Image";
            this.refreshImageButton.UseVisualStyleBackColor = true;
            this.refreshImageButton.Click += new System.EventHandler(this.refreshImageButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(6, 456);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 11;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rightSideSource);
            this.groupBox1.Controls.Add(this.leftSideSource);
            this.groupBox1.Location = new System.Drawing.Point(6, 104);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(157, 44);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source Data";
            // 
            // rightSideSource
            // 
            this.rightSideSource.AutoSize = true;
            this.rightSideSource.Location = new System.Drawing.Point(79, 19);
            this.rightSideSource.Name = "rightSideSource";
            this.rightSideSource.Size = new System.Drawing.Size(74, 17);
            this.rightSideSource.TabIndex = 15;
            this.rightSideSource.TabStop = true;
            this.rightSideSource.Text = "Right Side";
            this.rightSideSource.UseVisualStyleBackColor = true;
            // 
            // leftSideSource
            // 
            this.leftSideSource.AutoSize = true;
            this.leftSideSource.Location = new System.Drawing.Point(6, 19);
            this.leftSideSource.Name = "leftSideSource";
            this.leftSideSource.Size = new System.Drawing.Size(67, 17);
            this.leftSideSource.TabIndex = 14;
            this.leftSideSource.TabStop = true;
            this.leftSideSource.Text = "Left Side";
            this.leftSideSource.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Output Directory";
            // 
            // outputDirectoryText
            // 
            this.outputDirectoryText.Location = new System.Drawing.Point(86, 3);
            this.outputDirectoryText.Name = "outputDirectoryText";
            this.outputDirectoryText.Size = new System.Drawing.Size(138, 20);
            this.outputDirectoryText.TabIndex = 14;
            // 
            // outDirBrowseButton
            // 
            this.outDirBrowseButton.Location = new System.Drawing.Point(229, 2);
            this.outDirBrowseButton.Name = "outDirBrowseButton";
            this.outDirBrowseButton.Size = new System.Drawing.Size(24, 23);
            this.outDirBrowseButton.TabIndex = 15;
            this.outDirBrowseButton.Text = "...";
            this.outDirBrowseButton.UseVisualStyleBackColor = true;
            this.outDirBrowseButton.Click += new System.EventHandler(this.outDirBrowseButton_Click);
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Select the root folder to save presets into.";
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // BonePresetSaveWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(259, 482);
            this.Controls.Add(this.outDirBrowseButton);
            this.Controls.Add(this.outputDirectoryText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.refreshImageButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.typeCombo);
            this.Controls.Add(this.previewPicture);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.categoryText);
            this.Controls.Add(this.nameText);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Name = "BonePresetSaveWindow";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Float;
            this.Text = "Bone Preset Saver";
            this.ToolStripName = "Editing";
            ((System.ComponentModel.ISupportInitialize)(this.previewPicture)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox nameText;
        private System.Windows.Forms.TextBox categoryText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox previewPicture;
        private System.Windows.Forms.ComboBox typeCombo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button refreshImageButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rightSideSource;
        private System.Windows.Forms.RadioButton leftSideSource;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox outputDirectoryText;
        private System.Windows.Forms.Button outDirBrowseButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}