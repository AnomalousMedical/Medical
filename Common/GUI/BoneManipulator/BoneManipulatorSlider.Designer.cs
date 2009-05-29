namespace Medical.GUI
{
    partial class BoneManipulatorSlider
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
            this.valueTrackBar = new System.Windows.Forms.TrackBar();
            this.sliderNameLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.valueTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // valueTrackBar
            // 
            this.valueTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.valueTrackBar.LargeChange = 2000;
            this.valueTrackBar.Location = new System.Drawing.Point(3, 14);
            this.valueTrackBar.Maximum = 10000;
            this.valueTrackBar.Name = "valueTrackBar";
            this.valueTrackBar.Size = new System.Drawing.Size(191, 45);
            this.valueTrackBar.SmallChange = 1000;
            this.valueTrackBar.TabIndex = 0;
            this.valueTrackBar.TickFrequency = 1000;
            // 
            // sliderNameLabel
            // 
            this.sliderNameLabel.AutoSize = true;
            this.sliderNameLabel.Location = new System.Drawing.Point(4, -1);
            this.sliderNameLabel.Name = "sliderNameLabel";
            this.sliderNameLabel.Size = new System.Drawing.Size(61, 13);
            this.sliderNameLabel.TabIndex = 1;
            this.sliderNameLabel.Text = "SliderName";
            // 
            // BoneManipulatorSlider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sliderNameLabel);
            this.Controls.Add(this.valueTrackBar);
            this.Name = "BoneManipulatorSlider";
            this.Size = new System.Drawing.Size(197, 44);
            ((System.ComponentModel.ISupportInitialize)(this.valueTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar valueTrackBar;
        private System.Windows.Forms.Label sliderNameLabel;
    }
}
