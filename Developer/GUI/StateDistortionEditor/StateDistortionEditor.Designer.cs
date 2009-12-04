namespace Medical.GUI
{
    partial class StateDistortionEditor
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
            Medical.ImageRendererProperties imageRendererProperties1 = new Medical.ImageRendererProperties();
            this.saveButton = new System.Windows.Forms.Button();
            this.picturePreviewPanel = new Medical.GUI.PicturePreviewPanel();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(3, 207);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // picturePreviewPanel
            // 
            imageRendererProperties1.AntiAliasingMode = 1;
            imageRendererProperties1.Height = 100;
            imageRendererProperties1.LayerState = null;
            imageRendererProperties1.MaxGridHeight = 2048;
            imageRendererProperties1.MaxGridWidth = 2048;
            imageRendererProperties1.NavigationStateName = null;
            imageRendererProperties1.OverrideLayers = false;
            imageRendererProperties1.ShowBackground = true;
            imageRendererProperties1.ShowWatermark = true;
            imageRendererProperties1.TransparentBackground = false;
            imageRendererProperties1.UseActiveViewportLocation = true;
            imageRendererProperties1.UseCustomPosition = false;
            imageRendererProperties1.UseNavigationStatePosition = false;
            imageRendererProperties1.UseWindowBackgroundColor = true;
            imageRendererProperties1.Width = 100;
            this.picturePreviewPanel.ImageProperties = imageRendererProperties1;
            this.picturePreviewPanel.Location = new System.Drawing.Point(3, 3);
            this.picturePreviewPanel.Name = "picturePreviewPanel";
            this.picturePreviewPanel.Size = new System.Drawing.Size(194, 198);
            this.picturePreviewPanel.TabIndex = 0;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "dst";
            this.saveFileDialog.Filter = "Distortion Files|*.dst";
            // 
            // StateDistortionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ButtonText = "State Preset Editor";
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.picturePreviewPanel);
            this.DockAreas = ((Medical.GUI.DockLocations)((Medical.GUI.DockLocations.Right | Medical.GUI.DockLocations.Float)));
            this.Name = "StateDistortionEditor";
            this.ShowHint = Medical.GUI.DockLocations.Right;
            this.Size = new System.Drawing.Size(200, 236);
            this.ToolStripName = "Editing";
            this.ResumeLayout(false);

        }

        #endregion

        private PicturePreviewPanel picturePreviewPanel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}
