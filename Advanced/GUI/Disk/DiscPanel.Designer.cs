namespace Medical.GUI
{
    partial class DiscPanel
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
            this.discOffsetUpDown = new System.Windows.Forms.NumericUpDown();
            this.rdaOffsetUpDown = new System.Windows.Forms.NumericUpDown();
            this.discPopUpDown = new System.Windows.Forms.NumericUpDown();
            this.discLockedCheck = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.discOffsetSlider = new System.Windows.Forms.TrackBar();
            this.panel5 = new System.Windows.Forms.Panel();
            this.rdaOffsetSlider = new System.Windows.Forms.TrackBar();
            this.discPopSlider = new System.Windows.Forms.TrackBar();
            this.lateralPoleRotUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.lateralPoleRotSlider = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.discOffsetUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdaOffsetUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.discPopUpDown)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.discOffsetSlider)).BeginInit();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rdaOffsetSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.discPopSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lateralPoleRotUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lateralPoleRotSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // discOffsetUpDown
            // 
            this.discOffsetUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.discOffsetUpDown.DecimalPlaces = 4;
            this.discOffsetUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.discOffsetUpDown.Location = new System.Drawing.Point(191, 71);
            this.discOffsetUpDown.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.discOffsetUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.discOffsetUpDown.Name = "discOffsetUpDown";
            this.discOffsetUpDown.Size = new System.Drawing.Size(54, 20);
            this.discOffsetUpDown.TabIndex = 41;
            // 
            // rdaOffsetUpDown
            // 
            this.rdaOffsetUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rdaOffsetUpDown.DecimalPlaces = 4;
            this.rdaOffsetUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.rdaOffsetUpDown.Location = new System.Drawing.Point(191, 37);
            this.rdaOffsetUpDown.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.rdaOffsetUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.rdaOffsetUpDown.Name = "rdaOffsetUpDown";
            this.rdaOffsetUpDown.Size = new System.Drawing.Size(54, 20);
            this.rdaOffsetUpDown.TabIndex = 40;
            // 
            // discPopUpDown
            // 
            this.discPopUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.discPopUpDown.DecimalPlaces = 4;
            this.discPopUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.discPopUpDown.Location = new System.Drawing.Point(191, 4);
            this.discPopUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.discPopUpDown.Name = "discPopUpDown";
            this.discPopUpDown.Size = new System.Drawing.Size(54, 20);
            this.discPopUpDown.TabIndex = 39;
            // 
            // discLockedCheck
            // 
            this.discLockedCheck.AutoSize = true;
            this.discLockedCheck.Location = new System.Drawing.Point(5, 98);
            this.discLockedCheck.Name = "discLockedCheck";
            this.discLockedCheck.Size = new System.Drawing.Size(62, 17);
            this.discLockedCheck.TabIndex = 38;
            this.discLockedCheck.Text = "Locked";
            this.discLockedCheck.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label9.Location = new System.Drawing.Point(2, 11);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 13);
            this.label9.TabIndex = 36;
            this.label9.Text = "Position";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(2, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "RDA Space";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(2, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Disc Space";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Yellow;
            this.panel3.Controls.Add(this.discOffsetSlider);
            this.panel3.Location = new System.Drawing.Point(80, 69);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(108, 28);
            this.panel3.TabIndex = 32;
            // 
            // discOffsetSlider
            // 
            this.discOffsetSlider.LargeChange = 2000;
            this.discOffsetSlider.Location = new System.Drawing.Point(3, 0);
            this.discOffsetSlider.Maximum = 10000;
            this.discOffsetSlider.Name = "discOffsetSlider";
            this.discOffsetSlider.Size = new System.Drawing.Size(102, 45);
            this.discOffsetSlider.SmallChange = 1000;
            this.discOffsetSlider.TabIndex = 2;
            this.discOffsetSlider.TickFrequency = 10000;
            this.discOffsetSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Red;
            this.panel5.Controls.Add(this.rdaOffsetSlider);
            this.panel5.Location = new System.Drawing.Point(80, 35);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(108, 28);
            this.panel5.TabIndex = 33;
            // 
            // rdaOffsetSlider
            // 
            this.rdaOffsetSlider.LargeChange = 2000;
            this.rdaOffsetSlider.Location = new System.Drawing.Point(2, 0);
            this.rdaOffsetSlider.Maximum = 10000;
            this.rdaOffsetSlider.Name = "rdaOffsetSlider";
            this.rdaOffsetSlider.Size = new System.Drawing.Size(103, 45);
            this.rdaOffsetSlider.SmallChange = 1000;
            this.rdaOffsetSlider.TabIndex = 3;
            this.rdaOffsetSlider.TickFrequency = 10000;
            this.rdaOffsetSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // discPopSlider
            // 
            this.discPopSlider.LargeChange = 100;
            this.discPopSlider.Location = new System.Drawing.Point(82, 3);
            this.discPopSlider.Maximum = 10000;
            this.discPopSlider.Name = "discPopSlider";
            this.discPopSlider.Size = new System.Drawing.Size(103, 45);
            this.discPopSlider.SmallChange = 1000;
            this.discPopSlider.TabIndex = 37;
            this.discPopSlider.TickFrequency = 10000;
            this.discPopSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // lateralPoleRotUpDown
            // 
            this.lateralPoleRotUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lateralPoleRotUpDown.DecimalPlaces = 4;
            this.lateralPoleRotUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.lateralPoleRotUpDown.Location = new System.Drawing.Point(192, 111);
            this.lateralPoleRotUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.lateralPoleRotUpDown.Name = "lateralPoleRotUpDown";
            this.lateralPoleRotUpDown.Size = new System.Drawing.Size(54, 20);
            this.lateralPoleRotUpDown.TabIndex = 44;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(3, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 42;
            this.label1.Text = "Lateral Pole Rot";
            // 
            // lateralPoleRotSlider
            // 
            this.lateralPoleRotSlider.LargeChange = 100;
            this.lateralPoleRotSlider.Location = new System.Drawing.Point(83, 110);
            this.lateralPoleRotSlider.Maximum = 10000;
            this.lateralPoleRotSlider.Name = "lateralPoleRotSlider";
            this.lateralPoleRotSlider.Size = new System.Drawing.Size(103, 45);
            this.lateralPoleRotSlider.SmallChange = 1000;
            this.lateralPoleRotSlider.TabIndex = 43;
            this.lateralPoleRotSlider.TickFrequency = 10000;
            this.lateralPoleRotSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // DiscPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lateralPoleRotUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lateralPoleRotSlider);
            this.Controls.Add(this.discOffsetUpDown);
            this.Controls.Add(this.rdaOffsetUpDown);
            this.Controls.Add(this.discPopUpDown);
            this.Controls.Add(this.discLockedCheck);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.discPopSlider);
            this.Name = "DiscPanel";
            this.Size = new System.Drawing.Size(248, 163);
            ((System.ComponentModel.ISupportInitialize)(this.discOffsetUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdaOffsetUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.discPopUpDown)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.discOffsetSlider)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rdaOffsetSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.discPopSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lateralPoleRotUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lateralPoleRotSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown discOffsetUpDown;
        private System.Windows.Forms.NumericUpDown rdaOffsetUpDown;
        private System.Windows.Forms.NumericUpDown discPopUpDown;
        private System.Windows.Forms.CheckBox discLockedCheck;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TrackBar discOffsetSlider;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TrackBar rdaOffsetSlider;
        private System.Windows.Forms.TrackBar discPopSlider;
        private System.Windows.Forms.NumericUpDown lateralPoleRotUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar lateralPoleRotSlider;
    }
}
