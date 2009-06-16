namespace Medical.GUI
{
    partial class BlendEditor
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
            this.addStateButton = new System.Windows.Forms.Button();
            this.keyFrameTrackBar = new Medical.GUI.Animation.KeyFrameTrackBar();
            this.SuspendLayout();
            // 
            // addStateButton
            // 
            this.addStateButton.Location = new System.Drawing.Point(1, 27);
            this.addStateButton.Name = "addStateButton";
            this.addStateButton.Size = new System.Drawing.Size(75, 23);
            this.addStateButton.TabIndex = 0;
            this.addStateButton.Text = "Add State";
            this.addStateButton.UseVisualStyleBackColor = true;
            this.addStateButton.Click += new System.EventHandler(this.addStateButton_Click);
            // 
            // keyFrameTrackBar
            // 
            this.keyFrameTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.keyFrameTrackBar.CurrentTickPosition = 0;
            this.keyFrameTrackBar.Location = new System.Drawing.Point(0, -12);
            this.keyFrameTrackBar.Name = "keyFrameTrackBar";
            this.keyFrameTrackBar.Size = new System.Drawing.Size(528, 39);
            this.keyFrameTrackBar.TabIndex = 1;
            // 
            // BlendEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(532, 141);
            this.Controls.Add(this.keyFrameTrackBar);
            this.Controls.Add(this.addStateButton);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Name = "BlendEditor";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockTop;
            this.Text = "Blend";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button addStateButton;
        private Medical.GUI.Animation.KeyFrameTrackBar keyFrameTrackBar;

    }
}