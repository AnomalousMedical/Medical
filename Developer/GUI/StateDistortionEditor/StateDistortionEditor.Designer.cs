namespace Medical.GUI
{
    partial class StateDistortionEditor
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
            Medical.ImageRendererProperties imageRendererProperties1 = new Medical.ImageRendererProperties();
            this.saveButton = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.exportLeftCheck = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.exportRightCheck = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.exportTeethCheck = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.saveThumbnailCheck = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.exportGrowthDefect = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.exportRamusHeight = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.exportCondyleHeight = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.exportCondyleRotation = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.exportMandibularNotch = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.exportAntegonialNotch = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.exportMandibularDegenration = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.exportCondyleRoughness = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.exportMedialPoleDegeneration = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.exportLateralPoleDegeneration = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.exportCondyleDegeneration = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.exportFossa = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.picturePreviewPanel = new Medical.GUI.PicturePreviewPanel();
            this.exportDisc = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveButton.Location = new System.Drawing.Point(3, 642);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "dst";
            this.saveFileDialog.Filter = "Distortion Files|*.dst";
            // 
            // exportLeftCheck
            // 
            this.exportLeftCheck.Checked = true;
            this.exportLeftCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportLeftCheck.Location = new System.Drawing.Point(3, 232);
            this.exportLeftCheck.Name = "exportLeftCheck";
            this.exportLeftCheck.Size = new System.Drawing.Size(104, 19);
            this.exportLeftCheck.TabIndex = 2;
            this.exportLeftCheck.Values.Text = "Export Left Side";
            // 
            // exportRightCheck
            // 
            this.exportRightCheck.Checked = true;
            this.exportRightCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportRightCheck.Location = new System.Drawing.Point(3, 258);
            this.exportRightCheck.Name = "exportRightCheck";
            this.exportRightCheck.Size = new System.Drawing.Size(112, 19);
            this.exportRightCheck.TabIndex = 3;
            this.exportRightCheck.Values.Text = "Export Right Side";
            // 
            // exportTeethCheck
            // 
            this.exportTeethCheck.Checked = true;
            this.exportTeethCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportTeethCheck.Location = new System.Drawing.Point(3, 283);
            this.exportTeethCheck.Name = "exportTeethCheck";
            this.exportTeethCheck.Size = new System.Drawing.Size(88, 19);
            this.exportTeethCheck.TabIndex = 4;
            this.exportTeethCheck.Values.Text = "Export Teeth";
            // 
            // saveThumbnailCheck
            // 
            this.saveThumbnailCheck.Checked = true;
            this.saveThumbnailCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.saveThumbnailCheck.Location = new System.Drawing.Point(3, 207);
            this.saveThumbnailCheck.Name = "saveThumbnailCheck";
            this.saveThumbnailCheck.Size = new System.Drawing.Size(105, 19);
            this.saveThumbnailCheck.TabIndex = 5;
            this.saveThumbnailCheck.Values.Text = "Save Thumbnail";
            // 
            // exportGrowthDefect
            // 
            this.exportGrowthDefect.Checked = true;
            this.exportGrowthDefect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportGrowthDefect.Location = new System.Drawing.Point(3, 308);
            this.exportGrowthDefect.Name = "exportGrowthDefect";
            this.exportGrowthDefect.Size = new System.Drawing.Size(196, 19);
            this.exportGrowthDefect.TabIndex = 6;
            this.exportGrowthDefect.Values.Text = "Export Mandibular Growth Defect";
            // 
            // exportRamusHeight
            // 
            this.exportRamusHeight.Checked = true;
            this.exportRamusHeight.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportRamusHeight.Location = new System.Drawing.Point(25, 333);
            this.exportRamusHeight.Name = "exportRamusHeight";
            this.exportRamusHeight.Size = new System.Drawing.Size(96, 19);
            this.exportRamusHeight.TabIndex = 7;
            this.exportRamusHeight.Values.Text = "Ramus Height";
            // 
            // exportCondyleHeight
            // 
            this.exportCondyleHeight.Checked = true;
            this.exportCondyleHeight.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportCondyleHeight.Location = new System.Drawing.Point(25, 358);
            this.exportCondyleHeight.Name = "exportCondyleHeight";
            this.exportCondyleHeight.Size = new System.Drawing.Size(103, 19);
            this.exportCondyleHeight.TabIndex = 8;
            this.exportCondyleHeight.Values.Text = "Condyle Height";
            // 
            // exportCondyleRotation
            // 
            this.exportCondyleRotation.Checked = true;
            this.exportCondyleRotation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportCondyleRotation.Location = new System.Drawing.Point(25, 383);
            this.exportCondyleRotation.Name = "exportCondyleRotation";
            this.exportCondyleRotation.Size = new System.Drawing.Size(113, 19);
            this.exportCondyleRotation.TabIndex = 9;
            this.exportCondyleRotation.Values.Text = "Condyle Rotation";
            // 
            // exportMandibularNotch
            // 
            this.exportMandibularNotch.Checked = true;
            this.exportMandibularNotch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportMandibularNotch.Location = new System.Drawing.Point(25, 408);
            this.exportMandibularNotch.Name = "exportMandibularNotch";
            this.exportMandibularNotch.Size = new System.Drawing.Size(120, 19);
            this.exportMandibularNotch.TabIndex = 10;
            this.exportMandibularNotch.Values.Text = "Mandiblular Notch";
            // 
            // exportAntegonialNotch
            // 
            this.exportAntegonialNotch.Checked = true;
            this.exportAntegonialNotch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportAntegonialNotch.Location = new System.Drawing.Point(25, 433);
            this.exportAntegonialNotch.Name = "exportAntegonialNotch";
            this.exportAntegonialNotch.Size = new System.Drawing.Size(114, 19);
            this.exportAntegonialNotch.TabIndex = 11;
            this.exportAntegonialNotch.Values.Text = "Antegonial Notch";
            // 
            // exportMandibularDegenration
            // 
            this.exportMandibularDegenration.Checked = true;
            this.exportMandibularDegenration.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportMandibularDegenration.Location = new System.Drawing.Point(5, 458);
            this.exportMandibularDegenration.Name = "exportMandibularDegenration";
            this.exportMandibularDegenration.Size = new System.Drawing.Size(192, 19);
            this.exportMandibularDegenration.TabIndex = 12;
            this.exportMandibularDegenration.Values.Text = "Export Mandibular Degeneration";
            // 
            // exportCondyleRoughness
            // 
            this.exportCondyleRoughness.Checked = true;
            this.exportCondyleRoughness.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportCondyleRoughness.Location = new System.Drawing.Point(25, 558);
            this.exportCondyleRoughness.Name = "exportCondyleRoughness";
            this.exportCondyleRoughness.Size = new System.Drawing.Size(125, 19);
            this.exportCondyleRoughness.TabIndex = 16;
            this.exportCondyleRoughness.Values.Text = "Condyle Roughness";
            // 
            // exportMedialPoleDegeneration
            // 
            this.exportMedialPoleDegeneration.Checked = true;
            this.exportMedialPoleDegeneration.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportMedialPoleDegeneration.Location = new System.Drawing.Point(25, 533);
            this.exportMedialPoleDegeneration.Name = "exportMedialPoleDegeneration";
            this.exportMedialPoleDegeneration.Size = new System.Drawing.Size(157, 19);
            this.exportMedialPoleDegeneration.TabIndex = 15;
            this.exportMedialPoleDegeneration.Values.Text = "Medial Pole Degeneration";
            // 
            // exportLateralPoleDegeneration
            // 
            this.exportLateralPoleDegeneration.Checked = true;
            this.exportLateralPoleDegeneration.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportLateralPoleDegeneration.Location = new System.Drawing.Point(25, 508);
            this.exportLateralPoleDegeneration.Name = "exportLateralPoleDegeneration";
            this.exportLateralPoleDegeneration.Size = new System.Drawing.Size(156, 19);
            this.exportLateralPoleDegeneration.TabIndex = 14;
            this.exportLateralPoleDegeneration.Values.Text = "Lateral Pole Degeneration";
            // 
            // exportCondyleDegeneration
            // 
            this.exportCondyleDegeneration.Checked = true;
            this.exportCondyleDegeneration.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportCondyleDegeneration.Location = new System.Drawing.Point(25, 483);
            this.exportCondyleDegeneration.Name = "exportCondyleDegeneration";
            this.exportCondyleDegeneration.Size = new System.Drawing.Size(133, 19);
            this.exportCondyleDegeneration.TabIndex = 13;
            this.exportCondyleDegeneration.Values.Text = "Condyle Degneration";
            // 
            // exportFossa
            // 
            this.exportFossa.Checked = true;
            this.exportFossa.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportFossa.Location = new System.Drawing.Point(3, 583);
            this.exportFossa.Name = "exportFossa";
            this.exportFossa.Size = new System.Drawing.Size(88, 19);
            this.exportFossa.TabIndex = 17;
            this.exportFossa.Values.Text = "Export Fossa";
            // 
            // picturePreviewPanel
            // 
            imageRendererProperties1.AntiAliasingMode = 1;
            imageRendererProperties1.Height = 100;
            imageRendererProperties1.LayerState = null;
            imageRendererProperties1.MaxGridHeight = 2048;
            imageRendererProperties1.MaxGridWidth = 2048;
            imageRendererProperties1.NavigationStateName = null;
            imageRendererProperties1.OverrideLayers = false;
            imageRendererProperties1.ShowBackground = true;
            imageRendererProperties1.ShowWatermark = true;
            imageRendererProperties1.TransparentBackground = false;
            imageRendererProperties1.UseActiveViewportLocation = true;
            imageRendererProperties1.UseCustomPosition = false;
            imageRendererProperties1.UseNavigationStatePosition = false;
            imageRendererProperties1.UseWindowBackgroundColor = true;
            imageRendererProperties1.Width = 100;
            this.picturePreviewPanel.ImageProperties = imageRendererProperties1;
            this.picturePreviewPanel.Location = new System.Drawing.Point(3, 3);
            this.picturePreviewPanel.Name = "picturePreviewPanel";
            this.picturePreviewPanel.Size = new System.Drawing.Size(194, 198);
            this.picturePreviewPanel.TabIndex = 0;
            // 
            // exportDisc
            // 
            this.exportDisc.Checked = true;
            this.exportDisc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportDisc.Location = new System.Drawing.Point(3, 608);
            this.exportDisc.Name = "exportDisc";
            this.exportDisc.Size = new System.Drawing.Size(81, 19);
            this.exportDisc.TabIndex = 18;
            this.exportDisc.Values.Text = "Export Disc";
            // 
            // StateDistortionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ButtonText = "State Preset Editor";
            this.Controls.Add(this.exportDisc);
            this.Controls.Add(this.exportFossa);
            this.Controls.Add(this.exportCondyleRoughness);
            this.Controls.Add(this.exportMedialPoleDegeneration);
            this.Controls.Add(this.exportLateralPoleDegeneration);
            this.Controls.Add(this.exportCondyleDegeneration);
            this.Controls.Add(this.exportMandibularDegenration);
            this.Controls.Add(this.exportAntegonialNotch);
            this.Controls.Add(this.exportMandibularNotch);
            this.Controls.Add(this.exportCondyleRotation);
            this.Controls.Add(this.exportCondyleHeight);
            this.Controls.Add(this.exportRamusHeight);
            this.Controls.Add(this.exportGrowthDefect);
            this.Controls.Add(this.saveThumbnailCheck);
            this.Controls.Add(this.exportTeethCheck);
            this.Controls.Add(this.exportRightCheck);
            this.Controls.Add(this.exportLeftCheck);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.picturePreviewPanel);
            this.DockAreas = ((Medical.GUI.DockLocations)((Medical.GUI.DockLocations.Right | Medical.GUI.DockLocations.Float)));
            this.Name = "StateDistortionEditor";
            this.ShowHint = Medical.GUI.DockLocations.Right;
            this.Size = new System.Drawing.Size(200, 671);
            this.ToolStripName = "Editing";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PicturePreviewPanel picturePreviewPanel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox exportLeftCheck;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox exportRightCheck;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox exportTeethCheck;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox saveThumbnailCheck;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox exportGrowthDefect;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox exportRamusHeight;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox exportCondyleHeight;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox exportCondyleRotation;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox exportMandibularNotch;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox exportAntegonialNotch;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox exportMandibularDegenration;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox exportCondyleRoughness;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox exportMedialPoleDegeneration;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox exportLateralPoleDegeneration;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox exportCondyleDegeneration;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox exportFossa;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox exportDisc;
    }
}
