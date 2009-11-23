namespace Medical.GUI
{
    partial class LeftCondylarDegenrationPanel
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
            this.leftCondyleDegenerationSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftLateralPoleSlider = new Medical.GUI.BoneManipulatorSlider();
            this.leftMedialPoleScaleSlider = new Medical.GUI.BoneManipulatorSlider();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // makeNormalButton
            // 
            this.makeNormalButton.Location = new System.Drawing.Point(100, 200);
            this.makeNormalButton.Name = "makeNormalButton";
            this.makeNormalButton.Size = new System.Drawing.Size(90, 25);
            this.makeNormalButton.TabIndex = 33;
            this.makeNormalButton.Values.Text = "Make Normal";
            this.makeNormalButton.Click += new System.EventHandler(this.makeNormalButton_Click);
            // 
            // undoButton
            // 
            this.undoButton.Location = new System.Drawing.Point(3, 200);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(90, 25);
            this.undoButton.TabIndex = 32;
            this.undoButton.Values.Text = "Undo";
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // leftCondyleDegenerationSlider
            // 
            this.leftCondyleDegenerationSlider.LabelText = "Condyle";
            this.leftCondyleDegenerationSlider.Location = new System.Drawing.Point(64, 11);
            this.leftCondyleDegenerationSlider.Name = "leftCondyleDegenerationSlider";
            this.leftCondyleDegenerationSlider.Size = new System.Drawing.Size(197, 44);
            this.leftCondyleDegenerationSlider.TabIndex = 34;
            this.leftCondyleDegenerationSlider.Tag = "leftCondyleDegenerationMandible";
            this.leftCondyleDegenerationSlider.Value = 0F;
            // 
            // leftLateralPoleSlider
            // 
            this.leftLateralPoleSlider.LabelText = "Lateral Pole";
            this.leftLateralPoleSlider.Location = new System.Drawing.Point(64, 78);
            this.leftLateralPoleSlider.Name = "leftLateralPoleSlider";
            this.leftLateralPoleSlider.Size = new System.Drawing.Size(197, 44);
            this.leftLateralPoleSlider.TabIndex = 35;
            this.leftLateralPoleSlider.Tag = "leftLateralPoleMandible";
            this.leftLateralPoleSlider.Value = 0F;
            // 
            // leftMedialPoleScaleSlider
            // 
            this.leftMedialPoleScaleSlider.LabelText = "Medial Pole";
            this.leftMedialPoleScaleSlider.Location = new System.Drawing.Point(64, 142);
            this.leftMedialPoleScaleSlider.Name = "leftMedialPoleScaleSlider";
            this.leftMedialPoleScaleSlider.Size = new System.Drawing.Size(197, 44);
            this.leftMedialPoleScaleSlider.TabIndex = 36;
            this.leftMedialPoleScaleSlider.Tag = "leftMedialPoleScaleMandible";
            this.leftMedialPoleScaleSlider.Value = 0F;
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = global::Medical.Properties.Resources.DegenerationLeftCondyleMedialPoleDistorted;
            this.panel5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel5.Location = new System.Drawing.Point(262, 134);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(60, 60);
            this.panel5.TabIndex = 31;
            // 
            // panel6
            // 
            this.panel6.BackgroundImage = global::Medical.Properties.Resources.DegenerationLeftCondyleMedialPole;
            this.panel6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel6.Location = new System.Drawing.Point(3, 134);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(60, 60);
            this.panel6.TabIndex = 27;
            // 
            // panel3
            // 
            this.panel3.BackgroundImage = global::Medical.Properties.Resources.DegenerationLeftCondyleLateralPoleDistorted;
            this.panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel3.Location = new System.Drawing.Point(262, 69);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(60, 60);
            this.panel3.TabIndex = 30;
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::Medical.Properties.Resources.DegenerationLeftCondyleDistorted;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Location = new System.Drawing.Point(262, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(60, 60);
            this.panel2.TabIndex = 23;
            // 
            // panel4
            // 
            this.panel4.BackgroundImage = global::Medical.Properties.Resources.DegenerationLeftCondyleLateralPole;
            this.panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel4.Location = new System.Drawing.Point(3, 69);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(60, 60);
            this.panel4.TabIndex = 24;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Medical.Properties.Resources.DegenerationLeftCondyle;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Location = new System.Drawing.Point(3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(60, 60);
            this.panel1.TabIndex = 22;
            // 
            // LeftCondylarDegenrationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.leftCondyleDegenerationSlider);
            this.Controls.Add(this.leftLateralPoleSlider);
            this.Controls.Add(this.leftMedialPoleScaleSlider);
            this.Controls.Add(this.makeNormalButton);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.LayerState = "MandibleSliderSizeLayers";
            this.Name = "LeftCondylarDegenrationPanel";
            this.NavigationState = "DegenerationLeftCameraAngle";
            this.Size = new System.Drawing.Size(325, 229);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton makeNormalButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton undoButton;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel1;
        private BoneManipulatorSlider leftCondyleDegenerationSlider;
        private BoneManipulatorSlider leftLateralPoleSlider;
        private BoneManipulatorSlider leftMedialPoleScaleSlider;
    }
}
