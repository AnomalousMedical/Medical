namespace Medical.GUI
{
    partial class LayerSlider
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
            this.nameLabel = new System.Windows.Forms.Label();
            this.transparencySlider = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.transparencySlider)).BeginInit();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(3, 3);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(28, 13);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Text";
            // 
            // transparencySlider
            // 
            this.transparencySlider.LargeChange = 1;
            this.transparencySlider.Location = new System.Drawing.Point(3, 19);
            this.transparencySlider.Maximum = 2;
            this.transparencySlider.Name = "transparencySlider";
            this.transparencySlider.Size = new System.Drawing.Size(104, 45);
            this.transparencySlider.TabIndex = 1;
            // 
            // LayerSlider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.transparencySlider);
            this.Controls.Add(this.nameLabel);
            this.Name = "LayerSlider";
            this.Size = new System.Drawing.Size(107, 54);
            ((System.ComponentModel.ISupportInitialize)(this.transparencySlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TrackBar transparencySlider;
    }
}
