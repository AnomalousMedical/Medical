namespace Medical.GUI
{
    partial class TeethAdaptationPanel
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
            this.undoButton = new System.Windows.Forms.Button();
            this.resetButton = new System.Windows.Forms.Button();
            this.adaptButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // undoButton
            // 
            this.undoButton.Location = new System.Drawing.Point(5, 34);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(54, 23);
            this.undoButton.TabIndex = 1;
            this.undoButton.Text = "Undo";
            this.undoButton.UseVisualStyleBackColor = true;
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(65, 33);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(96, 23);
            this.resetButton.TabIndex = 2;
            this.resetButton.Text = "Reset Occlusion";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // adaptButton
            // 
            this.adaptButton.Location = new System.Drawing.Point(4, 4);
            this.adaptButton.Name = "adaptButton";
            this.adaptButton.Size = new System.Drawing.Size(75, 23);
            this.adaptButton.TabIndex = 3;
            this.adaptButton.Text = "Adapt";
            this.adaptButton.UseVisualStyleBackColor = true;
            this.adaptButton.Click += new System.EventHandler(this.adaptButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(86, 4);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 4;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // TeethAdaptationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.adaptButton);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.undoButton);
            this.LayerState = "TeethLayers";
            this.Name = "TeethAdaptationPanel";
            this.NavigationState = "Teeth Midline Anterior";
            this.Size = new System.Drawing.Size(318, 286);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button undoButton;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Button adaptButton;
        private System.Windows.Forms.Button stopButton;
    }
}
