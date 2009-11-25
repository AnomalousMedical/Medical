namespace Medical.GUI
{
    partial class DockPanelPictureWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockPanelPictureWindow));
            this.pictureWindow = new Medical.GUI.PictureWindow();
            this.SuspendLayout();
            // 
            // pictureWindow
            // 
            this.pictureWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureWindow.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pictureWindow.Location = new System.Drawing.Point(0, 0);
            this.pictureWindow.Name = "pictureWindow";
            this.pictureWindow.Size = new System.Drawing.Size(435, 399);
            this.pictureWindow.TabIndex = 0;
            // 
            // DockPanelPictureWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 399);
            this.Controls.Add(this.pictureWindow);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DockPanelPictureWindow";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Float;
            this.ResumeLayout(false);

        }

        #endregion

        private PictureWindow pictureWindow;
    }
}
