namespace Medical.GUI.Playback
{
    partial class PlaybackRibbonControl
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
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.playbackTime = new Medical.GUI.TimeTrackBar();
            this.SuspendLayout();
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(4, -5);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(79, 22);
            this.kryptonLabel1.TabIndex = 2;
            this.kryptonLabel1.Values.Text = "Now Playing";
            // 
            // playbackTime
            // 
            this.playbackTime.BarMenu = null;
            this.playbackTime.ChangeTimeOnSelection = false;
            this.playbackTime.CurrentTime = 0F;
            this.playbackTime.Location = new System.Drawing.Point(33, 23);
            this.playbackTime.MaximumTime = 1F;
            this.playbackTime.MoveMarks = false;
            this.playbackTime.MoveThumb = true;
            this.playbackTime.Name = "playbackTime";
            this.playbackTime.SelectedMark = null;
            this.playbackTime.Size = new System.Drawing.Size(150, 47);
            this.playbackTime.TabIndex = 3;
            this.playbackTime.TickMenu = null;
            // 
            // PlaybackRibbonControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.playbackTime);
            this.Controls.Add(this.kryptonLabel1);
            this.Name = "PlaybackRibbonControl";
            this.Size = new System.Drawing.Size(266, 64);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private TimeTrackBar playbackTime;

    }
}
