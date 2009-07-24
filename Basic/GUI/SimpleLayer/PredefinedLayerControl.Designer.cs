namespace Medical.GUI
{
    partial class PredefinedLayerControl
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
            this.skinButton = new System.Windows.Forms.RadioButton();
            this.transparentSkinButton = new System.Windows.Forms.RadioButton();
            this.bonesButton = new System.Windows.Forms.RadioButton();
            this.fossaCutAwayButton = new System.Windows.Forms.RadioButton();
            this.transparentBonesButton = new System.Windows.Forms.RadioButton();
            this.teethButton = new System.Windows.Forms.RadioButton();
            this.topTeethButton = new System.Windows.Forms.RadioButton();
            this.bottomTeethButton = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // skinButton
            // 
            this.skinButton.AutoSize = true;
            this.skinButton.Location = new System.Drawing.Point(6, 7);
            this.skinButton.Name = "skinButton";
            this.skinButton.Size = new System.Drawing.Size(46, 17);
            this.skinButton.TabIndex = 0;
            this.skinButton.TabStop = true;
            this.skinButton.Text = "Skin";
            this.skinButton.UseVisualStyleBackColor = true;
            this.skinButton.CheckedChanged += new System.EventHandler(this.skinButton_CheckedChanged);
            // 
            // transparentSkinButton
            // 
            this.transparentSkinButton.AutoSize = true;
            this.transparentSkinButton.Location = new System.Drawing.Point(6, 31);
            this.transparentSkinButton.Name = "transparentSkinButton";
            this.transparentSkinButton.Size = new System.Drawing.Size(106, 17);
            this.transparentSkinButton.TabIndex = 1;
            this.transparentSkinButton.TabStop = true;
            this.transparentSkinButton.Text = "Transparent Skin";
            this.transparentSkinButton.UseVisualStyleBackColor = true;
            this.transparentSkinButton.CheckedChanged += new System.EventHandler(this.transparentSkinButton_CheckedChanged);
            // 
            // bonesButton
            // 
            this.bonesButton.AutoSize = true;
            this.bonesButton.Location = new System.Drawing.Point(6, 55);
            this.bonesButton.Name = "bonesButton";
            this.bonesButton.Size = new System.Drawing.Size(55, 17);
            this.bonesButton.TabIndex = 2;
            this.bonesButton.TabStop = true;
            this.bonesButton.Text = "Bones";
            this.bonesButton.UseVisualStyleBackColor = true;
            this.bonesButton.CheckedChanged += new System.EventHandler(this.bonesButton_CheckedChanged);
            // 
            // fossaCutAwayButton
            // 
            this.fossaCutAwayButton.AutoSize = true;
            this.fossaCutAwayButton.Location = new System.Drawing.Point(6, 79);
            this.fossaCutAwayButton.Name = "fossaCutAwayButton";
            this.fossaCutAwayButton.Size = new System.Drawing.Size(101, 17);
            this.fossaCutAwayButton.TabIndex = 3;
            this.fossaCutAwayButton.TabStop = true;
            this.fossaCutAwayButton.Text = "Fossa Cut Away";
            this.fossaCutAwayButton.UseVisualStyleBackColor = true;
            this.fossaCutAwayButton.CheckedChanged += new System.EventHandler(this.fossaCutAwayButton_CheckedChanged);
            // 
            // transparentBonesButton
            // 
            this.transparentBonesButton.AutoSize = true;
            this.transparentBonesButton.Location = new System.Drawing.Point(6, 103);
            this.transparentBonesButton.Name = "transparentBonesButton";
            this.transparentBonesButton.Size = new System.Drawing.Size(115, 17);
            this.transparentBonesButton.TabIndex = 4;
            this.transparentBonesButton.TabStop = true;
            this.transparentBonesButton.Text = "Transparent Bones";
            this.transparentBonesButton.UseVisualStyleBackColor = true;
            this.transparentBonesButton.CheckedChanged += new System.EventHandler(this.transparentBonesButton_CheckedChanged);
            // 
            // teethButton
            // 
            this.teethButton.AutoSize = true;
            this.teethButton.Location = new System.Drawing.Point(6, 127);
            this.teethButton.Name = "teethButton";
            this.teethButton.Size = new System.Drawing.Size(77, 17);
            this.teethButton.TabIndex = 5;
            this.teethButton.TabStop = true;
            this.teethButton.Text = "Teeth Only";
            this.teethButton.UseVisualStyleBackColor = true;
            this.teethButton.CheckedChanged += new System.EventHandler(this.teethButton_CheckedChanged);
            // 
            // topTeethButton
            // 
            this.topTeethButton.AutoSize = true;
            this.topTeethButton.Location = new System.Drawing.Point(6, 151);
            this.topTeethButton.Name = "topTeethButton";
            this.topTeethButton.Size = new System.Drawing.Size(75, 17);
            this.topTeethButton.TabIndex = 6;
            this.topTeethButton.TabStop = true;
            this.topTeethButton.Text = "Top Teeth";
            this.topTeethButton.UseVisualStyleBackColor = true;
            this.topTeethButton.CheckedChanged += new System.EventHandler(this.topTeethButton_CheckedChanged);
            // 
            // bottomTeethButton
            // 
            this.bottomTeethButton.AutoSize = true;
            this.bottomTeethButton.Location = new System.Drawing.Point(6, 175);
            this.bottomTeethButton.Name = "bottomTeethButton";
            this.bottomTeethButton.Size = new System.Drawing.Size(86, 17);
            this.bottomTeethButton.TabIndex = 7;
            this.bottomTeethButton.TabStop = true;
            this.bottomTeethButton.Text = "BottomTeeth";
            this.bottomTeethButton.UseVisualStyleBackColor = true;
            this.bottomTeethButton.CheckedChanged += new System.EventHandler(this.bottomTeethButton_CheckedChanged);
            // 
            // PredefinedLayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(157, 264);
            this.Controls.Add(this.bottomTeethButton);
            this.Controls.Add(this.topTeethButton);
            this.Controls.Add(this.teethButton);
            this.Controls.Add(this.transparentBonesButton);
            this.Controls.Add(this.fossaCutAwayButton);
            this.Controls.Add(this.bonesButton);
            this.Controls.Add(this.transparentSkinButton);
            this.Controls.Add(this.skinButton);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Name = "PredefinedLayerControl";
            this.Text = "Predefined Layers";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton skinButton;
        private System.Windows.Forms.RadioButton transparentSkinButton;
        private System.Windows.Forms.RadioButton bonesButton;
        private System.Windows.Forms.RadioButton fossaCutAwayButton;
        private System.Windows.Forms.RadioButton transparentBonesButton;
        private System.Windows.Forms.RadioButton teethButton;
        private System.Windows.Forms.RadioButton topTeethButton;
        private System.Windows.Forms.RadioButton bottomTeethButton;
    }
}