namespace Medical.GUI
{
    partial class PictureForm
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
            this.pictureWindow1 = new Medical.GUI.PictureWindow();
            this.SuspendLayout();
            // 
            // pictureWindow1
            // 
            this.pictureWindow1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureWindow1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pictureWindow1.Location = new System.Drawing.Point(0, 0);
            this.pictureWindow1.Name = "pictureWindow1";
            this.pictureWindow1.Size = new System.Drawing.Size(284, 262);
            this.pictureWindow1.TabIndex = 0;
            // 
            // PictureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.pictureWindow1);
            this.Name = "PictureForm";
            this.Text = "PictureForm";
            this.ResumeLayout(false);

        }

        #endregion

        private PictureWindow pictureWindow1;
    }
}