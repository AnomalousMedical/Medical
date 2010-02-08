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
            this.SuspendLayout();
            // 
            // heightControl1
            // 
            this.heightControl1.Location = new System.Drawing.Point(6, 404);
            this.heightControl1.Name = "heightControl1";
            this.heightControl1.Size = new System.Drawing.Size(205, 165);
            this.heightControl1.TabIndex = 23;
            // 
            // TeethHeightAdaptationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.heightControl1);
            this.Name = "TeethHeightAdaptationPanel";
            this.Size = new System.Drawing.Size(291, 572);
            this.Controls.SetChildIndex(this.heightControl1, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private Medical.GUI.BoneManipulator.HeightControl heightControl1;

    }
}
