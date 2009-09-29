namespace Medical.GUI
{
    partial class MuscleControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MuscleControl));
            this.muscleSequenceView = new Medical.GUI.MuscleSequenceView();
            this.playbackPanel = new System.Windows.Forms.Panel();
            this.stopButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.playbackTrackBar = new Medical.GUI.TimeTrackBar();
            this.playbackPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // muscleSequenceView
            // 
            this.muscleSequenceView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.muscleSequenceView.Location = new System.Drawing.Point(0, 0);
            this.muscleSequenceView.Name = "muscleSequenceView";
            this.muscleSequenceView.Size = new System.Drawing.Size(236, 514);
            this.muscleSequenceView.TabIndex = 13;
            // 
            // playbackPanel
            // 
            this.playbackPanel.Controls.Add(this.stopButton);
            this.playbackPanel.Controls.Add(this.playButton);
            this.playbackPanel.Controls.Add(this.playbackTrackBar);
            this.playbackPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.playbackPanel.Location = new System.Drawing.Point(0, 514);
            this.playbackPanel.Name = "playbackPanel";
            this.playbackPanel.Size = new System.Drawing.Size(236, 85);
            this.playbackPanel.TabIndex = 14;
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(86, 57);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 3;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // playButton
            // 
            this.playButton.Location = new System.Drawing.Point(4, 57);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(75, 23);
            this.playButton.TabIndex = 2;
            this.playButton.Text = "Play";
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // playbackTrackBar
            // 
            this.playbackTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.playbackTrackBar.BarMenu = null;
            this.playbackTrackBar.ChangeTimeOnSelection = false;
            this.playbackTrackBar.CurrentTime = 0F;
            this.playbackTrackBar.Location = new System.Drawing.Point(3, 3);
            this.playbackTrackBar.MaximumTime = 1F;
            this.playbackTrackBar.MoveMarks = false;
            this.playbackTrackBar.MoveThumb = false;
            this.playbackTrackBar.Name = "playbackTrackBar";
            this.playbackTrackBar.SelectedMark = null;
            this.playbackTrackBar.Size = new System.Drawing.Size(230, 47);
            this.playbackTrackBar.TabIndex = 1;
            this.playbackTrackBar.TickMenu = null;
            // 
            // MuscleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(236, 599);
            this.Controls.Add(this.muscleSequenceView);
            this.Controls.Add(this.playbackPanel);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MuscleControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Text = "Muscles";
            this.ToolStripName = "Advanced";
            this.playbackPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MuscleSequenceView muscleSequenceView;
        private System.Windows.Forms.Panel playbackPanel;
        private TimeTrackBar playbackTrackBar;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button playButton;

    }
}
