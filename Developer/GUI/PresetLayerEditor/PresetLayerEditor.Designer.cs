namespace Medical.GUI
{
    partial class PresetLayerEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.layerList = new System.Windows.Forms.ListBox();
            this.addButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.updateButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.hiddenCheckBox = new System.Windows.Forms.CheckBox();
            this.thumbnailPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.renderThumbnailButton = new System.Windows.Forms.Button();
            this.mergeButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // layerList
            // 
            this.layerList.Dock = System.Windows.Forms.DockStyle.Top;
            this.layerList.FormattingEnabled = true;
            this.layerList.Location = new System.Drawing.Point(0, 0);
            this.layerList.Name = "layerList";
            this.layerList.Size = new System.Drawing.Size(178, 212);
            this.layerList.TabIndex = 0;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(9, 219);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(91, 219);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 2;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(8, 410);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "lay";
            this.saveFileDialog.Filter = "Layer Definitions|*.lay";
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(9, 273);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(75, 23);
            this.updateButton.TabIndex = 5;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(91, 410);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(75, 23);
            this.loadButton.TabIndex = 6;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "lay";
            this.openFileDialog.Filter = "Layer Definitions|*.lay";
            // 
            // hiddenCheckBox
            // 
            this.hiddenCheckBox.AutoSize = true;
            this.hiddenCheckBox.Location = new System.Drawing.Point(75, 300);
            this.hiddenCheckBox.Name = "hiddenCheckBox";
            this.hiddenCheckBox.Size = new System.Drawing.Size(60, 17);
            this.hiddenCheckBox.TabIndex = 7;
            this.hiddenCheckBox.Text = "Hidden";
            this.hiddenCheckBox.UseVisualStyleBackColor = true;
            // 
            // thumbnailPanel
            // 
            this.thumbnailPanel.Location = new System.Drawing.Point(0, 0);
            this.thumbnailPanel.Name = "thumbnailPanel";
            this.thumbnailPanel.Size = new System.Drawing.Size(48, 48);
            this.thumbnailPanel.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 301);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Thumbnail";
            // 
            // renderThumbnailButton
            // 
            this.renderThumbnailButton.Location = new System.Drawing.Point(7, 371);
            this.renderThumbnailButton.Name = "renderThumbnailButton";
            this.renderThumbnailButton.Size = new System.Drawing.Size(67, 23);
            this.renderThumbnailButton.TabIndex = 10;
            this.renderThumbnailButton.Text = "Render";
            this.renderThumbnailButton.UseVisualStyleBackColor = true;
            this.renderThumbnailButton.Click += new System.EventHandler(this.renderThumbnailButton_Click);
            // 
            // mergeButton
            // 
            this.mergeButton.Location = new System.Drawing.Point(9, 440);
            this.mergeButton.Name = "mergeButton";
            this.mergeButton.Size = new System.Drawing.Size(75, 23);
            this.mergeButton.TabIndex = 11;
            this.mergeButton.Text = "Merge";
            this.mergeButton.UseVisualStyleBackColor = true;
            this.mergeButton.Click += new System.EventHandler(this.mergeButton_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.thumbnailPanel);
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(16, 317);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(52, 52);
            this.panel1.TabIndex = 13;
            // 
            // PresetLayerEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ButtonText = "Preset Layer Editor";
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.mergeButton);
            this.Controls.Add(this.renderThumbnailButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.hiddenCheckBox);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.layerList);
            this.DockAreas = ((Medical.GUI.DockLocations)((Medical.GUI.DockLocations.Right | Medical.GUI.DockLocations.Float)));
            this.Name = "PresetLayerEditor";
            this.ShowHint = Medical.GUI.DockLocations.Right;
            this.Size = new System.Drawing.Size(178, 471);
            this.ToolStripName = "Editing";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox layerList;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.CheckBox hiddenCheckBox;
        private System.Windows.Forms.Panel thumbnailPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button renderThumbnailButton;
        private System.Windows.Forms.Button mergeButton;
        private System.Windows.Forms.Panel panel1;
    }
}
