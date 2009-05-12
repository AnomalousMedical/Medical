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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MedicalForm));
            this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.layersButton = new System.Windows.Forms.ToolStripButton();
            this.pictureButton = new System.Windows.Forms.ToolStripButton();
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
            this.toolStripContainer.ContentPanel.SuspendLayout();
            this.toolStripContainer.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer
            // 
            this.toolStripContainer.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer.ContentPanel
            // 
            this.toolStripContainer.ContentPanel.Controls.Add(this.drawingSplitHost);
            this.toolStripContainer.ContentPanel.Controls.Add(this.rightPanel);
            this.toolStripContainer.ContentPanel.Controls.Add(this.leftPanel);
            this.toolStripContainer.ContentPanel.Controls.Add(this.bottomPanel);
            this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(599, 358);
            this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer.LeftToolStripPanelVisible = false;
            this.toolStripContainer.Location = new System.Drawing.Point(0, 24);
            this.toolStripContainer.Name = "toolStripContainer";
            this.toolStripContainer.RightToolStripPanelVisible = false;
            this.toolStripContainer.Size = new System.Drawing.Size(599, 383);
            this.toolStripContainer.TabIndex = 0;
            this.toolStripContainer.Text = "toolStripContainer1";
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            this.toolStripContainer.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // rightPanel
            // 
            this.rightPanel.AutoScroll = true;
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightPanel.Location = new System.Drawing.Point(548, 0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(51, 325);
            this.rightPanel.TabIndex = 3;
            // 
            // leftPanel
            // 
            this.leftPanel.AutoScroll = true;
            this.leftPanel.AutoScrollMinSize = new System.Drawing.Size(10, 0);
            this.leftPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(51, 325);
            this.leftPanel.TabIndex = 2;
            // 
            // bottomPanel
            // 
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 325);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(599, 33);
            this.bottomPanel.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.layersButton,
            this.pictureButton});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(104, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // layersButton
            // 
            this.layersButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.layersButton.Image = ((System.Drawing.Image)(resources.GetObject("layersButton.Image")));
            this.layersButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.layersButton.Name = "layersButton";
            this.layersButton.Size = new System.Drawing.Size(44, 22);
            this.layersButton.Text = "Layers";
            this.layersButton.Click += new System.EventHandler(this.layersButton_Click);
            // 
            // pictureButton
            // 
            this.pictureButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.pictureButton.Image = ((System.Drawing.Image)(resources.GetObject("pictureButton.Image")));
            this.pictureButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pictureButton.Name = "pictureButton";
            this.pictureButton.Size = new System.Drawing.Size(48, 22);
            this.pictureButton.Text = "Picture";
            this.pictureButton.Click += new System.EventHandler(this.pictureButton_Click);
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
            this.newToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
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
            this.drawingSplitHost.Location = new System.Drawing.Point(51, 0);
            this.drawingSplitHost.Name = "drawingSplitHost";
            this.drawingSplitHost.Size = new System.Drawing.Size(497, 325);
            this.drawingSplitHost.TabIndex = 4;
            // 
            // MedicalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 407);
            this.Controls.Add(this.toolStripContainer);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MedicalForm";
            this.Text = "MainForm";
            this.toolStripContainer.ContentPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer.TopToolStripPanel.PerformLayout();
            this.toolStripContainer.ResumeLayout(false);
            this.toolStripContainer.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer;
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
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton layersButton;
        private System.Windows.Forms.ToolStripButton pictureButton;
        private Medical.GUI.View.DrawingSplitHost drawingSplitHost;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.Panel bottomPanel;
    }
}