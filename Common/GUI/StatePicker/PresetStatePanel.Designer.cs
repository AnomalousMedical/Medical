namespace Medical.GUI
{
    partial class PresetStatePanel
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
            clearImages();
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
            this.presetListView = new System.Windows.Forms.ListView();
            this.NameColumn = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // presetListView
            // 
            this.presetListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumn});
            this.presetListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.presetListView.HideSelection = false;
            this.presetListView.Location = new System.Drawing.Point(0, 0);
            this.presetListView.MultiSelect = false;
            this.presetListView.Name = "presetListView";
            this.presetListView.Size = new System.Drawing.Size(150, 150);
            this.presetListView.TabIndex = 0;
            this.presetListView.UseCompatibleStateImageBehavior = false;
            // 
            // NameColumn
            // 
            this.NameColumn.Text = "Name";
            this.NameColumn.Width = 150;
            // 
            // PresetStatePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.presetListView);
            this.Name = "PresetStatePanel";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView presetListView;
        private System.Windows.Forms.ColumnHeader NameColumn;
    }
}
