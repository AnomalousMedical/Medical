namespace Medical.GUI
{
    partial class DiscSpacePanel
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
            this.makeNormalButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.undoButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.discOffsetSlider = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.discOffsetSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // makeNormalButton
            // 
            this.makeNormalButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.makeNormalButton.Location = new System.Drawing.Point(100, 69);
            this.makeNormalButton.Name = "makeNormalButton";
            this.makeNormalButton.Size = new System.Drawing.Size(90, 25);
            this.makeNormalButton.TabIndex = 46;
            this.makeNormalButton.Values.Text = "Make Normal";
            // 
            // undoButton
            // 
            this.undoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.undoButton.Location = new System.Drawing.Point(3, 69);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(90, 25);
            this.undoButton.TabIndex = 45;
            this.undoButton.Values.Text = "Undo";
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::Medical.Properties.Resources.DegenerationRightCondyleDistorted;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Location = new System.Drawing.Point(262, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(60, 60);
            this.panel2.TabIndex = 49;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Medical.Properties.Resources.DegenerationRightCondyle;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Location = new System.Drawing.Point(3, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(60, 60);
            this.panel1.TabIndex = 48;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(70, 4);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(65, 19);
            this.kryptonLabel1.TabIndex = 50;
            this.kryptonLabel1.Values.Text = "Disc Space";
            // 
            // discOffsetSlider
            // 
            this.discOffsetSlider.LargeChange = 2000;
            this.discOffsetSlider.Location = new System.Drawing.Point(69, 21);
            this.discOffsetSlider.Maximum = 10000;
            this.discOffsetSlider.Name = "discOffsetSlider";
            this.discOffsetSlider.Size = new System.Drawing.Size(187, 45);
            this.discOffsetSlider.SmallChange = 1000;
            this.discOffsetSlider.TabIndex = 51;
            this.discOffsetSlider.TickFrequency = 10000;
            this.discOffsetSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // DiscSpacePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.discOffsetSlider);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.makeNormalButton);
            this.Controls.Add(this.undoButton);
            this.Name = "DiscSpacePanel";
            this.Size = new System.Drawing.Size(325, 97);
            ((System.ComponentModel.ISupportInitialize)(this.discOffsetSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton makeNormalButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton undoButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private System.Windows.Forms.TrackBar discOffsetSlider;
    }
}
