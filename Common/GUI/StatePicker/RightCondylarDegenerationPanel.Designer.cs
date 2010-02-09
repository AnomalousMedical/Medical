namespace Medical.GUI
{
    partial class RightCondylarDegenerationPanel
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
            this.rightCondyleDegenerationSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightLateralPoleSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightMedialPoleScaleSlider = new Medical.GUI.BoneManipulatorSlider();
            this.makeNormalButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.undoButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.wearSlider = new Medical.GUI.BoneManipulatorSlider();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.kryptonWrapLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.gridPropertiesControl1 = new Medical.GUI.Grid.GridPropertiesControl();
            this.SuspendLayout();
            // 
            // rightCondyleDegenerationSlider
            // 
            this.rightCondyleDegenerationSlider.LabelText = "Condyle";
            this.rightCondyleDegenerationSlider.Location = new System.Drawing.Point(64, 26);
            this.rightCondyleDegenerationSlider.Name = "rightCondyleDegenerationSlider";
            this.rightCondyleDegenerationSlider.Size = new System.Drawing.Size(197, 44);
            this.rightCondyleDegenerationSlider.TabIndex = 45;
            this.rightCondyleDegenerationSlider.Tag = "rightCondyleDegenerationMandible";
            this.rightCondyleDegenerationSlider.Value = 0F;
            // 
            // rightLateralPoleSlider
            // 
            this.rightLateralPoleSlider.LabelText = "Lateral Pole";
            this.rightLateralPoleSlider.Location = new System.Drawing.Point(64, 93);
            this.rightLateralPoleSlider.Name = "rightLateralPoleSlider";
            this.rightLateralPoleSlider.Size = new System.Drawing.Size(197, 44);
            this.rightLateralPoleSlider.TabIndex = 46;
            this.rightLateralPoleSlider.Tag = "rightLateralPoleMandible";
            this.rightLateralPoleSlider.Value = 0F;
            // 
            // rightMedialPoleScaleSlider
            // 
            this.rightMedialPoleScaleSlider.LabelText = "Medial Pole";
            this.rightMedialPoleScaleSlider.Location = new System.Drawing.Point(64, 157);
            this.rightMedialPoleScaleSlider.Name = "rightMedialPoleScaleSlider";
            this.rightMedialPoleScaleSlider.Size = new System.Drawing.Size(197, 44);
            this.rightMedialPoleScaleSlider.TabIndex = 47;
            this.rightMedialPoleScaleSlider.Tag = "rightMedialPoleScaleMandible";
            this.rightMedialPoleScaleSlider.Value = 0F;
            // 
            // makeNormalButton
            // 
            this.makeNormalButton.Location = new System.Drawing.Point(100, 281);
            this.makeNormalButton.Name = "makeNormalButton";
            this.makeNormalButton.Size = new System.Drawing.Size(90, 25);
            this.makeNormalButton.TabIndex = 44;
            this.makeNormalButton.Values.Text = "Make Normal";
            this.makeNormalButton.Click += new System.EventHandler(this.makeNormalButton_Click);
            // 
            // undoButton
            // 
            this.undoButton.Location = new System.Drawing.Point(3, 281);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(90, 25);
            this.undoButton.TabIndex = 43;
            this.undoButton.Values.Text = "Undo";
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = global::Medical.Properties.Resources.DegenerationRightCondyleMedialPoleDistorted;
            this.panel5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel5.Location = new System.Drawing.Point(262, 149);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(60, 60);
            this.panel5.TabIndex = 42;
            // 
            // panel6
            // 
            this.panel6.BackgroundImage = global::Medical.Properties.Resources.DegenerationRightCondyleMedialPole;
            this.panel6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel6.Location = new System.Drawing.Point(3, 149);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(60, 60);
            this.panel6.TabIndex = 40;
            // 
            // panel3
            // 
            this.panel3.BackgroundImage = global::Medical.Properties.Resources.DegenerationRightCondyleLateralPoleDistorted;
            this.panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel3.Location = new System.Drawing.Point(262, 84);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(60, 60);
            this.panel3.TabIndex = 41;
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::Medical.Properties.Resources.DegenerationRightCondyleDistorted;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Location = new System.Drawing.Point(262, 19);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(60, 60);
            this.panel2.TabIndex = 38;
            // 
            // panel4
            // 
            this.panel4.BackgroundImage = global::Medical.Properties.Resources.DegenerationRightCondyleLateralPole;
            this.panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel4.Location = new System.Drawing.Point(3, 84);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(60, 60);
            this.panel4.TabIndex = 39;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Medical.Properties.Resources.DegenerationRightCondyle;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Location = new System.Drawing.Point(3, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(60, 60);
            this.panel1.TabIndex = 37;
            // 
            // wearSlider
            // 
            this.wearSlider.LabelText = "Wear";
            this.wearSlider.Location = new System.Drawing.Point(64, 223);
            this.wearSlider.Name = "wearSlider";
            this.wearSlider.Size = new System.Drawing.Size(197, 44);
            this.wearSlider.TabIndex = 50;
            this.wearSlider.Tag = "rightCondyleRoughnessMandible";
            this.wearSlider.Value = 0F;
            // 
            // panel7
            // 
            this.panel7.BackgroundImage = global::Medical.Properties.Resources.DegenerationRightCondyleRoughnessDistorted;
            this.panel7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel7.Location = new System.Drawing.Point(262, 215);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(60, 60);
            this.panel7.TabIndex = 49;
            // 
            // panel8
            // 
            this.panel8.BackgroundImage = global::Medical.Properties.Resources.DegenerationRightCondyleRoughness;
            this.panel8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel8.Location = new System.Drawing.Point(3, 215);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(60, 60);
            this.panel8.TabIndex = 48;
            // 
            // kryptonWrapLabel1
            // 
            this.kryptonWrapLabel1.AutoSize = false;
            this.kryptonWrapLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonWrapLabel1.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(59)))), ((int)(((byte)(59)))));
            this.kryptonWrapLabel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonWrapLabel1.Name = "kryptonWrapLabel1";
            this.kryptonWrapLabel1.Size = new System.Drawing.Size(325, 17);
            this.kryptonWrapLabel1.Text = "Adjust the sliders to match the patient\'s right condyle.";
            // 
            // gridPropertiesControl1
            // 
            this.gridPropertiesControl1.GridSpacing = 5F;
            this.gridPropertiesControl1.GridVisible = true;
            this.gridPropertiesControl1.Location = new System.Drawing.Point(3, 312);
            this.gridPropertiesControl1.Name = "gridPropertiesControl1";
            this.gridPropertiesControl1.Size = new System.Drawing.Size(253, 44);
            this.gridPropertiesControl1.TabIndex = 51;
            // 
            // RightCondylarDegenerationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridPropertiesControl1);
            this.Controls.Add(this.kryptonWrapLabel1);
            this.Controls.Add(this.wearSlider);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.rightCondyleDegenerationSlider);
            this.Controls.Add(this.rightLateralPoleSlider);
            this.Controls.Add(this.rightMedialPoleScaleSlider);
            this.Controls.Add(this.makeNormalButton);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.LargeIcon = global::Medical.Properties.Resources.RightCondyleDegeneration;
            this.LayerState = "MandibleSliderSizeLayers";
            this.Name = "RightCondylarDegenerationPanel";
            this.NavigationState = "WizardDegenerationRightCameraAngle";
            this.Size = new System.Drawing.Size(325, 361);
            this.TextLine1 = "Right Condyle";
            this.TextLine2 = "Degeneration";
            this.ResumeLayout(false);

        }

        #endregion

        private BoneManipulatorSlider rightCondyleDegenerationSlider;
        private BoneManipulatorSlider rightLateralPoleSlider;
        private BoneManipulatorSlider rightMedialPoleScaleSlider;
        private ComponentFactory.Krypton.Toolkit.KryptonButton makeNormalButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton undoButton;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel1;
        private BoneManipulatorSlider wearSlider;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel8;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel1;
        private Medical.GUI.Grid.GridPropertiesControl gridPropertiesControl1;
    }
}
