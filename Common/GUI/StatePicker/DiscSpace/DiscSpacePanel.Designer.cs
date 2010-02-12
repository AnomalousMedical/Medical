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
            this.SuspendLayout();
            // 
            // discSpaceControl1
            // 
            this.discSpaceControl1.Location = new System.Drawing.Point(0, 0);
            this.discSpaceControl1.Name = "discSpaceControl1";
            this.discSpaceControl1.Size = new System.Drawing.Size(188, 467);
            this.discSpaceControl1.TabIndex = 0;
            // 
            // DiscSpacePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.discSpaceControl1);
            this.Name = "DiscSpacePanel";
            this.Size = new System.Drawing.Size(325, 616);
            this.ResumeLayout(false);

        }

        #endregion

        private DiscSpaceControl discSpaceControl1;


    }
}
