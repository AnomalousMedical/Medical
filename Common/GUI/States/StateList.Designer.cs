namespace Medical.GUI
{
    partial class StateList
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
            this.stateListBox = new ComponentFactory.Krypton.Toolkit.KryptonListBox();
            this.SuspendLayout();
            // 
            // stateListBox
            // 
            this.stateListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stateListBox.Location = new System.Drawing.Point(0, 0);
            this.stateListBox.Name = "stateListBox";
            this.stateListBox.Size = new System.Drawing.Size(236, 466);
            this.stateListBox.TabIndex = 0;
            // 
            // StateList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.stateListBox);
            this.MinimumSize = new System.Drawing.Size(236, 466);
            this.Name = "StateList";
            this.Size = new System.Drawing.Size(236, 466);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonListBox stateListBox;

    }
}
