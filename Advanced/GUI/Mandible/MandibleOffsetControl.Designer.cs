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
            this.distortionButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.leftForwardBack = new System.Windows.Forms.TrackBar();
            this.bothForwardBack = new System.Windows.Forms.TrackBar();
            this.rightForwardBack = new System.Windows.Forms.TrackBar();
            this.rightOffset = new System.Windows.Forms.NumericUpDown();
            this.leftOffset = new System.Windows.Forms.NumericUpDown();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leftForwardBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bothForwardBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightForwardBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftOffset)).BeginInit();
            this.SuspendLayout();
            // 
            // distortionButton
            // 
            this.distortionButton.Location = new System.Drawing.Point(67, 267);
            this.distortionButton.Name = "distortionButton";
            this.distortionButton.Size = new System.Drawing.Size(125, 23);
            this.distortionButton.TabIndex = 1;
            this.distortionButton.Text = "Reset";
            this.distortionButton.UseVisualStyleBackColor = true;
            this.distortionButton.Click += new System.EventHandler(this.distortionButton_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Medical.Properties.Resources.mandibletranslation;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(25, 31);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(206, 192);
            this.panel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(178, 6);
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
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Right";
            // 
            // leftForwardBack
            // 
            this.leftForwardBack.LargeChange = 2000;
            this.leftForwardBack.Location = new System.Drawing.Point(237, 31);
            this.leftForwardBack.Maximum = 10000;
            this.leftForwardBack.Name = "leftForwardBack";
            this.leftForwardBack.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.leftForwardBack.Size = new System.Drawing.Size(45, 192);
            this.leftForwardBack.SmallChange = 1000;
            this.leftForwardBack.TabIndex = 5;
            this.leftForwardBack.TickFrequency = 10000;
            this.leftForwardBack.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // bothForwardBack
            // 
            this.bothForwardBack.LargeChange = 2000;
            this.bothForwardBack.Location = new System.Drawing.Point(25, 229);
            this.bothForwardBack.Maximum = 10000;
            this.bothForwardBack.Name = "bothForwardBack";
            this.bothForwardBack.Size = new System.Drawing.Size(206, 45);
            this.bothForwardBack.SmallChange = 1000;
            this.bothForwardBack.TabIndex = 6;
            this.bothForwardBack.TickFrequency = 10000;
            this.bothForwardBack.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // rightForwardBack
            // 
            this.rightForwardBack.LargeChange = 2000;
            this.rightForwardBack.Location = new System.Drawing.Point(2, 31);
            this.rightForwardBack.Maximum = 10000;
            this.rightForwardBack.Name = "rightForwardBack";
            this.rightForwardBack.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.rightForwardBack.Size = new System.Drawing.Size(45, 192);
            this.rightForwardBack.SmallChange = 1000;
            this.rightForwardBack.TabIndex = 4;
            this.rightForwardBack.TickFrequency = 10000;
            this.rightForwardBack.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // rightOffset
            // 
            this.rightOffset.DecimalPlaces = 8;
            this.rightOffset.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.rightOffset.Location = new System.Drawing.Point(2, 5);
            this.rightOffset.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.rightOffset.Name = "rightOffset";
            this.rightOffset.Size = new System.Drawing.Size(120, 20);
            this.rightOffset.TabIndex = 7;
            // 
            // leftOffset
            // 
            this.leftOffset.DecimalPlaces = 8;
            this.leftOffset.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.leftOffset.Location = new System.Drawing.Point(140, 5);
            this.leftOffset.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.leftOffset.Name = "leftOffset";
            this.leftOffset.Size = new System.Drawing.Size(120, 20);
            this.leftOffset.TabIndex = 8;
            // 
            // MandibleOffsetControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(281, 320);
            this.Controls.Add(this.leftOffset);
            this.Controls.Add(this.rightOffset);
            this.Controls.Add(this.leftForwardBack);
            this.Controls.Add(this.distortionButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.rightForwardBack);
            this.Controls.Add(this.bothForwardBack);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Name = "MandibleOffsetControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Text = "Mandible Offset";
            this.ToolStripName = "Advanced";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leftForwardBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bothForwardBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightForwardBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftOffset)).EndInit();
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
    }
}
