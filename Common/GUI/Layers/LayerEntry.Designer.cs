namespace Medical.GUI.Layers
{
    partial class LayerEntry
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
            this.transparency = new System.Windows.Forms.NumericUpDown();
            this.entryCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.transparency)).BeginInit();
            this.SuspendLayout();
            // 
            // transparency
            // 
            this.transparency.AutoSize = true;
            this.transparency.DecimalPlaces = 1;
            this.transparency.Dock = System.Windows.Forms.DockStyle.Right;
            this.transparency.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.transparency.Location = new System.Drawing.Point(112, 0);
            this.transparency.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.transparency.Name = "transparency";
            this.transparency.Size = new System.Drawing.Size(38, 20);
            this.transparency.TabIndex = 3;
            this.transparency.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // entryCheckBox
            // 
            this.entryCheckBox.AutoSize = true;
            this.entryCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entryCheckBox.Location = new System.Drawing.Point(0, 0);
            this.entryCheckBox.Name = "entryCheckBox";
            this.entryCheckBox.Size = new System.Drawing.Size(112, 20);
            this.entryCheckBox.TabIndex = 4;
            this.entryCheckBox.Text = "Entry Name Text";
            this.entryCheckBox.UseVisualStyleBackColor = true;
            // 
            // LayerEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.entryCheckBox);
            this.Controls.Add(this.transparency);
            this.Name = "LayerEntry";
            this.Size = new System.Drawing.Size(150, 20);
            ((System.ComponentModel.ISupportInitialize)(this.transparency)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown transparency;
        private System.Windows.Forms.CheckBox entryCheckBox;

    }
}
