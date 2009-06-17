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
            this.playAllButton = new System.Windows.Forms.Button();
            this.pauseButton = new System.Windows.Forms.Button();
            this.previousButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.medicalStateTrackBar = new Medical.GUI.MedicalStateTrackBar();
            this.SuspendLayout();
            // 
            // addStateButton
            // 
            this.addStateButton.Location = new System.Drawing.Point(415, 27);
            this.addStateButton.Name = "addStateButton";
            this.addStateButton.Size = new System.Drawing.Size(75, 23);
            this.addStateButton.TabIndex = 1;
            this.addStateButton.Text = "Add State";
            this.addStateButton.UseVisualStyleBackColor = true;
            this.addStateButton.Click += new System.EventHandler(this.addStateButton_Click);
            // 
            // playAllButton
            // 
            this.playAllButton.Location = new System.Drawing.Point(82, 27);
            this.playAllButton.Name = "playAllButton";
            this.playAllButton.Size = new System.Drawing.Size(75, 23);
            this.playAllButton.TabIndex = 2;
            this.playAllButton.Text = "Play All";
            this.playAllButton.UseVisualStyleBackColor = true;
            this.playAllButton.Click += new System.EventHandler(this.playAllButton_Click);
            // 
            // pauseButton
            // 
            this.pauseButton.Location = new System.Drawing.Point(164, 27);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(75, 23);
            this.pauseButton.TabIndex = 3;
            this.pauseButton.Text = "Pause";
            this.pauseButton.UseVisualStyleBackColor = true;
            this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
            // 
            // previousButton
            // 
            this.previousButton.Location = new System.Drawing.Point(1, 27);
            this.previousButton.Name = "previousButton";
            this.previousButton.Size = new System.Drawing.Size(75, 23);
            this.previousButton.TabIndex = 4;
            this.previousButton.Text = "Previous";
            this.previousButton.UseVisualStyleBackColor = true;
            this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(246, 27);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(75, 23);
            this.nextButton.TabIndex = 5;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
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
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.previousButton);
            this.Controls.Add(this.pauseButton);
            this.Controls.Add(this.playAllButton);
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
        private System.Windows.Forms.Button playAllButton;
        private System.Windows.Forms.Button pauseButton;
        private System.Windows.Forms.Button previousButton;
        private System.Windows.Forms.Button nextButton;
    }
}