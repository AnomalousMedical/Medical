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
            this.panel3 = new System.Windows.Forms.Panel();
            this.rightForwardBack = new System.Windows.Forms.TrackBar();
            this.panel4 = new System.Windows.Forms.Panel();
            this.leftForwardBack = new System.Windows.Forms.TrackBar();
            this.panel7 = new System.Windows.Forms.Panel();
            this.bothForwardBack = new System.Windows.Forms.TrackBar();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightForwardBack)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leftForwardBack)).BeginInit();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bothForwardBack)).BeginInit();
            this.SuspendLayout();
            // 
            // distortionButton
            // 
            this.distortionButton.Location = new System.Drawing.Point(67, 248);
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
            this.panel1.Location = new System.Drawing.Point(25, 12);
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
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Yellow;
            this.panel3.Controls.Add(this.rightForwardBack);
            this.panel3.Location = new System.Drawing.Point(-1, 113);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(26, 88);
            this.panel3.TabIndex = 7;
            // 
            // rightForwardBack
            // 
            this.rightForwardBack.LargeChange = 2000;
            this.rightForwardBack.Location = new System.Drawing.Point(0, 3);
            this.rightForwardBack.Maximum = 10000;
            this.rightForwardBack.Name = "rightForwardBack";
            this.rightForwardBack.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.rightForwardBack.Size = new System.Drawing.Size(45, 82);
            this.rightForwardBack.SmallChange = 1000;
            this.rightForwardBack.TabIndex = 2;
            this.rightForwardBack.TickFrequency = 10000;
            this.rightForwardBack.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Yellow;
            this.panel4.Controls.Add(this.leftForwardBack);
            this.panel4.Location = new System.Drawing.Point(232, 113);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(26, 91);
            this.panel4.TabIndex = 8;
            // 
            // leftForwardBack
            // 
            this.leftForwardBack.LargeChange = 2000;
            this.leftForwardBack.Location = new System.Drawing.Point(3, 3);
            this.leftForwardBack.Maximum = 10000;
            this.leftForwardBack.Name = "leftForwardBack";
            this.leftForwardBack.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.leftForwardBack.Size = new System.Drawing.Size(45, 82);
            this.leftForwardBack.SmallChange = 1000;
            this.leftForwardBack.TabIndex = 3;
            this.leftForwardBack.TickFrequency = 10000;
            this.leftForwardBack.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.Yellow;
            this.panel7.Controls.Add(this.bothForwardBack);
            this.panel7.Location = new System.Drawing.Point(67, 210);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(125, 29);
            this.panel7.TabIndex = 10;
            // 
            // bothForwardBack
            // 
            this.bothForwardBack.LargeChange = 2000;
            this.bothForwardBack.Location = new System.Drawing.Point(4, 3);
            this.bothForwardBack.Maximum = 10000;
            this.bothForwardBack.Name = "bothForwardBack";
            this.bothForwardBack.Size = new System.Drawing.Size(118, 45);
            this.bothForwardBack.SmallChange = 1000;
            this.bothForwardBack.TabIndex = 3;
            this.bothForwardBack.TickFrequency = 10000;
            this.bothForwardBack.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // MandibleOffsetControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(259, 276);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.distortionButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel4);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Name = "MandibleOffsetControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Text = "Mandible Offset";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightForwardBack)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leftForwardBack)).EndInit();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bothForwardBack)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button distortionButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar rightForwardBack;
        private System.Windows.Forms.TrackBar leftForwardBack;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.TrackBar bothForwardBack;
    }
}
