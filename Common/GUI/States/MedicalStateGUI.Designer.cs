namespace Medical.GUI
{
    partial class MedicalStateGUI
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MedicalStateGUI));
            this.stateImageList = new System.Windows.Forms.ImageList(this.components);
            this.speedTrackBar = new System.Windows.Forms.TrackBar();
            this.stateTrackBar = new Medical.GUI.TimeTrackBar();
            this.trackMarkMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.barMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.insertStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertStateAtStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appendStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slowPanel = new System.Windows.Forms.Panel();
            this.fastPanel = new System.Windows.Forms.Panel();
            this.previousButton = new Medical.GUI.Common.FancyButton();
            this.pauseButton = new Medical.GUI.Common.FancyButton();
            this.nextButton = new Medical.GUI.Common.FancyButton();
            ((System.ComponentModel.ISupportInitialize)(this.speedTrackBar)).BeginInit();
            this.trackMarkMenu.SuspendLayout();
            this.barMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // stateImageList
            // 
            this.stateImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.stateImageList.ImageSize = new System.Drawing.Size(32, 32);
            this.stateImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // speedTrackBar
            // 
            this.speedTrackBar.LargeChange = 2;
            this.speedTrackBar.Location = new System.Drawing.Point(143, 30);
            this.speedTrackBar.Maximum = 4;
            this.speedTrackBar.Name = "speedTrackBar";
            this.speedTrackBar.Size = new System.Drawing.Size(104, 45);
            this.speedTrackBar.TabIndex = 8;
            this.speedTrackBar.Value = 2;
            // 
            // stateTrackBar
            // 
            this.stateTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.stateTrackBar.BarMenu = null;
            this.stateTrackBar.ChangeTimeOnSelection = true;
            this.stateTrackBar.CurrentTime = 0F;
            this.stateTrackBar.Location = new System.Drawing.Point(1, -15);
            this.stateTrackBar.MaximumTime = 1F;
            this.stateTrackBar.MoveMarks = false;
            this.stateTrackBar.MoveThumb = true;
            this.stateTrackBar.Name = "stateTrackBar";
            this.stateTrackBar.SelectedMark = null;
            this.stateTrackBar.Size = new System.Drawing.Size(486, 47);
            this.stateTrackBar.TabIndex = 10;
            this.stateTrackBar.TickMenu = this.trackMarkMenu;
            // 
            // trackMarkMenu
            // 
            this.trackMarkMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.trackMarkMenu.Name = "trackMarkMenu";
            this.trackMarkMenu.Size = new System.Drawing.Size(108, 26);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // barMenu
            // 
            this.barMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertStateToolStripMenuItem,
            this.insertStateAtStartToolStripMenuItem,
            this.appendStateToolStripMenuItem});
            this.barMenu.Name = "barMenu";
            this.barMenu.Size = new System.Drawing.Size(186, 70);
            // 
            // insertStateToolStripMenuItem
            // 
            this.insertStateToolStripMenuItem.Name = "insertStateToolStripMenuItem";
            this.insertStateToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.insertStateToolStripMenuItem.Text = "Insert State Here";
            this.insertStateToolStripMenuItem.Click += new System.EventHandler(this.insertStateToolStripMenuItem_Click);
            // 
            // insertStateAtStartToolStripMenuItem
            // 
            this.insertStateAtStartToolStripMenuItem.Name = "insertStateAtStartToolStripMenuItem";
            this.insertStateAtStartToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.insertStateAtStartToolStripMenuItem.Text = "Insert State At Start";
            this.insertStateAtStartToolStripMenuItem.Click += new System.EventHandler(this.insertStateAtStartToolStripMenuItem_Click);
            // 
            // appendStateToolStripMenuItem
            // 
            this.appendStateToolStripMenuItem.Name = "appendStateToolStripMenuItem";
            this.appendStateToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.appendStateToolStripMenuItem.Text = "Append State To End";
            this.appendStateToolStripMenuItem.Click += new System.EventHandler(this.appendStateToolStripMenuItem_Click);
            // 
            // slowPanel
            // 
            this.slowPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("slowPanel.BackgroundImage")));
            this.slowPanel.Location = new System.Drawing.Point(115, 32);
            this.slowPanel.Name = "slowPanel";
            this.slowPanel.Size = new System.Drawing.Size(32, 32);
            this.slowPanel.TabIndex = 11;
            // 
            // fastPanel
            // 
            this.fastPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("fastPanel.BackgroundImage")));
            this.fastPanel.Location = new System.Drawing.Point(242, 32);
            this.fastPanel.Name = "fastPanel";
            this.fastPanel.Size = new System.Drawing.Size(32, 32);
            this.fastPanel.TabIndex = 12;
            // 
            // previousButton
            // 
            this.previousButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("previousButton.BackgroundImage")));
            this.previousButton.ClickIndex = 2;
            this.previousButton.HoverIndex = 1;
            this.previousButton.ImageHeight = 32;
            this.previousButton.ImageWidth = 32;
            this.previousButton.Location = new System.Drawing.Point(2, 30);
            this.previousButton.Name = "previousButton";
            this.previousButton.NormalIndex = 0;
            this.previousButton.Size = new System.Drawing.Size(33, 33);
            this.previousButton.TabIndex = 13;
            this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
            // 
            // pauseButton
            // 
            this.pauseButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pauseButton.BackgroundImage")));
            this.pauseButton.ClickIndex = 5;
            this.pauseButton.HoverIndex = 4;
            this.pauseButton.ImageHeight = 32;
            this.pauseButton.ImageWidth = 32;
            this.pauseButton.Location = new System.Drawing.Point(36, 30);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.NormalIndex = 3;
            this.pauseButton.Size = new System.Drawing.Size(33, 33);
            this.pauseButton.TabIndex = 14;
            this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("nextButton.BackgroundImage")));
            this.nextButton.ClickIndex = 8;
            this.nextButton.HoverIndex = 7;
            this.nextButton.ImageHeight = 32;
            this.nextButton.ImageWidth = 32;
            this.nextButton.Location = new System.Drawing.Point(71, 30);
            this.nextButton.Name = "nextButton";
            this.nextButton.NormalIndex = 6;
            this.nextButton.Size = new System.Drawing.Size(33, 33);
            this.nextButton.TabIndex = 15;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // MedicalStateGUI
            // 
            this.ButtonImageIndex = 2;
            this.ButtonText = "States";
            this.ClientSize = new System.Drawing.Size(491, 65);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.pauseButton);
            this.Controls.Add(this.fastPanel);
            this.Controls.Add(this.slowPanel);
            this.Controls.Add(this.previousButton);
            this.Controls.Add(this.stateTrackBar);
            this.Controls.Add(this.speedTrackBar);
            this.DockAreas = ((DockAreas)(((DockAreas.Float | DockAreas.DockTop)
                        | DockAreas.DockBottom)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MedicalStateGUI";
            this.ShortcutKey = System.Windows.Forms.Keys.S;
            this.ShowHint = DockState.DockBottom;
            this.Text = "States";
            ((System.ComponentModel.ISupportInitialize)(this.speedTrackBar)).EndInit();
            this.trackMarkMenu.ResumeLayout(false);
            this.barMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar speedTrackBar;
        private TimeTrackBar stateTrackBar;
        private System.Windows.Forms.ContextMenuStrip trackMarkMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip barMenu;
        private System.Windows.Forms.ToolStripMenuItem appendStateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertStateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertStateAtStartToolStripMenuItem;
        private System.Windows.Forms.ImageList stateImageList;
        private System.Windows.Forms.Panel slowPanel;
        private System.Windows.Forms.Panel fastPanel;
        private Medical.GUI.Common.FancyButton previousButton;
        private Medical.GUI.Common.FancyButton pauseButton;
        private Medical.GUI.Common.FancyButton nextButton;
    }
}