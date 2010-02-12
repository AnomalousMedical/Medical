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
            this.heightControl1 = new Medical.GUI.BoneManipulator.HeightControl();
            this.kryptonWrapLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.SuspendLayout();
            // 
            // heightControl1
            // 
            this.heightControl1.Location = new System.Drawing.Point(3, 378);
            this.heightControl1.Name = "heightControl1";
            this.heightControl1.Size = new System.Drawing.Size(205, 165);
            this.heightControl1.TabIndex = 23;
            // 
            // kryptonWrapLabel4
            // 
            this.kryptonWrapLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonWrapLabel4.AutoSize = false;
            this.kryptonWrapLabel4.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(59)))), ((int)(((byte)(59)))));
            this.kryptonWrapLabel4.Location = new System.Drawing.Point(6, 344);
            this.kryptonWrapLabel4.Name = "kryptonWrapLabel4";
            this.kryptonWrapLabel4.Size = new System.Drawing.Size(277, 33);
            this.kryptonWrapLabel4.Text = "Adjust the horizontal arch alignment using the following sliders.";
            // 
            // TeethHeightAdaptationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonWrapLabel4);
            this.Controls.Add(this.heightControl1);
            this.Name = "TeethHeightAdaptationPanel";
            this.Size = new System.Drawing.Size(291, 536);
            this.Controls.SetChildIndex(this.heightControl1, 0);
            this.Controls.SetChildIndex(this.kryptonWrapLabel4, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private Medical.GUI.BoneManipulator.HeightControl heightControl1;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel4;

    }
}
