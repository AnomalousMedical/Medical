namespace Medical.GUI.Grid
{
    partial class GridPropertiesControl
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
            this.kryptonWrapLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.showGridCheckBox = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.gridSpaceControl = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.SuspendLayout();
            // 
            // kryptonWrapLabel4
            // 
            this.kryptonWrapLabel4.AutoSize = false;
            this.kryptonWrapLabel4.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(59)))), ((int)(((byte)(59)))));
            this.kryptonWrapLabel4.Location = new System.Drawing.Point(3, 0);
            this.kryptonWrapLabel4.Name = "kryptonWrapLabel4";
            this.kryptonWrapLabel4.Size = new System.Drawing.Size(279, 17);
            this.kryptonWrapLabel4.Text = "Turn the grid on or off and adjust its spacing.";
            // 
            // showGridCheckBox
            // 
            this.showGridCheckBox.Location = new System.Drawing.Point(4, 20);
            this.showGridCheckBox.Name = "showGridCheckBox";
            this.showGridCheckBox.Size = new System.Drawing.Size(76, 19);
            this.showGridCheckBox.TabIndex = 3;
            this.showGridCheckBox.Values.Text = "Show Grid";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(83, 20);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(51, 19);
            this.kryptonLabel1.TabIndex = 4;
            this.kryptonLabel1.Values.Text = "Spacing";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(215, 21);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(31, 19);
            this.kryptonLabel2.TabIndex = 5;
            this.kryptonLabel2.Values.Text = "MM";
            // 
            // gridSpaceControl
            // 
            this.gridSpaceControl.Location = new System.Drawing.Point(135, 20);
            this.gridSpaceControl.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.gridSpaceControl.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.gridSpaceControl.Name = "gridSpaceControl";
            this.gridSpaceControl.Size = new System.Drawing.Size(76, 22);
            this.gridSpaceControl.TabIndex = 6;
            this.gridSpaceControl.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // GridPropertiesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridSpaceControl);
            this.Controls.Add(this.kryptonLabel2);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.showGridCheckBox);
            this.Controls.Add(this.kryptonWrapLabel4);
            this.Name = "GridPropertiesControl";
            this.Size = new System.Drawing.Size(253, 44);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox showGridCheckBox;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown gridSpaceControl;
    }
}
