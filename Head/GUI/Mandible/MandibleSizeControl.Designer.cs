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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.leftDegenerated = new System.Windows.Forms.Button();
            this.rightDegenerated = new System.Windows.Forms.Button();
            this.leftGrowth = new System.Windows.Forms.Button();
            this.rightGrowth = new System.Windows.Forms.Button();
            this.leftNormal = new System.Windows.Forms.Button();
            this.rightNormal = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Right = new System.Windows.Forms.Label();
            this.advancedPage = new System.Windows.Forms.TabPage();
            this.sliderPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.leftAntegonialNotchSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftCondyleDegenerationSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftCondyleHeightSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftCondyleRotationSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftMandibularNotchSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftRamusHeightSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightAntegonailNotchSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightCondyleDegenerationSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightCondyleHeightSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightCondyleRotationSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightMandibularNotchSlider = new Medical.GUI.BoneManipulatorSlider();
            this.rightRamusHeightSlider = new Medical.GUI.BoneManipulatorSlider();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.advancedPage.SuspendLayout();
            this.sliderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.advancedPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(259, 534);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.leftDegenerated);
            this.tabPage1.Controls.Add(this.rightDegenerated);
            this.tabPage1.Controls.Add(this.leftGrowth);
            this.tabPage1.Controls.Add(this.rightGrowth);
            this.tabPage1.Controls.Add(this.leftNormal);
            this.tabPage1.Controls.Add(this.rightNormal);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.Right);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(251, 508);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Basic";
            // 
            // leftDegenerated
            // 
            this.leftDegenerated.Image = global::Medical.Properties.Resources.extremeLeftSide;
            this.leftDegenerated.Location = new System.Drawing.Point(132, 338);
            this.leftDegenerated.Name = "leftDegenerated";
            this.leftDegenerated.Size = new System.Drawing.Size(111, 159);
            this.leftDegenerated.TabIndex = 31;
            this.leftDegenerated.Text = "Growth Defect Degenerated";
            this.leftDegenerated.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.leftDegenerated.UseVisualStyleBackColor = true;
            // 
            // rightDegenerated
            // 
            this.rightDegenerated.Image = global::Medical.Properties.Resources.extremeRightSide;
            this.rightDegenerated.Location = new System.Drawing.Point(8, 338);
            this.rightDegenerated.Name = "rightDegenerated";
            this.rightDegenerated.Size = new System.Drawing.Size(111, 159);
            this.rightDegenerated.TabIndex = 30;
            this.rightDegenerated.Text = "Growth Defect Degenerated";
            this.rightDegenerated.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.rightDegenerated.UseVisualStyleBackColor = true;
            // 
            // leftGrowth
            // 
            this.leftGrowth.Image = global::Medical.Properties.Resources.middleLeftSide;
            this.leftGrowth.Location = new System.Drawing.Point(132, 200);
            this.leftGrowth.Name = "leftGrowth";
            this.leftGrowth.Size = new System.Drawing.Size(111, 132);
            this.leftGrowth.TabIndex = 29;
            this.leftGrowth.Text = "Growth Defect";
            this.leftGrowth.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.leftGrowth.UseVisualStyleBackColor = true;
            // 
            // rightGrowth
            // 
            this.rightGrowth.Image = global::Medical.Properties.Resources.middleRightSide;
            this.rightGrowth.Location = new System.Drawing.Point(8, 200);
            this.rightGrowth.Name = "rightGrowth";
            this.rightGrowth.Size = new System.Drawing.Size(111, 132);
            this.rightGrowth.TabIndex = 28;
            this.rightGrowth.Text = "Growth Defect";
            this.rightGrowth.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.rightGrowth.UseVisualStyleBackColor = true;
            // 
            // leftNormal
            // 
            this.leftNormal.Image = global::Medical.Properties.Resources.normalLeftSide;
            this.leftNormal.Location = new System.Drawing.Point(132, 24);
            this.leftNormal.Name = "leftNormal";
            this.leftNormal.Size = new System.Drawing.Size(111, 170);
            this.leftNormal.TabIndex = 27;
            this.leftNormal.Text = "Normal";
            this.leftNormal.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.leftNormal.UseVisualStyleBackColor = true;
            this.leftNormal.Click += new System.EventHandler(this.leftNormal_Click);
            // 
            // rightNormal
            // 
            this.rightNormal.Image = global::Medical.Properties.Resources.normalRightSide;
            this.rightNormal.Location = new System.Drawing.Point(8, 24);
            this.rightNormal.Name = "rightNormal";
            this.rightNormal.Size = new System.Drawing.Size(111, 170);
            this.rightNormal.TabIndex = 26;
            this.rightNormal.Text = "Normal";
            this.rightNormal.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.rightNormal.UseVisualStyleBackColor = true;
            this.rightNormal.Click += new System.EventHandler(this.rightNormal_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(174, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Left";
            // 
            // Right
            // 
            this.Right.AutoSize = true;
            this.Right.Location = new System.Drawing.Point(59, 8);
            this.Right.Name = "Right";
            this.Right.Size = new System.Drawing.Size(32, 13);
            this.Right.TabIndex = 24;
            this.Right.Text = "Right";
            // 
            // advancedPage
            // 
            this.advancedPage.AutoScroll = true;
            this.advancedPage.BackColor = System.Drawing.SystemColors.Control;
            this.advancedPage.Controls.Add(this.sliderPanel);
            this.advancedPage.Location = new System.Drawing.Point(4, 22);
            this.advancedPage.Name = "advancedPage";
            this.advancedPage.Padding = new System.Windows.Forms.Padding(3);
            this.advancedPage.Size = new System.Drawing.Size(251, 508);
            this.advancedPage.TabIndex = 1;
            this.advancedPage.Text = "Advanced";
            // 
            // sliderPanel
            // 
            this.sliderPanel.AutoSize = true;
            this.sliderPanel.Controls.Add(this.leftAntegonialNotchSlider);
            this.sliderPanel.Controls.Add(this.leftCondyleDegenerationSlider);
            this.sliderPanel.Controls.Add(this.leftCondyleHeightSlider);
            this.sliderPanel.Controls.Add(this.leftCondyleRotationSlider);
            this.sliderPanel.Controls.Add(this.leftMandibularNotchSlider);
            this.sliderPanel.Controls.Add(this.leftRamusHeightSlider);
            this.sliderPanel.Controls.Add(this.rightAntegonailNotchSlider);
            this.sliderPanel.Controls.Add(this.rightCondyleDegenerationSlider);
            this.sliderPanel.Controls.Add(this.rightCondyleHeightSlider);
            this.sliderPanel.Controls.Add(this.rightCondyleRotationSlider);
            this.sliderPanel.Controls.Add(this.rightMandibularNotchSlider);
            this.sliderPanel.Controls.Add(this.rightRamusHeightSlider);
            this.sliderPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.sliderPanel.Location = new System.Drawing.Point(9, 7);
            this.sliderPanel.Name = "sliderPanel";
            this.sliderPanel.Size = new System.Drawing.Size(211, 600);
            this.sliderPanel.TabIndex = 0;
            // 
            // leftAntegonialNotchSlider
            // 
            this.leftAntegonialNotchSlider.LabelText = "Left Antegonial Notch";
            this.leftAntegonialNotchSlider.Location = new System.Drawing.Point(3, 3);
            this.leftAntegonialNotchSlider.Name = "leftAntegonialNotchSlider";
            this.leftAntegonialNotchSlider.Size = new System.Drawing.Size(197, 44);
            this.leftAntegonialNotchSlider.TabIndex = 0;
            this.leftAntegonialNotchSlider.Tag = "leftAntegonialNotchMandible";
            // 
            // leftCondyleDegenerationSlider
            // 
            this.leftCondyleDegenerationSlider.LabelText = "Left Condyle Degenertaion";
            this.leftCondyleDegenerationSlider.Location = new System.Drawing.Point(3, 53);
            this.leftCondyleDegenerationSlider.Name = "leftCondyleDegenerationSlider";
            this.leftCondyleDegenerationSlider.Size = new System.Drawing.Size(197, 44);
            this.leftCondyleDegenerationSlider.TabIndex = 1;
            this.leftCondyleDegenerationSlider.Tag = "leftCondyleDegenerationMandible";
            // 
            // leftCondyleHeightSlider
            // 
            this.leftCondyleHeightSlider.LabelText = "Left Condyle Height";
            this.leftCondyleHeightSlider.Location = new System.Drawing.Point(3, 103);
            this.leftCondyleHeightSlider.Name = "leftCondyleHeightSlider";
            this.leftCondyleHeightSlider.Size = new System.Drawing.Size(197, 44);
            this.leftCondyleHeightSlider.TabIndex = 2;
            this.leftCondyleHeightSlider.Tag = "leftCondyleHeightMandible";
            // 
            // leftCondyleRotationSlider
            // 
            this.leftCondyleRotationSlider.LabelText = "Left Condyle Rotation";
            this.leftCondyleRotationSlider.Location = new System.Drawing.Point(3, 153);
            this.leftCondyleRotationSlider.Name = "leftCondyleRotationSlider";
            this.leftCondyleRotationSlider.Size = new System.Drawing.Size(197, 44);
            this.leftCondyleRotationSlider.TabIndex = 3;
            this.leftCondyleRotationSlider.Tag = "leftCondyleRotationMandible";
            // 
            // leftMandibularNotchSlider
            // 
            this.leftMandibularNotchSlider.LabelText = "Left Mandibular Notch";
            this.leftMandibularNotchSlider.Location = new System.Drawing.Point(3, 203);
            this.leftMandibularNotchSlider.Name = "leftMandibularNotchSlider";
            this.leftMandibularNotchSlider.Size = new System.Drawing.Size(197, 44);
            this.leftMandibularNotchSlider.TabIndex = 4;
            this.leftMandibularNotchSlider.Tag = "leftMandibularNotchMandible";
            // 
            // leftRamusHeightSlider
            // 
            this.leftRamusHeightSlider.LabelText = "Left Ramus Height";
            this.leftRamusHeightSlider.Location = new System.Drawing.Point(3, 253);
            this.leftRamusHeightSlider.Name = "leftRamusHeightSlider";
            this.leftRamusHeightSlider.Size = new System.Drawing.Size(197, 44);
            this.leftRamusHeightSlider.TabIndex = 5;
            this.leftRamusHeightSlider.Tag = "leftRamusHeightMandible";
            // 
            // rightAntegonailNotchSlider
            // 
            this.rightAntegonailNotchSlider.LabelText = "Right Antegonial Notch";
            this.rightAntegonailNotchSlider.Location = new System.Drawing.Point(3, 303);
            this.rightAntegonailNotchSlider.Name = "rightAntegonailNotchSlider";
            this.rightAntegonailNotchSlider.Size = new System.Drawing.Size(197, 44);
            this.rightAntegonailNotchSlider.TabIndex = 6;
            this.rightAntegonailNotchSlider.Tag = "rightAntegonailNotchMandible";
            // 
            // rightCondyleDegenerationSlider
            // 
            this.rightCondyleDegenerationSlider.LabelText = "Right Condyle Degenertaion";
            this.rightCondyleDegenerationSlider.Location = new System.Drawing.Point(3, 353);
            this.rightCondyleDegenerationSlider.Name = "rightCondyleDegenerationSlider";
            this.rightCondyleDegenerationSlider.Size = new System.Drawing.Size(197, 44);
            this.rightCondyleDegenerationSlider.TabIndex = 7;
            this.rightCondyleDegenerationSlider.Tag = "rightCondyleDegenerationMandible";
            // 
            // rightCondyleHeightSlider
            // 
            this.rightCondyleHeightSlider.LabelText = "Right Condyle Height";
            this.rightCondyleHeightSlider.Location = new System.Drawing.Point(3, 403);
            this.rightCondyleHeightSlider.Name = "rightCondyleHeightSlider";
            this.rightCondyleHeightSlider.Size = new System.Drawing.Size(197, 44);
            this.rightCondyleHeightSlider.TabIndex = 8;
            this.rightCondyleHeightSlider.Tag = "rightCondyleHeightMandible";
            // 
            // rightCondyleRotationSlider
            // 
            this.rightCondyleRotationSlider.LabelText = "Right Condyle Rotation";
            this.rightCondyleRotationSlider.Location = new System.Drawing.Point(3, 453);
            this.rightCondyleRotationSlider.Name = "rightCondyleRotationSlider";
            this.rightCondyleRotationSlider.Size = new System.Drawing.Size(197, 44);
            this.rightCondyleRotationSlider.TabIndex = 9;
            this.rightCondyleRotationSlider.Tag = "rightCondyleRotationMandible";
            // 
            // rightMandibularNotchSlider
            // 
            this.rightMandibularNotchSlider.LabelText = "Right Mandibular Notch";
            this.rightMandibularNotchSlider.Location = new System.Drawing.Point(3, 503);
            this.rightMandibularNotchSlider.Name = "rightMandibularNotchSlider";
            this.rightMandibularNotchSlider.Size = new System.Drawing.Size(197, 44);
            this.rightMandibularNotchSlider.TabIndex = 10;
            this.rightMandibularNotchSlider.Tag = "rightMandibularNotchMandible";
            // 
            // rightRamusHeightSlider
            // 
            this.rightRamusHeightSlider.LabelText = "Right Ramus Height";
            this.rightRamusHeightSlider.Location = new System.Drawing.Point(3, 553);
            this.rightRamusHeightSlider.Name = "rightRamusHeightSlider";
            this.rightRamusHeightSlider.Size = new System.Drawing.Size(197, 44);
            this.rightRamusHeightSlider.TabIndex = 11;
            this.rightRamusHeightSlider.Tag = "rightRamusHeightMandible";
            // 
            // MandibleSizeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(259, 534);
            this.Controls.Add(this.tabControl1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Name = "MandibleSizeControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Text = "Mandible Size";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.advancedPage.ResumeLayout(false);
            this.advancedPage.PerformLayout();
            this.sliderPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Right;
        private System.Windows.Forms.TabPage advancedPage;
        private System.Windows.Forms.Button rightNormal;
        private System.Windows.Forms.Button leftDegenerated;
        private System.Windows.Forms.Button rightDegenerated;
        private System.Windows.Forms.Button leftGrowth;
        private System.Windows.Forms.Button rightGrowth;
        private System.Windows.Forms.Button leftNormal;
        private System.Windows.Forms.FlowLayoutPanel sliderPanel;
        private BoneManipulatorSlider leftAntegonialNotchSlider;
        private BoneManipulatorSlider leftCondyleDegenerationSlider;
        private BoneManipulatorSlider leftCondyleHeightSlider;
        private BoneManipulatorSlider leftCondyleRotationSlider;
        private BoneManipulatorSlider leftMandibularNotchSlider;
        private BoneManipulatorSlider leftRamusHeightSlider;
        private BoneManipulatorSlider rightAntegonailNotchSlider;
        private BoneManipulatorSlider rightCondyleDegenerationSlider;
        private BoneManipulatorSlider rightCondyleHeightSlider;
        private BoneManipulatorSlider rightCondyleRotationSlider;
        private BoneManipulatorSlider rightMandibularNotchSlider;
        private BoneManipulatorSlider rightRamusHeightSlider;

    }
}