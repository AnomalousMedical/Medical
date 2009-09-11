﻿namespace Medical.GUI
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
            this.nextButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.speedTrackBar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.previousButton = new System.Windows.Forms.Button();
            this.pauseButton = new System.Windows.Forms.Button();
            this.stateTrackBar = new Medical.GUI.TimeTrackBar();
            this.trackMarkMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.barMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.appendStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.speedTrackBar)).BeginInit();
            this.trackMarkMenu.SuspendLayout();
            this.barMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // nextButton
            // 
            this.nextButton.Image = global::Medical.Properties.Resources.Button_Last;
            this.nextButton.Location = new System.Drawing.Point(77, 32);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(32, 32);
            this.nextButton.TabIndex = 5;
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(114, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Slow";
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(242, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Fast";
            // 
            // previousButton
            // 
            this.previousButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.previousButton.Image = global::Medical.Properties.Resources.Button_First;
            this.previousButton.Location = new System.Drawing.Point(1, 32);
            this.previousButton.Name = "previousButton";
            this.previousButton.Size = new System.Drawing.Size(32, 32);
            this.previousButton.TabIndex = 4;
            this.previousButton.UseVisualStyleBackColor = true;
            this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
            // 
            // pauseButton
            // 
            this.pauseButton.Image = global::Medical.Properties.Resources.Button_Pause;
            this.pauseButton.Location = new System.Drawing.Point(39, 32);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(32, 32);
            this.pauseButton.TabIndex = 3;
            this.pauseButton.UseVisualStyleBackColor = true;
            this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
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
            this.appendStateToolStripMenuItem,
            this.insertStateToolStripMenuItem});
            this.barMenu.Name = "barMenu";
            this.barMenu.Size = new System.Drawing.Size(153, 70);
            // 
            // appendStateToolStripMenuItem
            // 
            this.appendStateToolStripMenuItem.Name = "appendStateToolStripMenuItem";
            this.appendStateToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.appendStateToolStripMenuItem.Text = "Append State";
            this.appendStateToolStripMenuItem.Click += new System.EventHandler(this.appendStateToolStripMenuItem_Click);
            // 
            // insertStateToolStripMenuItem
            // 
            this.insertStateToolStripMenuItem.Name = "insertStateToolStripMenuItem";
            this.insertStateToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.insertStateToolStripMenuItem.Text = "Insert State";
            this.insertStateToolStripMenuItem.Click += new System.EventHandler(this.insertStateToolStripMenuItem_Click);
            // 
            // MedicalStateGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 65);
            this.Controls.Add(this.stateTrackBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.speedTrackBar);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.previousButton);
            this.Controls.Add(this.pauseButton);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MedicalStateGUI";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottom;
            this.Text = "States";
            ((System.ComponentModel.ISupportInitialize)(this.speedTrackBar)).EndInit();
            this.trackMarkMenu.ResumeLayout(false);
            this.barMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button pauseButton;
        private System.Windows.Forms.Button previousButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar speedTrackBar;
        private System.Windows.Forms.Label label2;
        private TimeTrackBar stateTrackBar;
        private System.Windows.Forms.ContextMenuStrip trackMarkMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip barMenu;
        private System.Windows.Forms.ToolStripMenuItem appendStateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertStateToolStripMenuItem;
    }
}