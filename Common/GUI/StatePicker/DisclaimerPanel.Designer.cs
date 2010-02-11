namespace Medical.GUI
{
    partial class DisclaimerPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisclaimerPanel));
            this.disclaimer = new ComponentFactory.Krypton.Toolkit.KryptonRichTextBox();
            this.kryptonBorderEdge3 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge2 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge1 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonWrapLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonWrapLabel();
            this.SuspendLayout();
            // 
            // disclaimer
            // 
            this.disclaimer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.disclaimer.Location = new System.Drawing.Point(5, 52);
            this.disclaimer.Name = "disclaimer";
            this.disclaimer.ReadOnly = true;
            this.disclaimer.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.disclaimer.Size = new System.Drawing.Size(263, 546);
            this.disclaimer.TabIndex = 8;
            this.disclaimer.Text = resources.GetString("disclaimer.Text");
            // 
            // kryptonBorderEdge3
            // 
            this.kryptonBorderEdge3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge3.Location = new System.Drawing.Point(5, 598);
            this.kryptonBorderEdge3.Name = "kryptonBorderEdge3";
            this.kryptonBorderEdge3.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge3.Size = new System.Drawing.Size(263, 2);
            this.kryptonBorderEdge3.StateCommon.Color1 = System.Drawing.Color.Transparent;
            this.kryptonBorderEdge3.StateCommon.Color2 = System.Drawing.Color.Transparent;
            this.kryptonBorderEdge3.Text = "kryptonBorderEdge3";
            // 
            // kryptonBorderEdge2
            // 
            this.kryptonBorderEdge2.Dock = System.Windows.Forms.DockStyle.Right;
            this.kryptonBorderEdge2.Location = new System.Drawing.Point(268, 0);
            this.kryptonBorderEdge2.Name = "kryptonBorderEdge2";
            this.kryptonBorderEdge2.Size = new System.Drawing.Size(5, 600);
            this.kryptonBorderEdge2.StateCommon.Color1 = System.Drawing.Color.Transparent;
            this.kryptonBorderEdge2.StateCommon.Color2 = System.Drawing.Color.Transparent;
            this.kryptonBorderEdge2.Text = "kryptonBorderEdge2";
            // 
            // kryptonBorderEdge1
            // 
            this.kryptonBorderEdge1.Dock = System.Windows.Forms.DockStyle.Left;
            this.kryptonBorderEdge1.Location = new System.Drawing.Point(0, 0);
            this.kryptonBorderEdge1.Name = "kryptonBorderEdge1";
            this.kryptonBorderEdge1.Size = new System.Drawing.Size(5, 600);
            this.kryptonBorderEdge1.StateCommon.Color1 = System.Drawing.Color.Transparent;
            this.kryptonBorderEdge1.StateCommon.Color2 = System.Drawing.Color.Transparent;
            this.kryptonBorderEdge1.Text = "kryptonBorderEdge1";
            // 
            // kryptonWrapLabel1
            // 
            this.kryptonWrapLabel1.AutoSize = false;
            this.kryptonWrapLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonWrapLabel1.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.kryptonWrapLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            this.kryptonWrapLabel1.Location = new System.Drawing.Point(5, 0);
            this.kryptonWrapLabel1.Name = "kryptonWrapLabel1";
            this.kryptonWrapLabel1.Size = new System.Drawing.Size(263, 52);
            this.kryptonWrapLabel1.Text = "Clicking the finish button for this wizard means you agree to the following state" +
                "ment. If you do not click cancel now.";
            // 
            // DisclaimerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.disclaimer);
            this.Controls.Add(this.kryptonBorderEdge3);
            this.Controls.Add(this.kryptonWrapLabel1);
            this.Controls.Add(this.kryptonBorderEdge2);
            this.Controls.Add(this.kryptonBorderEdge1);
            this.LargeIcon = global::Medical.Properties.Resources.DisclaimerIcon;
            this.Name = "DisclaimerPanel";
            this.Size = new System.Drawing.Size(273, 600);
            this.TextLine1 = "Disclaimer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonRichTextBox disclaimer;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge3;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge2;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge1;
        private ComponentFactory.Krypton.Toolkit.KryptonWrapLabel kryptonWrapLabel1;


    }
}
