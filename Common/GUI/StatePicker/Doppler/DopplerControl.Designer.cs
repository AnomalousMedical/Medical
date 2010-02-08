namespace Medical.GUI
{
    partial class DopplerControl
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
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.rotatoryCombo = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.translatoryCombo = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.clickCombo = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.stageIButton = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.stageIIButton = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.stageIIIaButton = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.stageIIIbButton = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.stageIVaButton = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.stageIVbButton = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.stageVaButton = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.stageVbButton = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.kryptonWrapLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.kryptonWrapLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.kryptonGroupBox1 = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.rdaReductionGroupBox = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.severeRDAReductionButton = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.mildRDAReductionButton = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.moderateRDAReductionButton = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.resetButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.rotatoryCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.translatoryCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.clickCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1.Panel)).BeginInit();
            this.kryptonGroupBox1.Panel.SuspendLayout();
            this.kryptonGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rdaReductionGroupBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdaReductionGroupBox.Panel)).BeginInit();
            this.rdaReductionGroupBox.Panel.SuspendLayout();
            this.rdaReductionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(4, 18);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(100, 19);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "Rotatory Crepitus";
            // 
            // rotatoryCombo
            // 
            this.rotatoryCombo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.rotatoryCombo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.rotatoryCombo.DropDownWidth = 179;
            this.rotatoryCombo.Items.AddRange(new object[] {
            "None",
            "Mild",
            "Moderate",
            "Coarse Rough",
            "Coarse Eburnated",
            "Unknown"});
            this.rotatoryCombo.Location = new System.Drawing.Point(4, 37);
            this.rotatoryCombo.Name = "rotatoryCombo";
            this.rotatoryCombo.Size = new System.Drawing.Size(179, 21);
            this.rotatoryCombo.TabIndex = 1;
            this.rotatoryCombo.Text = "Unknown";
            // 
            // translatoryCombo
            // 
            this.translatoryCombo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.translatoryCombo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.translatoryCombo.DropDownWidth = 179;
            this.translatoryCombo.Items.AddRange(new object[] {
            "None",
            "Mild",
            "Moderate",
            "Coarse Rough",
            "Coarse Eburnated",
            "Unknown"});
            this.translatoryCombo.Location = new System.Drawing.Point(4, 83);
            this.translatoryCombo.Name = "translatoryCombo";
            this.translatoryCombo.Size = new System.Drawing.Size(179, 21);
            this.translatoryCombo.TabIndex = 3;
            this.translatoryCombo.Text = "Unknown";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(4, 64);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(113, 19);
            this.kryptonLabel2.TabIndex = 2;
            this.kryptonLabel2.Values.Text = "Translatory Crepitus";
            // 
            // clickCombo
            // 
            this.clickCombo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.clickCombo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.clickCombo.DropDownWidth = 179;
            this.clickCombo.Items.AddRange(new object[] {
            "None",
            "Reciprocal",
            "Surface",
            "Unknown"});
            this.clickCombo.Location = new System.Drawing.Point(4, 129);
            this.clickCombo.Name = "clickCombo";
            this.clickCombo.Size = new System.Drawing.Size(179, 21);
            this.clickCombo.TabIndex = 5;
            this.clickCombo.Text = "Unknown";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(4, 110);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(34, 19);
            this.kryptonLabel3.TabIndex = 4;
            this.kryptonLabel3.Values.Text = "Click";
            // 
            // stageIButton
            // 
            this.stageIButton.Checked = true;
            this.stageIButton.Location = new System.Drawing.Point(6, 3);
            this.stageIButton.Name = "stageIButton";
            this.stageIButton.Size = new System.Drawing.Size(25, 19);
            this.stageIButton.TabIndex = 7;
            this.stageIButton.Values.Text = "I";
            // 
            // stageIIButton
            // 
            this.stageIIButton.Location = new System.Drawing.Point(6, 28);
            this.stageIIButton.Name = "stageIIButton";
            this.stageIIButton.Size = new System.Drawing.Size(28, 19);
            this.stageIIButton.TabIndex = 8;
            this.stageIIButton.Values.Text = "II";
            // 
            // stageIIIaButton
            // 
            this.stageIIIaButton.Location = new System.Drawing.Point(6, 53);
            this.stageIIIaButton.Name = "stageIIIaButton";
            this.stageIIIaButton.Size = new System.Drawing.Size(38, 19);
            this.stageIIIaButton.TabIndex = 9;
            this.stageIIIaButton.Values.Text = "IIIa";
            // 
            // stageIIIbButton
            // 
            this.stageIIIbButton.Location = new System.Drawing.Point(6, 78);
            this.stageIIIbButton.Name = "stageIIIbButton";
            this.stageIIIbButton.Size = new System.Drawing.Size(38, 19);
            this.stageIIIbButton.TabIndex = 10;
            this.stageIIIbButton.Values.Text = "IIIb";
            // 
            // stageIVaButton
            // 
            this.stageIVaButton.Location = new System.Drawing.Point(6, 103);
            this.stageIVaButton.Name = "stageIVaButton";
            this.stageIVaButton.Size = new System.Drawing.Size(39, 19);
            this.stageIVaButton.TabIndex = 11;
            this.stageIVaButton.Values.Text = "IVa";
            // 
            // stageIVbButton
            // 
            this.stageIVbButton.Location = new System.Drawing.Point(6, 128);
            this.stageIVbButton.Name = "stageIVbButton";
            this.stageIVbButton.Size = new System.Drawing.Size(39, 19);
            this.stageIVbButton.TabIndex = 12;
            this.stageIVbButton.Values.Text = "IVb";
            // 
            // stageVaButton
            // 
            this.stageVaButton.Location = new System.Drawing.Point(6, 153);
            this.stageVaButton.Name = "stageVaButton";
            this.stageVaButton.Size = new System.Drawing.Size(35, 19);
            this.stageVaButton.TabIndex = 13;
            this.stageVaButton.Values.Text = "Va";
            // 
            // stageVbButton
            // 
            this.stageVbButton.Location = new System.Drawing.Point(6, 178);
            this.stageVbButton.Name = "stageVbButton";
            this.stageVbButton.Size = new System.Drawing.Size(36, 19);
            this.stageVbButton.TabIndex = 14;
            this.stageVbButton.Values.Text = "Vb";
            // 
            // kryptonWrapLabel1
            // 
            this.kryptonWrapLabel1.AutoSize = false;
            this.kryptonWrapLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonWrapLabel1.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(59)))), ((int)(((byte)(59)))));
            this.kryptonWrapLabel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonWrapLabel1.Name = "kryptonWrapLabel1";
            this.kryptonWrapLabel1.Size = new System.Drawing.Size(189, 19);
            this.kryptonWrapLabel1.Text = "Input Joint Sounds";
            // 
            // kryptonWrapLabel2
            // 
            this.kryptonWrapLabel2.AutoSize = false;
            this.kryptonWrapLabel2.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(59)))), ((int)(((byte)(59)))));
            this.kryptonWrapLabel2.Location = new System.Drawing.Point(0, 156);
            this.kryptonWrapLabel2.Name = "kryptonWrapLabel2";
            this.kryptonWrapLabel2.Size = new System.Drawing.Size(189, 18);
            this.kryptonWrapLabel2.Text = "Piper Classification Stage";
            // 
            // kryptonGroupBox1
            // 
            this.kryptonGroupBox1.Location = new System.Drawing.Point(4, 173);
            this.kryptonGroupBox1.Name = "kryptonGroupBox1";
            // 
            // kryptonGroupBox1.Panel
            // 
            this.kryptonGroupBox1.Panel.Controls.Add(this.rdaReductionGroupBox);
            this.kryptonGroupBox1.Panel.Controls.Add(this.stageIButton);
            this.kryptonGroupBox1.Panel.Controls.Add(this.stageIIButton);
            this.kryptonGroupBox1.Panel.Controls.Add(this.stageVbButton);
            this.kryptonGroupBox1.Panel.Controls.Add(this.stageIIIaButton);
            this.kryptonGroupBox1.Panel.Controls.Add(this.stageVaButton);
            this.kryptonGroupBox1.Panel.Controls.Add(this.stageIIIbButton);
            this.kryptonGroupBox1.Panel.Controls.Add(this.stageIVbButton);
            this.kryptonGroupBox1.Panel.Controls.Add(this.stageIVaButton);
            this.kryptonGroupBox1.Size = new System.Drawing.Size(179, 223);
            this.kryptonGroupBox1.TabIndex = 16;
            this.kryptonGroupBox1.Values.Heading = "Stage";
            // 
            // rdaReductionGroupBox
            // 
            this.rdaReductionGroupBox.Location = new System.Drawing.Point(68, 78);
            this.rdaReductionGroupBox.Name = "rdaReductionGroupBox";
            // 
            // rdaReductionGroupBox.Panel
            // 
            this.rdaReductionGroupBox.Panel.Controls.Add(this.severeRDAReductionButton);
            this.rdaReductionGroupBox.Panel.Controls.Add(this.mildRDAReductionButton);
            this.rdaReductionGroupBox.Panel.Controls.Add(this.moderateRDAReductionButton);
            this.rdaReductionGroupBox.Size = new System.Drawing.Size(104, 94);
            this.rdaReductionGroupBox.TabIndex = 15;
            this.rdaReductionGroupBox.Values.Heading = "RDA Reduction";
            // 
            // severeRDAReductionButton
            // 
            this.severeRDAReductionButton.Location = new System.Drawing.Point(3, 53);
            this.severeRDAReductionButton.Name = "severeRDAReductionButton";
            this.severeRDAReductionButton.Size = new System.Drawing.Size(56, 19);
            this.severeRDAReductionButton.TabIndex = 18;
            this.severeRDAReductionButton.Values.Text = "Severe";
            // 
            // mildRDAReductionButton
            // 
            this.mildRDAReductionButton.Location = new System.Drawing.Point(3, 3);
            this.mildRDAReductionButton.Name = "mildRDAReductionButton";
            this.mildRDAReductionButton.Size = new System.Drawing.Size(45, 19);
            this.mildRDAReductionButton.TabIndex = 16;
            this.mildRDAReductionButton.Values.Text = "Mild";
            // 
            // moderateRDAReductionButton
            // 
            this.moderateRDAReductionButton.Location = new System.Drawing.Point(3, 28);
            this.moderateRDAReductionButton.Name = "moderateRDAReductionButton";
            this.moderateRDAReductionButton.Size = new System.Drawing.Size(73, 19);
            this.moderateRDAReductionButton.TabIndex = 17;
            this.moderateRDAReductionButton.Values.Text = "Moderate";
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(4, 402);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(90, 25);
            this.resetButton.TabIndex = 19;
            this.resetButton.Values.Text = "Reset";
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // DopplerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.kryptonGroupBox1);
            this.Controls.Add(this.kryptonWrapLabel2);
            this.Controls.Add(this.clickCombo);
            this.Controls.Add(this.kryptonLabel3);
            this.Controls.Add(this.translatoryCombo);
            this.Controls.Add(this.kryptonLabel2);
            this.Controls.Add(this.rotatoryCombo);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.kryptonWrapLabel1);
            this.Name = "DopplerControl";
            this.Size = new System.Drawing.Size(189, 432);
            ((System.ComponentModel.ISupportInitialize)(this.rotatoryCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.translatoryCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.clickCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1.Panel)).EndInit();
            this.kryptonGroupBox1.Panel.ResumeLayout(false);
            this.kryptonGroupBox1.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonGroupBox1)).EndInit();
            this.kryptonGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rdaReductionGroupBox.Panel)).EndInit();
            this.rdaReductionGroupBox.Panel.ResumeLayout(false);
            this.rdaReductionGroupBox.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rdaReductionGroupBox)).EndInit();
            this.rdaReductionGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox rotatoryCombo;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox translatoryCombo;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox clickCombo;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton stageIButton;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton stageIIButton;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton stageIIIaButton;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton stageIIIbButton;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton stageIVaButton;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton stageIVbButton;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton stageVaButton;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton stageVbButton;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox kryptonGroupBox1;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton severeRDAReductionButton;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton mildRDAReductionButton;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton moderateRDAReductionButton;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox rdaReductionGroupBox;
        private ComponentFactory.Krypton.Toolkit.KryptonButton resetButton;
    }
}
