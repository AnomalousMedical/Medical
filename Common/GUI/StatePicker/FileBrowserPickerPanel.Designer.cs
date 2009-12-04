namespace Medical.GUI
{
    partial class FileBrowserPickerPanel
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
            this.breadCrumbs = new ComponentFactory.Krypton.Toolkit.KryptonBreadCrumb();
            this.fileListBox = new ComponentFactory.Krypton.Toolkit.KryptonListBox();
            ((System.ComponentModel.ISupportInitialize)(this.breadCrumbs)).BeginInit();
            this.SuspendLayout();
            // 
            // breadCrumbs
            // 
            this.breadCrumbs.AutoSize = false;
            this.breadCrumbs.Dock = System.Windows.Forms.DockStyle.Top;
            this.breadCrumbs.Location = new System.Drawing.Point(0, 0);
            this.breadCrumbs.Name = "breadCrumbs";
            // 
            // 
            // 
            this.breadCrumbs.RootItem.ShortText = "Root";
            this.breadCrumbs.Size = new System.Drawing.Size(249, 28);
            this.breadCrumbs.TabIndex = 0;
            // 
            // fileListBox
            // 
            this.fileListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileListBox.Location = new System.Drawing.Point(0, 28);
            this.fileListBox.Name = "fileListBox";
            this.fileListBox.Size = new System.Drawing.Size(249, 420);
            this.fileListBox.TabIndex = 1;
            // 
            // FileBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fileListBox);
            this.Controls.Add(this.breadCrumbs);
            this.Name = "FileBrowser";
            this.Size = new System.Drawing.Size(249, 448);
            ((System.ComponentModel.ISupportInitialize)(this.breadCrumbs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonBreadCrumb breadCrumbs;
        private ComponentFactory.Krypton.Toolkit.KryptonListBox fileListBox;
    }
}
