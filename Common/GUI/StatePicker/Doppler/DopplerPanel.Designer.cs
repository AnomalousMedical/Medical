namespace Medical.GUI
{
    partial class DopplerPanel
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
            this.dopplerControl1 = new Medical.GUI.DopplerControl();
            this.SuspendLayout();
            // 
            // dopplerControl1
            // 
            this.dopplerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dopplerControl1.Location = new System.Drawing.Point(0, 0);
            this.dopplerControl1.Name = "dopplerControl1";
            this.dopplerControl1.Size = new System.Drawing.Size(197, 367);
            this.dopplerControl1.TabIndex = 0;
            // 
            // DopplerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dopplerControl1);
            this.Name = "DopplerPanel";
            this.Size = new System.Drawing.Size(197, 367);
            this.ResumeLayout(false);

        }

        #endregion

        private DopplerControl dopplerControl1;
    }
}
