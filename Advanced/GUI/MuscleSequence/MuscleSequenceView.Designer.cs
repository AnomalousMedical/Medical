namespace Medical.GUI
{
    partial class MuscleSequenceView
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
            this.muscleStateList = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // muscleStateList
            // 
            this.muscleStateList.BackColor = System.Drawing.SystemColors.Window;
            this.muscleStateList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.muscleStateList.ForeColor = System.Drawing.SystemColors.WindowText;
            this.muscleStateList.HideSelection = false;
            this.muscleStateList.Location = new System.Drawing.Point(0, 0);
            this.muscleStateList.MultiSelect = false;
            this.muscleStateList.Name = "muscleStateList";
            this.muscleStateList.Size = new System.Drawing.Size(150, 150);
            this.muscleStateList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.muscleStateList.TabIndex = 14;
            this.muscleStateList.TileSize = new System.Drawing.Size(100, 100);
            this.muscleStateList.UseCompatibleStateImageBehavior = false;
            this.muscleStateList.View = System.Windows.Forms.View.Details;
            // 
            // MuscleSequenceView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.muscleStateList);
            this.Name = "MuscleSequenceView";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView muscleStateList;
    }
}
