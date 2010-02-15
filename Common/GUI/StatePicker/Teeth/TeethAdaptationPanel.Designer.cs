namespace Medical.GUI
{
    partial class TeethAdaptationPanel
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
            this.resetButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.undoButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.gridPropertiesControl1 = new Medical.GUI.Grid.GridPropertiesControl();
            this.teethMovementPanel1 = new Medical.GUI.StatePicker.Teeth.TeethMovementPanel();
            this.SuspendLayout();
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(100, 307);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(90, 25);
            this.resetButton.TabIndex = 11;
            this.resetButton.Values.Text = "Make Normal";
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // undoButton
            // 
            this.undoButton.Location = new System.Drawing.Point(4, 307);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(90, 25);
            this.undoButton.TabIndex = 10;
            this.undoButton.Values.Text = "Undo";
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // gridPropertiesControl1
            // 
            this.gridPropertiesControl1.GridSpacing = 2F;
            this.gridPropertiesControl1.GridVisible = true;
            this.gridPropertiesControl1.Location = new System.Drawing.Point(4, 258);
            this.gridPropertiesControl1.Name = "gridPropertiesControl1";
            this.gridPropertiesControl1.Size = new System.Drawing.Size(253, 44);
            this.gridPropertiesControl1.TabIndex = 19;
            // 
            // teethMovementPanel1
            // 
            this.teethMovementPanel1.Location = new System.Drawing.Point(0, 0);
            this.teethMovementPanel1.Name = "teethMovementPanel1";
            this.teethMovementPanel1.Size = new System.Drawing.Size(291, 259);
            this.teethMovementPanel1.TabIndex = 20;
            // 
            // TeethAdaptationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridPropertiesControl1);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.teethMovementPanel1);
            this.LargeIcon = global::Medical.Properties.Resources.AdaptationIcon;
            this.LayerState = "TeethLayers";
            this.Name = "TeethAdaptationPanel";
            this.NavigationState = "WizardTeethMidlineAnterior";
            this.Size = new System.Drawing.Size(291, 340);
            this.TextLine1 = "Teeth";
            this.TextLine2 = "Adaptation";
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton resetButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton undoButton;
        private Medical.GUI.Grid.GridPropertiesControl gridPropertiesControl1;
        private Medical.GUI.StatePicker.Teeth.TeethMovementPanel teethMovementPanel1;

    }
}
