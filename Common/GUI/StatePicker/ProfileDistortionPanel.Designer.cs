namespace Medical.GUI
{
    partial class ProfileDistortionPanel
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
            this.kryptonWrapLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.rightSideCamera = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.rightMidCamera = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.midlineCamera = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonWrapLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.leftMidCamera = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.leftSideCamera = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonWrapLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.adaptButton = new ComponentFactory.Krypton.Toolkit.KryptonCheckButton();
            this.gridPropertiesControl1 = new Medical.GUI.Grid.GridPropertiesControl();
            this.heightControl1 = new Medical.GUI.BoneManipulator.HeightControl();
            this.SuspendLayout();
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
            // rightSideCamera
            // 
            this.rightSideCamera.Location = new System.Drawing.Point(6, 20);
            this.rightSideCamera.Name = "rightSideCamera";
            this.rightSideCamera.Size = new System.Drawing.Size(53, 53);
            this.rightSideCamera.TabIndex = 22;
            this.rightSideCamera.Values.Image = global::Medical.Properties.Resources.ProfileRightLateral;
            this.rightSideCamera.Values.Text = "";
            this.rightSideCamera.Click += new System.EventHandler(this.rightSideCamera_Click);
            // 
            // rightMidCamera
            // 
            this.rightMidCamera.Location = new System.Drawing.Point(62, 20);
            this.rightMidCamera.Name = "rightMidCamera";
            this.rightMidCamera.Size = new System.Drawing.Size(53, 53);
            this.rightMidCamera.TabIndex = 21;
            this.rightMidCamera.Values.Image = global::Medical.Properties.Resources.ProfileRightMidLateral;
            this.rightMidCamera.Values.Text = "";
            this.rightMidCamera.Click += new System.EventHandler(this.rightMidCamera_Click);
            // 
            // midlineCamera
            // 
            this.midlineCamera.Location = new System.Drawing.Point(118, 20);
            this.midlineCamera.Name = "midlineCamera";
            this.midlineCamera.Size = new System.Drawing.Size(53, 53);
            this.midlineCamera.TabIndex = 20;
            this.midlineCamera.Values.Image = global::Medical.Properties.Resources.ProfileMidline;
            this.midlineCamera.Values.Text = "";
            this.midlineCamera.Click += new System.EventHandler(this.midlineCamera_Click);
            // 
            // kryptonWrapLabel2
            // 
            this.kryptonWrapLabel2.AutoSize = false;
            this.kryptonWrapLabel2.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.kryptonWrapLabel2.Location = new System.Drawing.Point(3, 76);
            this.kryptonWrapLabel2.Name = "kryptonWrapLabel2";
            this.kryptonWrapLabel2.Size = new System.Drawing.Size(280, 17);
            this.kryptonWrapLabel2.Text = "Subtract height from the profile moving the chin backward";
            // 
            // leftMidCamera
            // 
            this.leftMidCamera.Location = new System.Drawing.Point(174, 20);
            this.leftMidCamera.Name = "leftMidCamera";
            this.leftMidCamera.Size = new System.Drawing.Size(53, 53);
            this.leftMidCamera.TabIndex = 26;
            this.leftMidCamera.Values.Image = global::Medical.Properties.Resources.ProfileLeftMidLateral;
            this.leftMidCamera.Values.Text = "";
            this.leftMidCamera.Click += new System.EventHandler(this.leftMidCamera_Click);
            // 
            // leftSideCamera
            // 
            this.leftSideCamera.Location = new System.Drawing.Point(230, 20);
            this.leftSideCamera.Name = "leftSideCamera";
            this.leftSideCamera.Size = new System.Drawing.Size(53, 53);
            this.leftSideCamera.TabIndex = 27;
            this.leftSideCamera.Values.Image = global::Medical.Properties.Resources.ProfileLeftLateral;
            this.leftSideCamera.Values.Text = "";
            this.leftSideCamera.Click += new System.EventHandler(this.leftSideCamera_Click);
            // 
            // kryptonWrapLabel3
            // 
            this.kryptonWrapLabel3.AutoSize = false;
            this.kryptonWrapLabel3.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.kryptonWrapLabel3.Location = new System.Drawing.Point(6, 255);
            this.kryptonWrapLabel3.Name = "kryptonWrapLabel3";
            this.kryptonWrapLabel3.Size = new System.Drawing.Size(279, 48);
            this.kryptonWrapLabel3.Text = "Click the button to automatically adapt the teeth to fit the profile you have cre" +
                "ated. Click it again to stop the adaptation if it is suitable.";
            // 
            // adaptButton
            // 
            this.adaptButton.Location = new System.Drawing.Point(6, 305);
            this.adaptButton.Name = "adaptButton";
            this.adaptButton.Size = new System.Drawing.Size(90, 25);
            this.adaptButton.TabIndex = 31;
            this.adaptButton.Values.Text = "Adapt Teeth";
            // 
            // gridPropertiesControl1
            // 
            this.gridPropertiesControl1.GridSpacing = 5F;
            this.gridPropertiesControl1.GridVisible = true;
            this.gridPropertiesControl1.Location = new System.Drawing.Point(6, 336);
            this.gridPropertiesControl1.Name = "gridPropertiesControl1";
            this.gridPropertiesControl1.Size = new System.Drawing.Size(253, 48);
            this.gridPropertiesControl1.TabIndex = 34;
            // 
            // heightControl1
            // 
            this.heightControl1.Location = new System.Drawing.Point(6, 97);
            this.heightControl1.Name = "heightControl1";
            this.heightControl1.Size = new System.Drawing.Size(279, 155);
            this.heightControl1.TabIndex = 38;
            // 
            // ProfileDistortionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.heightControl1);
            this.Controls.Add(this.gridPropertiesControl1);
            this.Controls.Add(this.kryptonWrapLabel3);
            this.Controls.Add(this.adaptButton);
            this.Controls.Add(this.leftSideCamera);
            this.Controls.Add(this.leftMidCamera);
            this.Controls.Add(this.kryptonWrapLabel2);
            this.Controls.Add(this.kryptonWrapLabel1);
            this.Controls.Add(this.rightSideCamera);
            this.Controls.Add(this.rightMidCamera);
            this.Controls.Add(this.midlineCamera);
            this.Name = "ProfileDistortionPanel";
            this.Size = new System.Drawing.Size(293, 398);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton rightSideCamera;
        private ComponentFactory.Krypton.Toolkit.KryptonButton rightMidCamera;
        private ComponentFactory.Krypton.Toolkit.KryptonButton midlineCamera;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonButton leftMidCamera;
        private ComponentFactory.Krypton.Toolkit.KryptonButton leftSideCamera;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckButton adaptButton;
        private Medical.GUI.Grid.GridPropertiesControl gridPropertiesControl1;
        private Medical.GUI.BoneManipulator.HeightControl heightControl1;
    }
}
