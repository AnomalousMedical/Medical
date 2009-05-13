namespace Medical.GUI
{
    partial class MandibleSizeControl
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
            this.distortionButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Right = new System.Windows.Forms.Label();
            this.leftCondyleSize = new System.Windows.Forms.HScrollBar();
            this.leftRhemusHeight = new System.Windows.Forms.VScrollBar();
            this.rightCondyleSize = new System.Windows.Forms.HScrollBar();
            this.rightRhemusHeight = new System.Windows.Forms.VScrollBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // distortionButton
            // 
            this.distortionButton.Location = new System.Drawing.Point(62, 146);
            this.distortionButton.Name = "distortionButton";
            this.distortionButton.Size = new System.Drawing.Size(137, 23);
            this.distortionButton.TabIndex = 17;
            this.distortionButton.Text = "Make Normal";
            this.distortionButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(174, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Left";
            // 
            // Right
            // 
            this.Right.AutoSize = true;
            this.Right.Location = new System.Drawing.Point(59, 9);
            this.Right.Name = "Right";
            this.Right.Size = new System.Drawing.Size(32, 13);
            this.Right.TabIndex = 15;
            this.Right.Text = "Right";
            // 
            // leftCondyleSize
            // 
            this.leftCondyleSize.LargeChange = 1000;
            this.leftCondyleSize.Location = new System.Drawing.Point(131, 28);
            this.leftCondyleSize.Maximum = 30000;
            this.leftCondyleSize.Name = "leftCondyleSize";
            this.leftCondyleSize.Size = new System.Drawing.Size(95, 17);
            this.leftCondyleSize.SmallChange = 500;
            this.leftCondyleSize.TabIndex = 14;
            this.leftCondyleSize.Value = 10000;
            // 
            // leftRhemusHeight
            // 
            this.leftRhemusHeight.LargeChange = 1000;
            this.leftRhemusHeight.Location = new System.Drawing.Point(229, 50);
            this.leftRhemusHeight.Maximum = 30000;
            this.leftRhemusHeight.Name = "leftRhemusHeight";
            this.leftRhemusHeight.Size = new System.Drawing.Size(17, 90);
            this.leftRhemusHeight.SmallChange = 500;
            this.leftRhemusHeight.TabIndex = 13;
            this.leftRhemusHeight.Value = 10000;
            // 
            // rightCondyleSize
            // 
            this.rightCondyleSize.LargeChange = 1000;
            this.rightCondyleSize.Location = new System.Drawing.Point(25, 27);
            this.rightCondyleSize.Maximum = 30000;
            this.rightCondyleSize.Name = "rightCondyleSize";
            this.rightCondyleSize.Size = new System.Drawing.Size(100, 17);
            this.rightCondyleSize.SmallChange = 500;
            this.rightCondyleSize.TabIndex = 11;
            this.rightCondyleSize.Value = 10000;
            // 
            // rightRhemusHeight
            // 
            this.rightRhemusHeight.LargeChange = 1000;
            this.rightRhemusHeight.Location = new System.Drawing.Point(5, 50);
            this.rightRhemusHeight.Maximum = 30000;
            this.rightRhemusHeight.Name = "rightRhemusHeight";
            this.rightRhemusHeight.Size = new System.Drawing.Size(17, 90);
            this.rightRhemusHeight.SmallChange = 500;
            this.rightRhemusHeight.TabIndex = 10;
            this.rightRhemusHeight.Value = 10000;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.BackgroundImage = global::Medical.Properties.Resources.leftcondyleresize;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Location = new System.Drawing.Point(128, 50);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(98, 90);
            this.panel2.TabIndex = 12;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Medical.Properties.Resources.rightcondyleresize;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Location = new System.Drawing.Point(25, 50);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(100, 90);
            this.panel1.TabIndex = 9;
            // 
            // MandibleSizeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(259, 174);
            this.Controls.Add(this.distortionButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Right);
            this.Controls.Add(this.leftCondyleSize);
            this.Controls.Add(this.leftRhemusHeight);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.rightCondyleSize);
            this.Controls.Add(this.rightRhemusHeight);
            this.Controls.Add(this.panel1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Name = "MandibleSizeControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Text = "Mandible Size";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button distortionButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Right;
        private System.Windows.Forms.HScrollBar leftCondyleSize;
        private System.Windows.Forms.VScrollBar leftRhemusHeight;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.HScrollBar rightCondyleSize;
        private System.Windows.Forms.VScrollBar rightRhemusHeight;
        private System.Windows.Forms.Panel panel1;
    }
}