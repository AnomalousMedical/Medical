namespace Medical.GUI
{
    partial class DopplerPanel
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
            this.dopplerControl1 = new Medical.GUI.DopplerControl();
            this.kryptonWrapLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.lateralJointCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.superiorJointCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.bothJointsCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.SuspendLayout();
            // 
            // dopplerControl1
            // 
            this.dopplerControl1.Location = new System.Drawing.Point(0, 101);
            this.dopplerControl1.Name = "dopplerControl1";
            this.dopplerControl1.Size = new System.Drawing.Size(197, 450);
            this.dopplerControl1.TabIndex = 0;
            // 
            // kryptonWrapLabel1
            // 
            this.kryptonWrapLabel1.AutoSize = false;
            this.kryptonWrapLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonWrapLabel1.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(59)))), ((int)(((byte)(59)))));
            this.kryptonWrapLabel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonWrapLabel1.Name = "kryptonWrapLabel1";
            this.kryptonWrapLabel1.Size = new System.Drawing.Size(197, 18);
            this.kryptonWrapLabel1.Text = "Choose a camera angle.";
            // 
            // lateralJointCameraButton
            // 
            this.lateralJointCameraButton.Location = new System.Drawing.Point(7, 22);
            this.lateralJointCameraButton.Name = "lateralJointCameraButton";
            this.lateralJointCameraButton.Size = new System.Drawing.Size(53, 53);
            this.lateralJointCameraButton.TabIndex = 3;
            this.lateralJointCameraButton.Values.Image = global::Medical.Properties.Resources.LeftJointLateral;
            this.lateralJointCameraButton.Values.Text = "";
            this.lateralJointCameraButton.Click += new System.EventHandler(this.lateralJointCamerButton_Click);
            // 
            // superiorJointCameraButton
            // 
            this.superiorJointCameraButton.Location = new System.Drawing.Point(68, 22);
            this.superiorJointCameraButton.Name = "superiorJointCameraButton";
            this.superiorJointCameraButton.Size = new System.Drawing.Size(53, 53);
            this.superiorJointCameraButton.TabIndex = 4;
            this.superiorJointCameraButton.Values.Image = global::Medical.Properties.Resources.LeftJointSuperior;
            this.superiorJointCameraButton.Values.Text = "";
            this.superiorJointCameraButton.Click += new System.EventHandler(this.superiorJointCameraButton_Click);
            // 
            // bothJointsCameraButton
            // 
            this.bothJointsCameraButton.Location = new System.Drawing.Point(127, 22);
            this.bothJointsCameraButton.Name = "bothJointsCameraButton";
            this.bothJointsCameraButton.Size = new System.Drawing.Size(53, 53);
            this.bothJointsCameraButton.TabIndex = 6;
            this.bothJointsCameraButton.Values.Image = global::Medical.Properties.Resources.BothJointSuperior;
            this.bothJointsCameraButton.Values.Text = "";
            this.bothJointsCameraButton.Click += new System.EventHandler(this.bothJointsCameraButton_Click);
            // 
            // DopplerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bothJointsCameraButton);
            this.Controls.Add(this.superiorJointCameraButton);
            this.Controls.Add(this.lateralJointCameraButton);
            this.Controls.Add(this.kryptonWrapLabel1);
            this.Controls.Add(this.dopplerControl1);
            this.Name = "DopplerPanel";
            this.Size = new System.Drawing.Size(197, 556);
            this.ResumeLayout(false);

        }

        #endregion

        private DopplerControl dopplerControl1;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton lateralJointCameraButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton superiorJointCameraButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton bothJointsCameraButton;
    }
}
