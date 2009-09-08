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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PictureControl));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.megapixel1dot3 = new System.Windows.Forms.RadioButton();
            this.megapixel4 = new System.Windows.Forms.RadioButton();
            this.megapixel6 = new System.Windows.Forms.RadioButton();
            this.megapixel8 = new System.Windows.Forms.RadioButton();
            this.megapixel10 = new System.Windows.Forms.RadioButton();
            this.megapixel12 = new System.Windows.Forms.RadioButton();
            this.custom = new System.Windows.Forms.RadioButton();
            this.sizeGroup = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.resolutionWidth = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.resolutionHeight = new System.Windows.Forms.NumericUpDown();
            this.renderSingleButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.sizeGroup.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resolutionWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resolutionHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Controls.Add(this.sizeGroup);
            this.flowLayoutPanel1.Controls.Add(this.renderSingleButton);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(141, 322);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flowLayoutPanel3);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(134, 180);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Size";
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.megapixel1dot3);
            this.flowLayoutPanel3.Controls.Add(this.megapixel4);
            this.flowLayoutPanel3.Controls.Add(this.megapixel6);
            this.flowLayoutPanel3.Controls.Add(this.megapixel8);
            this.flowLayoutPanel3.Controls.Add(this.megapixel10);
            this.flowLayoutPanel3.Controls.Add(this.megapixel12);
            this.flowLayoutPanel3.Controls.Add(this.custom);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(128, 161);
            this.flowLayoutPanel3.TabIndex = 0;
            // 
            // megapixel1dot3
            // 
            this.megapixel1dot3.AutoSize = true;
            this.megapixel1dot3.Location = new System.Drawing.Point(3, 3);
            this.megapixel1dot3.Name = "megapixel1dot3";
            this.megapixel1dot3.Size = new System.Drawing.Size(91, 17);
            this.megapixel1dot3.TabIndex = 0;
            this.megapixel1dot3.Text = "1.3 Megapixel";
            this.megapixel1dot3.UseVisualStyleBackColor = true;
            this.megapixel1dot3.CheckedChanged += new System.EventHandler(this.megapixel1dot3_CheckedChanged);
            // 
            // megapixel4
            // 
            this.megapixel4.AutoSize = true;
            this.megapixel4.Location = new System.Drawing.Point(3, 26);
            this.megapixel4.Name = "megapixel4";
            this.megapixel4.Size = new System.Drawing.Size(82, 17);
            this.megapixel4.TabIndex = 1;
            this.megapixel4.Text = "4 Megapixel";
            this.megapixel4.UseVisualStyleBackColor = true;
            this.megapixel4.CheckedChanged += new System.EventHandler(this.megapixel4_CheckedChanged);
            // 
            // megapixel6
            // 
            this.megapixel6.AutoSize = true;
            this.megapixel6.Location = new System.Drawing.Point(3, 49);
            this.megapixel6.Name = "megapixel6";
            this.megapixel6.Size = new System.Drawing.Size(82, 17);
            this.megapixel6.TabIndex = 2;
            this.megapixel6.Text = "6 Megapixel";
            this.megapixel6.UseVisualStyleBackColor = true;
            this.megapixel6.CheckedChanged += new System.EventHandler(this.megapixel6_CheckedChanged);
            // 
            // megapixel8
            // 
            this.megapixel8.AutoSize = true;
            this.megapixel8.Location = new System.Drawing.Point(3, 72);
            this.megapixel8.Name = "megapixel8";
            this.megapixel8.Size = new System.Drawing.Size(82, 17);
            this.megapixel8.TabIndex = 3;
            this.megapixel8.Text = "8 Megapixel";
            this.megapixel8.UseVisualStyleBackColor = true;
            this.megapixel8.CheckedChanged += new System.EventHandler(this.megapixel8_CheckedChanged);
            // 
            // megapixel10
            // 
            this.megapixel10.AutoSize = true;
            this.megapixel10.Location = new System.Drawing.Point(3, 95);
            this.megapixel10.Name = "megapixel10";
            this.megapixel10.Size = new System.Drawing.Size(88, 17);
            this.megapixel10.TabIndex = 4;
            this.megapixel10.Text = "10 Megapixel";
            this.megapixel10.UseVisualStyleBackColor = true;
            this.megapixel10.CheckedChanged += new System.EventHandler(this.megapixel10_CheckedChanged);
            // 
            // megapixel12
            // 
            this.megapixel12.AutoSize = true;
            this.megapixel12.Location = new System.Drawing.Point(3, 118);
            this.megapixel12.Name = "megapixel12";
            this.megapixel12.Size = new System.Drawing.Size(88, 17);
            this.megapixel12.TabIndex = 5;
            this.megapixel12.Text = "12 Megapixel";
            this.megapixel12.UseVisualStyleBackColor = true;
            this.megapixel12.CheckedChanged += new System.EventHandler(this.megapixel12_CheckedChanged);
            // 
            // custom
            // 
            this.custom.AutoSize = true;
            this.custom.Checked = true;
            this.custom.Location = new System.Drawing.Point(3, 141);
            this.custom.Name = "custom";
            this.custom.Size = new System.Drawing.Size(60, 17);
            this.custom.TabIndex = 6;
            this.custom.TabStop = true;
            this.custom.Text = "Custom";
            this.custom.UseVisualStyleBackColor = true;
            this.custom.CheckedChanged += new System.EventHandler(this.custom_CheckedChanged);
            // 
            // sizeGroup
            // 
            this.sizeGroup.Controls.Add(this.flowLayoutPanel2);
            this.sizeGroup.Location = new System.Drawing.Point(3, 189);
            this.sizeGroup.Name = "sizeGroup";
            this.sizeGroup.Size = new System.Drawing.Size(134, 101);
            this.sizeGroup.TabIndex = 4;
            this.sizeGroup.TabStop = false;
            this.sizeGroup.Text = "Custom Size";
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
            8192,
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
            8192,
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
            this.renderSingleButton.Location = new System.Drawing.Point(3, 296);
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
            this.ClientSize = new System.Drawing.Size(142, 322);
            this.Controls.Add(this.flowLayoutPanel1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PictureControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
            this.Text = "Picture";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.RadioButton megapixel1dot3;
        private System.Windows.Forms.RadioButton megapixel4;
        private System.Windows.Forms.RadioButton megapixel6;
        private System.Windows.Forms.RadioButton megapixel8;
        private System.Windows.Forms.RadioButton megapixel10;
        private System.Windows.Forms.RadioButton megapixel12;
        private System.Windows.Forms.RadioButton custom;



    }
}
