namespace Medical.GUI
{
    partial class SimpleLayerControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleLayerControl));
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.skinSlider = new Medical.GUI.LayerSlider();
            this.muscleSlider = new Medical.GUI.LayerSlider();
            this.tmjSlider = new Medical.GUI.LayerSlider();
            this.panel1 = new System.Windows.Forms.Panel();
            this.showEminence = new System.Windows.Forms.CheckBox();
            this.skullSlider = new Medical.GUI.LayerSlider();
            this.mandibleSlider = new Medical.GUI.LayerSlider();
            this.hyoidSlider = new Medical.GUI.LayerSlider();
            this.spineSlider = new Medical.GUI.LayerSlider();
            this.topTeethSlider = new Medical.GUI.LayerSlider();
            this.bottomTeethSlider = new Medical.GUI.LayerSlider();
            this.flowLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.skinSlider);
            this.flowLayoutPanel2.Controls.Add(this.muscleSlider);
            this.flowLayoutPanel2.Controls.Add(this.tmjSlider);
            this.flowLayoutPanel2.Controls.Add(this.panel1);
            this.flowLayoutPanel2.Controls.Add(this.mandibleSlider);
            this.flowLayoutPanel2.Controls.Add(this.hyoidSlider);
            this.flowLayoutPanel2.Controls.Add(this.spineSlider);
            this.flowLayoutPanel2.Controls.Add(this.topTeethSlider);
            this.flowLayoutPanel2.Controls.Add(this.bottomTeethSlider);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(2, 0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(117, 592);
            this.flowLayoutPanel2.TabIndex = 7;
            // 
            // skinSlider
            // 
            this.skinSlider.LabelText = "Skin";
            this.skinSlider.Location = new System.Drawing.Point(3, 3);
            this.skinSlider.Name = "skinSlider";
            this.skinSlider.Size = new System.Drawing.Size(107, 54);
            this.skinSlider.TabIndex = 0;
            // 
            // muscleSlider
            // 
            this.muscleSlider.LabelText = "Muscles";
            this.muscleSlider.Location = new System.Drawing.Point(3, 63);
            this.muscleSlider.Name = "muscleSlider";
            this.muscleSlider.Size = new System.Drawing.Size(107, 54);
            this.muscleSlider.TabIndex = 1;
            // 
            // tmjSlider
            // 
            this.tmjSlider.LabelText = "TMJ Discs";
            this.tmjSlider.Location = new System.Drawing.Point(3, 123);
            this.tmjSlider.Name = "tmjSlider";
            this.tmjSlider.Size = new System.Drawing.Size(107, 54);
            this.tmjSlider.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.showEminence);
            this.panel1.Controls.Add(this.skullSlider);
            this.panel1.Location = new System.Drawing.Point(3, 183);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(111, 83);
            this.panel1.TabIndex = 8;
            // 
            // showEminence
            // 
            this.showEminence.AutoSize = true;
            this.showEminence.Checked = true;
            this.showEminence.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showEminence.Location = new System.Drawing.Point(3, 64);
            this.showEminence.Name = "showEminence";
            this.showEminence.Size = new System.Drawing.Size(103, 17);
            this.showEminence.TabIndex = 3;
            this.showEminence.Text = "Show Eminence";
            this.showEminence.UseVisualStyleBackColor = true;
            this.showEminence.CheckedChanged += new System.EventHandler(this.showEminence_CheckedChanged);
            // 
            // skullSlider
            // 
            this.skullSlider.LabelText = "Skull";
            this.skullSlider.Location = new System.Drawing.Point(3, 3);
            this.skullSlider.Name = "skullSlider";
            this.skullSlider.Size = new System.Drawing.Size(107, 54);
            this.skullSlider.TabIndex = 2;
            // 
            // mandibleSlider
            // 
            this.mandibleSlider.LabelText = "Mandible";
            this.mandibleSlider.Location = new System.Drawing.Point(3, 272);
            this.mandibleSlider.Name = "mandibleSlider";
            this.mandibleSlider.Size = new System.Drawing.Size(107, 54);
            this.mandibleSlider.TabIndex = 4;
            // 
            // hyoidSlider
            // 
            this.hyoidSlider.LabelText = "Hyoid";
            this.hyoidSlider.Location = new System.Drawing.Point(3, 332);
            this.hyoidSlider.Name = "hyoidSlider";
            this.hyoidSlider.Size = new System.Drawing.Size(107, 54);
            this.hyoidSlider.TabIndex = 8;
            // 
            // spineSlider
            // 
            this.spineSlider.LabelText = "Spine";
            this.spineSlider.Location = new System.Drawing.Point(3, 392);
            this.spineSlider.Name = "spineSlider";
            this.spineSlider.Size = new System.Drawing.Size(107, 54);
            this.spineSlider.TabIndex = 5;
            // 
            // topTeethSlider
            // 
            this.topTeethSlider.LabelText = "Top Teeth";
            this.topTeethSlider.Location = new System.Drawing.Point(3, 452);
            this.topTeethSlider.Name = "topTeethSlider";
            this.topTeethSlider.Size = new System.Drawing.Size(107, 54);
            this.topTeethSlider.TabIndex = 6;
            // 
            // bottomTeethSlider
            // 
            this.bottomTeethSlider.LabelText = "Bottom Teeth";
            this.bottomTeethSlider.Location = new System.Drawing.Point(3, 512);
            this.bottomTeethSlider.Name = "bottomTeethSlider";
            this.bottomTeethSlider.Size = new System.Drawing.Size(107, 54);
            this.bottomTeethSlider.TabIndex = 7;
            // 
            // SimpleLayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(393, 617);
            this.Controls.Add(this.flowLayoutPanel2);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SimpleLayerControl";
            this.ShortcutKey = System.Windows.Forms.Keys.L;
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
            this.Text = "Layers";
            this.flowLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private LayerSlider skinSlider;
        private LayerSlider muscleSlider;
        private LayerSlider skullSlider;
        private LayerSlider tmjSlider;
        private LayerSlider mandibleSlider;
        private LayerSlider spineSlider;
        private LayerSlider topTeethSlider;
        private LayerSlider bottomTeethSlider;
        private LayerSlider hyoidSlider;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox showEminence;

    }
}