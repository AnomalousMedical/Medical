namespace Medical.GUI
{
    partial class MedicalForm
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
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oneWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.twoWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.threeWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fourWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawingSplitHost = new Medical.GUI.View.DrawingSplitHost();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.drawingSplitHost);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(599, 358);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 24);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(599, 383);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.displayToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(599, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // displayToolStripMenuItem
            // 
            this.displayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.layoutToolStripMenuItem});
            this.displayToolStripMenuItem.Name = "displayToolStripMenuItem";
            this.displayToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.displayToolStripMenuItem.Text = "Display";
            // 
            // layoutToolStripMenuItem
            // 
            this.layoutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.oneWindowToolStripMenuItem,
            this.twoWindowsToolStripMenuItem,
            this.threeWindowsToolStripMenuItem,
            this.fourWindowsToolStripMenuItem});
            this.layoutToolStripMenuItem.Name = "layoutToolStripMenuItem";
            this.layoutToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.layoutToolStripMenuItem.Text = "Layout";
            // 
            // oneWindowToolStripMenuItem
            // 
            this.oneWindowToolStripMenuItem.Name = "oneWindowToolStripMenuItem";
            this.oneWindowToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.oneWindowToolStripMenuItem.Text = "One Window";
            this.oneWindowToolStripMenuItem.Click += new System.EventHandler(this.oneWindowToolStripMenuItem_Click);
            // 
            // twoWindowsToolStripMenuItem
            // 
            this.twoWindowsToolStripMenuItem.Name = "twoWindowsToolStripMenuItem";
            this.twoWindowsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.twoWindowsToolStripMenuItem.Text = "Two Windows";
            this.twoWindowsToolStripMenuItem.Click += new System.EventHandler(this.twoWindowsToolStripMenuItem_Click);
            // 
            // threeWindowsToolStripMenuItem
            // 
            this.threeWindowsToolStripMenuItem.Name = "threeWindowsToolStripMenuItem";
            this.threeWindowsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.threeWindowsToolStripMenuItem.Text = "Three Windows";
            this.threeWindowsToolStripMenuItem.Click += new System.EventHandler(this.threeWindowsToolStripMenuItem_Click);
            // 
            // fourWindowsToolStripMenuItem
            // 
            this.fourWindowsToolStripMenuItem.Name = "fourWindowsToolStripMenuItem";
            this.fourWindowsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.fourWindowsToolStripMenuItem.Text = "Four Windows";
            this.fourWindowsToolStripMenuItem.Click += new System.EventHandler(this.fourWindowsToolStripMenuItem_Click);
            // 
            // drawingSplitHost
            // 
            this.drawingSplitHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.drawingSplitHost.Location = new System.Drawing.Point(0, 0);
            this.drawingSplitHost.Name = "drawingSplitHost";
            this.drawingSplitHost.Size = new System.Drawing.Size(599, 358);
            this.drawingSplitHost.TabIndex = 0;
            // 
            // MedicalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 407);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MedicalForm";
            this.Text = "MainForm";
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private Medical.GUI.View.DrawingSplitHost drawingSplitHost;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oneWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem twoWindowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem threeWindowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fourWindowsToolStripMenuItem;
    }
}