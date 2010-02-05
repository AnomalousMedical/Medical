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
            this.kryptonWrapLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.kryptonWrapLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.kryptonWrapLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.gridPropertiesControl1 = new Medical.GUI.Grid.GridPropertiesControl();
            this.SuspendLayout();
            // 
            // adaptButton
            // 
            this.adaptButton.Location = new System.Drawing.Point(4, 194);
            this.adaptButton.Name = "adaptButton";
            this.adaptButton.Size = new System.Drawing.Size(90, 25);
            this.adaptButton.TabIndex = 7;
            this.adaptButton.Values.Text = "Automatic";
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(100, 360);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(90, 25);
            this.resetButton.TabIndex = 11;
            this.resetButton.Values.Text = "Make Normal";
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // moveButton
            // 
            this.moveButton.Location = new System.Drawing.Point(3, 279);
            this.moveButton.Name = "moveButton";
            this.moveButton.Size = new System.Drawing.Size(90, 25);
            this.moveButton.TabIndex = 8;
            this.moveButton.Values.Text = "Manual Move";
            // 
            // undoButton
            // 
            this.undoButton.Location = new System.Drawing.Point(4, 360);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(90, 25);
            this.undoButton.TabIndex = 10;
            this.undoButton.Values.Text = "Undo";
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // rotateButton
            // 
            this.rotateButton.Location = new System.Drawing.Point(100, 280);
            this.rotateButton.Name = "rotateButton";
            this.rotateButton.Size = new System.Drawing.Size(90, 25);
            this.rotateButton.TabIndex = 9;
            this.rotateButton.Values.Text = "Manual Rotate";
            // 
            // topCameraButton
            // 
            this.topCameraButton.Location = new System.Drawing.Point(6, 21);
            this.topCameraButton.Name = "topCameraButton";
            this.topCameraButton.Size = new System.Drawing.Size(53, 53);
            this.topCameraButton.TabIndex = 12;
            this.topCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptTopTeeth;
            this.topCameraButton.Values.Text = "";
            this.topCameraButton.Click += new System.EventHandler(this.topCameraButton_Click);
            // 
            // bottomCameraButton
            // 
            this.bottomCameraButton.Location = new System.Drawing.Point(62, 21);
            this.bottomCameraButton.Name = "bottomCameraButton";
            this.bottomCameraButton.Size = new System.Drawing.Size(53, 53);
            this.bottomCameraButton.TabIndex = 13;
            this.bottomCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptBottomTeeth;
            this.bottomCameraButton.Values.Text = "";
            this.bottomCameraButton.Click += new System.EventHandler(this.bottomCameraButton_Click);
            // 
            // leftLateralCameraButton
            // 
            this.leftLateralCameraButton.Location = new System.Drawing.Point(230, 77);
            this.leftLateralCameraButton.Name = "leftLateralCameraButton";
            this.leftLateralCameraButton.Size = new System.Drawing.Size(53, 53);
            this.leftLateralCameraButton.TabIndex = 14;
            this.leftLateralCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptLeftLateral;
            this.leftLateralCameraButton.Values.Text = "";
            this.leftLateralCameraButton.Click += new System.EventHandler(this.leftLateralCameraButton_Click);
            // 
            // leftMidLateralCameraButton
            // 
            this.leftMidLateralCameraButton.Location = new System.Drawing.Point(174, 77);
            this.leftMidLateralCameraButton.Name = "leftMidLateralCameraButton";
            this.leftMidLateralCameraButton.Size = new System.Drawing.Size(53, 53);
            this.leftMidLateralCameraButton.TabIndex = 15;
            this.leftMidLateralCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptLeftMidAnterior;
            this.leftMidLateralCameraButton.Values.Text = "";
            this.leftMidLateralCameraButton.Click += new System.EventHandler(this.leftMidLateralCameraButton_Click);
            // 
            // midlineAnteriorCameraButton
            // 
            this.midlineAnteriorCameraButton.Location = new System.Drawing.Point(118, 77);
            this.midlineAnteriorCameraButton.Name = "midlineAnteriorCameraButton";
            this.midlineAnteriorCameraButton.Size = new System.Drawing.Size(53, 53);
            this.midlineAnteriorCameraButton.TabIndex = 16;
            this.midlineAnteriorCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptMidlineAnterior;
            this.midlineAnteriorCameraButton.Values.Text = "";
            this.midlineAnteriorCameraButton.Click += new System.EventHandler(this.midlineAnteriorCameraButton_Click);
            // 
            // rightMidLateralCameraButton
            // 
            this.rightMidLateralCameraButton.Location = new System.Drawing.Point(62, 77);
            this.rightMidLateralCameraButton.Name = "rightMidLateralCameraButton";
            this.rightMidLateralCameraButton.Size = new System.Drawing.Size(53, 53);
            this.rightMidLateralCameraButton.TabIndex = 17;
            this.rightMidLateralCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptRightMidAnterior;
            this.rightMidLateralCameraButton.Values.Text = "";
            this.rightMidLateralCameraButton.Click += new System.EventHandler(this.rightMidLateralCameraButton_Click);
            // 
            // rightLateralCameraButton
            // 
            this.rightLateralCameraButton.Location = new System.Drawing.Point(6, 77);
            this.rightLateralCameraButton.Name = "rightLateralCameraButton";
            this.rightLateralCameraButton.Size = new System.Drawing.Size(53, 53);
            this.rightLateralCameraButton.TabIndex = 18;
            this.rightLateralCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptRightLateral;
            this.rightLateralCameraButton.Values.Text = "";
            this.rightLateralCameraButton.Click += new System.EventHandler(this.rightLateralCameraButton_Click);
            // 
            // kryptonWrapLabel1
            // 
            this.kryptonWrapLabel1.AutoSize = false;
            this.kryptonWrapLabel1.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.kryptonWrapLabel1.Location = new System.Drawing.Point(3, 0);
            this.kryptonWrapLabel1.Name = "kryptonWrapLabel1";
            this.kryptonWrapLabel1.Size = new System.Drawing.Size(135, 17);
            this.kryptonWrapLabel1.Text = "Choose a camera angle.";
            // 
            // kryptonWrapLabel2
            // 
            this.kryptonWrapLabel2.AutoSize = false;
            this.kryptonWrapLabel2.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.kryptonWrapLabel2.Location = new System.Drawing.Point(4, 144);
            this.kryptonWrapLabel2.Name = "kryptonWrapLabel2";
            this.kryptonWrapLabel2.Size = new System.Drawing.Size(279, 48);
            this.kryptonWrapLabel2.Text = "Click the button to automatically adapt the teeth to fit the mandible you have cr" +
                "eated. Click it again to stop the adaptation if it is suitable.";
            // 
            // kryptonWrapLabel3
            // 
            this.kryptonWrapLabel3.AutoSize = false;
            this.kryptonWrapLabel3.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.kryptonWrapLabel3.Location = new System.Drawing.Point(4, 229);
            this.kryptonWrapLabel3.Name = "kryptonWrapLabel3";
            this.kryptonWrapLabel3.Size = new System.Drawing.Size(279, 48);
            this.kryptonWrapLabel3.Text = "Manually move the teeth to fine tune their positions. Or manually move and rotate" +
                " the teeth to match your patient exactly.";
            // 
            // gridPropertiesControl1
            // 
            this.gridPropertiesControl1.GridSpacing = 2F;
            this.gridPropertiesControl1.GridVisible = true;
            this.gridPropertiesControl1.Location = new System.Drawing.Point(4, 311);
            this.gridPropertiesControl1.Name = "gridPropertiesControl1";
            this.gridPropertiesControl1.Size = new System.Drawing.Size(253, 44);
            this.gridPropertiesControl1.TabIndex = 19;
            // 
            // TeethAdaptationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridPropertiesControl1);
            this.Controls.Add(this.kryptonWrapLabel3);
            this.Controls.Add(this.kryptonWrapLabel2);
            this.Controls.Add(this.kryptonWrapLabel1);
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
            this.LargeIcon = global::Medical.Properties.Resources.AdaptationIcon;
            this.LayerState = "TeethLayers";
            this.Name = "TeethAdaptationPanel";
            this.NavigationState = "Teeth Midline Anterior";
            this.Size = new System.Drawing.Size(291, 390);
            this.TextLine1 = "Teeth";
            this.TextLine2 = "Adaptation";
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
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel3;
        private Medical.GUI.Grid.GridPropertiesControl gridPropertiesControl1;

    }
}
