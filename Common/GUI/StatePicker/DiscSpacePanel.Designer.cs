namespace Medical.GUI
{
    partial class DiscSpacePanel
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
            this.makeNormalButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.undoButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.discOffsetSlider = new System.Windows.Forms.TrackBar();
            this.distortedPanel = new System.Windows.Forms.Panel();
            this.normalPanel = new System.Windows.Forms.Panel();
            this.kryptonWrapLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.dopplerControl1 = new Medical.GUI.DopplerControl();
            this.dopplerDataCheck = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.discOffsetSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // makeNormalButton
            // 
            this.makeNormalButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.makeNormalButton.Location = new System.Drawing.Point(100, 588);
            this.makeNormalButton.Name = "makeNormalButton";
            this.makeNormalButton.Size = new System.Drawing.Size(90, 25);
            this.makeNormalButton.TabIndex = 46;
            this.makeNormalButton.Values.Text = "Make Normal";
            this.makeNormalButton.Click += new System.EventHandler(this.makeNormalButton_Click);
            // 
            // undoButton
            // 
            this.undoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.undoButton.Location = new System.Drawing.Point(3, 588);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(90, 25);
            this.undoButton.TabIndex = 45;
            this.undoButton.Values.Text = "Undo";
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(70, 22);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(65, 19);
            this.kryptonLabel1.TabIndex = 50;
            this.kryptonLabel1.Values.Text = "Disc Space";
            // 
            // discOffsetSlider
            // 
            this.discOffsetSlider.LargeChange = 2000;
            this.discOffsetSlider.Location = new System.Drawing.Point(69, 39);
            this.discOffsetSlider.Maximum = 10000;
            this.discOffsetSlider.Name = "discOffsetSlider";
            this.discOffsetSlider.Size = new System.Drawing.Size(187, 45);
            this.discOffsetSlider.SmallChange = 1000;
            this.discOffsetSlider.TabIndex = 51;
            this.discOffsetSlider.TickFrequency = 10000;
            this.discOffsetSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // distortedPanel
            // 
            this.distortedPanel.BackgroundImage = global::Medical.Properties.Resources.DegenerationRightCondyleDistorted;
            this.distortedPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.distortedPanel.Location = new System.Drawing.Point(262, 23);
            this.distortedPanel.Name = "distortedPanel";
            this.distortedPanel.Size = new System.Drawing.Size(60, 60);
            this.distortedPanel.TabIndex = 49;
            // 
            // normalPanel
            // 
            this.normalPanel.BackgroundImage = global::Medical.Properties.Resources.DegenerationRightCondyle;
            this.normalPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.normalPanel.Location = new System.Drawing.Point(3, 23);
            this.normalPanel.Name = "normalPanel";
            this.normalPanel.Size = new System.Drawing.Size(60, 60);
            this.normalPanel.TabIndex = 48;
            // 
            // kryptonWrapLabel1
            // 
            this.kryptonWrapLabel1.AutoSize = false;
            this.kryptonWrapLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonWrapLabel1.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(59)))), ((int)(((byte)(59)))));
            this.kryptonWrapLabel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonWrapLabel1.Name = "kryptonWrapLabel1";
            this.kryptonWrapLabel1.Size = new System.Drawing.Size(325, 17);
            this.kryptonWrapLabel1.Text = "Adjust the slider to change the disc space.";
            // 
            // dopplerControl1
            // 
            this.dopplerControl1.Location = new System.Drawing.Point(5, 117);
            this.dopplerControl1.Name = "dopplerControl1";
            this.dopplerControl1.Size = new System.Drawing.Size(189, 449);
            this.dopplerControl1.TabIndex = 52;
            // 
            // dopplerDataCheck
            // 
            this.dopplerDataCheck.Location = new System.Drawing.Point(6, 90);
            this.dopplerDataCheck.Name = "dopplerDataCheck";
            this.dopplerDataCheck.Size = new System.Drawing.Size(126, 19);
            this.dopplerDataCheck.TabIndex = 53;
            this.dopplerDataCheck.Values.Text = "I have doppler data.";
            // 
            // DiscSpacePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dopplerDataCheck);
            this.Controls.Add(this.dopplerControl1);
            this.Controls.Add(this.kryptonWrapLabel1);
            this.Controls.Add(this.discOffsetSlider);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.distortedPanel);
            this.Controls.Add(this.normalPanel);
            this.Controls.Add(this.makeNormalButton);
            this.Controls.Add(this.undoButton);
            this.Name = "DiscSpacePanel";
            this.Size = new System.Drawing.Size(325, 616);
            ((System.ComponentModel.ISupportInitialize)(this.discOffsetSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton makeNormalButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton undoButton;
        private System.Windows.Forms.Panel distortedPanel;
        private System.Windows.Forms.Panel normalPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private System.Windows.Forms.TrackBar discOffsetSlider;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel1;
        private DopplerControl dopplerControl1;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox dopplerDataCheck;
    }
}
