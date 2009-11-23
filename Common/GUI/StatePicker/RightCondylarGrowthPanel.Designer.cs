namespace Medical.GUI
{
    partial class RightCondylarGrowthPanel
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
            this.makeNormalButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.undoButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rightRamusHeightSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightCondyleHeightSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightCondyleRotationSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightMandibularNotchSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightAntegonialNotchSlider = new Medical.GUI.BoneManipulatorSlider();
            this.SuspendLayout();
            // 
            // makeNormalButton
            // 
            this.makeNormalButton.Location = new System.Drawing.Point(101, 331);
            this.makeNormalButton.Name = "makeNormalButton";
            this.makeNormalButton.Size = new System.Drawing.Size(90, 25);
            this.makeNormalButton.TabIndex = 33;
            this.makeNormalButton.Values.Text = "Make Normal";
            this.makeNormalButton.Click += new System.EventHandler(this.makeNormalButton_Click);
            // 
            // undoButton
            // 
            this.undoButton.Location = new System.Drawing.Point(4, 331);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(90, 25);
            this.undoButton.TabIndex = 32;
            this.undoButton.Values.Text = "Undo";
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // panel9
            // 
            this.panel9.BackgroundImage = global::Medical.Properties.Resources.GrowthRightAntegonialNotchDistorted;
            this.panel9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel9.Location = new System.Drawing.Point(262, 264);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(60, 60);
            this.panel9.TabIndex = 29;
            // 
            // panel10
            // 
            this.panel10.BackgroundImage = global::Medical.Properties.Resources.GrowthRightAntegonialNotch;
            this.panel10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel10.Location = new System.Drawing.Point(3, 264);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(60, 60);
            this.panel10.TabIndex = 25;
            // 
            // panel7
            // 
            this.panel7.BackgroundImage = global::Medical.Properties.Resources.GrowthRightMandibularNotchDistorted;
            this.panel7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel7.Location = new System.Drawing.Point(262, 199);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(60, 60);
            this.panel7.TabIndex = 28;
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = global::Medical.Properties.Resources.GrowthRightCondyleRotationRotated;
            this.panel5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel5.Location = new System.Drawing.Point(262, 134);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(60, 60);
            this.panel5.TabIndex = 31;
            // 
            // panel8
            // 
            this.panel8.BackgroundImage = global::Medical.Properties.Resources.GrowthRightMandibularNotch;
            this.panel8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel8.Location = new System.Drawing.Point(3, 199);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(60, 60);
            this.panel8.TabIndex = 26;
            // 
            // panel6
            // 
            this.panel6.BackgroundImage = global::Medical.Properties.Resources.GrowthRightCondyleRotation;
            this.panel6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel6.Location = new System.Drawing.Point(3, 134);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(60, 60);
            this.panel6.TabIndex = 27;
            // 
            // panel3
            // 
            this.panel3.BackgroundImage = global::Medical.Properties.Resources.GrowthRightCondyleHeightSmall;
            this.panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel3.Location = new System.Drawing.Point(262, 69);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(60, 60);
            this.panel3.TabIndex = 30;
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::Medical.Properties.Resources.GrowthRightRamusHeightSmall;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Location = new System.Drawing.Point(262, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(60, 60);
            this.panel2.TabIndex = 23;
            // 
            // panel4
            // 
            this.panel4.BackgroundImage = global::Medical.Properties.Resources.GrowthRightCondyleHeight;
            this.panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel4.Location = new System.Drawing.Point(3, 69);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(60, 60);
            this.panel4.TabIndex = 24;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Medical.Properties.Resources.GrowthRightRamusHeight;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Location = new System.Drawing.Point(3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(60, 60);
            this.panel1.TabIndex = 22;
            // 
            // rightRamusHeightSlider
            // 
            this.rightRamusHeightSlider.LabelText = "Ramus Height";
            this.rightRamusHeightSlider.Location = new System.Drawing.Point(69, 12);
            this.rightRamusHeightSlider.Name = "rightRamusHeightSlider";
            this.rightRamusHeightSlider.Size = new System.Drawing.Size(187, 44);
            this.rightRamusHeightSlider.TabIndex = 21;
            this.rightRamusHeightSlider.Tag = "rightRamusHeightMandible";
            this.rightRamusHeightSlider.Value = 0F;
            // 
            // rightCondyleHeightSlider
            // 
            this.rightCondyleHeightSlider.LabelText = "Condyle Height";
            this.rightCondyleHeightSlider.Location = new System.Drawing.Point(69, 77);
            this.rightCondyleHeightSlider.Name = "rightCondyleHeightSlider";
            this.rightCondyleHeightSlider.Size = new System.Drawing.Size(187, 44);
            this.rightCondyleHeightSlider.TabIndex = 18;
            this.rightCondyleHeightSlider.Tag = "rightCondyleHeightMandible";
            this.rightCondyleHeightSlider.Value = 0F;
            // 
            // rightCondyleRotationSlider
            // 
            this.rightCondyleRotationSlider.LabelText = "Condyle Rotation";
            this.rightCondyleRotationSlider.Location = new System.Drawing.Point(69, 142);
            this.rightCondyleRotationSlider.Name = "rightCondyleRotationSlider";
            this.rightCondyleRotationSlider.Size = new System.Drawing.Size(187, 44);
            this.rightCondyleRotationSlider.TabIndex = 19;
            this.rightCondyleRotationSlider.Tag = "rightCondyleRotationMandible";
            this.rightCondyleRotationSlider.Value = 0F;
            // 
            // rightMandibularNotchSlider
            // 
            this.rightMandibularNotchSlider.LabelText = "Mandibular Notch";
            this.rightMandibularNotchSlider.Location = new System.Drawing.Point(69, 207);
            this.rightMandibularNotchSlider.Name = "rightMandibularNotchSlider";
            this.rightMandibularNotchSlider.Size = new System.Drawing.Size(187, 44);
            this.rightMandibularNotchSlider.TabIndex = 20;
            this.rightMandibularNotchSlider.Tag = "rightMandibularNotchMandible";
            this.rightMandibularNotchSlider.Value = 0F;
            // 
            // rightAntegonialNotchSlider
            // 
            this.rightAntegonialNotchSlider.LabelText = "Antegonial Notch";
            this.rightAntegonialNotchSlider.Location = new System.Drawing.Point(69, 272);
            this.rightAntegonialNotchSlider.Name = "rightAntegonialNotchSlider";
            this.rightAntegonialNotchSlider.Size = new System.Drawing.Size(187, 44);
            this.rightAntegonialNotchSlider.TabIndex = 17;
            this.rightAntegonialNotchSlider.Tag = "rightAntegonialNotchMandible";
            this.rightAntegonialNotchSlider.Value = 0F;
            // 
            // RightCondylarGrowthPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.makeNormalButton);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.panel9);
            this.Controls.Add(this.panel10);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.rightRamusHeightSlider);
            this.Controls.Add(this.rightCondyleHeightSlider);
            this.Controls.Add(this.rightCondyleRotationSlider);
            this.Controls.Add(this.rightMandibularNotchSlider);
            this.Controls.Add(this.rightAntegonialNotchSlider);
            this.LayerState = "MandibleSliderSizeLayers";
            this.Name = "RightCondylarGrowthPanel";
            this.NavigationState = "GrowthRightCameraAngle";
            this.Size = new System.Drawing.Size(325, 361);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton makeNormalButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton undoButton;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel1;
        private BoneManipulatorSlider rightRamusHeightSlider;
        private BoneManipulatorSlider rightCondyleHeightSlider;
        private BoneManipulatorSlider rightCondyleRotationSlider;
        private BoneManipulatorSlider rightMandibularNotchSlider;
        private BoneManipulatorSlider rightAntegonialNotchSlider;
    }
}
