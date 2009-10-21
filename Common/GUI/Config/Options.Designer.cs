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
            this.components = new System.ComponentModel.Container();
            this.graphicsGroup = new System.Windows.Forms.GroupBox();
            this.vsyncCheck = new System.Windows.Forms.CheckBox();
            this.antiAliasingCombo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.applyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.graphicsGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // graphicsGroup
            // 
            this.graphicsGroup.Controls.Add(this.vsyncCheck);
            this.graphicsGroup.Controls.Add(this.antiAliasingCombo);
            this.graphicsGroup.Controls.Add(this.label2);
            this.graphicsGroup.Location = new System.Drawing.Point(2, 2);
            this.graphicsGroup.Name = "graphicsGroup";
            this.graphicsGroup.Size = new System.Drawing.Size(211, 66);
            this.graphicsGroup.TabIndex = 4;
            this.graphicsGroup.TabStop = false;
            this.graphicsGroup.Text = "Graphics";
            // 
            // vsyncCheck
            // 
            this.vsyncCheck.AutoSize = true;
            this.vsyncCheck.Location = new System.Drawing.Point(9, 44);
            this.vsyncCheck.Name = "vsyncCheck";
            this.vsyncCheck.Size = new System.Drawing.Size(88, 17);
            this.vsyncCheck.TabIndex = 10;
            this.vsyncCheck.Text = "Vertical Sync";
            this.vsyncCheck.UseVisualStyleBackColor = true;
            // 
            // antiAliasingCombo
            // 
            this.antiAliasingCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.antiAliasingCombo.FormattingEnabled = true;
            this.antiAliasingCombo.Location = new System.Drawing.Point(76, 17);
            this.antiAliasingCombo.Name = "antiAliasingCombo";
            this.antiAliasingCombo.Size = new System.Drawing.Size(127, 21);
            this.antiAliasingCombo.TabIndex = 9;
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
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(28, 74);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 5;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(110, 74);
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
            this.ClientSize = new System.Drawing.Size(216, 106);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.graphicsGroup);
            this.Name = "Options";
            this.Text = "Options";
            this.graphicsGroup.ResumeLayout(false);
            this.graphicsGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox graphicsGroup;
        private System.Windows.Forms.ComboBox antiAliasingCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox vsyncCheck;
        private System.Windows.Forms.ToolTip tooltip;


    }
}