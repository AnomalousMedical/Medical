namespace Medical.GUI.Layers
{
    partial class LayerSection
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
            this.headerPanel = new System.Windows.Forms.Panel();
            this.expandButton = new System.Windows.Forms.Button();
            this.groupTransparency = new System.Windows.Forms.NumericUpDown();
            this.categoryLabel = new System.Windows.Forms.Label();
            this.alphaDataGrid = new System.Windows.Forms.DataGridView();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupTransparency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alphaDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.Controls.Add(this.categoryLabel);
            this.headerPanel.Controls.Add(this.groupTransparency);
            this.headerPanel.Controls.Add(this.expandButton);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(192, 21);
            this.headerPanel.TabIndex = 0;
            // 
            // expandButton
            // 
            this.expandButton.AutoSize = true;
            this.expandButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.expandButton.Location = new System.Drawing.Point(0, 0);
            this.expandButton.Name = "expandButton";
            this.expandButton.Size = new System.Drawing.Size(23, 21);
            this.expandButton.TabIndex = 0;
            this.expandButton.Text = "+";
            this.expandButton.UseVisualStyleBackColor = true;
            this.expandButton.Click += new System.EventHandler(this.expandButton_Click);
            // 
            // groupTransparency
            // 
            this.groupTransparency.AutoSize = true;
            this.groupTransparency.DecimalPlaces = 1;
            this.groupTransparency.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupTransparency.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.groupTransparency.Location = new System.Drawing.Point(154, 0);
            this.groupTransparency.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.groupTransparency.Name = "groupTransparency";
            this.groupTransparency.Size = new System.Drawing.Size(38, 20);
            this.groupTransparency.TabIndex = 1;
            // 
            // categoryLabel
            // 
            this.categoryLabel.BackColor = System.Drawing.SystemColors.Window;
            this.categoryLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.categoryLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.categoryLabel.Location = new System.Drawing.Point(23, 0);
            this.categoryLabel.Name = "categoryLabel";
            this.categoryLabel.Size = new System.Drawing.Size(131, 21);
            this.categoryLabel.TabIndex = 2;
            this.categoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // alphaDataGrid
            // 
            this.alphaDataGrid.AllowUserToAddRows = false;
            this.alphaDataGrid.AllowUserToDeleteRows = false;
            this.alphaDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.alphaDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alphaDataGrid.Location = new System.Drawing.Point(0, 21);
            this.alphaDataGrid.Name = "alphaDataGrid";
            this.alphaDataGrid.Size = new System.Drawing.Size(192, 0);
            this.alphaDataGrid.TabIndex = 1;
            // 
            // LayerSection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.alphaDataGrid);
            this.Controls.Add(this.headerPanel);
            this.Name = "LayerSection";
            this.Size = new System.Drawing.Size(192, 21);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupTransparency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alphaDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label categoryLabel;
        private System.Windows.Forms.NumericUpDown groupTransparency;
        private System.Windows.Forms.Button expandButton;
        private System.Windows.Forms.DataGridView alphaDataGrid;


    }
}
