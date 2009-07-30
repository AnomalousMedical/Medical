namespace Medical.GUI
{
    partial class DiskControl
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
            this.distortionButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.centerTrackBar = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rightDiscOffset = new System.Windows.Forms.TrackBar();
            this.panel5 = new System.Windows.Forms.Panel();
            this.rightRDAOffset = new System.Windows.Forms.TrackBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.leftRDAOffset = new System.Windows.Forms.TrackBar();
            this.panel4 = new System.Windows.Forms.Panel();
            this.leftDiscOffset = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.rightDiscPositionSlider = new System.Windows.Forms.TrackBar();
            this.leftDiscPositionSlider = new System.Windows.Forms.TrackBar();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.centerTrackBar)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightDiscOffset)).BeginInit();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightRDAOffset)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leftRDAOffset)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leftDiscOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightDiscPositionSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftDiscPositionSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // distortionButton
            // 
            this.distortionButton.Location = new System.Drawing.Point(68, 471);
            this.distortionButton.Name = "distortionButton";
            this.distortionButton.Size = new System.Drawing.Size(125, 23);
            this.distortionButton.TabIndex = 12;
            this.distortionButton.Text = "Make Normal";
            this.distortionButton.UseVisualStyleBackColor = true;
            this.distortionButton.Click += new System.EventHandler(this.distortionButton_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(111)))), ((int)(((byte)(243)))));
            this.panel2.Controls.Add(this.centerTrackBar);
            this.panel2.Location = new System.Drawing.Point(68, 440);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(125, 25);
            this.panel2.TabIndex = 13;
            // 
            // centerTrackBar
            // 
            this.centerTrackBar.LargeChange = 2000;
            this.centerTrackBar.Location = new System.Drawing.Point(0, 0);
            this.centerTrackBar.Maximum = 10000;
            this.centerTrackBar.Minimum = -10000;
            this.centerTrackBar.Name = "centerTrackBar";
            this.centerTrackBar.Size = new System.Drawing.Size(122, 45);
            this.centerTrackBar.SmallChange = 1000;
            this.centerTrackBar.TabIndex = 0;
            this.centerTrackBar.TickFrequency = 10000;
            this.centerTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Medical.Properties.Resources.disc;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(2, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(206, 192);
            this.panel1.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(9, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "RDA Space";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.Yellow;
            this.label1.Location = new System.Drawing.Point(63, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Disc Space";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.panel3);
            this.groupBox1.Controls.Add(this.panel5);
            this.groupBox1.Controls.Add(this.rightDiscPositionSlider);
            this.groupBox1.Location = new System.Drawing.Point(8, 198);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 117);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Right Disc";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Yellow;
            this.panel3.Controls.Add(this.rightDiscOffset);
            this.panel3.Location = new System.Drawing.Point(86, 80);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(108, 28);
            this.panel3.TabIndex = 18;
            // 
            // rightDiscOffset
            // 
            this.rightDiscOffset.LargeChange = 2000;
            this.rightDiscOffset.Location = new System.Drawing.Point(3, 0);
            this.rightDiscOffset.Maximum = 10000;
            this.rightDiscOffset.Name = "rightDiscOffset";
            this.rightDiscOffset.Size = new System.Drawing.Size(102, 45);
            this.rightDiscOffset.SmallChange = 1000;
            this.rightDiscOffset.TabIndex = 2;
            this.rightDiscOffset.TickFrequency = 10000;
            this.rightDiscOffset.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Red;
            this.panel5.Controls.Add(this.rightRDAOffset);
            this.panel5.Location = new System.Drawing.Point(86, 46);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(108, 28);
            this.panel5.TabIndex = 19;
            // 
            // rightRDAOffset
            // 
            this.rightRDAOffset.LargeChange = 2000;
            this.rightRDAOffset.Location = new System.Drawing.Point(2, 0);
            this.rightRDAOffset.Maximum = 10000;
            this.rightRDAOffset.Name = "rightRDAOffset";
            this.rightRDAOffset.Size = new System.Drawing.Size(103, 45);
            this.rightRDAOffset.SmallChange = 1000;
            this.rightRDAOffset.TabIndex = 3;
            this.rightRDAOffset.TickFrequency = 10000;
            this.rightRDAOffset.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.panel6);
            this.groupBox2.Controls.Add(this.panel4);
            this.groupBox2.Controls.Add(this.leftDiscPositionSlider);
            this.groupBox2.Location = new System.Drawing.Point(8, 321);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 113);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Left Disc";
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.Red;
            this.panel6.Controls.Add(this.leftRDAOffset);
            this.panel6.Location = new System.Drawing.Point(86, 41);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(108, 28);
            this.panel6.TabIndex = 17;
            // 
            // leftRDAOffset
            // 
            this.leftRDAOffset.LargeChange = 2000;
            this.leftRDAOffset.Location = new System.Drawing.Point(2, 3);
            this.leftRDAOffset.Maximum = 10000;
            this.leftRDAOffset.Name = "leftRDAOffset";
            this.leftRDAOffset.Size = new System.Drawing.Size(103, 45);
            this.leftRDAOffset.SmallChange = 1000;
            this.leftRDAOffset.TabIndex = 3;
            this.leftRDAOffset.TickFrequency = 10000;
            this.leftRDAOffset.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Yellow;
            this.panel4.Controls.Add(this.leftDiscOffset);
            this.panel4.Location = new System.Drawing.Point(86, 75);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(108, 28);
            this.panel4.TabIndex = 18;
            // 
            // leftDiscOffset
            // 
            this.leftDiscOffset.LargeChange = 2000;
            this.leftDiscOffset.Location = new System.Drawing.Point(1, 3);
            this.leftDiscOffset.Maximum = 10000;
            this.leftDiscOffset.Name = "leftDiscOffset";
            this.leftDiscOffset.Size = new System.Drawing.Size(104, 45);
            this.leftDiscOffset.SmallChange = 1000;
            this.leftDiscOffset.TabIndex = 3;
            this.leftDiscOffset.TickFrequency = 10000;
            this.leftDiscOffset.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(125, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Horizontal Shift";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(8, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "RDA Space";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(8, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Disc Space";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(6, 44);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "RDA Space";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label7.Location = new System.Drawing.Point(8, 76);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Disc Space";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(6, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 13);
            this.label8.TabIndex = 25;
            this.label8.Text = "Position";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label9.Location = new System.Drawing.Point(8, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 13);
            this.label9.TabIndex = 26;
            this.label9.Text = "Position";
            // 
            // rightDiscPositionSlider
            // 
            this.rightDiscPositionSlider.LargeChange = 2000;
            this.rightDiscPositionSlider.Location = new System.Drawing.Point(88, 14);
            this.rightDiscPositionSlider.Maximum = 10000;
            this.rightDiscPositionSlider.Name = "rightDiscPositionSlider";
            this.rightDiscPositionSlider.Size = new System.Drawing.Size(103, 45);
            this.rightDiscPositionSlider.SmallChange = 1000;
            this.rightDiscPositionSlider.TabIndex = 27;
            this.rightDiscPositionSlider.TickFrequency = 10000;
            this.rightDiscPositionSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // leftDiscPositionSlider
            // 
            this.leftDiscPositionSlider.LargeChange = 2000;
            this.leftDiscPositionSlider.Location = new System.Drawing.Point(88, 12);
            this.leftDiscPositionSlider.Maximum = 10000;
            this.leftDiscPositionSlider.Name = "leftDiscPositionSlider";
            this.leftDiscPositionSlider.Size = new System.Drawing.Size(103, 45);
            this.leftDiscPositionSlider.SmallChange = 1000;
            this.leftDiscPositionSlider.TabIndex = 28;
            this.leftDiscPositionSlider.TickFrequency = 10000;
            this.leftDiscPositionSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // DiskControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(261, 550);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.distortionButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Name = "DiskControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Tag = "Disc";
            this.Text = "Disc";
            this.ToolStripName = "Advanced";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.centerTrackBar)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightDiscOffset)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightRDAOffset)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leftRDAOffset)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leftDiscOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightDiscPositionSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftDiscPositionSlider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button distortionButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TrackBar centerTrackBar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TrackBar rightDiscOffset;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TrackBar rightRDAOffset;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TrackBar leftRDAOffset;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TrackBar leftDiscOffset;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar rightDiscPositionSlider;
        private System.Windows.Forms.TrackBar leftDiscPositionSlider;
    }
}
