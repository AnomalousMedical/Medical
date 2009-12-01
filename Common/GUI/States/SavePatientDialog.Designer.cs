namespace Medical.GUI
{
    partial class SavePatientDialog
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
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.cancelButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.saveButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.lastText = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.firstText = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.cancelButton);
            this.kryptonPanel1.Controls.Add(this.saveButton);
            this.kryptonPanel1.Controls.Add(this.lastText);
            this.kryptonPanel1.Controls.Add(this.firstText);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(201, 126);
            this.kryptonPanel1.TabIndex = 6;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(104, 95);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(90, 25);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Values.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(8, 95);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(90, 25);
            this.saveButton.TabIndex = 4;
            this.saveButton.Values.Text = "Save";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // lastText
            // 
            this.lastText.Location = new System.Drawing.Point(7, 69);
            this.lastText.Name = "lastText";
            this.lastText.Size = new System.Drawing.Size(189, 20);
            this.lastText.TabIndex = 3;
            // 
            // firstText
            // 
            this.firstText.Location = new System.Drawing.Point(7, 25);
            this.firstText.Name = "firstText";
            this.firstText.Size = new System.Drawing.Size(189, 20);
            this.firstText.TabIndex = 2;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(3, 47);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(31, 19);
            this.kryptonLabel2.TabIndex = 1;
            this.kryptonLabel2.Values.Text = "Last";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(3, 3);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(32, 19);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "First";
            // 
            // SavePatientDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(201, 126);
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "SavePatientDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Save";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox lastText;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox firstText;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton cancelButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton saveButton;
    }
}