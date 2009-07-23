namespace Medical.GUI
{
    partial class Options
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
            this.graphicsGroup = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fpsUpDown = new System.Windows.Forms.NumericUpDown();
            this.frameCapButton = new System.Windows.Forms.RadioButton();
            this.vsyncButton = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.antiAliasingCombo = new System.Windows.Forms.ComboBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.graphicsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpsUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // graphicsGroup
            // 
            this.graphicsGroup.Controls.Add(this.antiAliasingCombo);
            this.graphicsGroup.Controls.Add(this.label2);
            this.graphicsGroup.Controls.Add(this.label1);
            this.graphicsGroup.Controls.Add(this.fpsUpDown);
            this.graphicsGroup.Controls.Add(this.frameCapButton);
            this.graphicsGroup.Controls.Add(this.vsyncButton);
            this.graphicsGroup.Location = new System.Drawing.Point(2, 2);
            this.graphicsGroup.Name = "graphicsGroup";
            this.graphicsGroup.Size = new System.Drawing.Size(211, 95);
            this.graphicsGroup.TabIndex = 4;
            this.graphicsGroup.TabStop = false;
            this.graphicsGroup.Text = "Graphics";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(183, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Hz";
            // 
            // fpsUpDown
            // 
            this.fpsUpDown.Location = new System.Drawing.Point(107, 68);
            this.fpsUpDown.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.fpsUpDown.Name = "fpsUpDown";
            this.fpsUpDown.Size = new System.Drawing.Size(70, 20);
            this.fpsUpDown.TabIndex = 6;
            this.fpsUpDown.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // frameCapButton
            // 
            this.frameCapButton.AutoSize = true;
            this.frameCapButton.Location = new System.Drawing.Point(6, 68);
            this.frameCapButton.Name = "frameCapButton";
            this.frameCapButton.Size = new System.Drawing.Size(94, 17);
            this.frameCapButton.TabIndex = 5;
            this.frameCapButton.Text = "Framerate Cap";
            this.frameCapButton.UseVisualStyleBackColor = true;
            // 
            // vsyncButton
            // 
            this.vsyncButton.AutoSize = true;
            this.vsyncButton.Location = new System.Drawing.Point(6, 44);
            this.vsyncButton.Name = "vsyncButton";
            this.vsyncButton.Size = new System.Drawing.Size(87, 17);
            this.vsyncButton.TabIndex = 4;
            this.vsyncButton.Text = "Vertical Sync";
            this.vsyncButton.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Anti Aliasing";
            // 
            // antiAliasingCombo
            // 
            this.antiAliasingCombo.FormattingEnabled = true;
            this.antiAliasingCombo.Location = new System.Drawing.Point(76, 17);
            this.antiAliasingCombo.Name = "antiAliasingCombo";
            this.antiAliasingCombo.Size = new System.Drawing.Size(127, 21);
            this.antiAliasingCombo.TabIndex = 9;
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(27, 200);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 5;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(109, 200);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(216, 235);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.graphicsGroup);
            this.Name = "Options";
            this.Text = "Options";
            this.graphicsGroup.ResumeLayout(false);
            this.graphicsGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpsUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox graphicsGroup;
        private System.Windows.Forms.ComboBox antiAliasingCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown fpsUpDown;
        private System.Windows.Forms.RadioButton frameCapButton;
        private System.Windows.Forms.RadioButton vsyncButton;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button cancelButton;


    }
}