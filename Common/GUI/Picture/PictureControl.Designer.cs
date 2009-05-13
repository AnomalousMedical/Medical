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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.aspectWidth = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.aspectHeight = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.sizeGroup = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.resolutionWidth = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.resolutionHeight = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.directoryText = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.prefixText = new System.Windows.Forms.TextBox();
            this.renderSequenceButton = new System.Windows.Forms.Button();
            this.renderSingleButton = new System.Windows.Forms.Button();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aspectWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aspectHeight)).BeginInit();
            this.sizeGroup.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resolutionWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resolutionHeight)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.groupBox2);
            this.flowLayoutPanel1.Controls.Add(this.sizeGroup);
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Controls.Add(this.renderSequenceButton);
            this.flowLayoutPanel1.Controls.Add(this.renderSingleButton);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(141, 405);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.flowLayoutPanel3);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(134, 128);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Aspect Ratio";
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.label3);
            this.flowLayoutPanel3.Controls.Add(this.aspectWidth);
            this.flowLayoutPanel3.Controls.Add(this.label4);
            this.flowLayoutPanel3.Controls.Add(this.aspectHeight);
            this.flowLayoutPanel3.Controls.Add(this.button1);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(128, 109);
            this.flowLayoutPanel3.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Width";
            // 
            // aspectWidth
            // 
            this.aspectWidth.Location = new System.Drawing.Point(3, 16);
            this.aspectWidth.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.aspectWidth.Name = "aspectWidth";
            this.aspectWidth.Size = new System.Drawing.Size(120, 20);
            this.aspectWidth.TabIndex = 1;
            this.aspectWidth.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Height";
            // 
            // aspectHeight
            // 
            this.aspectHeight.Location = new System.Drawing.Point(3, 55);
            this.aspectHeight.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.aspectHeight.Name = "aspectHeight";
            this.aspectHeight.Size = new System.Drawing.Size(120, 20);
            this.aspectHeight.TabIndex = 3;
            this.aspectHeight.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 81);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Lock Aspect Ratio";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // sizeGroup
            // 
            this.sizeGroup.Controls.Add(this.flowLayoutPanel2);
            this.sizeGroup.Location = new System.Drawing.Point(3, 137);
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flowLayoutPanel4);
            this.groupBox1.Location = new System.Drawing.Point(3, 244);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(131, 99);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sequence";
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Controls.Add(this.label5);
            this.flowLayoutPanel4.Controls.Add(this.directoryText);
            this.flowLayoutPanel4.Controls.Add(this.browseButton);
            this.flowLayoutPanel4.Controls.Add(this.label6);
            this.flowLayoutPanel4.Controls.Add(this.prefixText);
            this.flowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel4.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(125, 80);
            this.flowLayoutPanel4.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Output Directory";
            // 
            // directoryText
            // 
            this.directoryText.Location = new System.Drawing.Point(3, 16);
            this.directoryText.Name = "directoryText";
            this.directoryText.Size = new System.Drawing.Size(84, 20);
            this.directoryText.TabIndex = 1;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(93, 16);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(25, 20);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Prefix";
            // 
            // prefixText
            // 
            this.prefixText.Location = new System.Drawing.Point(3, 55);
            this.prefixText.Name = "prefixText";
            this.prefixText.Size = new System.Drawing.Size(115, 20);
            this.prefixText.TabIndex = 4;
            // 
            // renderSequenceButton
            // 
            this.renderSequenceButton.AutoSize = true;
            this.renderSequenceButton.Location = new System.Drawing.Point(3, 349);
            this.renderSequenceButton.Name = "renderSequenceButton";
            this.renderSequenceButton.Size = new System.Drawing.Size(104, 23);
            this.renderSequenceButton.TabIndex = 6;
            this.renderSequenceButton.Text = "Render Sequence";
            this.renderSequenceButton.UseVisualStyleBackColor = true;
            // 
            // renderSingleButton
            // 
            this.renderSingleButton.Location = new System.Drawing.Point(3, 378);
            this.renderSingleButton.Name = "renderSingleButton";
            this.renderSingleButton.Size = new System.Drawing.Size(90, 23);
            this.renderSingleButton.TabIndex = 7;
            this.renderSingleButton.Text = "Render Single";
            this.renderSingleButton.UseVisualStyleBackColor = true;
            // 
            // PictureControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(142, 406);
            this.Controls.Add(this.flowLayoutPanel1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Name = "PictureControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
            this.Text = "Picture";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aspectWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aspectHeight)).EndInit();
            this.sizeGroup.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.resolutionWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resolutionHeight)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown aspectWidth;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox sizeGroup;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown resolutionWidth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown resolutionHeight;
        private System.Windows.Forms.NumericUpDown aspectHeight;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox directoryText;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox prefixText;
        private System.Windows.Forms.Button renderSequenceButton;
        private System.Windows.Forms.Button renderSingleButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;



    }
}
