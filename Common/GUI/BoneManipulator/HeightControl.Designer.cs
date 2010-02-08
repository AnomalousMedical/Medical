namespace Medical.GUI.BoneManipulator
{
    partial class HeightControl
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
            this.bothSidesSlider = new System.Windows.Forms.TrackBar();
            this.sliderNameLabel = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.rightHeightSlider = new Medical.GUI.HeightComboSlider();
            this.leftHeightSlider = new Medical.GUI.HeightComboSlider();
            ((System.ComponentModel.ISupportInitialize)(this.bothSidesSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // bothSidesSlider
            // 
            this.bothSidesSlider.LargeChange = 2000;
            this.bothSidesSlider.Location = new System.Drawing.Point(6, 116);
            this.bothSidesSlider.Maximum = 10000;
            this.bothSidesSlider.Name = "bothSidesSlider";
            this.bothSidesSlider.Size = new System.Drawing.Size(191, 45);
            this.bothSidesSlider.SmallChange = 1000;
            this.bothSidesSlider.TabIndex = 27;
            this.bothSidesSlider.TickFrequency = 1000;
            // 
            // sliderNameLabel
            // 
            this.sliderNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderNameLabel.AutoSize = false;
            this.sliderNameLabel.Location = new System.Drawing.Point(7, 99);
            this.sliderNameLabel.Name = "sliderNameLabel";
            this.sliderNameLabel.Size = new System.Drawing.Size(190, 19);
            this.sliderNameLabel.TabIndex = 28;
            this.sliderNameLabel.Values.Text = "Both Sides";
            // 
            // rightHeightSlider
            // 
            this.rightHeightSlider.LabelText = "Right Side";
            this.rightHeightSlider.Location = new System.Drawing.Point(3, 53);
            this.rightHeightSlider.Name = "rightHeightSlider";
            this.rightHeightSlider.Size = new System.Drawing.Size(197, 44);
            this.rightHeightSlider.TabIndex = 26;
            this.rightHeightSlider.Value = 0F;
            // 
            // leftHeightSlider
            // 
            this.leftHeightSlider.LabelText = "Left Side";
            this.leftHeightSlider.Location = new System.Drawing.Point(3, 3);
            this.leftHeightSlider.Name = "leftHeightSlider";
            this.leftHeightSlider.Size = new System.Drawing.Size(197, 44);
            this.leftHeightSlider.TabIndex = 25;
            this.leftHeightSlider.Value = 0F;
            // 
            // HeightControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bothSidesSlider);
            this.Controls.Add(this.sliderNameLabel);
            this.Controls.Add(this.rightHeightSlider);
            this.Controls.Add(this.leftHeightSlider);
            this.Name = "HeightControl";
            this.Size = new System.Drawing.Size(203, 165);
            ((System.ComponentModel.ISupportInitialize)(this.bothSidesSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HeightComboSlider rightHeightSlider;
        private HeightComboSlider leftHeightSlider;
        private System.Windows.Forms.TrackBar bothSidesSlider;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel sliderNameLabel;
    }
}
