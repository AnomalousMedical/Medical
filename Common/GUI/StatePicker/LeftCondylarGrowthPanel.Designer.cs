namespace Medical.GUI
{
    partial class LeftCondylarGrowthPanel
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
            this.leftRamusHeightSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftCondyleHeightSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftCondyleRotationSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftMandibularNotchSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftAntegonialNotchSlider = new Medical.GUI.BoneManipulatorSlider();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.undoButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.makeNormalButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonWrapLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.gridPropertiesControl1 = new Medical.GUI.Grid.GridPropertiesControl();
            this.SuspendLayout();
            // 
            // leftRamusHeightSlider
            // 
            this.leftRamusHeightSlider.LabelText = "Ramus Height";
            this.leftRamusHeightSlider.Location = new System.Drawing.Point(69, 48);
            this.leftRamusHeightSlider.Name = "leftRamusHeightSlider";
            this.leftRamusHeightSlider.Size = new System.Drawing.Size(187, 44);
            this.leftRamusHeightSlider.TabIndex = 10;
            this.leftRamusHeightSlider.Tag = "leftRamusHeightMandible";
            this.leftRamusHeightSlider.Value = 0F;
            // 
            // leftCondyleHeightSlider
            // 
            this.leftCondyleHeightSlider.LabelText = "Condyle Height";
            this.leftCondyleHeightSlider.Location = new System.Drawing.Point(69, 113);
            this.leftCondyleHeightSlider.Name = "leftCondyleHeightSlider";
            this.leftCondyleHeightSlider.Size = new System.Drawing.Size(187, 44);
            this.leftCondyleHeightSlider.TabIndex = 7;
            this.leftCondyleHeightSlider.Tag = "leftCondyleHeightMandible";
            this.leftCondyleHeightSlider.Value = 0F;
            // 
            // leftCondyleRotationSlider
            // 
            this.leftCondyleRotationSlider.LabelText = "Condyle Rotation";
            this.leftCondyleRotationSlider.Location = new System.Drawing.Point(69, 178);
            this.leftCondyleRotationSlider.Name = "leftCondyleRotationSlider";
            this.leftCondyleRotationSlider.Size = new System.Drawing.Size(187, 44);
            this.leftCondyleRotationSlider.TabIndex = 8;
            this.leftCondyleRotationSlider.Tag = "leftCondyleRotationMandible";
            this.leftCondyleRotationSlider.Value = 0F;
            // 
            // leftMandibularNotchSlider
            // 
            this.leftMandibularNotchSlider.LabelText = "Mandibular Notch";
            this.leftMandibularNotchSlider.Location = new System.Drawing.Point(69, 243);
            this.leftMandibularNotchSlider.Name = "leftMandibularNotchSlider";
            this.leftMandibularNotchSlider.Size = new System.Drawing.Size(187, 44);
            this.leftMandibularNotchSlider.TabIndex = 9;
            this.leftMandibularNotchSlider.Tag = "leftMandibularNotchMandible";
            this.leftMandibularNotchSlider.Value = 0F;
            // 
            // leftAntegonialNotchSlider
            // 
            this.leftAntegonialNotchSlider.LabelText = "Antegonial Notch";
            this.leftAntegonialNotchSlider.Location = new System.Drawing.Point(69, 308);
            this.leftAntegonialNotchSlider.Name = "leftAntegonialNotchSlider";
            this.leftAntegonialNotchSlider.Size = new System.Drawing.Size(187, 44);
            this.leftAntegonialNotchSlider.TabIndex = 6;
            this.leftAntegonialNotchSlider.Tag = "leftAntegonialNotchMandible";
            this.leftAntegonialNotchSlider.Value = 0F;
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::Medical.Properties.Resources.GrowthLeftRamusHeightSmall;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Location = new System.Drawing.Point(262, 40);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(60, 60);
            this.panel2.TabIndex = 12;
            // 
            // panel3
            // 
            this.panel3.BackgroundImage = global::Medical.Properties.Resources.GrowthLeftCondyleHeightSmall;
            this.panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel3.Location = new System.Drawing.Point(262, 105);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(60, 60);
            this.panel3.TabIndex = 14;
            // 
            // panel4
            // 
            this.panel4.BackgroundImage = global::Medical.Properties.Resources.GrowthLeftCondyleHeight;
            this.panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel4.Location = new System.Drawing.Point(3, 105);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(60, 60);
            this.panel4.TabIndex = 13;
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = global::Medical.Properties.Resources.GrowthLeftCondyleRotationRotated;
            this.panel5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel5.Location = new System.Drawing.Point(262, 170);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(60, 60);
            this.panel5.TabIndex = 14;
            // 
            // panel6
            // 
            this.panel6.BackgroundImage = global::Medical.Properties.Resources.GrowthLeftCondyleRotation;
            this.panel6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel6.Location = new System.Drawing.Point(3, 170);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(60, 60);
            this.panel6.TabIndex = 13;
            // 
            // panel7
            // 
            this.panel7.BackgroundImage = global::Medical.Properties.Resources.GrowthLeftMandibularNotchDistorted;
            this.panel7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel7.Location = new System.Drawing.Point(262, 235);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(60, 60);
            this.panel7.TabIndex = 14;
            // 
            // panel8
            // 
            this.panel8.BackgroundImage = global::Medical.Properties.Resources.GrowthLeftMandibularNotch;
            this.panel8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel8.Location = new System.Drawing.Point(3, 235);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(60, 60);
            this.panel8.TabIndex = 13;
            // 
            // panel9
            // 
            this.panel9.BackgroundImage = global::Medical.Properties.Resources.GrowthLeftAntegonialNotchDistorted;
            this.panel9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel9.Location = new System.Drawing.Point(262, 300);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(60, 60);
            this.panel9.TabIndex = 14;
            // 
            // panel10
            // 
            this.panel10.BackgroundImage = global::Medical.Properties.Resources.GrowthLeftAntegonialNotch;
            this.panel10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel10.Location = new System.Drawing.Point(3, 300);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(60, 60);
            this.panel10.TabIndex = 13;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Medical.Properties.Resources.GrowthLeftRamusHeight;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Location = new System.Drawing.Point(3, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(60, 60);
            this.panel1.TabIndex = 11;
            // 
            // undoButton
            // 
            this.undoButton.Location = new System.Drawing.Point(4, 367);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(90, 25);
            this.undoButton.TabIndex = 15;
            this.undoButton.Values.Text = "Undo";
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // makeNormalButton
            // 
            this.makeNormalButton.Location = new System.Drawing.Point(101, 367);
            this.makeNormalButton.Name = "makeNormalButton";
            this.makeNormalButton.Size = new System.Drawing.Size(90, 25);
            this.makeNormalButton.TabIndex = 16;
            this.makeNormalButton.Values.Text = "Make Normal";
            this.makeNormalButton.Click += new System.EventHandler(this.makeNormalButton_Click);
            // 
            // kryptonWrapLabel1
            // 
            this.kryptonWrapLabel1.AutoSize = false;
            this.kryptonWrapLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonWrapLabel1.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(59)))), ((int)(((byte)(59)))));
            this.kryptonWrapLabel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonWrapLabel1.Name = "kryptonWrapLabel1";
            this.kryptonWrapLabel1.Size = new System.Drawing.Size(325, 31);
            this.kryptonWrapLabel1.Text = "Adjust the sliders to match the left side of the patient\'s mandible.";
            // 
            // gridPropertiesControl1
            // 
            this.gridPropertiesControl1.GridSpacing = 5F;
            this.gridPropertiesControl1.GridVisible = true;
            this.gridPropertiesControl1.Location = new System.Drawing.Point(4, 399);
            this.gridPropertiesControl1.Name = "gridPropertiesControl1";
            this.gridPropertiesControl1.Size = new System.Drawing.Size(253, 44);
            this.gridPropertiesControl1.TabIndex = 17;
            // 
            // LeftCondylarGrowthPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridPropertiesControl1);
            this.Controls.Add(this.kryptonWrapLabel1);
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
            this.Controls.Add(this.leftRamusHeightSlider);
            this.Controls.Add(this.leftCondyleHeightSlider);
            this.Controls.Add(this.leftCondyleRotationSlider);
            this.Controls.Add(this.leftMandibularNotchSlider);
            this.Controls.Add(this.leftAntegonialNotchSlider);
            this.LargeIcon = global::Medical.Properties.Resources.LeftCondyleGrowth;
            this.LayerState = "MandibleSliderSizeLayers";
            this.Name = "LeftCondylarGrowthPanel";
            this.NavigationState = "GrowthLeftCameraAngle";
            this.Size = new System.Drawing.Size(325, 452);
            this.TextLine1 = "Left Condyle";
            this.TextLine2 = "Growth";
            this.ResumeLayout(false);

        }

        #endregion

        private BoneManipulatorSlider leftRamusHeightSlider;
        private BoneManipulatorSlider leftCondyleHeightSlider;
        private BoneManipulatorSlider leftCondyleRotationSlider;
        private BoneManipulatorSlider leftMandibularNotchSlider;
        private BoneManipulatorSlider leftAntegonialNotchSlider;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel10;
        private ComponentFactory.Krypton.Toolkit.KryptonButton undoButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton makeNormalButton;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel1;
        private Medical.GUI.Grid.GridPropertiesControl gridPropertiesControl1;

    }
}
