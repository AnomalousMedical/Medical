using Medical.GUI;
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.depthCheckCheck = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // enableAllCheckBox
            // 
            this.enableAllCheckBox.AutoSize = true;
            this.enableAllCheckBox.Location = new System.Drawing.Point(3, 3);
            this.enableAllCheckBox.Name = "enableAllCheckBox";
            this.enableAllCheckBox.Size = new System.Drawing.Size(130, 17);
            this.enableAllCheckBox.TabIndex = 0;
            this.enableAllCheckBox.Text = "Enable Debug Visuals";
            this.enableAllCheckBox.UseVisualStyleBackColor = true;
            // 
            // visualListBox
            // 
            this.visualListBox.CheckOnClick = true;
            this.visualListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.visualListBox.FormattingEnabled = true;
            this.visualListBox.Location = new System.Drawing.Point(0, 39);
            this.visualListBox.Name = "visualListBox";
            this.visualListBox.Size = new System.Drawing.Size(184, 379);
            this.visualListBox.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.depthCheckCheck);
            this.panel1.Controls.Add(this.enableAllCheckBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(184, 39);
            this.panel1.TabIndex = 2;
            // 
            // depthCheckCheck
            // 
            this.depthCheckCheck.AutoSize = true;
            this.depthCheckCheck.Location = new System.Drawing.Point(3, 22);
            this.depthCheckCheck.Name = "depthCheckCheck";
            this.depthCheckCheck.Size = new System.Drawing.Size(125, 17);
            this.depthCheckCheck.TabIndex = 1;
            this.depthCheckCheck.Text = "Enable Depth Check";
            this.depthCheckCheck.UseVisualStyleBackColor = true;
            this.depthCheckCheck.CheckedChanged += new System.EventHandler(this.depthCheckCheck_CheckedChanged);
            // 
            // DebugVisualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.visualListBox);
            this.Controls.Add(this.panel1);
            this.DockAreas = ((Medical.GUI.DockLocations)((Medical.GUI.DockLocations.Right | Medical.GUI.DockLocations.Float)));
            this.Name = "DebugVisualizer";
            this.ShowHint = Medical.GUI.DockLocations.Right;
            this.Size = new System.Drawing.Size(184, 418);
            this.ToolStripName = "Debug Visualizers";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox enableAllCheckBox;
        private System.Windows.Forms.CheckedListBox visualListBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox depthCheckCheck;
    }
}