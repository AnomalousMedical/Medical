namespace Medical.GUI
{
    partial class FourWaySplit
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
            this.rightVertical = new System.Windows.Forms.SplitContainer();
            this.leftVertical = new System.Windows.Forms.SplitContainer();
            this.horizontal = new System.Windows.Forms.SplitContainer();
            this.rightVertical.SuspendLayout();
            this.leftVertical.SuspendLayout();
            this.horizontal.Panel1.SuspendLayout();
            this.horizontal.Panel2.SuspendLayout();
            this.horizontal.SuspendLayout();
            this.SuspendLayout();
            // 
            // rightVertical
            // 
            this.rightVertical.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rightVertical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightVertical.Location = new System.Drawing.Point(0, 0);
            this.rightVertical.Name = "rightVertical";
            this.rightVertical.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.rightVertical.Size = new System.Drawing.Size(292, 505);
            this.rightVertical.SplitterDistance = 246;
            this.rightVertical.TabIndex = 0;
            // 
            // leftVertical
            // 
            this.leftVertical.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.leftVertical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftVertical.Location = new System.Drawing.Point(0, 0);
            this.leftVertical.Name = "leftVertical";
            this.leftVertical.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.leftVertical.Size = new System.Drawing.Size(287, 505);
            this.leftVertical.SplitterDistance = 246;
            this.leftVertical.TabIndex = 0;
            // 
            // horizontal
            // 
            this.horizontal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.horizontal.Location = new System.Drawing.Point(0, 0);
            this.horizontal.Name = "horizontal";
            // 
            // horizontal.Panel1
            // 
            this.horizontal.Panel1.Controls.Add(this.leftVertical);
            // 
            // horizontal.Panel2
            // 
            this.horizontal.Panel2.Controls.Add(this.rightVertical);
            this.horizontal.Size = new System.Drawing.Size(583, 505);
            this.horizontal.SplitterDistance = 287;
            this.horizontal.TabIndex = 0;
            // 
            // FourWaySplit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.horizontal);
            this.Name = "FourWaySplit";
            this.Size = new System.Drawing.Size(583, 505);
            this.rightVertical.ResumeLayout(false);
            this.leftVertical.ResumeLayout(false);
            this.horizontal.Panel1.ResumeLayout(false);
            this.horizontal.Panel2.ResumeLayout(false);
            this.horizontal.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer rightVertical;
        private System.Windows.Forms.SplitContainer leftVertical;
        private System.Windows.Forms.SplitContainer horizontal;

    }
}
