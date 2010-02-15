namespace Medical.GUI
{
    partial class TeethHeightAdaptationPanel
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
            this.heightControl1 = new Medical.GUI.BoneManipulator.HeightControl();
            this.kryptonWrapLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.gridPropertiesControl1 = new Medical.GUI.Grid.GridPropertiesControl();
            this.resetButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.undoButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.teethMovementPanel1 = new Medical.GUI.StatePicker.Teeth.TeethMovementPanel();
            this.SuspendLayout();
            // 
            // heightControl1
            // 
            this.heightControl1.Location = new System.Drawing.Point(4, 298);
            this.heightControl1.Name = "heightControl1";
            this.heightControl1.Size = new System.Drawing.Size(205, 165);
            this.heightControl1.TabIndex = 23;
            // 
            // kryptonWrapLabel4
            // 
            this.kryptonWrapLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonWrapLabel4.AutoSize = false;
            this.kryptonWrapLabel4.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.kryptonWrapLabel4.Location = new System.Drawing.Point(4, 262);
            this.kryptonWrapLabel4.Name = "kryptonWrapLabel4";
            this.kryptonWrapLabel4.Size = new System.Drawing.Size(277, 33);
            this.kryptonWrapLabel4.Text = "Adjust the horizontal arch alignment using the following sliders.";
            // 
            // gridPropertiesControl1
            // 
            this.gridPropertiesControl1.GridSpacing = 2F;
            this.gridPropertiesControl1.GridVisible = true;
            this.gridPropertiesControl1.Location = new System.Drawing.Point(6, 449);
            this.gridPropertiesControl1.Name = "gridPropertiesControl1";
            this.gridPropertiesControl1.Size = new System.Drawing.Size(253, 44);
            this.gridPropertiesControl1.TabIndex = 26;
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(98, 499);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(90, 25);
            this.resetButton.TabIndex = 25;
            this.resetButton.Values.Text = "Make Normal";
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // undoButton
            // 
            this.undoButton.Location = new System.Drawing.Point(2, 499);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(90, 25);
            this.undoButton.TabIndex = 24;
            this.undoButton.Values.Text = "Undo";
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // teethMovementPanel1
            // 
            this.teethMovementPanel1.Location = new System.Drawing.Point(0, 0);
            this.teethMovementPanel1.Name = "teethMovementPanel1";
            this.teethMovementPanel1.Size = new System.Drawing.Size(291, 259);
            this.teethMovementPanel1.TabIndex = 27;
            // 
            // TeethHeightAdaptationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridPropertiesControl1);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.teethMovementPanel1);
            this.Controls.Add(this.kryptonWrapLabel4);
            this.Controls.Add(this.heightControl1);
            this.Name = "TeethHeightAdaptationPanel";
            this.Size = new System.Drawing.Size(291, 534);
            this.ResumeLayout(false);

        }

        #endregion

        private Medical.GUI.BoneManipulator.HeightControl heightControl1;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel4;
        private Medical.GUI.Grid.GridPropertiesControl gridPropertiesControl1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton resetButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton undoButton;
        private Medical.GUI.StatePicker.Teeth.TeethMovementPanel teethMovementPanel1;

    }
}
