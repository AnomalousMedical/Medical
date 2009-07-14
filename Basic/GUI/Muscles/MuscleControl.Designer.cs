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
            this.closeButton = new System.Windows.Forms.Button();
            this.openButton = new System.Windows.Forms.Button();
            this.clenchButton = new System.Windows.Forms.Button();
            this.neutralButton = new System.Windows.Forms.Button();
            this.resetTeethButton = new System.Windows.Forms.Button();
            this.translateBackward = new System.Windows.Forms.Button();
            this.translateForward = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Image = global::Medical.Properties.Resources.neutralmuscle;
            this.closeButton.Location = new System.Drawing.Point(121, 4);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(111, 140);
            this.closeButton.TabIndex = 9;
            this.closeButton.Text = "Close";
            this.closeButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // openButton
            // 
            this.openButton.Image = global::Medical.Properties.Resources.openmuscle;
            this.openButton.Location = new System.Drawing.Point(4, 4);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(111, 140);
            this.openButton.TabIndex = 8;
            this.openButton.Text = "Open";
            this.openButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // clenchButton
            // 
            this.clenchButton.Image = global::Medical.Properties.Resources.clenchedmuscle;
            this.clenchButton.Location = new System.Drawing.Point(12, 447);
            this.clenchButton.Name = "clenchButton";
            this.clenchButton.Size = new System.Drawing.Size(111, 140);
            this.clenchButton.TabIndex = 7;
            this.clenchButton.Text = "Clench and Adapt";
            this.clenchButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.clenchButton.UseVisualStyleBackColor = true;
            this.clenchButton.Click += new System.EventHandler(this.clenchButton_Click);
            // 
            // neutralButton
            // 
            this.neutralButton.Image = global::Medical.Properties.Resources.neutralmuscle;
            this.neutralButton.Location = new System.Drawing.Point(4, 150);
            this.neutralButton.Name = "neutralButton";
            this.neutralButton.Size = new System.Drawing.Size(111, 140);
            this.neutralButton.TabIndex = 6;
            this.neutralButton.Text = "Neutral";
            this.neutralButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.neutralButton.UseVisualStyleBackColor = true;
            this.neutralButton.Click += new System.EventHandler(this.neutralButton_Click);
            // 
            // resetTeethButton
            // 
            this.resetTeethButton.Image = global::Medical.Properties.Resources.neutralmuscle;
            this.resetTeethButton.Location = new System.Drawing.Point(129, 447);
            this.resetTeethButton.Name = "resetTeethButton";
            this.resetTeethButton.Size = new System.Drawing.Size(111, 140);
            this.resetTeethButton.TabIndex = 10;
            this.resetTeethButton.Text = "Reset Teeth";
            this.resetTeethButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.resetTeethButton.UseVisualStyleBackColor = true;
            this.resetTeethButton.Click += new System.EventHandler(this.resetTeethButton_Click);
            // 
            // translateBackward
            // 
            this.translateBackward.Image = global::Medical.Properties.Resources.neutralmuscle;
            this.translateBackward.Location = new System.Drawing.Point(129, 296);
            this.translateBackward.Name = "translateBackward";
            this.translateBackward.Size = new System.Drawing.Size(111, 140);
            this.translateBackward.TabIndex = 12;
            this.translateBackward.Text = "Translate Backward";
            this.translateBackward.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.translateBackward.UseVisualStyleBackColor = true;
            this.translateBackward.Click += new System.EventHandler(this.translateBackward_Click);
            // 
            // translateForward
            // 
            this.translateForward.Image = global::Medical.Properties.Resources.openmuscle;
            this.translateForward.Location = new System.Drawing.Point(12, 296);
            this.translateForward.Name = "translateForward";
            this.translateForward.Size = new System.Drawing.Size(111, 140);
            this.translateForward.TabIndex = 11;
            this.translateForward.Text = "Translate Forward";
            this.translateForward.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.translateForward.UseVisualStyleBackColor = true;
            this.translateForward.Click += new System.EventHandler(this.translateForward_Click);
            // 
            // MuscleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 599);
            this.Controls.Add(this.translateBackward);
            this.Controls.Add(this.translateForward);
            this.Controls.Add(this.resetTeethButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.clenchButton);
            this.Controls.Add(this.neutralButton);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Name = "MuscleControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Text = "Muscles";
            this.ToolStripName = "Advanced";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Button clenchButton;
        private System.Windows.Forms.Button neutralButton;
        private System.Windows.Forms.Button resetTeethButton;
        private System.Windows.Forms.Button translateBackward;
        private System.Windows.Forms.Button translateForward;

    }
}
