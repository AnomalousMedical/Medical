namespace Medical.GUI.StateWizard
{
    partial class JointPanel
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rightEminenceSevere = new System.Windows.Forms.RadioButton();
            this.rightEminenceModerate = new System.Windows.Forms.RadioButton();
            this.rightEminenceNormal = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rightBoneOnBone = new System.Windows.Forms.RadioButton();
            this.rightReducedJointSpace = new System.Windows.Forms.RadioButton();
            this.rightNormalJointSpace = new System.Windows.Forms.RadioButton();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.leftEminenceSevere = new System.Windows.Forms.RadioButton();
            this.leftEminenceModerate = new System.Windows.Forms.RadioButton();
            this.leftEminenceNormal = new System.Windows.Forms.RadioButton();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.leftBoneOnBone = new System.Windows.Forms.RadioButton();
            this.leftReducedJointSpace = new System.Windows.Forms.RadioButton();
            this.leftNormalJointSpace = new System.Windows.Forms.RadioButton();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Location = new System.Drawing.Point(7, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(123, 232);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Right";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rightEminenceSevere);
            this.groupBox5.Controls.Add(this.rightEminenceModerate);
            this.groupBox5.Controls.Add(this.rightEminenceNormal);
            this.groupBox5.Location = new System.Drawing.Point(6, 125);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(111, 100);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Eminence Flatness";
            // 
            // rightEminenceSevere
            // 
            this.rightEminenceSevere.AutoSize = true;
            this.rightEminenceSevere.Location = new System.Drawing.Point(6, 65);
            this.rightEminenceSevere.Name = "rightEminenceSevere";
            this.rightEminenceSevere.Size = new System.Drawing.Size(59, 17);
            this.rightEminenceSevere.TabIndex = 2;
            this.rightEminenceSevere.TabStop = true;
            this.rightEminenceSevere.Text = "Severe";
            this.rightEminenceSevere.UseVisualStyleBackColor = true;
            // 
            // rightEminenceModerate
            // 
            this.rightEminenceModerate.AutoSize = true;
            this.rightEminenceModerate.Location = new System.Drawing.Point(6, 42);
            this.rightEminenceModerate.Name = "rightEminenceModerate";
            this.rightEminenceModerate.Size = new System.Drawing.Size(70, 17);
            this.rightEminenceModerate.TabIndex = 1;
            this.rightEminenceModerate.TabStop = true;
            this.rightEminenceModerate.Text = "Moderate";
            this.rightEminenceModerate.UseVisualStyleBackColor = true;
            // 
            // rightEminenceNormal
            // 
            this.rightEminenceNormal.AutoSize = true;
            this.rightEminenceNormal.Location = new System.Drawing.Point(6, 19);
            this.rightEminenceNormal.Name = "rightEminenceNormal";
            this.rightEminenceNormal.Size = new System.Drawing.Size(58, 17);
            this.rightEminenceNormal.TabIndex = 0;
            this.rightEminenceNormal.TabStop = true;
            this.rightEminenceNormal.Text = "Normal";
            this.rightEminenceNormal.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rightBoneOnBone);
            this.groupBox4.Controls.Add(this.rightReducedJointSpace);
            this.groupBox4.Controls.Add(this.rightNormalJointSpace);
            this.groupBox4.Location = new System.Drawing.Point(6, 19);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(111, 100);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Joint Space";
            // 
            // rightBoneOnBone
            // 
            this.rightBoneOnBone.AutoSize = true;
            this.rightBoneOnBone.Location = new System.Drawing.Point(6, 65);
            this.rightBoneOnBone.Name = "rightBoneOnBone";
            this.rightBoneOnBone.Size = new System.Drawing.Size(95, 17);
            this.rightBoneOnBone.TabIndex = 2;
            this.rightBoneOnBone.TabStop = true;
            this.rightBoneOnBone.Text = "Bone On Bone";
            this.rightBoneOnBone.UseVisualStyleBackColor = true;
            // 
            // rightReducedJointSpace
            // 
            this.rightReducedJointSpace.AutoSize = true;
            this.rightReducedJointSpace.Location = new System.Drawing.Point(6, 42);
            this.rightReducedJointSpace.Name = "rightReducedJointSpace";
            this.rightReducedJointSpace.Size = new System.Drawing.Size(69, 17);
            this.rightReducedJointSpace.TabIndex = 1;
            this.rightReducedJointSpace.TabStop = true;
            this.rightReducedJointSpace.Text = "Reduced";
            this.rightReducedJointSpace.UseVisualStyleBackColor = true;
            // 
            // rightNormalJointSpace
            // 
            this.rightNormalJointSpace.AutoSize = true;
            this.rightNormalJointSpace.Location = new System.Drawing.Point(6, 19);
            this.rightNormalJointSpace.Name = "rightNormalJointSpace";
            this.rightNormalJointSpace.Size = new System.Drawing.Size(58, 17);
            this.rightNormalJointSpace.TabIndex = 0;
            this.rightNormalJointSpace.TabStop = true;
            this.rightNormalJointSpace.Text = "Normal";
            this.rightNormalJointSpace.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.groupBox7);
            this.groupBox6.Controls.Add(this.groupBox10);
            this.groupBox6.Location = new System.Drawing.Point(402, 9);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(123, 232);
            this.groupBox6.TabIndex = 8;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Left";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.leftEminenceSevere);
            this.groupBox7.Controls.Add(this.leftEminenceModerate);
            this.groupBox7.Controls.Add(this.leftEminenceNormal);
            this.groupBox7.Location = new System.Drawing.Point(6, 125);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(111, 100);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Eminence Flatness";
            // 
            // leftEminenceSevere
            // 
            this.leftEminenceSevere.AutoSize = true;
            this.leftEminenceSevere.Location = new System.Drawing.Point(6, 65);
            this.leftEminenceSevere.Name = "leftEminenceSevere";
            this.leftEminenceSevere.Size = new System.Drawing.Size(59, 17);
            this.leftEminenceSevere.TabIndex = 2;
            this.leftEminenceSevere.TabStop = true;
            this.leftEminenceSevere.Text = "Severe";
            this.leftEminenceSevere.UseVisualStyleBackColor = true;
            // 
            // leftEminenceModerate
            // 
            this.leftEminenceModerate.AutoSize = true;
            this.leftEminenceModerate.Location = new System.Drawing.Point(6, 42);
            this.leftEminenceModerate.Name = "leftEminenceModerate";
            this.leftEminenceModerate.Size = new System.Drawing.Size(70, 17);
            this.leftEminenceModerate.TabIndex = 1;
            this.leftEminenceModerate.TabStop = true;
            this.leftEminenceModerate.Text = "Moderate";
            this.leftEminenceModerate.UseVisualStyleBackColor = true;
            // 
            // leftEminenceNormal
            // 
            this.leftEminenceNormal.AutoSize = true;
            this.leftEminenceNormal.Location = new System.Drawing.Point(6, 19);
            this.leftEminenceNormal.Name = "leftEminenceNormal";
            this.leftEminenceNormal.Size = new System.Drawing.Size(58, 17);
            this.leftEminenceNormal.TabIndex = 0;
            this.leftEminenceNormal.TabStop = true;
            this.leftEminenceNormal.Text = "Normal";
            this.leftEminenceNormal.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.leftBoneOnBone);
            this.groupBox10.Controls.Add(this.leftReducedJointSpace);
            this.groupBox10.Controls.Add(this.leftNormalJointSpace);
            this.groupBox10.Location = new System.Drawing.Point(6, 19);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(111, 100);
            this.groupBox10.TabIndex = 5;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Joint Space";
            // 
            // leftBoneOnBone
            // 
            this.leftBoneOnBone.AutoSize = true;
            this.leftBoneOnBone.Location = new System.Drawing.Point(6, 65);
            this.leftBoneOnBone.Name = "leftBoneOnBone";
            this.leftBoneOnBone.Size = new System.Drawing.Size(95, 17);
            this.leftBoneOnBone.TabIndex = 2;
            this.leftBoneOnBone.TabStop = true;
            this.leftBoneOnBone.Text = "Bone On Bone";
            this.leftBoneOnBone.UseVisualStyleBackColor = true;
            // 
            // leftReducedJointSpace
            // 
            this.leftReducedJointSpace.AutoSize = true;
            this.leftReducedJointSpace.Location = new System.Drawing.Point(6, 42);
            this.leftReducedJointSpace.Name = "leftReducedJointSpace";
            this.leftReducedJointSpace.Size = new System.Drawing.Size(69, 17);
            this.leftReducedJointSpace.TabIndex = 1;
            this.leftReducedJointSpace.TabStop = true;
            this.leftReducedJointSpace.Text = "Reduced";
            this.leftReducedJointSpace.UseVisualStyleBackColor = true;
            // 
            // leftNormalJointSpace
            // 
            this.leftNormalJointSpace.AutoSize = true;
            this.leftNormalJointSpace.Location = new System.Drawing.Point(6, 19);
            this.leftNormalJointSpace.Name = "leftNormalJointSpace";
            this.leftNormalJointSpace.Size = new System.Drawing.Size(58, 17);
            this.leftNormalJointSpace.TabIndex = 0;
            this.leftNormalJointSpace.TabStop = true;
            this.leftNormalJointSpace.Text = "Normal";
            this.leftNormalJointSpace.UseVisualStyleBackColor = true;
            // 
            // pictureBox8
            // 
            this.pictureBox8.Image = global::Medical.Properties.Resources.LeftEminenceNormal;
            this.pictureBox8.Location = new System.Drawing.Point(291, 133);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(105, 100);
            this.pictureBox8.TabIndex = 17;
            this.pictureBox8.TabStop = false;
            // 
            // pictureBox7
            // 
            this.pictureBox7.Image = global::Medical.Properties.Resources.LeftJointSpaceNormal;
            this.pictureBox7.Location = new System.Drawing.Point(291, 27);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(105, 100);
            this.pictureBox7.TabIndex = 16;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::Medical.Properties.Resources.RightEminenceNormal;
            this.pictureBox5.Location = new System.Drawing.Point(136, 132);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(105, 100);
            this.pictureBox5.TabIndex = 13;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::Medical.Properties.Resources.RightJointSpaceNormal;
            this.pictureBox4.Location = new System.Drawing.Point(136, 26);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(105, 100);
            this.pictureBox4.TabIndex = 12;
            this.pictureBox4.TabStop = false;
            // 
            // JointPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox8);
            this.Controls.Add(this.pictureBox7);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox1);
            this.Name = "JointPanel";
            this.groupBox1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rightBoneOnBone;
        private System.Windows.Forms.RadioButton rightReducedJointSpace;
        private System.Windows.Forms.RadioButton rightNormalJointSpace;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton rightEminenceSevere;
        private System.Windows.Forms.RadioButton rightEminenceModerate;
        private System.Windows.Forms.RadioButton rightEminenceNormal;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.RadioButton leftEminenceSevere;
        private System.Windows.Forms.RadioButton leftEminenceModerate;
        private System.Windows.Forms.RadioButton leftEminenceNormal;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.RadioButton leftBoneOnBone;
        private System.Windows.Forms.RadioButton leftReducedJointSpace;
        private System.Windows.Forms.RadioButton leftNormalJointSpace;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.PictureBox pictureBox8;

    }
}
