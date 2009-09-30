namespace Medical.GUI
{
    partial class TeethAdaptationPanel
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
            this.adaptButton = new System.Windows.Forms.CheckBox();
            this.resetButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // adaptButton
            // 
            this.adaptButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.adaptButton.AutoSize = true;
            this.adaptButton.Location = new System.Drawing.Point(5, 5);
            this.adaptButton.Name = "adaptButton";
            this.adaptButton.Size = new System.Drawing.Size(45, 23);
            this.adaptButton.TabIndex = 0;
            this.adaptButton.Text = "Adapt";
            this.adaptButton.UseVisualStyleBackColor = true;
            this.adaptButton.CheckedChanged += new System.EventHandler(this.adaptButton_CheckedChanged);
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(56, 5);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(45, 23);
            this.resetButton.TabIndex = 1;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // TeethAdaptationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.adaptButton);
            this.Name = "TeethAdaptationPanel";
            this.Size = new System.Drawing.Size(318, 286);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox adaptButton;
        private System.Windows.Forms.Button resetButton;
    }
}
