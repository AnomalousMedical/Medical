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
            this.topCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.bottomCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.leftLateralCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.leftMidLateralCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.midlineAnteriorCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.rightMidLateralCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.rightLateralCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
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
            this.resetButton.Values.Text = "Make Normal";
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
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
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // rotateButton
            // 
            this.rotateButton.Location = new System.Drawing.Point(197, 4);
            this.rotateButton.Name = "rotateButton";
            this.rotateButton.Size = new System.Drawing.Size(90, 25);
            this.rotateButton.TabIndex = 9;
            this.rotateButton.Values.Text = "Manual Rotate";
            // 
            // topCameraButton
            // 
            this.topCameraButton.Location = new System.Drawing.Point(3, 66);
            this.topCameraButton.Name = "topCameraButton";
            this.topCameraButton.Size = new System.Drawing.Size(53, 53);
            this.topCameraButton.TabIndex = 12;
            this.topCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptTopTeeth;
            this.topCameraButton.Values.Text = "";
            this.topCameraButton.Click += new System.EventHandler(this.topCameraButton_Click);
            // 
            // bottomCameraButton
            // 
            this.bottomCameraButton.Location = new System.Drawing.Point(59, 66);
            this.bottomCameraButton.Name = "bottomCameraButton";
            this.bottomCameraButton.Size = new System.Drawing.Size(53, 53);
            this.bottomCameraButton.TabIndex = 13;
            this.bottomCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptBottomTeeth;
            this.bottomCameraButton.Values.Text = "";
            this.bottomCameraButton.Click += new System.EventHandler(this.bottomCameraButton_Click);
            // 
            // leftLateralCameraButton
            // 
            this.leftLateralCameraButton.Location = new System.Drawing.Point(227, 122);
            this.leftLateralCameraButton.Name = "leftLateralCameraButton";
            this.leftLateralCameraButton.Size = new System.Drawing.Size(53, 53);
            this.leftLateralCameraButton.TabIndex = 14;
            this.leftLateralCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptLeftLateral;
            this.leftLateralCameraButton.Values.Text = "";
            this.leftLateralCameraButton.Click += new System.EventHandler(this.leftLateralCameraButton_Click);
            // 
            // leftMidLateralCameraButton
            // 
            this.leftMidLateralCameraButton.Location = new System.Drawing.Point(171, 122);
            this.leftMidLateralCameraButton.Name = "leftMidLateralCameraButton";
            this.leftMidLateralCameraButton.Size = new System.Drawing.Size(53, 53);
            this.leftMidLateralCameraButton.TabIndex = 15;
            this.leftMidLateralCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptLeftMidAnterior;
            this.leftMidLateralCameraButton.Values.Text = "";
            this.leftMidLateralCameraButton.Click += new System.EventHandler(this.leftMidLateralCameraButton_Click);
            // 
            // midlineAnteriorCameraButton
            // 
            this.midlineAnteriorCameraButton.Location = new System.Drawing.Point(115, 122);
            this.midlineAnteriorCameraButton.Name = "midlineAnteriorCameraButton";
            this.midlineAnteriorCameraButton.Size = new System.Drawing.Size(53, 53);
            this.midlineAnteriorCameraButton.TabIndex = 16;
            this.midlineAnteriorCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptMidlineAnterior;
            this.midlineAnteriorCameraButton.Values.Text = "";
            this.midlineAnteriorCameraButton.Click += new System.EventHandler(this.midlineAnteriorCameraButton_Click);
            // 
            // rightMidLateralCameraButton
            // 
            this.rightMidLateralCameraButton.Location = new System.Drawing.Point(59, 122);
            this.rightMidLateralCameraButton.Name = "rightMidLateralCameraButton";
            this.rightMidLateralCameraButton.Size = new System.Drawing.Size(53, 53);
            this.rightMidLateralCameraButton.TabIndex = 17;
            this.rightMidLateralCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptRightMidAnterior;
            this.rightMidLateralCameraButton.Values.Text = "";
            this.rightMidLateralCameraButton.Click += new System.EventHandler(this.rightMidLateralCameraButton_Click);
            // 
            // rightLateralCameraButton
            // 
            this.rightLateralCameraButton.Location = new System.Drawing.Point(3, 122);
            this.rightLateralCameraButton.Name = "rightLateralCameraButton";
            this.rightLateralCameraButton.Size = new System.Drawing.Size(53, 53);
            this.rightLateralCameraButton.TabIndex = 18;
            this.rightLateralCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptRightLateral;
            this.rightLateralCameraButton.Values.Text = "";
            this.rightLateralCameraButton.Click += new System.EventHandler(this.rightLateralCameraButton_Click);
            // 
            // TeethAdaptationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.rightLateralCameraButton);
            this.Controls.Add(this.rightMidLateralCameraButton);
            this.Controls.Add(this.midlineAnteriorCameraButton);
            this.Controls.Add(this.leftMidLateralCameraButton);
            this.Controls.Add(this.leftLateralCameraButton);
            this.Controls.Add(this.bottomCameraButton);
            this.Controls.Add(this.topCameraButton);
            this.Controls.Add(this.adaptButton);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.moveButton);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.rotateButton);
            this.LayerState = "TeethLayers";
            this.Name = "TeethAdaptationPanel";
            this.NavigationState = "Teeth Midline Anterior";
            this.Size = new System.Drawing.Size(336, 408);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton adaptButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton resetButton;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton moveButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton undoButton;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton rotateButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton topCameraButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton bottomCameraButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton leftLateralCameraButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton leftMidLateralCameraButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton midlineAnteriorCameraButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton rightMidLateralCameraButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton rightLateralCameraButton;

    }
}
