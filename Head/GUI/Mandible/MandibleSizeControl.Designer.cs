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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.distortionButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Right = new System.Windows.Forms.Label();
            this.leftCondyleSize = new System.Windows.Forms.HScrollBar();
            this.leftRhemusHeight = new System.Windows.Forms.VScrollBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rightCondyleSize = new System.Windows.Forms.HScrollBar();
            this.rightRhemusHeight = new System.Windows.Forms.VScrollBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.layoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(259, 233);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.distortionButton);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.Right);
            this.tabPage1.Controls.Add(this.leftCondyleSize);
            this.tabPage1.Controls.Add(this.leftRhemusHeight);
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.rightCondyleSize);
            this.tabPage1.Controls.Add(this.rightRhemusHeight);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(251, 207);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Basic";
            // 
            // distortionButton
            // 
            this.distortionButton.Location = new System.Drawing.Point(62, 145);
            this.distortionButton.Name = "distortionButton";
            this.distortionButton.Size = new System.Drawing.Size(137, 23);
            this.distortionButton.TabIndex = 26;
            this.distortionButton.Text = "Make Normal";
            this.distortionButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(174, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Left";
            // 
            // Right
            // 
            this.Right.AutoSize = true;
            this.Right.Location = new System.Drawing.Point(59, 8);
            this.Right.Name = "Right";
            this.Right.Size = new System.Drawing.Size(32, 13);
            this.Right.TabIndex = 24;
            this.Right.Text = "Right";
            // 
            // leftCondyleSize
            // 
            this.leftCondyleSize.LargeChange = 1000;
            this.leftCondyleSize.Location = new System.Drawing.Point(131, 27);
            this.leftCondyleSize.Maximum = 30000;
            this.leftCondyleSize.Name = "leftCondyleSize";
            this.leftCondyleSize.Size = new System.Drawing.Size(95, 17);
            this.leftCondyleSize.SmallChange = 500;
            this.leftCondyleSize.TabIndex = 23;
            this.leftCondyleSize.Value = 10000;
            // 
            // leftRhemusHeight
            // 
            this.leftRhemusHeight.LargeChange = 1000;
            this.leftRhemusHeight.Location = new System.Drawing.Point(229, 49);
            this.leftRhemusHeight.Maximum = 30000;
            this.leftRhemusHeight.Name = "leftRhemusHeight";
            this.leftRhemusHeight.Size = new System.Drawing.Size(17, 90);
            this.leftRhemusHeight.SmallChange = 500;
            this.leftRhemusHeight.TabIndex = 22;
            this.leftRhemusHeight.Value = 10000;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.BackgroundImage = global::Medical.Properties.Resources.leftcondyleresize;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Location = new System.Drawing.Point(128, 49);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(98, 90);
            this.panel2.TabIndex = 21;
            // 
            // rightCondyleSize
            // 
            this.rightCondyleSize.LargeChange = 1000;
            this.rightCondyleSize.Location = new System.Drawing.Point(25, 26);
            this.rightCondyleSize.Maximum = 30000;
            this.rightCondyleSize.Name = "rightCondyleSize";
            this.rightCondyleSize.Size = new System.Drawing.Size(100, 17);
            this.rightCondyleSize.SmallChange = 500;
            this.rightCondyleSize.TabIndex = 20;
            this.rightCondyleSize.Value = 10000;
            // 
            // rightRhemusHeight
            // 
            this.rightRhemusHeight.LargeChange = 1000;
            this.rightRhemusHeight.Location = new System.Drawing.Point(5, 49);
            this.rightRhemusHeight.Maximum = 30000;
            this.rightRhemusHeight.Name = "rightRhemusHeight";
            this.rightRhemusHeight.Size = new System.Drawing.Size(17, 90);
            this.rightRhemusHeight.SmallChange = 500;
            this.rightRhemusHeight.TabIndex = 19;
            this.rightRhemusHeight.Value = 10000;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Medical.Properties.Resources.rightcondyleresize;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Location = new System.Drawing.Point(25, 49);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(100, 90);
            this.panel1.TabIndex = 18;
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.layoutPanel);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(251, 207);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Advanced";
            // 
            // layoutPanel
            // 
            this.layoutPanel.AutoSize = true;
            this.layoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.layoutPanel.Location = new System.Drawing.Point(7, 7);
            this.layoutPanel.Name = "layoutPanel";
            this.layoutPanel.Size = new System.Drawing.Size(212, 100);
            this.layoutPanel.TabIndex = 0;
            // 
            // MandibleSizeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(259, 233);
            this.Controls.Add(this.tabControl1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Name = "MandibleSizeControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Text = "Mandible Size";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button distortionButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Right;
        private System.Windows.Forms.HScrollBar leftCondyleSize;
        private System.Windows.Forms.VScrollBar leftRhemusHeight;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.HScrollBar rightCondyleSize;
        private System.Windows.Forms.VScrollBar rightRhemusHeight;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.FlowLayoutPanel layoutPanel;

    }
}