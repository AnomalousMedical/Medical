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
            this.jointCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.midlineCameraButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.SuspendLayout();
            // 
            // dopplerControl1
            // 
            this.dopplerControl1.Location = new System.Drawing.Point(0, 101);
            this.dopplerControl1.Name = "dopplerControl1";
            this.dopplerControl1.Size = new System.Drawing.Size(197, 422);
            this.dopplerControl1.TabIndex = 0;
            // 
            // kryptonWrapLabel1
            // 
            this.kryptonWrapLabel1.AutoSize = false;
            this.kryptonWrapLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonWrapLabel1.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.kryptonWrapLabel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonWrapLabel1.Name = "kryptonWrapLabel1";
            this.kryptonWrapLabel1.Size = new System.Drawing.Size(197, 18);
            this.kryptonWrapLabel1.Text = "Choose a camera angle.";
            // 
            // jointCameraButton
            // 
            this.jointCameraButton.Location = new System.Drawing.Point(4, 22);
            this.jointCameraButton.Name = "jointCameraButton";
            this.jointCameraButton.Size = new System.Drawing.Size(90, 25);
            this.jointCameraButton.TabIndex = 3;
            this.jointCameraButton.Values.Text = "Joint";
            this.jointCameraButton.Click += new System.EventHandler(this.jointCameraButton_Click);
            // 
            // midlineCameraButton
            // 
            this.midlineCameraButton.Location = new System.Drawing.Point(4, 54);
            this.midlineCameraButton.Name = "midlineCameraButton";
            this.midlineCameraButton.Size = new System.Drawing.Size(90, 25);
            this.midlineCameraButton.TabIndex = 4;
            this.midlineCameraButton.Values.Text = "Midline";
            this.midlineCameraButton.Click += new System.EventHandler(this.midlineCameraButton_Click);
            // 
            // DopplerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.midlineCameraButton);
            this.Controls.Add(this.jointCameraButton);
            this.Controls.Add(this.kryptonWrapLabel1);
            this.Controls.Add(this.dopplerControl1);
            this.Name = "DopplerPanel";
            this.Size = new System.Drawing.Size(197, 526);
            this.ResumeLayout(false);

        }

        #endregion

        private DopplerControl dopplerControl1;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton jointCameraButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton midlineCameraButton;
    }
}
