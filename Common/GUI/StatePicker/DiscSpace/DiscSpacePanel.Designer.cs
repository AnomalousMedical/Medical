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
            this.discSpaceControl1 = new Medical.GUI.DiscSpaceControl();
            this.showDiscCheckBox = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.kryptonWrapLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.SuspendLayout();
            // 
            // discSpaceControl1
            // 
            this.discSpaceControl1.Location = new System.Drawing.Point(0, 49);
            this.discSpaceControl1.Name = "discSpaceControl1";
            this.discSpaceControl1.Size = new System.Drawing.Size(188, 467);
            this.discSpaceControl1.TabIndex = 0;
            // 
            // showDiscCheckBox
            // 
            this.showDiscCheckBox.Location = new System.Drawing.Point(11, 24);
            this.showDiscCheckBox.Name = "showDiscCheckBox";
            this.showDiscCheckBox.Size = new System.Drawing.Size(76, 19);
            this.showDiscCheckBox.TabIndex = 1;
            this.showDiscCheckBox.Values.Text = "Show Disc";
            this.showDiscCheckBox.CheckedChanged += new System.EventHandler(this.showDiscCheckBox_CheckedChanged);
            // 
            // kryptonWrapLabel1
            // 
            this.kryptonWrapLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonWrapLabel1.AutoSize = false;
            this.kryptonWrapLabel1.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.kryptonWrapLabel1.Location = new System.Drawing.Point(4, 4);
            this.kryptonWrapLabel1.Name = "kryptonWrapLabel1";
            this.kryptonWrapLabel1.Size = new System.Drawing.Size(318, 23);
            this.kryptonWrapLabel1.Text = "Check this box to show the disc.";
            // 
            // DiscSpacePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonWrapLabel1);
            this.Controls.Add(this.showDiscCheckBox);
            this.Controls.Add(this.discSpaceControl1);
            this.Name = "DiscSpacePanel";
            this.Size = new System.Drawing.Size(325, 616);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DiscSpaceControl discSpaceControl1;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox showDiscCheckBox;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel1;


    }
}
