namespace Medical.GUI
{
    partial class ThreeWayUpperSplit
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
            this.horizontalSplit = new System.Windows.Forms.SplitContainer();
            this.verticalSplit = new System.Windows.Forms.SplitContainer();
            this.horizontalSplit.Panel1.SuspendLayout();
            this.horizontalSplit.SuspendLayout();
            this.verticalSplit.SuspendLayout();
            this.SuspendLayout();
            // 
            // horizontalSplit
            // 
            this.horizontalSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.horizontalSplit.Location = new System.Drawing.Point(0, 0);
            this.horizontalSplit.Name = "horizontalSplit";
            this.horizontalSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // horizontalSplit.Panel1
            // 
            this.horizontalSplit.Panel1.Controls.Add(this.verticalSplit);
            this.horizontalSplit.Size = new System.Drawing.Size(449, 352);
            this.horizontalSplit.SplitterDistance = 174;
            this.horizontalSplit.TabIndex = 0;
            // 
            // verticalSplit
            // 
            this.verticalSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.verticalSplit.Location = new System.Drawing.Point(0, 0);
            this.verticalSplit.Name = "verticalSplit";
            this.verticalSplit.Size = new System.Drawing.Size(449, 174);
            this.verticalSplit.SplitterDistance = 226;
            this.verticalSplit.TabIndex = 0;
            // 
            // ThreeWayUpperSplit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.horizontalSplit);
            this.Name = "ThreeWayUpperSplit";
            this.Size = new System.Drawing.Size(449, 352);
            this.horizontalSplit.Panel1.ResumeLayout(false);
            this.horizontalSplit.ResumeLayout(false);
            this.verticalSplit.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer horizontalSplit;
        private System.Windows.Forms.SplitContainer verticalSplit;
    }
}
