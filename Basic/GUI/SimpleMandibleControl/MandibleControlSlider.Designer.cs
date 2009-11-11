namespace Medical.GUI
{
    partial class MandibleControlSlider
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
            this.amountTrackBar = new System.Windows.Forms.TrackBar();
            this.openPicturePanel = new System.Windows.Forms.Panel();
            this.closedPicturePanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.amountTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // amountTrackBar
            // 
            this.amountTrackBar.LargeChange = 2000;
            this.amountTrackBar.Location = new System.Drawing.Point(66, 14);
            this.amountTrackBar.Maximum = 10000;
            this.amountTrackBar.Name = "amountTrackBar";
            this.amountTrackBar.Size = new System.Drawing.Size(136, 45);
            this.amountTrackBar.SmallChange = 1000;
            this.amountTrackBar.TabIndex = 27;
            this.amountTrackBar.TickFrequency = 10000;
            this.amountTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // openPicturePanel
            // 
            this.openPicturePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.openPicturePanel.Location = new System.Drawing.Point(203, 1);
            this.openPicturePanel.Name = "openPicturePanel";
            this.openPicturePanel.Size = new System.Drawing.Size(60, 60);
            this.openPicturePanel.TabIndex = 29;
            // 
            // closedPicturePanel
            // 
            this.closedPicturePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.closedPicturePanel.Location = new System.Drawing.Point(4, 1);
            this.closedPicturePanel.Name = "closedPicturePanel";
            this.closedPicturePanel.Size = new System.Drawing.Size(60, 60);
            this.closedPicturePanel.TabIndex = 28;
            // 
            // OpeningMandibleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.openPicturePanel);
            this.Controls.Add(this.closedPicturePanel);
            this.Controls.Add(this.amountTrackBar);
            this.Name = "OpeningMandibleControl";
            this.Size = new System.Drawing.Size(266, 64);
            ((System.ComponentModel.ISupportInitialize)(this.amountTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar amountTrackBar;
        private System.Windows.Forms.Panel closedPicturePanel;
        private System.Windows.Forms.Panel openPicturePanel;

    }
}
