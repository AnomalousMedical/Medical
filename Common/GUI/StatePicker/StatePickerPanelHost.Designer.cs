namespace Medical.GUI
{
    partial class StatePickerPanelHost
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
            this.finishButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.nextButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.previousButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.cancelButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.panelHost = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            ((System.ComponentModel.ISupportInitialize)(this.panelHost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // finishButton
            // 
            this.finishButton.Location = new System.Drawing.Point(247, 3);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 2;
            this.finishButton.Values.Text = "Finish";
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(166, 3);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(75, 23);
            this.nextButton.TabIndex = 0;
            this.nextButton.Values.Text = "Next";
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // previousButton
            // 
            this.previousButton.Location = new System.Drawing.Point(85, 3);
            this.previousButton.Name = "previousButton";
            this.previousButton.Size = new System.Drawing.Size(75, 23);
            this.previousButton.TabIndex = 1;
            this.previousButton.Values.Text = "Previous";
            this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(4, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Values.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // panelHost
            // 
            this.panelHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHost.Location = new System.Drawing.Point(0, 29);
            this.panelHost.Name = "panelHost";
            this.panelHost.Size = new System.Drawing.Size(325, 480);
            this.panelHost.TabIndex = 3;
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.finishButton);
            this.kryptonPanel1.Controls.Add(this.nextButton);
            this.kryptonPanel1.Controls.Add(this.previousButton);
            this.kryptonPanel1.Controls.Add(this.cancelButton);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(325, 29);
            this.kryptonPanel1.TabIndex = 4;
            // 
            // StatePickerPanelHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelHost);
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "StatePickerPanelHost";
            this.Size = new System.Drawing.Size(325, 509);
            ((System.ComponentModel.ISupportInitialize)(this.panelHost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton cancelButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton finishButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton nextButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton previousButton;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel panelHost;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
    }
}