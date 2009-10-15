namespace Medical.GUI
{
    partial class MandibleOffsetControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MandibleOffsetControl));
            this.distortionButton = new System.Windows.Forms.Button();
            this.leftForwardBack = new System.Windows.Forms.TrackBar();
            this.bothForwardBack = new System.Windows.Forms.TrackBar();
            this.rightForwardBack = new System.Windows.Forms.TrackBar();
            this.rightOffset = new System.Windows.Forms.NumericUpDown();
            this.leftOffset = new System.Windows.Forms.NumericUpDown();
            this.openTrackBar = new System.Windows.Forms.TrackBar();
            this.forceUpDown = new System.Windows.Forms.NumericUpDown();
            this.forceLabel = new System.Windows.Forms.Label();
            this.forceSlider = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.openUpDown = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.leftForwardBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bothForwardBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightForwardBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.openTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.forceUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.forceSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.openUpDown)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // distortionButton
            // 
            this.distortionButton.Location = new System.Drawing.Point(65, 309);
            this.distortionButton.Name = "distortionButton";
            this.distortionButton.Size = new System.Drawing.Size(125, 23);
            this.distortionButton.TabIndex = 1;
            this.distortionButton.Text = "Reset";
            this.distortionButton.UseVisualStyleBackColor = true;
            this.distortionButton.Click += new System.EventHandler(this.distortionButton_Click);
            // 
            // leftForwardBack
            // 
            this.leftForwardBack.LargeChange = 2000;
            this.leftForwardBack.Location = new System.Drawing.Point(36, 1);
            this.leftForwardBack.Maximum = 10000;
            this.leftForwardBack.Name = "leftForwardBack";
            this.leftForwardBack.Size = new System.Drawing.Size(164, 45);
            this.leftForwardBack.SmallChange = 1000;
            this.leftForwardBack.TabIndex = 5;
            this.leftForwardBack.TickFrequency = 10000;
            this.leftForwardBack.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // bothForwardBack
            // 
            this.bothForwardBack.LargeChange = 2000;
            this.bothForwardBack.Location = new System.Drawing.Point(36, 251);
            this.bothForwardBack.Maximum = 10000;
            this.bothForwardBack.Name = "bothForwardBack";
            this.bothForwardBack.Size = new System.Drawing.Size(164, 45);
            this.bothForwardBack.SmallChange = 1000;
            this.bothForwardBack.TabIndex = 6;
            this.bothForwardBack.TickFrequency = 10000;
            this.bothForwardBack.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // rightForwardBack
            // 
            this.rightForwardBack.LargeChange = 2000;
            this.rightForwardBack.Location = new System.Drawing.Point(36, 227);
            this.rightForwardBack.Maximum = 10000;
            this.rightForwardBack.Name = "rightForwardBack";
            this.rightForwardBack.Size = new System.Drawing.Size(164, 45);
            this.rightForwardBack.SmallChange = 1000;
            this.rightForwardBack.TabIndex = 4;
            this.rightForwardBack.TickFrequency = 10000;
            this.rightForwardBack.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // rightOffset
            // 
            this.rightOffset.DecimalPlaces = 4;
            this.rightOffset.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.rightOffset.Location = new System.Drawing.Point(197, 227);
            this.rightOffset.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.rightOffset.Name = "rightOffset";
            this.rightOffset.Size = new System.Drawing.Size(69, 20);
            this.rightOffset.TabIndex = 7;
            // 
            // leftOffset
            // 
            this.leftOffset.DecimalPlaces = 4;
            this.leftOffset.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.leftOffset.Location = new System.Drawing.Point(197, 1);
            this.leftOffset.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.leftOffset.Name = "leftOffset";
            this.leftOffset.Size = new System.Drawing.Size(69, 20);
            this.leftOffset.TabIndex = 8;
            // 
            // openTrackBar
            // 
            this.openTrackBar.LargeChange = 2000;
            this.openTrackBar.Location = new System.Drawing.Point(210, 49);
            this.openTrackBar.Maximum = 3000;
            this.openTrackBar.Minimum = -10000;
            this.openTrackBar.Name = "openTrackBar";
            this.openTrackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.openTrackBar.Size = new System.Drawing.Size(45, 180);
            this.openTrackBar.SmallChange = 1000;
            this.openTrackBar.TabIndex = 9;
            this.openTrackBar.TickFrequency = 10000;
            this.openTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // forceUpDown
            // 
            this.forceUpDown.Location = new System.Drawing.Point(186, 283);
            this.forceUpDown.Name = "forceUpDown";
            this.forceUpDown.Size = new System.Drawing.Size(69, 20);
            this.forceUpDown.TabIndex = 10;
            // 
            // forceLabel
            // 
            this.forceLabel.AutoSize = true;
            this.forceLabel.Location = new System.Drawing.Point(232, 33);
            this.forceLabel.Name = "forceLabel";
            this.forceLabel.Size = new System.Drawing.Size(34, 13);
            this.forceLabel.TabIndex = 11;
            this.forceLabel.Text = "Force";
            // 
            // forceSlider
            // 
            this.forceSlider.LargeChange = 10;
            this.forceSlider.Location = new System.Drawing.Point(235, 49);
            this.forceSlider.Maximum = 100;
            this.forceSlider.Name = "forceSlider";
            this.forceSlider.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.forceSlider.Size = new System.Drawing.Size(45, 180);
            this.forceSlider.SmallChange = 5;
            this.forceSlider.TabIndex = 12;
            this.forceSlider.TickFrequency = 10000;
            this.forceSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(5, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Left";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(5, 232);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Right";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(138, 285);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Force";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label7.Location = new System.Drawing.Point(5, 283);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Opening";
            // 
            // openUpDown
            // 
            this.openUpDown.DecimalPlaces = 4;
            this.openUpDown.Location = new System.Drawing.Point(53, 281);
            this.openUpDown.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.openUpDown.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            -2147483648});
            this.openUpDown.Name = "openUpDown";
            this.openUpDown.Size = new System.Drawing.Size(80, 20);
            this.openUpDown.TabIndex = 16;
            this.openUpDown.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(5, 256);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Both";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(2, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(206, 192);
            this.panel1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(156, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Opening";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Left";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Right";
            // 
            // MandibleOffsetControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(268, 355);
            this.Controls.Add(this.forceUpDown);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.rightOffset);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.openUpDown);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.leftOffset);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.forceSlider);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.bothForwardBack);
            this.Controls.Add(this.forceLabel);
            this.Controls.Add(this.leftForwardBack);
            this.Controls.Add(this.openTrackBar);
            this.Controls.Add(this.distortionButton);
            this.Controls.Add(this.rightForwardBack);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MandibleOffsetControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Text = "Mandible Offset";
            this.ToolStripName = "Advanced";
            ((System.ComponentModel.ISupportInitialize)(this.leftForwardBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bothForwardBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightForwardBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.openTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.forceUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.forceSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.openUpDown)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button distortionButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar leftForwardBack;
        private System.Windows.Forms.TrackBar bothForwardBack;
        private System.Windows.Forms.TrackBar rightForwardBack;
        private System.Windows.Forms.NumericUpDown rightOffset;
        private System.Windows.Forms.NumericUpDown leftOffset;
        private System.Windows.Forms.TrackBar openTrackBar;
        private System.Windows.Forms.NumericUpDown forceUpDown;
        private System.Windows.Forms.Label forceLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar forceSlider;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown openUpDown;
        private System.Windows.Forms.Label label8;
    }
}
