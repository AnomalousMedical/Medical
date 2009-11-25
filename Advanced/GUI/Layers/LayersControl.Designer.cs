namespace Medical.GUI
{
    partial class LayersControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayersControl));
            this.sectionsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // sectionsPanel
            // 
            this.sectionsPanel.AutoSize = true;
            this.sectionsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.sectionsPanel.Location = new System.Drawing.Point(0, 0);
            this.sectionsPanel.Name = "sectionsPanel";
            this.sectionsPanel.Size = new System.Drawing.Size(192, 94);
            this.sectionsPanel.TabIndex = 0;
            // 
            // LayersControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ButtonText = "Layers";
            this.ClientSize = new System.Drawing.Size(209, 95);
            this.Controls.Add(this.sectionsPanel);
            this.DockAreas = ((Medical.GUI.DockLocations)(((Medical.GUI.DockLocations.Float | Medical.GUI.DockLocations.Left)
                        | Medical.GUI.DockLocations.Right)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LayersControl";
            this.ShortcutKey = System.Windows.Forms.Keys.L;
            this.ShowHint = Medical.GUI.DockLocations.Right;
            this.Text = "Layers";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel sectionsPanel;
    }
}
