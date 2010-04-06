namespace Medical.GUI
{
    partial class MouthInteriorControl
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
            this.tongueCollisionCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tongueModeCombo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tonguePosition = new System.Windows.Forms.NumericUpDown();
            this.lipCollisionCheck = new System.Windows.Forms.CheckBox();
            this.topLipsRigidCheckBox = new System.Windows.Forms.CheckBox();
            this.resetLipsButton = new System.Windows.Forms.Button();
            this.bottomLipsRigidCheck = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.tonguePosition)).BeginInit();
            this.SuspendLayout();
            // 
            // tongueCollisionCheckBox
            // 
            this.tongueCollisionCheckBox.AutoSize = true;
            this.tongueCollisionCheckBox.Location = new System.Drawing.Point(3, 93);
            this.tongueCollisionCheckBox.Name = "tongueCollisionCheckBox";
            this.tongueCollisionCheckBox.Size = new System.Drawing.Size(140, 17);
            this.tongueCollisionCheckBox.TabIndex = 0;
            this.tongueCollisionCheckBox.Text = "Enable Tongue Collision";
            this.tongueCollisionCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Tongue Mode";
            // 
            // tongueModeCombo
            // 
            this.tongueModeCombo.FormattingEnabled = true;
            this.tongueModeCombo.Location = new System.Drawing.Point(7, 21);
            this.tongueModeCombo.Name = "tongueModeCombo";
            this.tongueModeCombo.Size = new System.Drawing.Size(121, 21);
            this.tongueModeCombo.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Tongue Position";
            // 
            // tonguePosition
            // 
            this.tonguePosition.DecimalPlaces = 4;
            this.tonguePosition.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.tonguePosition.Location = new System.Drawing.Point(8, 65);
            this.tonguePosition.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tonguePosition.Name = "tonguePosition";
            this.tonguePosition.Size = new System.Drawing.Size(120, 20);
            this.tonguePosition.TabIndex = 4;
            // 
            // lipCollisionCheck
            // 
            this.lipCollisionCheck.AutoSize = true;
            this.lipCollisionCheck.Location = new System.Drawing.Point(3, 210);
            this.lipCollisionCheck.Name = "lipCollisionCheck";
            this.lipCollisionCheck.Size = new System.Drawing.Size(117, 17);
            this.lipCollisionCheck.TabIndex = 5;
            this.lipCollisionCheck.Text = "Enable Lip Collision";
            this.lipCollisionCheck.UseVisualStyleBackColor = true;
            // 
            // topLipsRigidCheckBox
            // 
            this.topLipsRigidCheckBox.AutoSize = true;
            this.topLipsRigidCheckBox.Location = new System.Drawing.Point(3, 166);
            this.topLipsRigidCheckBox.Name = "topLipsRigidCheckBox";
            this.topLipsRigidCheckBox.Size = new System.Drawing.Size(94, 17);
            this.topLipsRigidCheckBox.TabIndex = 6;
            this.topLipsRigidCheckBox.Text = "Top Lips Rigid";
            this.topLipsRigidCheckBox.UseVisualStyleBackColor = true;
            // 
            // resetLipsButton
            // 
            this.resetLipsButton.Location = new System.Drawing.Point(4, 234);
            this.resetLipsButton.Name = "resetLipsButton";
            this.resetLipsButton.Size = new System.Drawing.Size(75, 23);
            this.resetLipsButton.TabIndex = 7;
            this.resetLipsButton.Text = "Reset Lips";
            this.resetLipsButton.UseVisualStyleBackColor = true;
            this.resetLipsButton.Click += new System.EventHandler(this.resetLipsButton_Click);
            // 
            // bottomLipsRigidCheck
            // 
            this.bottomLipsRigidCheck.AutoSize = true;
            this.bottomLipsRigidCheck.Location = new System.Drawing.Point(3, 187);
            this.bottomLipsRigidCheck.Name = "bottomLipsRigidCheck";
            this.bottomLipsRigidCheck.Size = new System.Drawing.Size(108, 17);
            this.bottomLipsRigidCheck.TabIndex = 10;
            this.bottomLipsRigidCheck.Text = "Bottom Lips Rigid";
            this.bottomLipsRigidCheck.UseVisualStyleBackColor = true;
            // 
            // MouthInteriorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ButtonText = "Mouth Interior";
            this.Controls.Add(this.bottomLipsRigidCheck);
            this.Controls.Add(this.resetLipsButton);
            this.Controls.Add(this.topLipsRigidCheckBox);
            this.Controls.Add(this.lipCollisionCheck);
            this.Controls.Add(this.tonguePosition);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tongueModeCombo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tongueCollisionCheckBox);
            this.Name = "MouthInteriorControl";
            this.Size = new System.Drawing.Size(176, 304);
            this.ToolStripName = "Advanced";
            ((System.ComponentModel.ISupportInitialize)(this.tonguePosition)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox tongueCollisionCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox tongueModeCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown tonguePosition;
        private System.Windows.Forms.CheckBox lipCollisionCheck;
        private System.Windows.Forms.CheckBox topLipsRigidCheckBox;
        private System.Windows.Forms.Button resetLipsButton;
        private System.Windows.Forms.CheckBox bottomLipsRigidCheck;
    }
}
