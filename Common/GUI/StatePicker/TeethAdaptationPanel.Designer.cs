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
            this.adaptButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.resetButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.moveButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.undoButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.rotateButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.SuspendLayout();
            // 
            // adaptButton
            // 
            this.adaptButton.Location = new System.Drawing.Point(3, 3);
            this.adaptButton.Name = "adaptButton";
            this.adaptButton.Size = new System.Drawing.Size(90, 25);
            this.adaptButton.TabIndex = 7;
            this.adaptButton.Values.Text = "Automatic";
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(100, 35);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(90, 25);
            this.resetButton.TabIndex = 11;
            this.resetButton.Values.Text = "Reset";
            // 
            // moveButton
            // 
            this.moveButton.Location = new System.Drawing.Point(100, 3);
            this.moveButton.Name = "moveButton";
            this.moveButton.Size = new System.Drawing.Size(90, 25);
            this.moveButton.TabIndex = 8;
            this.moveButton.Values.Text = "Manual Move";
            // 
            // undoButton
            // 
            this.undoButton.Location = new System.Drawing.Point(4, 35);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(90, 25);
            this.undoButton.TabIndex = 10;
            this.undoButton.Values.Text = "Undo";
            // 
            // rotateButton
            // 
            this.rotateButton.Location = new System.Drawing.Point(197, 4);
            this.rotateButton.Name = "rotateButton";
            this.rotateButton.Size = new System.Drawing.Size(90, 25);
            this.rotateButton.TabIndex = 9;
            this.rotateButton.Values.Text = "Manual Rotate";
            // 
            // TeethAdaptationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.adaptButton);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.moveButton);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.rotateButton);
            this.LayerState = "TeethLayers";
            this.Name = "TeethAdaptationPanel";
            this.NavigationState = "Teeth Midline Anterior";
            this.Size = new System.Drawing.Size(336, 474);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton adaptButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton resetButton;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton moveButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton undoButton;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton rotateButton;

    }
}
