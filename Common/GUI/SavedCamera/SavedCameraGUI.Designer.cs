namespace Medical.GUI
{
    partial class SavedCameraGUI
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SavedCameraGUI));
            this.saveCameraButton = new System.Windows.Forms.Button();
            this.deleteCameraButton = new System.Windows.Forms.Button();
            this.activateButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.cameraNameList = new System.Windows.Forms.ListView();
            this.Camera = new System.Windows.Forms.ColumnHeader();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // saveCameraButton
            // 
            this.saveCameraButton.Location = new System.Drawing.Point(84, 3);
            this.saveCameraButton.Name = "saveCameraButton";
            this.saveCameraButton.Size = new System.Drawing.Size(75, 23);
            this.saveCameraButton.TabIndex = 1;
            this.saveCameraButton.Text = "Save";
            this.saveCameraButton.UseVisualStyleBackColor = true;
            this.saveCameraButton.Click += new System.EventHandler(this.saveCameraButton_Click);
            // 
            // deleteCameraButton
            // 
            this.deleteCameraButton.Location = new System.Drawing.Point(165, 3);
            this.deleteCameraButton.Name = "deleteCameraButton";
            this.deleteCameraButton.Size = new System.Drawing.Size(75, 23);
            this.deleteCameraButton.TabIndex = 2;
            this.deleteCameraButton.Text = "Delete";
            this.deleteCameraButton.UseVisualStyleBackColor = true;
            this.deleteCameraButton.Click += new System.EventHandler(this.deleteCameraButton_Click);
            // 
            // activateButton
            // 
            this.activateButton.Location = new System.Drawing.Point(3, 3);
            this.activateButton.Name = "activateButton";
            this.activateButton.Size = new System.Drawing.Size(75, 23);
            this.activateButton.TabIndex = 3;
            this.activateButton.Text = "Activate";
            this.activateButton.UseVisualStyleBackColor = true;
            this.activateButton.Click += new System.EventHandler(this.activateButton_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.activateButton);
            this.flowLayoutPanel1.Controls.Add(this.saveCameraButton);
            this.flowLayoutPanel1.Controls.Add(this.deleteCameraButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 83);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(311, 29);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // cameraNameList
            // 
            this.cameraNameList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Camera});
            this.cameraNameList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cameraNameList.Location = new System.Drawing.Point(0, 0);
            this.cameraNameList.MultiSelect = false;
            this.cameraNameList.Name = "cameraNameList";
            this.cameraNameList.Size = new System.Drawing.Size(311, 83);
            this.cameraNameList.TabIndex = 5;
            this.cameraNameList.UseCompatibleStateImageBehavior = false;
            this.cameraNameList.View = System.Windows.Forms.View.Details;
            // 
            // Camera
            // 
            this.Camera.Text = "Camera";
            this.Camera.Width = 158;
            // 
            // SavedCameraGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(311, 112);
            this.Controls.Add(this.cameraNameList);
            this.Controls.Add(this.flowLayoutPanel1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SavedCameraGUI";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
            this.Text = "Cameras";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button saveCameraButton;
        private System.Windows.Forms.Button deleteCameraButton;
        private System.Windows.Forms.Button activateButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ListView cameraNameList;
        private System.Windows.Forms.ColumnHeader Camera;
    }
}