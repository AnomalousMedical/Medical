namespace Medical
{
    partial class MovePanel
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
            this.xLoc = new System.Windows.Forms.NumericUpDown();
            this.yLoc = new System.Windows.Forms.NumericUpDown();
            this.zLoc = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.xLoc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yLoc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zLoc)).BeginInit();
            this.SuspendLayout();
            // 
            // xLoc
            // 
            this.xLoc.DecimalPlaces = 4;
            this.xLoc.Location = new System.Drawing.Point(1, 22);
            this.xLoc.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.xLoc.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.xLoc.Name = "xLoc";
            this.xLoc.Size = new System.Drawing.Size(68, 20);
            this.xLoc.TabIndex = 0;
            // 
            // yLoc
            // 
            this.yLoc.DecimalPlaces = 4;
            this.yLoc.Location = new System.Drawing.Point(75, 22);
            this.yLoc.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.yLoc.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.yLoc.Name = "yLoc";
            this.yLoc.Size = new System.Drawing.Size(68, 20);
            this.yLoc.TabIndex = 1;
            // 
            // zLoc
            // 
            this.zLoc.DecimalPlaces = 4;
            this.zLoc.Location = new System.Drawing.Point(149, 22);
            this.zLoc.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.zLoc.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.zLoc.Name = "zLoc";
            this.zLoc.Size = new System.Drawing.Size(68, 20);
            this.zLoc.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "X";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(97, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Y";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(171, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Z";
            // 
            // MovePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(219, 43);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.zLoc);
            this.Controls.Add(this.yLoc);
            this.Controls.Add(this.xLoc);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Name = "MovePanel";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottom;
            this.Text = "Move";
            ((System.ComponentModel.ISupportInitialize)(this.xLoc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yLoc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zLoc)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown xLoc;
        private System.Windows.Forms.NumericUpDown yLoc;
        private System.Windows.Forms.NumericUpDown zLoc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}
