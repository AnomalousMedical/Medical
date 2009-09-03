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
            this.addStateButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.timeTrackBar = new Medical.GUI.TimeTrackBar();
            this.SuspendLayout();
            // 
            // addStateButton
            // 
            this.addStateButton.Location = new System.Drawing.Point(12, 32);
            this.addStateButton.Name = "addStateButton";
            this.addStateButton.Size = new System.Drawing.Size(75, 23);
            this.addStateButton.TabIndex = 0;
            this.addStateButton.Text = "Add State";
            this.addStateButton.UseVisualStyleBackColor = true;
            this.addStateButton.Click += new System.EventHandler(this.addStateButton_Click);
            // 
            // playButton
            // 
            this.playButton.Location = new System.Drawing.Point(93, 32);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(75, 23);
            this.playButton.TabIndex = 1;
            this.playButton.Text = "Play";
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(174, 32);
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
            this.timeTrackBar.CurrentTime = 0F;
            this.timeTrackBar.Location = new System.Drawing.Point(12, -16);
            this.timeTrackBar.MaximumTime = 1F;
            this.timeTrackBar.Name = "timeTrackBar";
            this.timeTrackBar.Size = new System.Drawing.Size(686, 47);
            this.timeTrackBar.TabIndex = 3;
            // 
            // MovementStateControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 57);
            this.Controls.Add(this.timeTrackBar);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.playButton);
            this.Controls.Add(this.addStateButton);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Name = "MovementStateControl";
            this.Text = "Movement State";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button addStateButton;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Button stopButton;
        private TimeTrackBar timeTrackBar;
    }
}