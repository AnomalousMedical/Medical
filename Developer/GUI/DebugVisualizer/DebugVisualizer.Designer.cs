namespace Medical.GUI
{
    partial class DebugVisualizer
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
            this.enableAllCheckBox = new System.Windows.Forms.CheckBox();
            this.visualListBox = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // enableAllCheckBox
            // 
            this.enableAllCheckBox.AutoSize = true;
            this.enableAllCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.enableAllCheckBox.Location = new System.Drawing.Point(0, 0);
            this.enableAllCheckBox.Name = "enableAllCheckBox";
            this.enableAllCheckBox.Size = new System.Drawing.Size(184, 17);
            this.enableAllCheckBox.TabIndex = 0;
            this.enableAllCheckBox.Text = "Enable Debug Visuals";
            this.enableAllCheckBox.UseVisualStyleBackColor = true;
            // 
            // visualListBox
            // 
            this.visualListBox.CheckOnClick = true;
            this.visualListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.visualListBox.FormattingEnabled = true;
            this.visualListBox.Location = new System.Drawing.Point(0, 17);
            this.visualListBox.Name = "visualListBox";
            this.visualListBox.Size = new System.Drawing.Size(184, 424);
            this.visualListBox.TabIndex = 1;
            // 
            // DebugVisualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.visualListBox);
            this.Controls.Add(this.enableAllCheckBox);
            this.DockAreas = Medical.GUI.DockLocations.Right;
            this.Name = "DebugVisualizer";
            this.ShowHint = Medical.GUI.DockLocations.Right;
            this.Size = new System.Drawing.Size(184, 442);
            this.ToolStripName = "Debug Visualizers";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox enableAllCheckBox;
        private System.Windows.Forms.CheckedListBox visualListBox;
    }
}