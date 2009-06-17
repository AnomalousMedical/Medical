namespace Medical.GUI
{
    partial class MedicalStateGUI
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
            this.medicalStateTrackBar = new Medical.GUI.MedicalStateTrackBar();
            this.SuspendLayout();
            // 
            // addStateButton
            // 
            this.addStateButton.Location = new System.Drawing.Point(13, 39);
            this.addStateButton.Name = "addStateButton";
            this.addStateButton.Size = new System.Drawing.Size(75, 23);
            this.addStateButton.TabIndex = 1;
            this.addStateButton.Text = "Add State";
            this.addStateButton.UseVisualStyleBackColor = true;
            this.addStateButton.Click += new System.EventHandler(this.addStateButton_Click);
            // 
            // medicalStateTrackBar
            // 
            this.medicalStateTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.medicalStateTrackBar.CurrentBlend = 0;
            this.medicalStateTrackBar.Location = new System.Drawing.Point(1, -15);
            this.medicalStateTrackBar.MaxBlend = 60;
            this.medicalStateTrackBar.Name = "medicalStateTrackBar";
            this.medicalStateTrackBar.Size = new System.Drawing.Size(489, 47);
            this.medicalStateTrackBar.TabIndex = 0;
            // 
            // MedicalStateGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 125);
            this.Controls.Add(this.addStateButton);
            this.Controls.Add(this.medicalStateTrackBar);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Name = "MedicalStateGUI";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottom;
            this.Text = "States";
            this.ResumeLayout(false);

        }

        #endregion

        private MedicalStateTrackBar medicalStateTrackBar;
        private System.Windows.Forms.Button addStateButton;
    }
}