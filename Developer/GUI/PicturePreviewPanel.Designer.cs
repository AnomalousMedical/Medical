namespace Medical.GUI
{
    partial class PicturePreviewPanel
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
            this.refreshImageButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.previewPicture = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.previewPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // refreshImageButton
            // 
            this.refreshImageButton.Location = new System.Drawing.Point(2, 274);
            this.refreshImageButton.Name = "refreshImageButton";
            this.refreshImageButton.Size = new System.Drawing.Size(89, 23);
            this.refreshImageButton.TabIndex = 28;
            this.refreshImageButton.Text = "Refresh Image";
            this.refreshImageButton.UseVisualStyleBackColor = true;
            this.refreshImageButton.Click += new System.EventHandler(this.refreshImageButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 2);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Image Preview";
            // 
            // previewPicture
            // 
            this.previewPicture.Location = new System.Drawing.Point(2, 18);
            this.previewPicture.Name = "previewPicture";
            this.previewPicture.Size = new System.Drawing.Size(250, 250);
            this.previewPicture.TabIndex = 26;
            this.previewPicture.TabStop = false;
            // 
            // PicturePreviewPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.refreshImageButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.previewPicture);
            this.Name = "PicturePreviewPanel";
            this.Size = new System.Drawing.Size(254, 298);
            ((System.ComponentModel.ISupportInitialize)(this.previewPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button refreshImageButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox previewPicture;
    }
}
