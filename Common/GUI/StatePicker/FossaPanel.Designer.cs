namespace Medical.GUI
{
    partial class FossaPanel
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
            this.eminanceSlider = new System.Windows.Forms.TrackBar();
            this.distortedImagePanel = new System.Windows.Forms.Panel();
            this.normalImagePanel = new System.Windows.Forms.Panel();
            this.undoButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.makeNormalButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.eminanceSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // eminanceSlider
            // 
            this.eminanceSlider.LargeChange = 5000;
            this.eminanceSlider.Location = new System.Drawing.Point(69, 9);
            this.eminanceSlider.Maximum = 10000;
            this.eminanceSlider.Name = "eminanceSlider";
            this.eminanceSlider.Size = new System.Drawing.Size(187, 45);
            this.eminanceSlider.SmallChange = 1000;
            this.eminanceSlider.TabIndex = 6;
            this.eminanceSlider.TickFrequency = 1000;
            // 
            // distortedImagePanel
            // 
            this.distortedImagePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.distortedImagePanel.Location = new System.Drawing.Point(262, 2);
            this.distortedImagePanel.Name = "distortedImagePanel";
            this.distortedImagePanel.Size = new System.Drawing.Size(60, 60);
            this.distortedImagePanel.TabIndex = 25;
            // 
            // normalImagePanel
            // 
            this.normalImagePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.normalImagePanel.Location = new System.Drawing.Point(3, 2);
            this.normalImagePanel.Name = "normalImagePanel";
            this.normalImagePanel.Size = new System.Drawing.Size(60, 60);
            this.normalImagePanel.TabIndex = 24;
            // 
            // undoButton
            // 
            this.undoButton.Location = new System.Drawing.Point(4, 69);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(90, 25);
            this.undoButton.TabIndex = 26;
            this.undoButton.Values.Text = "Undo";
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // makeNormalButton
            // 
            this.makeNormalButton.Location = new System.Drawing.Point(101, 69);
            this.makeNormalButton.Name = "makeNormalButton";
            this.makeNormalButton.Size = new System.Drawing.Size(90, 25);
            this.makeNormalButton.TabIndex = 27;
            this.makeNormalButton.Values.Text = "Make Normal";
            this.makeNormalButton.Click += new System.EventHandler(this.makeNormalButton_Click);
            // 
            // FossaPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.makeNormalButton);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.distortedImagePanel);
            this.Controls.Add(this.normalImagePanel);
            this.Controls.Add(this.eminanceSlider);
            this.Name = "FossaPanel";
            this.Size = new System.Drawing.Size(325, 102);
            ((System.ComponentModel.ISupportInitialize)(this.eminanceSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar eminanceSlider;
        private System.Windows.Forms.Panel distortedImagePanel;
        private System.Windows.Forms.Panel normalImagePanel;
        private ComponentFactory.Krypton.Toolkit.KryptonButton undoButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton makeNormalButton;
    }
}
