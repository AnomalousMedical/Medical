namespace Medical.GUI
{
    partial class MandibleControlSlider
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
            this.amountTrackBar = new Medical.GUI.TimeTrackBar();
            this.SuspendLayout();
            // 
            // amountTrackBar
            // 
            this.amountTrackBar.BarMenu = null;
            this.amountTrackBar.ChangeTimeOnSelection = false;
            this.amountTrackBar.CurrentTime = 0F;
            this.amountTrackBar.Location = new System.Drawing.Point(0, -19);
            this.amountTrackBar.MaximumTime = 0F;
            this.amountTrackBar.MoveMarks = false;
            this.amountTrackBar.MoveThumb = true;
            this.amountTrackBar.Name = "amountTrackBar";
            this.amountTrackBar.SelectedMark = null;
            this.amountTrackBar.Size = new System.Drawing.Size(79, 47);
            this.amountTrackBar.TabIndex = 0;
            this.amountTrackBar.TickMenu = null;
            // 
            // MandibleControlSlider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.amountTrackBar);
            this.Name = "MandibleControlSlider";
            this.Size = new System.Drawing.Size(180, 66);
            this.ResumeLayout(false);

        }

        #endregion

        private TimeTrackBar amountTrackBar;


    }
}
