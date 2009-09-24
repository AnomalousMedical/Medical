namespace Medical.GUI
{
    partial class FossaStatePanel
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
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rightEminenceSevere = new System.Windows.Forms.RadioButton();
            this.rightEminenceModerate = new System.Windows.Forms.RadioButton();
            this.rightEminenceNormal = new System.Windows.Forms.RadioButton();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.leftEminenceSevere = new System.Windows.Forms.RadioButton();
            this.leftEminenceModerate = new System.Windows.Forms.RadioButton();
            this.leftEminenceNormal = new System.Windows.Forms.RadioButton();
            this.leftEminanceImage = new System.Windows.Forms.PictureBox();
            this.rightEminanceImage = new System.Windows.Forms.PictureBox();
            this.groupBox5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leftEminanceImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightEminanceImage)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rightEminenceSevere);
            this.groupBox5.Controls.Add(this.rightEminenceModerate);
            this.groupBox5.Controls.Add(this.rightEminenceNormal);
            this.groupBox5.Location = new System.Drawing.Point(5, 5);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(111, 87);
            this.groupBox5.TabIndex = 7;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Right Flatness";
            // 
            // rightEminenceSevere
            // 
            this.rightEminenceSevere.AutoSize = true;
            this.rightEminenceSevere.Location = new System.Drawing.Point(6, 65);
            this.rightEminenceSevere.Name = "rightEminenceSevere";
            this.rightEminenceSevere.Size = new System.Drawing.Size(59, 17);
            this.rightEminenceSevere.TabIndex = 2;
            this.rightEminenceSevere.Text = "Severe";
            this.rightEminenceSevere.UseVisualStyleBackColor = true;
            this.rightEminenceSevere.CheckedChanged += new System.EventHandler(this.rightEminenceSevere_CheckedChanged);
            // 
            // rightEminenceModerate
            // 
            this.rightEminenceModerate.AutoSize = true;
            this.rightEminenceModerate.Location = new System.Drawing.Point(6, 42);
            this.rightEminenceModerate.Name = "rightEminenceModerate";
            this.rightEminenceModerate.Size = new System.Drawing.Size(70, 17);
            this.rightEminenceModerate.TabIndex = 1;
            this.rightEminenceModerate.Text = "Moderate";
            this.rightEminenceModerate.UseVisualStyleBackColor = true;
            this.rightEminenceModerate.CheckedChanged += new System.EventHandler(this.rightEminenceModerate_CheckedChanged);
            // 
            // rightEminenceNormal
            // 
            this.rightEminenceNormal.AutoSize = true;
            this.rightEminenceNormal.Checked = true;
            this.rightEminenceNormal.Location = new System.Drawing.Point(6, 19);
            this.rightEminenceNormal.Name = "rightEminenceNormal";
            this.rightEminenceNormal.Size = new System.Drawing.Size(58, 17);
            this.rightEminenceNormal.TabIndex = 0;
            this.rightEminenceNormal.TabStop = true;
            this.rightEminenceNormal.Text = "Normal";
            this.rightEminenceNormal.UseVisualStyleBackColor = true;
            this.rightEminenceNormal.CheckedChanged += new System.EventHandler(this.rightEminenceNormal_CheckedChanged);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.leftEminenceSevere);
            this.groupBox7.Controls.Add(this.leftEminenceModerate);
            this.groupBox7.Controls.Add(this.leftEminenceNormal);
            this.groupBox7.Location = new System.Drawing.Point(5, 145);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(111, 87);
            this.groupBox7.TabIndex = 8;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Left Flatness";
            // 
            // leftEminenceSevere
            // 
            this.leftEminenceSevere.AutoSize = true;
            this.leftEminenceSevere.Location = new System.Drawing.Point(6, 65);
            this.leftEminenceSevere.Name = "leftEminenceSevere";
            this.leftEminenceSevere.Size = new System.Drawing.Size(59, 17);
            this.leftEminenceSevere.TabIndex = 2;
            this.leftEminenceSevere.Text = "Severe";
            this.leftEminenceSevere.UseVisualStyleBackColor = true;
            this.leftEminenceSevere.CheckedChanged += new System.EventHandler(this.leftEminenceSevere_CheckedChanged);
            // 
            // leftEminenceModerate
            // 
            this.leftEminenceModerate.AutoSize = true;
            this.leftEminenceModerate.Location = new System.Drawing.Point(6, 42);
            this.leftEminenceModerate.Name = "leftEminenceModerate";
            this.leftEminenceModerate.Size = new System.Drawing.Size(70, 17);
            this.leftEminenceModerate.TabIndex = 1;
            this.leftEminenceModerate.Text = "Moderate";
            this.leftEminenceModerate.UseVisualStyleBackColor = true;
            this.leftEminenceModerate.CheckedChanged += new System.EventHandler(this.leftEminenceModerate_CheckedChanged);
            // 
            // leftEminenceNormal
            // 
            this.leftEminenceNormal.AutoSize = true;
            this.leftEminenceNormal.Checked = true;
            this.leftEminenceNormal.Location = new System.Drawing.Point(6, 19);
            this.leftEminenceNormal.Name = "leftEminenceNormal";
            this.leftEminenceNormal.Size = new System.Drawing.Size(58, 17);
            this.leftEminenceNormal.TabIndex = 0;
            this.leftEminenceNormal.TabStop = true;
            this.leftEminenceNormal.Text = "Normal";
            this.leftEminenceNormal.UseVisualStyleBackColor = true;
            this.leftEminenceNormal.CheckedChanged += new System.EventHandler(this.leftEminenceNormal_CheckedChanged);
            // 
            // leftEminanceImage
            // 
            this.leftEminanceImage.Location = new System.Drawing.Point(122, 145);
            this.leftEminanceImage.Name = "leftEminanceImage";
            this.leftEminanceImage.Size = new System.Drawing.Size(163, 128);
            this.leftEminanceImage.TabIndex = 10;
            this.leftEminanceImage.TabStop = false;
            // 
            // rightEminanceImage
            // 
            this.rightEminanceImage.Location = new System.Drawing.Point(122, 5);
            this.rightEminanceImage.Name = "rightEminanceImage";
            this.rightEminanceImage.Size = new System.Drawing.Size(163, 128);
            this.rightEminanceImage.TabIndex = 9;
            this.rightEminanceImage.TabStop = false;
            // 
            // FossaStatePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.leftEminanceImage);
            this.Controls.Add(this.rightEminanceImage);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox5);
            this.Name = "FossaStatePanel";
            this.NavigationState = "Left TMJ";
            this.Size = new System.Drawing.Size(282, 245);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leftEminanceImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightEminanceImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton rightEminenceSevere;
        private System.Windows.Forms.RadioButton rightEminenceModerate;
        private System.Windows.Forms.RadioButton rightEminenceNormal;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.RadioButton leftEminenceSevere;
        private System.Windows.Forms.RadioButton leftEminenceModerate;
        private System.Windows.Forms.RadioButton leftEminenceNormal;
        private System.Windows.Forms.PictureBox rightEminanceImage;
        private System.Windows.Forms.PictureBox leftEminanceImage;

    }
}
