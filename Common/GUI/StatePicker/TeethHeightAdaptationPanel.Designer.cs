namespace Medical.GUI
{
    partial class TeethHeightAdaptationPanel
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
            this.rightHeightSlider = new Medical.GUI.HeightComboSlider();
            this.leftHeightSlider = new Medical.GUI.HeightComboSlider();
            this.SuspendLayout();
            // 
            // rightHeightSlider
            // 
            this.rightHeightSlider.LabelText = "Right Side";
            this.rightHeightSlider.Location = new System.Drawing.Point(4, 414);
            this.rightHeightSlider.Name = "rightHeightSlider";
            this.rightHeightSlider.Size = new System.Drawing.Size(197, 44);
            this.rightHeightSlider.TabIndex = 26;
            this.rightHeightSlider.Value = 0F;
            // 
            // leftHeightSlider
            // 
            this.leftHeightSlider.LabelText = "Left Side";
            this.leftHeightSlider.Location = new System.Drawing.Point(4, 364);
            this.leftHeightSlider.Name = "leftHeightSlider";
            this.leftHeightSlider.Size = new System.Drawing.Size(197, 44);
            this.leftHeightSlider.TabIndex = 25;
            this.leftHeightSlider.Value = 0F;
            // 
            // TeethHeightAdaptationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rightHeightSlider);
            this.Controls.Add(this.leftHeightSlider);
            this.Name = "TeethHeightAdaptationPanel";
            this.Size = new System.Drawing.Size(291, 515);
            this.Controls.SetChildIndex(this.leftHeightSlider, 0);
            this.Controls.SetChildIndex(this.rightHeightSlider, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private Medical.GUI.HeightComboSlider rightHeightSlider;
        private Medical.GUI.HeightComboSlider leftHeightSlider;
    }
}
