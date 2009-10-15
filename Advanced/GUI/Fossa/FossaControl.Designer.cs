namespace Medical.GUI
{
    partial class FossaControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FossaControl));
            this.label2 = new System.Windows.Forms.Label();
            this.rightEminanceSlider = new System.Windows.Forms.TrackBar();
            this.leftEminanceSlider = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.rightEminanceSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftEminanceSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Right Eminance";
            // 
            // rightEminanceSlider
            // 
            this.rightEminanceSlider.LargeChange = 5000;
            this.rightEminanceSlider.Location = new System.Drawing.Point(3, 81);
            this.rightEminanceSlider.Maximum = 10000;
            this.rightEminanceSlider.Name = "rightEminanceSlider";
            this.rightEminanceSlider.Size = new System.Drawing.Size(256, 45);
            this.rightEminanceSlider.SmallChange = 1000;
            this.rightEminanceSlider.TabIndex = 6;
            this.rightEminanceSlider.TickFrequency = 1000;
            // 
            // leftEminanceSlider
            // 
            this.leftEminanceSlider.LargeChange = 5000;
            this.leftEminanceSlider.Location = new System.Drawing.Point(3, 17);
            this.leftEminanceSlider.Maximum = 10000;
            this.leftEminanceSlider.Name = "leftEminanceSlider";
            this.leftEminanceSlider.Size = new System.Drawing.Size(256, 45);
            this.leftEminanceSlider.SmallChange = 1000;
            this.leftEminanceSlider.TabIndex = 5;
            this.leftEminanceSlider.TickFrequency = 1000;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Left Eminance";
            // 
            // FossaControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ButtonImageIndex = 10;
            this.ButtonText = "Fossa";
            this.ClientSize = new System.Drawing.Size(284, 138);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rightEminanceSlider);
            this.Controls.Add(this.leftEminanceSlider);
            this.Controls.Add(this.label1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FossaControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Text = "Fossa";
            this.ToolStripName = "Advanced";
            ((System.ComponentModel.ISupportInitialize)(this.rightEminanceSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftEminanceSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar rightEminanceSlider;
        private System.Windows.Forms.TrackBar leftEminanceSlider;
        private System.Windows.Forms.Label label1;

    }
}