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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiskControl));
            this.distortionButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.horizontalOffsetTrackBar = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rightDiscPanel = new Medical.GUI.DiscPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.leftDiscPanel = new Medical.GUI.DiscPanel();
            this.horizontalOffsetUpDown = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.horizontalOffsetTrackBar)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.horizontalOffsetUpDown)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // distortionButton
            // 
            this.distortionButton.Location = new System.Drawing.Point(90, 503);
            this.distortionButton.Name = "distortionButton";
            this.distortionButton.Size = new System.Drawing.Size(76, 23);
            this.distortionButton.TabIndex = 12;
            this.distortionButton.Text = "Reset";
            this.distortionButton.UseVisualStyleBackColor = true;
            this.distortionButton.Click += new System.EventHandler(this.distortionButton_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(111)))), ((int)(((byte)(243)))));
            this.panel2.Controls.Add(this.horizontalOffsetTrackBar);
            this.panel2.Location = new System.Drawing.Point(44, 472);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(125, 25);
            this.panel2.TabIndex = 13;
            // 
            // horizontalOffsetTrackBar
            // 
            this.horizontalOffsetTrackBar.LargeChange = 2000;
            this.horizontalOffsetTrackBar.Location = new System.Drawing.Point(0, 0);
            this.horizontalOffsetTrackBar.Maximum = 10000;
            this.horizontalOffsetTrackBar.Minimum = -10000;
            this.horizontalOffsetTrackBar.Name = "horizontalOffsetTrackBar";
            this.horizontalOffsetTrackBar.Size = new System.Drawing.Size(122, 45);
            this.horizontalOffsetTrackBar.SmallChange = 1000;
            this.horizontalOffsetTrackBar.TabIndex = 0;
            this.horizontalOffsetTrackBar.TickFrequency = 10000;
            this.horizontalOffsetTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rightDiscPanel);
            this.groupBox1.Location = new System.Drawing.Point(8, 198);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(261, 132);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Right Disc";
            // 
            // rightDiscPanel
            // 
            this.rightDiscPanel.DiscName = "RightTMJDisc";
            this.rightDiscPanel.Location = new System.Drawing.Point(4, 12);
            this.rightDiscPanel.Name = "rightDiscPanel";
            this.rightDiscPanel.Size = new System.Drawing.Size(255, 116);
            this.rightDiscPanel.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.leftDiscPanel);
            this.groupBox2.Location = new System.Drawing.Point(8, 336);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(261, 126);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Left Disc";
            // 
            // leftDiscPanel
            // 
            this.leftDiscPanel.DiscName = "LeftTMJDisc";
            this.leftDiscPanel.Location = new System.Drawing.Point(3, 11);
            this.leftDiscPanel.Name = "leftDiscPanel";
            this.leftDiscPanel.Size = new System.Drawing.Size(255, 116);
            this.leftDiscPanel.TabIndex = 0;
            // 
            // horizontalOffsetUpDown
            // 
            this.horizontalOffsetUpDown.DecimalPlaces = 4;
            this.horizontalOffsetUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.horizontalOffsetUpDown.Location = new System.Drawing.Point(172, 474);
            this.horizontalOffsetUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.horizontalOffsetUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.horizontalOffsetUpDown.Name = "horizontalOffsetUpDown";
            this.horizontalOffsetUpDown.Size = new System.Drawing.Size(62, 20);
            this.horizontalOffsetUpDown.TabIndex = 35;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Medical.Properties.Resources.disc;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(33, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(206, 192);
            this.panel1.TabIndex = 11;
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
            // DiskControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(272, 550);
            this.Controls.Add(this.horizontalOffsetUpDown);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.distortionButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DiskControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Tag = "Disc";
            this.Text = "Disc";
            this.ToolStripName = "Advanced";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.horizontalOffsetTrackBar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.horizontalOffsetUpDown)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button distortionButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TrackBar horizontalOffsetTrackBar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown horizontalOffsetUpDown;
        private Medical.GUI.DiscPanel rightDiscPanel;
        private Medical.GUI.DiscPanel leftDiscPanel;
    }
}
