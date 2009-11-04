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
            this.highlightTeeth = new System.Windows.Forms.CheckBox();
            this.stopButton = new Medical.GUI.Common.FancyButton();
            this.playButton = new Medical.GUI.Common.FancyButton();
            this.nowPlayingLabel = new System.Windows.Forms.Label();
            this.playbackTrackBar = new Medical.GUI.TimeTrackBar();
            this.playbackPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // muscleSequenceView
            // 
            this.muscleSequenceView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.muscleSequenceView.Location = new System.Drawing.Point(0, 0);
            this.muscleSequenceView.Name = "muscleSequenceView";
            this.muscleSequenceView.Size = new System.Drawing.Size(236, 492);
            this.muscleSequenceView.TabIndex = 13;
            // 
            // playbackPanel
            // 
            this.playbackPanel.Controls.Add(this.highlightTeeth);
            this.playbackPanel.Controls.Add(this.stopButton);
            this.playbackPanel.Controls.Add(this.playButton);
            this.playbackPanel.Controls.Add(this.nowPlayingLabel);
            this.playbackPanel.Controls.Add(this.playbackTrackBar);
            this.playbackPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.playbackPanel.Location = new System.Drawing.Point(0, 492);
            this.playbackPanel.Name = "playbackPanel";
            this.playbackPanel.Size = new System.Drawing.Size(236, 107);
            this.playbackPanel.TabIndex = 14;
            // 
            // highlightTeeth
            // 
            this.highlightTeeth.AutoSize = true;
            this.highlightTeeth.Location = new System.Drawing.Point(78, 78);
            this.highlightTeeth.Name = "highlightTeeth";
            this.highlightTeeth.Size = new System.Drawing.Size(144, 17);
            this.highlightTeeth.TabIndex = 7;
            this.highlightTeeth.Text = "Highlight Teeth Collisions";
            this.highlightTeeth.UseVisualStyleBackColor = true;
            this.highlightTeeth.CheckedChanged += new System.EventHandler(this.highlightTeeth_CheckedChanged);
            // 
            // stopButton
            // 
            this.stopButton.BackgroundImage = global::Medical.Properties.Resources.PlaybackButtons;
            this.stopButton.ClickIndex = 5;
            this.stopButton.HoverIndex = 4;
            this.stopButton.ImageHeight = 32;
            this.stopButton.ImageWidth = 32;
            this.stopButton.Location = new System.Drawing.Point(39, 70);
            this.stopButton.Name = "stopButton";
            this.stopButton.NormalIndex = 3;
            this.stopButton.Size = new System.Drawing.Size(32, 32);
            this.stopButton.TabIndex = 6;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // playButton
            // 
            this.playButton.BackgroundImage = global::Medical.Properties.Resources.PlaybackButtons;
            this.playButton.ClickIndex = 2;
            this.playButton.HoverIndex = 1;
            this.playButton.ImageHeight = 32;
            this.playButton.ImageWidth = 32;
            this.playButton.Location = new System.Drawing.Point(5, 70);
            this.playButton.Name = "playButton";
            this.playButton.NormalIndex = 0;
            this.playButton.Size = new System.Drawing.Size(32, 32);
            this.playButton.TabIndex = 5;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // nowPlayingLabel
            // 
            this.nowPlayingLabel.AutoSize = true;
            this.nowPlayingLabel.Location = new System.Drawing.Point(4, 7);
            this.nowPlayingLabel.Name = "nowPlayingLabel";
            this.nowPlayingLabel.Size = new System.Drawing.Size(0, 13);
            this.nowPlayingLabel.TabIndex = 4;
            // 
            // playbackTrackBar
            // 
            this.playbackTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.playbackTrackBar.BarMenu = null;
            this.playbackTrackBar.ChangeTimeOnSelection = false;
            this.playbackTrackBar.CurrentTime = 0F;
            this.playbackTrackBar.Location = new System.Drawing.Point(3, 22);
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
            this.ButtonImageIndex = 4;
            this.ButtonText = "Muscle Sequences";
            this.ClientSize = new System.Drawing.Size(236, 599);
            this.Controls.Add(this.muscleSequenceView);
            this.Controls.Add(this.playbackPanel);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MuscleControl";
            this.ShortcutKey = System.Windows.Forms.Keys.V;
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Text = "Muscle Sequences";
            this.ToolStripName = "Manipulation";
            this.playbackPanel.ResumeLayout(false);
            this.playbackPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MuscleSequenceView muscleSequenceView;
        private System.Windows.Forms.Panel playbackPanel;
        private TimeTrackBar playbackTrackBar;
        private System.Windows.Forms.Label nowPlayingLabel;
        private Medical.GUI.Common.FancyButton playButton;
        private Medical.GUI.Common.FancyButton stopButton;
        private System.Windows.Forms.CheckBox highlightTeeth;

    }
}
