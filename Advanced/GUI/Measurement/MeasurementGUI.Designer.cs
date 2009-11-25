namespace Medical.GUI
{
    partial class MeasurementGUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MeasurementGUI));
            this.panel1 = new System.Windows.Forms.Panel();
            this.showLines = new System.Windows.Forms.CheckBox();
            this.measurementPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.showLines);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(222, 24);
            this.panel1.TabIndex = 0;
            // 
            // showLines
            // 
            this.showLines.AutoSize = true;
            this.showLines.Location = new System.Drawing.Point(4, 4);
            this.showLines.Name = "showLines";
            this.showLines.Size = new System.Drawing.Size(148, 17);
            this.showLines.TabIndex = 0;
            this.showLines.Text = "Show Measurement Lines";
            this.showLines.UseVisualStyleBackColor = true;
            this.showLines.CheckedChanged += new System.EventHandler(this.showLines_CheckedChanged);
            // 
            // measurementPanel
            // 
            this.measurementPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.measurementPanel.Location = new System.Drawing.Point(0, 24);
            this.measurementPanel.Name = "measurementPanel";
            this.measurementPanel.Size = new System.Drawing.Size(222, 415);
            this.measurementPanel.TabIndex = 1;
            // 
            // MeasurementGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ButtonImageIndex = 11;
            this.ButtonText = "Measurement";
            this.ClientSize = new System.Drawing.Size(222, 439);
            this.Controls.Add(this.measurementPanel);
            this.Controls.Add(this.panel1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MeasurementGUI";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
            this.Text = "Measurement";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox showLines;
        private System.Windows.Forms.FlowLayoutPanel measurementPanel;
    }
}