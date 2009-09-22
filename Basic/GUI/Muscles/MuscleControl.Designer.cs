namespace Medical.GUI
{
    partial class MuscleControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MuscleControl));
            this.muscleSequenceView = new Medical.GUI.MuscleSequenceView();
            this.SuspendLayout();
            // 
            // muscleSequenceView
            // 
            this.muscleSequenceView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.muscleSequenceView.LargeImageList = null;
            this.muscleSequenceView.Location = new System.Drawing.Point(0, 0);
            this.muscleSequenceView.Name = "muscleSequenceView";
            this.muscleSequenceView.Size = new System.Drawing.Size(236, 599);
            this.muscleSequenceView.TabIndex = 13;
            // 
            // MuscleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(236, 599);
            this.Controls.Add(this.muscleSequenceView);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MuscleControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Text = "Muscles";
            this.ToolStripName = "Advanced";
            this.ResumeLayout(false);

        }

        #endregion

        private MuscleSequenceView muscleSequenceView;

    }
}
