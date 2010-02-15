namespace Medical.GUI.StatePicker.Teeth
{
    partial class TeethMovementPanel
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
            this.kryptonWrapLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.kryptonWrapLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.kryptonWrapLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.rightLateralCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.midlineAnteriorCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.leftLateralCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.bottomCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.topCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.adaptButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.moveButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.rotateButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.SuspendLayout();
            // 
            // kryptonWrapLabel3
            // 
            this.kryptonWrapLabel3.AutoSize = false;
            this.kryptonWrapLabel3.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.kryptonWrapLabel3.Location = new System.Drawing.Point(3, 178);
            this.kryptonWrapLabel3.Name = "kryptonWrapLabel3";
            this.kryptonWrapLabel3.Size = new System.Drawing.Size(279, 48);
            this.kryptonWrapLabel3.Text = "Manually move the teeth to fine tune their positions. Or manually move and rotate" +
                " the teeth to match your patient exactly.";
            // 
            // kryptonWrapLabel2
            // 
            this.kryptonWrapLabel2.AutoSize = false;
            this.kryptonWrapLabel2.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.kryptonWrapLabel2.Location = new System.Drawing.Point(3, 90);
            this.kryptonWrapLabel2.Name = "kryptonWrapLabel2";
            this.kryptonWrapLabel2.Size = new System.Drawing.Size(279, 48);
            this.kryptonWrapLabel2.Text = "Click the Automatic button to automatically adapt the teeth to fit the mandible y" +
                "ou have created. Click it again to stop the adaptation if it is suitable.";
            // 
            // kryptonWrapLabel1
            // 
            this.kryptonWrapLabel1.AutoSize = false;
            this.kryptonWrapLabel1.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.kryptonWrapLabel1.Location = new System.Drawing.Point(2, 4);
            this.kryptonWrapLabel1.Name = "kryptonWrapLabel1";
            this.kryptonWrapLabel1.Size = new System.Drawing.Size(288, 17);
            this.kryptonWrapLabel1.Text = "Choose a camera angle.  Change when required.";
            // 
            // rightLateralCameraButton
            // 
            this.rightLateralCameraButton.Location = new System.Drawing.Point(118, 28);
            this.rightLateralCameraButton.Name = "rightLateralCameraButton";
            this.rightLateralCameraButton.Size = new System.Drawing.Size(53, 53);
            this.rightLateralCameraButton.TabIndex = 29;
            this.rightLateralCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptRightLateral;
            this.rightLateralCameraButton.Values.Text = "";
            this.rightLateralCameraButton.Click += new System.EventHandler(this.rightLateralCameraButton_Click);
            // 
            // midlineAnteriorCameraButton
            // 
            this.midlineAnteriorCameraButton.Location = new System.Drawing.Point(177, 28);
            this.midlineAnteriorCameraButton.Name = "midlineAnteriorCameraButton";
            this.midlineAnteriorCameraButton.Size = new System.Drawing.Size(53, 53);
            this.midlineAnteriorCameraButton.TabIndex = 28;
            this.midlineAnteriorCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptMidlineAnterior;
            this.midlineAnteriorCameraButton.Values.Text = "";
            this.midlineAnteriorCameraButton.Click += new System.EventHandler(this.midlineAnteriorCameraButton_Click);
            // 
            // leftLateralCameraButton
            // 
            this.leftLateralCameraButton.Location = new System.Drawing.Point(236, 28);
            this.leftLateralCameraButton.Name = "leftLateralCameraButton";
            this.leftLateralCameraButton.Size = new System.Drawing.Size(53, 53);
            this.leftLateralCameraButton.TabIndex = 27;
            this.leftLateralCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptLeftLateral;
            this.leftLateralCameraButton.Values.Text = "";
            this.leftLateralCameraButton.Click += new System.EventHandler(this.leftLateralCameraButton_Click);
            // 
            // bottomCameraButton
            // 
            this.bottomCameraButton.Location = new System.Drawing.Point(59, 28);
            this.bottomCameraButton.Name = "bottomCameraButton";
            this.bottomCameraButton.Size = new System.Drawing.Size(53, 53);
            this.bottomCameraButton.TabIndex = 26;
            this.bottomCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptBottomTeeth;
            this.bottomCameraButton.Values.Text = "";
            this.bottomCameraButton.Click += new System.EventHandler(this.bottomCameraButton_Click);
            // 
            // topCameraButton
            // 
            this.topCameraButton.Location = new System.Drawing.Point(0, 28);
            this.topCameraButton.Name = "topCameraButton";
            this.topCameraButton.Size = new System.Drawing.Size(53, 53);
            this.topCameraButton.TabIndex = 25;
            this.topCameraButton.Values.Image = global::Medical.Properties.Resources.AdaptTopTeeth;
            this.topCameraButton.Values.Text = "";
            this.topCameraButton.Click += new System.EventHandler(this.topCameraButton_Click);
            // 
            // adaptButton
            // 
            this.adaptButton.Location = new System.Drawing.Point(3, 145);
            this.adaptButton.Name = "adaptButton";
            this.adaptButton.Size = new System.Drawing.Size(90, 25);
            this.adaptButton.TabIndex = 22;
            this.adaptButton.Values.Text = "Automatic";
            // 
            // moveButton
            // 
            this.moveButton.Location = new System.Drawing.Point(2, 230);
            this.moveButton.Name = "moveButton";
            this.moveButton.Size = new System.Drawing.Size(112, 25);
            this.moveButton.TabIndex = 23;
            this.moveButton.Values.Text = "Linear Movements";
            // 
            // rotateButton
            // 
            this.rotateButton.Location = new System.Drawing.Point(123, 230);
            this.rotateButton.Name = "rotateButton";
            this.rotateButton.Size = new System.Drawing.Size(127, 25);
            this.rotateButton.TabIndex = 24;
            this.rotateButton.Values.Text = "Rotational Movements";
            // 
            // TeethMovementPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonWrapLabel3);
            this.Controls.Add(this.kryptonWrapLabel2);
            this.Controls.Add(this.kryptonWrapLabel1);
            this.Controls.Add(this.rightLateralCameraButton);
            this.Controls.Add(this.midlineAnteriorCameraButton);
            this.Controls.Add(this.leftLateralCameraButton);
            this.Controls.Add(this.bottomCameraButton);
            this.Controls.Add(this.topCameraButton);
            this.Controls.Add(this.adaptButton);
            this.Controls.Add(this.moveButton);
            this.Controls.Add(this.rotateButton);
            this.Name = "TeethMovementPanel";
            this.Size = new System.Drawing.Size(291, 259);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton rightLateralCameraButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton midlineAnteriorCameraButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton leftLateralCameraButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton bottomCameraButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton topCameraButton;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton adaptButton;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton moveButton;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton rotateButton;
    }
}
