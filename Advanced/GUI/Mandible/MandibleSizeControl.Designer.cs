namespace Medical.GUI
{
    partial class MandibleSizeControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MandibleSizeControl));
            this.sliderPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.leftRamusHeightSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftCondyleHeightSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftCondyleRotationSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftCondyleDegenerationSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftLateralPoleSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftMedialPoleScaleSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftMandibularNotchSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftAntegonialNotchSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightRamusHeightSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightCondyleHeightSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightCondyleRotationSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightCondyleDegenerationSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightLateralPoleDegeneration = new Medical.GUI.BoneManipulatorSlider();
            this.rightMedialPoleScaleDegeneration = new Medical.GUI.BoneManipulatorSlider();
            this.rightMandibularNotchSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightAntegonialNotchSlider = new Medical.GUI.BoneManipulatorSlider();
            this.resetButton = new System.Windows.Forms.Button();
            this.sliderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // sliderPanel
            // 
            this.sliderPanel.AutoSize = true;
            this.sliderPanel.Controls.Add(this.leftRamusHeightSlider);
            this.sliderPanel.Controls.Add(this.leftCondyleHeightSlider);
            this.sliderPanel.Controls.Add(this.leftCondyleRotationSlider);
            this.sliderPanel.Controls.Add(this.leftCondyleDegenerationSlider);
            this.sliderPanel.Controls.Add(this.leftLateralPoleSlider);
            this.sliderPanel.Controls.Add(this.leftMedialPoleScaleSlider);
            this.sliderPanel.Controls.Add(this.leftMandibularNotchSlider);
            this.sliderPanel.Controls.Add(this.leftAntegonialNotchSlider);
            this.sliderPanel.Controls.Add(this.rightRamusHeightSlider);
            this.sliderPanel.Controls.Add(this.rightCondyleHeightSlider);
            this.sliderPanel.Controls.Add(this.rightCondyleRotationSlider);
            this.sliderPanel.Controls.Add(this.rightCondyleDegenerationSlider);
            this.sliderPanel.Controls.Add(this.rightLateralPoleDegeneration);
            this.sliderPanel.Controls.Add(this.rightMedialPoleScaleDegeneration);
            this.sliderPanel.Controls.Add(this.rightMandibularNotchSlider);
            this.sliderPanel.Controls.Add(this.rightAntegonialNotchSlider);
            this.sliderPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.sliderPanel.Location = new System.Drawing.Point(2, 32);
            this.sliderPanel.Name = "sliderPanel";
            this.sliderPanel.Size = new System.Drawing.Size(211, 800);
            this.sliderPanel.TabIndex = 1;
            // 
            // leftRamusHeightSlider
            // 
            this.leftRamusHeightSlider.LabelText = "Left Ramus Height";
            this.leftRamusHeightSlider.Location = new System.Drawing.Point(3, 3);
            this.leftRamusHeightSlider.Name = "leftRamusHeightSlider";
            this.leftRamusHeightSlider.Size = new System.Drawing.Size(197, 44);
            this.leftRamusHeightSlider.TabIndex = 5;
            this.leftRamusHeightSlider.Tag = "leftRamusHeightMandible";
            this.leftRamusHeightSlider.Value = 0F;
            // 
            // leftCondyleHeightSlider
            // 
            this.leftCondyleHeightSlider.LabelText = "Left Condyle Height";
            this.leftCondyleHeightSlider.Location = new System.Drawing.Point(3, 53);
            this.leftCondyleHeightSlider.Name = "leftCondyleHeightSlider";
            this.leftCondyleHeightSlider.Size = new System.Drawing.Size(197, 44);
            this.leftCondyleHeightSlider.TabIndex = 2;
            this.leftCondyleHeightSlider.Tag = "leftCondyleHeightMandible";
            this.leftCondyleHeightSlider.Value = 0F;
            // 
            // leftCondyleRotationSlider
            // 
            this.leftCondyleRotationSlider.LabelText = "Left Condyle Rotation";
            this.leftCondyleRotationSlider.Location = new System.Drawing.Point(3, 103);
            this.leftCondyleRotationSlider.Name = "leftCondyleRotationSlider";
            this.leftCondyleRotationSlider.Size = new System.Drawing.Size(197, 44);
            this.leftCondyleRotationSlider.TabIndex = 3;
            this.leftCondyleRotationSlider.Tag = "leftCondyleRotationMandible";
            this.leftCondyleRotationSlider.Value = 0F;
            // 
            // leftCondyleDegenerationSlider
            // 
            this.leftCondyleDegenerationSlider.LabelText = "Left Condyle Degenertaion";
            this.leftCondyleDegenerationSlider.Location = new System.Drawing.Point(3, 153);
            this.leftCondyleDegenerationSlider.Name = "leftCondyleDegenerationSlider";
            this.leftCondyleDegenerationSlider.Size = new System.Drawing.Size(197, 44);
            this.leftCondyleDegenerationSlider.TabIndex = 1;
            this.leftCondyleDegenerationSlider.Tag = "leftCondyleDegenerationMandible";
            this.leftCondyleDegenerationSlider.Value = 0F;
            // 
            // leftLateralPoleSlider
            // 
            this.leftLateralPoleSlider.LabelText = "Left Lateral Pole Degeneration";
            this.leftLateralPoleSlider.Location = new System.Drawing.Point(3, 203);
            this.leftLateralPoleSlider.Name = "leftLateralPoleSlider";
            this.leftLateralPoleSlider.Size = new System.Drawing.Size(197, 44);
            this.leftLateralPoleSlider.TabIndex = 12;
            this.leftLateralPoleSlider.Tag = "leftLateralPoleMandible";
            this.leftLateralPoleSlider.Value = 0F;
            // 
            // leftMedialPoleScaleSlider
            // 
            this.leftMedialPoleScaleSlider.LabelText = "Left Medial Pole Degeneration";
            this.leftMedialPoleScaleSlider.Location = new System.Drawing.Point(3, 253);
            this.leftMedialPoleScaleSlider.Name = "leftMedialPoleScaleSlider";
            this.leftMedialPoleScaleSlider.Size = new System.Drawing.Size(197, 44);
            this.leftMedialPoleScaleSlider.TabIndex = 13;
            this.leftMedialPoleScaleSlider.Tag = "leftMedialPoleScaleMandible";
            this.leftMedialPoleScaleSlider.Value = 0F;
            // 
            // leftMandibularNotchSlider
            // 
            this.leftMandibularNotchSlider.LabelText = "Left Mandibular Notch";
            this.leftMandibularNotchSlider.Location = new System.Drawing.Point(3, 303);
            this.leftMandibularNotchSlider.Name = "leftMandibularNotchSlider";
            this.leftMandibularNotchSlider.Size = new System.Drawing.Size(197, 44);
            this.leftMandibularNotchSlider.TabIndex = 4;
            this.leftMandibularNotchSlider.Tag = "leftMandibularNotchMandible";
            this.leftMandibularNotchSlider.Value = 0F;
            // 
            // leftAntegonialNotchSlider
            // 
            this.leftAntegonialNotchSlider.LabelText = "Left Antegonial Notch";
            this.leftAntegonialNotchSlider.Location = new System.Drawing.Point(3, 353);
            this.leftAntegonialNotchSlider.Name = "leftAntegonialNotchSlider";
            this.leftAntegonialNotchSlider.Size = new System.Drawing.Size(197, 44);
            this.leftAntegonialNotchSlider.TabIndex = 0;
            this.leftAntegonialNotchSlider.Tag = "leftAntegonialNotchMandible";
            this.leftAntegonialNotchSlider.Value = 0F;
            // 
            // rightRamusHeightSlider
            // 
            this.rightRamusHeightSlider.LabelText = "Right Ramus Height";
            this.rightRamusHeightSlider.Location = new System.Drawing.Point(3, 403);
            this.rightRamusHeightSlider.Name = "rightRamusHeightSlider";
            this.rightRamusHeightSlider.Size = new System.Drawing.Size(197, 44);
            this.rightRamusHeightSlider.TabIndex = 11;
            this.rightRamusHeightSlider.Tag = "rightRamusHeightMandible";
            this.rightRamusHeightSlider.Value = 0F;
            // 
            // rightCondyleHeightSlider
            // 
            this.rightCondyleHeightSlider.LabelText = "Right Condyle Height";
            this.rightCondyleHeightSlider.Location = new System.Drawing.Point(3, 453);
            this.rightCondyleHeightSlider.Name = "rightCondyleHeightSlider";
            this.rightCondyleHeightSlider.Size = new System.Drawing.Size(197, 44);
            this.rightCondyleHeightSlider.TabIndex = 8;
            this.rightCondyleHeightSlider.Tag = "rightCondyleHeightMandible";
            this.rightCondyleHeightSlider.Value = 0F;
            // 
            // rightCondyleRotationSlider
            // 
            this.rightCondyleRotationSlider.LabelText = "Right Condyle Rotation";
            this.rightCondyleRotationSlider.Location = new System.Drawing.Point(3, 503);
            this.rightCondyleRotationSlider.Name = "rightCondyleRotationSlider";
            this.rightCondyleRotationSlider.Size = new System.Drawing.Size(197, 44);
            this.rightCondyleRotationSlider.TabIndex = 9;
            this.rightCondyleRotationSlider.Tag = "rightCondyleRotationMandible";
            this.rightCondyleRotationSlider.Value = 0F;
            // 
            // rightCondyleDegenerationSlider
            // 
            this.rightCondyleDegenerationSlider.LabelText = "Right Condyle Degenertaion";
            this.rightCondyleDegenerationSlider.Location = new System.Drawing.Point(3, 553);
            this.rightCondyleDegenerationSlider.Name = "rightCondyleDegenerationSlider";
            this.rightCondyleDegenerationSlider.Size = new System.Drawing.Size(197, 44);
            this.rightCondyleDegenerationSlider.TabIndex = 7;
            this.rightCondyleDegenerationSlider.Tag = "rightCondyleDegenerationMandible";
            this.rightCondyleDegenerationSlider.Value = 0F;
            // 
            // rightLateralPoleDegeneration
            // 
            this.rightLateralPoleDegeneration.LabelText = "Right Lateral Pole Degeneration";
            this.rightLateralPoleDegeneration.Location = new System.Drawing.Point(3, 603);
            this.rightLateralPoleDegeneration.Name = "rightLateralPoleDegeneration";
            this.rightLateralPoleDegeneration.Size = new System.Drawing.Size(197, 44);
            this.rightLateralPoleDegeneration.TabIndex = 14;
            this.rightLateralPoleDegeneration.Tag = "rightLateralPoleMandible";
            this.rightLateralPoleDegeneration.Value = 0F;
            // 
            // rightMedialPoleScaleDegeneration
            // 
            this.rightMedialPoleScaleDegeneration.LabelText = "Right Medial Pole Degeneration";
            this.rightMedialPoleScaleDegeneration.Location = new System.Drawing.Point(3, 653);
            this.rightMedialPoleScaleDegeneration.Name = "rightMedialPoleScaleDegeneration";
            this.rightMedialPoleScaleDegeneration.Size = new System.Drawing.Size(197, 44);
            this.rightMedialPoleScaleDegeneration.TabIndex = 15;
            this.rightMedialPoleScaleDegeneration.Tag = "rightMedialPoleScaleMandible";
            this.rightMedialPoleScaleDegeneration.Value = 0F;
            // 
            // rightMandibularNotchSlider
            // 
            this.rightMandibularNotchSlider.LabelText = "Right Mandibular Notch";
            this.rightMandibularNotchSlider.Location = new System.Drawing.Point(3, 703);
            this.rightMandibularNotchSlider.Name = "rightMandibularNotchSlider";
            this.rightMandibularNotchSlider.Size = new System.Drawing.Size(197, 44);
            this.rightMandibularNotchSlider.TabIndex = 10;
            this.rightMandibularNotchSlider.Tag = "rightMandibularNotchMandible";
            this.rightMandibularNotchSlider.Value = 0F;
            // 
            // rightAntegonialNotchSlider
            // 
            this.rightAntegonialNotchSlider.LabelText = "Right Antegonial Notch";
            this.rightAntegonialNotchSlider.Location = new System.Drawing.Point(3, 753);
            this.rightAntegonialNotchSlider.Name = "rightAntegonialNotchSlider";
            this.rightAntegonialNotchSlider.Size = new System.Drawing.Size(197, 44);
            this.rightAntegonialNotchSlider.TabIndex = 6;
            this.rightAntegonialNotchSlider.Tag = "rightAntegonialNotchMandible";
            this.rightAntegonialNotchSlider.Value = 0F;
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(5, 3);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(75, 23);
            this.resetButton.TabIndex = 2;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // MandibleSizeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ButtonImageIndex = 8;
            this.ButtonText = "Mandible Size";
            this.ClientSize = new System.Drawing.Size(259, 611);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.sliderPanel);
            this.DockAreas = ((Medical.DockAreas)(((Medical.DockAreas.Float | Medical.DockAreas.DockLeft)
                        | Medical.DockAreas.DockRight)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MandibleSizeControl";
            this.ShowHint = Medical.DockState.DockLeft;
            this.Text = "Mandible Size";
            this.ToolStripName = "Advanced";
            this.sliderPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel sliderPanel;
        private BoneManipulatorSlider leftRamusHeightSlider;
        private BoneManipulatorSlider leftCondyleHeightSlider;
        private BoneManipulatorSlider leftCondyleRotationSlider;
        private BoneManipulatorSlider leftCondyleDegenerationSlider;
        private BoneManipulatorSlider leftMandibularNotchSlider;
        private BoneManipulatorSlider leftAntegonialNotchSlider;
        private BoneManipulatorSlider rightRamusHeightSlider;
        private BoneManipulatorSlider rightCondyleHeightSlider;
        private BoneManipulatorSlider rightCondyleRotationSlider;
        private BoneManipulatorSlider rightCondyleDegenerationSlider;
        private BoneManipulatorSlider rightMandibularNotchSlider;
        private BoneManipulatorSlider rightAntegonialNotchSlider;
        private BoneManipulatorSlider leftLateralPoleSlider;
        private BoneManipulatorSlider leftMedialPoleScaleSlider;
        private BoneManipulatorSlider rightLateralPoleDegeneration;
        private BoneManipulatorSlider rightMedialPoleScaleDegeneration;
        private System.Windows.Forms.Button resetButton;


    }
}