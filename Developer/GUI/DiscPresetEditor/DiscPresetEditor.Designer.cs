namespace Medical.GUI
{
    partial class DiscPresetEditor
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
            this.copySideButton = new System.Windows.Forms.Button();
            this.rightSideSource = new System.Windows.Forms.RadioButton();
            this.leftSideSource = new System.Windows.Forms.RadioButton();
            this.outDirBrowseButton = new System.Windows.Forms.Button();
            this.outputDirectoryText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.categoryText = new System.Windows.Forms.TextBox();
            this.nameText = new System.Windows.Forms.TextBox();
            this.picturePreviewPanel = new Medical.GUI.PicturePreviewPanel();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // copySideButton
            // 
            this.copySideButton.Location = new System.Drawing.Point(88, 434);
            this.copySideButton.Name = "copySideButton";
            this.copySideButton.Size = new System.Drawing.Size(75, 23);
            this.copySideButton.TabIndex = 31;
            this.copySideButton.Text = "Copy Side";
            this.copySideButton.UseVisualStyleBackColor = true;
            this.copySideButton.Click += new System.EventHandler(this.copySideButton_Click);
            // 
            // rightSideSource
            // 
            this.rightSideSource.AutoSize = true;
            this.rightSideSource.Location = new System.Drawing.Point(79, 19);
            this.rightSideSource.Name = "rightSideSource";
            this.rightSideSource.Size = new System.Drawing.Size(74, 17);
            this.rightSideSource.TabIndex = 15;
            this.rightSideSource.Text = "Right Side";
            this.rightSideSource.UseVisualStyleBackColor = true;
            // 
            // leftSideSource
            // 
            this.leftSideSource.AutoSize = true;
            this.leftSideSource.Checked = true;
            this.leftSideSource.Location = new System.Drawing.Point(6, 19);
            this.leftSideSource.Name = "leftSideSource";
            this.leftSideSource.Size = new System.Drawing.Size(67, 17);
            this.leftSideSource.TabIndex = 14;
            this.leftSideSource.TabStop = true;
            this.leftSideSource.Text = "Left Side";
            this.leftSideSource.UseVisualStyleBackColor = true;
            // 
            // outDirBrowseButton
            // 
            this.outDirBrowseButton.Location = new System.Drawing.Point(229, 1);
            this.outDirBrowseButton.Name = "outDirBrowseButton";
            this.outDirBrowseButton.Size = new System.Drawing.Size(24, 23);
            this.outDirBrowseButton.TabIndex = 30;
            this.outDirBrowseButton.Text = "...";
            this.outDirBrowseButton.UseVisualStyleBackColor = true;
            this.outDirBrowseButton.Click += new System.EventHandler(this.outDirBrowseButton_Click);
            // 
            // outputDirectoryText
            // 
            this.outputDirectoryText.Location = new System.Drawing.Point(86, 2);
            this.outputDirectoryText.Name = "outputDirectoryText";
            this.outputDirectoryText.Size = new System.Drawing.Size(138, 20);
            this.outputDirectoryText.TabIndex = 29;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 28;
            this.label3.Text = "Output Directory";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rightSideSource);
            this.groupBox1.Controls.Add(this.leftSideSource);
            this.groupBox1.Location = new System.Drawing.Point(6, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(157, 44);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source Data";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(6, 434);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 26;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Select the root folder to save presets into.";
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Category";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Name";
            // 
            // categoryText
            // 
            this.categoryText.Location = new System.Drawing.Point(76, 58);
            this.categoryText.Name = "categoryText";
            this.categoryText.Size = new System.Drawing.Size(180, 20);
            this.categoryText.TabIndex = 18;
            // 
            // nameText
            // 
            this.nameText.Location = new System.Drawing.Point(76, 31);
            this.nameText.Name = "nameText";
            this.nameText.Size = new System.Drawing.Size(180, 20);
            this.nameText.TabIndex = 17;
            // 
            // picturePreviewPanel
            // 
            this.picturePreviewPanel.Location = new System.Drawing.Point(2, 131);
            this.picturePreviewPanel.Name = "picturePreviewPanel";
            this.picturePreviewPanel.Size = new System.Drawing.Size(254, 298);
            this.picturePreviewPanel.TabIndex = 32;
            // 
            // DiscPresetEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ButtonText = "Disc Preset Editor";
            this.Controls.Add(this.picturePreviewPanel);
            this.Controls.Add(this.copySideButton);
            this.Controls.Add(this.outDirBrowseButton);
            this.Controls.Add(this.outputDirectoryText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.categoryText);
            this.Controls.Add(this.nameText);
            this.DockAreas = ((Medical.GUI.DockLocations)((Medical.GUI.DockLocations.Right | Medical.GUI.DockLocations.Float)));
            this.Name = "DiscPresetEditor";
            this.ShowHint = Medical.GUI.DockLocations.Right;
            this.Size = new System.Drawing.Size(259, 460);
            this.ToolStripName = "Editing";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button copySideButton;
        private System.Windows.Forms.RadioButton rightSideSource;
        private System.Windows.Forms.RadioButton leftSideSource;
        private System.Windows.Forms.Button outDirBrowseButton;
        private System.Windows.Forms.TextBox outputDirectoryText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox categoryText;
        private System.Windows.Forms.TextBox nameText;
        private PicturePreviewPanel picturePreviewPanel;
    }
}