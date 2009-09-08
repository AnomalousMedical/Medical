namespace Medical.GUI
{
    partial class MovementStateControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MovementStateControl));
            this.playButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.timeTrackBar = new Medical.GUI.TimeTrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.timeUpDown = new System.Windows.Forms.NumericUpDown();
            this.tickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.barMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addKeyStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lockButton = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.timeUpDown)).BeginInit();
            this.tickMenu.SuspendLayout();
            this.barMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // playButton
            // 
            this.playButton.Location = new System.Drawing.Point(12, 32);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(75, 23);
            this.playButton.TabIndex = 1;
            this.playButton.Text = "Play";
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(93, 32);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 2;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // timeTrackBar
            // 
            this.timeTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.timeTrackBar.BarMenu = null;
            this.timeTrackBar.CurrentTime = 0F;
            this.timeTrackBar.Location = new System.Drawing.Point(12, -19);
            this.timeTrackBar.MaximumTime = 5F;
            this.timeTrackBar.MoveMarks = false;
            this.timeTrackBar.Name = "timeTrackBar";
            this.timeTrackBar.SelectedMark = null;
            this.timeTrackBar.Size = new System.Drawing.Size(639, 47);
            this.timeTrackBar.TabIndex = 3;
            this.timeTrackBar.TickMenu = null;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(563, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Duration (s)";
            // 
            // timeUpDown
            // 
            this.timeUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.timeUpDown.DecimalPlaces = 2;
            this.timeUpDown.Location = new System.Drawing.Point(630, 32);
            this.timeUpDown.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.timeUpDown.Name = "timeUpDown";
            this.timeUpDown.Size = new System.Drawing.Size(68, 20);
            this.timeUpDown.TabIndex = 5;
            this.timeUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // tickMenu
            // 
            this.tickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.tickMenu.Name = "tickMenu";
            this.tickMenu.Size = new System.Drawing.Size(108, 26);
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
            this.addKeyStateToolStripMenuItem});
            this.barMenu.Name = "barMenu";
            this.barMenu.Size = new System.Drawing.Size(148, 26);
            // 
            // addKeyStateToolStripMenuItem
            // 
            this.addKeyStateToolStripMenuItem.Name = "addKeyStateToolStripMenuItem";
            this.addKeyStateToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.addKeyStateToolStripMenuItem.Text = "Add Key State";
            this.addKeyStateToolStripMenuItem.Click += new System.EventHandler(this.addKeyStateToolStripMenuItem_Click);
            // 
            // lockButton
            // 
            this.lockButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lockButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.lockButton.AutoSize = true;
            this.lockButton.Location = new System.Drawing.Point(657, 3);
            this.lockButton.Name = "lockButton";
            this.lockButton.Size = new System.Drawing.Size(41, 23);
            this.lockButton.TabIndex = 7;
            this.lockButton.Text = "Lock";
            this.lockButton.UseVisualStyleBackColor = true;
            this.lockButton.CheckedChanged += new System.EventHandler(this.lockButtonCheckChanged);
            // 
            // MovementStateControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 57);
            this.Controls.Add(this.timeUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.timeTrackBar);
            this.Controls.Add(this.playButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.lockButton);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MovementStateControl";
            this.Text = "Movement Sequence";
            ((System.ComponentModel.ISupportInitialize)(this.timeUpDown)).EndInit();
            this.tickMenu.ResumeLayout(false);
            this.barMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Button stopButton;
        private TimeTrackBar timeTrackBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown timeUpDown;
        private System.Windows.Forms.ContextMenuStrip tickMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip barMenu;
        private System.Windows.Forms.ToolStripMenuItem addKeyStateToolStripMenuItem;
        private System.Windows.Forms.CheckBox lockButton;
    }
}