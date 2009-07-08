namespace Medical.GUI
{
    partial class PictureControl
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.sizeGroup = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.resolutionWidth = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.resolutionHeight = new System.Windows.Forms.NumericUpDown();
            this.renderSingleButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.sizeGroup.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resolutionWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resolutionHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.sizeGroup);
            this.flowLayoutPanel1.Controls.Add(this.renderSingleButton);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(141, 139);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // sizeGroup
            // 
            this.sizeGroup.Controls.Add(this.flowLayoutPanel2);
            this.sizeGroup.Location = new System.Drawing.Point(3, 3);
            this.sizeGroup.Name = "sizeGroup";
            this.sizeGroup.Size = new System.Drawing.Size(134, 101);
            this.sizeGroup.TabIndex = 4;
            this.sizeGroup.TabStop = false;
            this.sizeGroup.Text = "Size";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.label1);
            this.flowLayoutPanel2.Controls.Add(this.resolutionWidth);
            this.flowLayoutPanel2.Controls.Add(this.label2);
            this.flowLayoutPanel2.Controls.Add(this.resolutionHeight);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(128, 82);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Width";
            // 
            // resolutionWidth
            // 
            this.resolutionWidth.Location = new System.Drawing.Point(3, 16);
            this.resolutionWidth.Maximum = new decimal(new int[] {
            8000,
            0,
            0,
            0});
            this.resolutionWidth.Name = "resolutionWidth";
            this.resolutionWidth.Size = new System.Drawing.Size(120, 20);
            this.resolutionWidth.TabIndex = 12;
            this.resolutionWidth.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Height";
            // 
            // resolutionHeight
            // 
            this.resolutionHeight.Location = new System.Drawing.Point(3, 55);
            this.resolutionHeight.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.resolutionHeight.Name = "resolutionHeight";
            this.resolutionHeight.Size = new System.Drawing.Size(120, 20);
            this.resolutionHeight.TabIndex = 14;
            this.resolutionHeight.Value = new decimal(new int[] {
            768,
            0,
            0,
            0});
            // 
            // renderSingleButton
            // 
            this.renderSingleButton.Location = new System.Drawing.Point(3, 110);
            this.renderSingleButton.Name = "renderSingleButton";
            this.renderSingleButton.Size = new System.Drawing.Size(90, 23);
            this.renderSingleButton.TabIndex = 7;
            this.renderSingleButton.Text = "Render";
            this.renderSingleButton.UseVisualStyleBackColor = true;
            this.renderSingleButton.Click += new System.EventHandler(this.renderSingleButton_Click);
            // 
            // PictureControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(142, 142);
            this.Controls.Add(this.flowLayoutPanel1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Name = "PictureControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
            this.Text = "Picture";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.sizeGroup.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.resolutionWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resolutionHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox sizeGroup;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown resolutionWidth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown resolutionHeight;
        private System.Windows.Forms.Button renderSingleButton;



    }
}
