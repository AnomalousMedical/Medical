namespace Medical
{
    partial class EulerRotatePanel
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
            this.radians = new System.Windows.Forms.RadioButton();
            this.degrees = new System.Windows.Forms.RadioButton();
            this.roll = new System.Windows.Forms.NumericUpDown();
            this.pitch = new System.Windows.Forms.NumericUpDown();
            this.yaw = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.roll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yaw)).BeginInit();
            this.SuspendLayout();
            // 
            // radians
            // 
            this.radians.AutoSize = true;
            this.radians.Location = new System.Drawing.Point(230, 24);
            this.radians.Name = "radians";
            this.radians.Size = new System.Drawing.Size(64, 17);
            this.radians.TabIndex = 17;
            this.radians.Text = "Radians";
            this.radians.UseVisualStyleBackColor = true;
            // 
            // degrees
            // 
            this.degrees.AutoSize = true;
            this.degrees.Checked = true;
            this.degrees.Location = new System.Drawing.Point(230, 2);
            this.degrees.Name = "degrees";
            this.degrees.Size = new System.Drawing.Size(65, 17);
            this.degrees.TabIndex = 16;
            this.degrees.TabStop = true;
            this.degrees.Text = "Degrees";
            this.degrees.UseVisualStyleBackColor = true;
            // 
            // roll
            // 
            this.roll.DecimalPlaces = 4;
            this.roll.Location = new System.Drawing.Point(150, 22);
            this.roll.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.roll.Minimum = new decimal(new int[] {
            10000000,
            0,
            0,
            -2147483648});
            this.roll.Name = "roll";
            this.roll.Size = new System.Drawing.Size(68, 20);
            this.roll.TabIndex = 15;
            // 
            // pitch
            // 
            this.pitch.DecimalPlaces = 4;
            this.pitch.Location = new System.Drawing.Point(76, 22);
            this.pitch.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.pitch.Minimum = new decimal(new int[] {
            10000000,
            0,
            0,
            -2147483648});
            this.pitch.Name = "pitch";
            this.pitch.Size = new System.Drawing.Size(68, 20);
            this.pitch.TabIndex = 14;
            // 
            // yaw
            // 
            this.yaw.DecimalPlaces = 4;
            this.yaw.Location = new System.Drawing.Point(2, 22);
            this.yaw.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.yaw.Minimum = new decimal(new int[] {
            10000000,
            0,
            0,
            -2147483648});
            this.yaw.Name = "yaw";
            this.yaw.Size = new System.Drawing.Size(68, 20);
            this.yaw.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(163, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Roll";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(87, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Pitch";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Yaw";
            // 
            // EulerRotatePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 46);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.radians);
            this.Controls.Add(this.degrees);
            this.Controls.Add(this.roll);
            this.Controls.Add(this.pitch);
            this.Controls.Add(this.yaw);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Name = "EulerRotatePanel";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottom;
            this.Text = "Rotate";
            ((System.ComponentModel.ISupportInitialize)(this.roll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yaw)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radians;
        private System.Windows.Forms.RadioButton degrees;
        private System.Windows.Forms.NumericUpDown roll;
        private System.Windows.Forms.NumericUpDown pitch;
        private System.Windows.Forms.NumericUpDown yaw;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
