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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.skullHidden = new System.Windows.Forms.RadioButton();
            this.skullTransparent = new System.Windows.Forms.RadioButton();
            this.skullOpaque = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mandibleHidden = new System.Windows.Forms.RadioButton();
            this.mandibleTransparent = new System.Windows.Forms.RadioButton();
            this.mandibleOpaque = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.topTeethHidden = new System.Windows.Forms.RadioButton();
            this.topTeethTransparent = new System.Windows.Forms.RadioButton();
            this.topTeethOpaque = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.bottomTeethHidden = new System.Windows.Forms.RadioButton();
            this.bottomTeethTransparent = new System.Windows.Forms.RadioButton();
            this.bottomTeethOpaque = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.skullInteriorHidden = new System.Windows.Forms.RadioButton();
            this.skullInteriorTransparent = new System.Windows.Forms.RadioButton();
            this.skullInteriorOpaque = new System.Windows.Forms.RadioButton();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.eminenceHidden = new System.Windows.Forms.RadioButton();
            this.eminenceVisible = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.skullHidden);
            this.groupBox1.Controls.Add(this.skullTransparent);
            this.groupBox1.Controls.Add(this.skullOpaque);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(125, 91);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Skull";
            // 
            // skullHidden
            // 
            this.skullHidden.AutoSize = true;
            this.skullHidden.Location = new System.Drawing.Point(7, 68);
            this.skullHidden.Name = "skullHidden";
            this.skullHidden.Size = new System.Drawing.Size(59, 17);
            this.skullHidden.TabIndex = 2;
            this.skullHidden.TabStop = true;
            this.skullHidden.Text = "Hidden";
            this.skullHidden.UseVisualStyleBackColor = true;
            this.skullHidden.CheckedChanged += new System.EventHandler(this.skullHidden_CheckedChanged);
            // 
            // skullTransparent
            // 
            this.skullTransparent.AutoSize = true;
            this.skullTransparent.Location = new System.Drawing.Point(7, 44);
            this.skullTransparent.Name = "skullTransparent";
            this.skullTransparent.Size = new System.Drawing.Size(82, 17);
            this.skullTransparent.TabIndex = 1;
            this.skullTransparent.TabStop = true;
            this.skullTransparent.Text = "Transparent";
            this.skullTransparent.UseVisualStyleBackColor = true;
            this.skullTransparent.CheckedChanged += new System.EventHandler(this.skullTransparent_CheckedChanged);
            // 
            // skullOpaque
            // 
            this.skullOpaque.AutoSize = true;
            this.skullOpaque.Location = new System.Drawing.Point(7, 20);
            this.skullOpaque.Name = "skullOpaque";
            this.skullOpaque.Size = new System.Drawing.Size(63, 17);
            this.skullOpaque.TabIndex = 0;
            this.skullOpaque.TabStop = true;
            this.skullOpaque.Text = "Opaque";
            this.skullOpaque.UseVisualStyleBackColor = true;
            this.skullOpaque.CheckedChanged += new System.EventHandler(this.skullOpaque_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mandibleHidden);
            this.groupBox2.Controls.Add(this.mandibleTransparent);
            this.groupBox2.Controls.Add(this.mandibleOpaque);
            this.groupBox2.Location = new System.Drawing.Point(12, 275);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(125, 91);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mandible";
            // 
            // mandibleHidden
            // 
            this.mandibleHidden.AutoSize = true;
            this.mandibleHidden.Location = new System.Drawing.Point(7, 68);
            this.mandibleHidden.Name = "mandibleHidden";
            this.mandibleHidden.Size = new System.Drawing.Size(59, 17);
            this.mandibleHidden.TabIndex = 2;
            this.mandibleHidden.TabStop = true;
            this.mandibleHidden.Text = "Hidden";
            this.mandibleHidden.UseVisualStyleBackColor = true;
            this.mandibleHidden.CheckedChanged += new System.EventHandler(this.mandibleHidden_CheckedChanged);
            // 
            // mandibleTransparent
            // 
            this.mandibleTransparent.AutoSize = true;
            this.mandibleTransparent.Location = new System.Drawing.Point(7, 44);
            this.mandibleTransparent.Name = "mandibleTransparent";
            this.mandibleTransparent.Size = new System.Drawing.Size(82, 17);
            this.mandibleTransparent.TabIndex = 1;
            this.mandibleTransparent.TabStop = true;
            this.mandibleTransparent.Text = "Transparent";
            this.mandibleTransparent.UseVisualStyleBackColor = true;
            this.mandibleTransparent.CheckedChanged += new System.EventHandler(this.mandibleTransparent_CheckedChanged);
            // 
            // mandibleOpaque
            // 
            this.mandibleOpaque.AutoSize = true;
            this.mandibleOpaque.Location = new System.Drawing.Point(7, 20);
            this.mandibleOpaque.Name = "mandibleOpaque";
            this.mandibleOpaque.Size = new System.Drawing.Size(63, 17);
            this.mandibleOpaque.TabIndex = 0;
            this.mandibleOpaque.TabStop = true;
            this.mandibleOpaque.Text = "Opaque";
            this.mandibleOpaque.UseVisualStyleBackColor = true;
            this.mandibleOpaque.CheckedChanged += new System.EventHandler(this.mandibleOpaque_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.topTeethHidden);
            this.groupBox3.Controls.Add(this.topTeethTransparent);
            this.groupBox3.Controls.Add(this.topTeethOpaque);
            this.groupBox3.Location = new System.Drawing.Point(13, 375);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(125, 91);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Top Teeth";
            // 
            // topTeethHidden
            // 
            this.topTeethHidden.AutoSize = true;
            this.topTeethHidden.Location = new System.Drawing.Point(7, 68);
            this.topTeethHidden.Name = "topTeethHidden";
            this.topTeethHidden.Size = new System.Drawing.Size(59, 17);
            this.topTeethHidden.TabIndex = 2;
            this.topTeethHidden.TabStop = true;
            this.topTeethHidden.Text = "Hidden";
            this.topTeethHidden.UseVisualStyleBackColor = true;
            this.topTeethHidden.CheckedChanged += new System.EventHandler(this.teethHidden_CheckedChanged);
            // 
            // topTeethTransparent
            // 
            this.topTeethTransparent.AutoSize = true;
            this.topTeethTransparent.Location = new System.Drawing.Point(7, 44);
            this.topTeethTransparent.Name = "topTeethTransparent";
            this.topTeethTransparent.Size = new System.Drawing.Size(82, 17);
            this.topTeethTransparent.TabIndex = 1;
            this.topTeethTransparent.TabStop = true;
            this.topTeethTransparent.Text = "Transparent";
            this.topTeethTransparent.UseVisualStyleBackColor = true;
            this.topTeethTransparent.CheckedChanged += new System.EventHandler(this.teethTransparent_CheckedChanged);
            // 
            // topTeethOpaque
            // 
            this.topTeethOpaque.AutoSize = true;
            this.topTeethOpaque.Location = new System.Drawing.Point(7, 20);
            this.topTeethOpaque.Name = "topTeethOpaque";
            this.topTeethOpaque.Size = new System.Drawing.Size(63, 17);
            this.topTeethOpaque.TabIndex = 0;
            this.topTeethOpaque.TabStop = true;
            this.topTeethOpaque.Text = "Opaque";
            this.topTeethOpaque.UseVisualStyleBackColor = true;
            this.topTeethOpaque.CheckedChanged += new System.EventHandler(this.teethOpaque_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.bottomTeethHidden);
            this.groupBox4.Controls.Add(this.bottomTeethTransparent);
            this.groupBox4.Controls.Add(this.bottomTeethOpaque);
            this.groupBox4.Location = new System.Drawing.Point(13, 472);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(125, 91);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Bottom Teeth";
            // 
            // bottomTeethHidden
            // 
            this.bottomTeethHidden.AutoSize = true;
            this.bottomTeethHidden.Location = new System.Drawing.Point(7, 68);
            this.bottomTeethHidden.Name = "bottomTeethHidden";
            this.bottomTeethHidden.Size = new System.Drawing.Size(59, 17);
            this.bottomTeethHidden.TabIndex = 2;
            this.bottomTeethHidden.TabStop = true;
            this.bottomTeethHidden.Text = "Hidden";
            this.bottomTeethHidden.UseVisualStyleBackColor = true;
            this.bottomTeethHidden.CheckedChanged += new System.EventHandler(this.bottomTeethHidden_CheckedChanged);
            // 
            // bottomTeethTransparent
            // 
            this.bottomTeethTransparent.AutoSize = true;
            this.bottomTeethTransparent.Location = new System.Drawing.Point(7, 44);
            this.bottomTeethTransparent.Name = "bottomTeethTransparent";
            this.bottomTeethTransparent.Size = new System.Drawing.Size(82, 17);
            this.bottomTeethTransparent.TabIndex = 1;
            this.bottomTeethTransparent.TabStop = true;
            this.bottomTeethTransparent.Text = "Transparent";
            this.bottomTeethTransparent.UseVisualStyleBackColor = true;
            this.bottomTeethTransparent.CheckedChanged += new System.EventHandler(this.bottomTeethTransparent_CheckedChanged);
            // 
            // bottomTeethOpaque
            // 
            this.bottomTeethOpaque.AutoSize = true;
            this.bottomTeethOpaque.Location = new System.Drawing.Point(7, 20);
            this.bottomTeethOpaque.Name = "bottomTeethOpaque";
            this.bottomTeethOpaque.Size = new System.Drawing.Size(63, 17);
            this.bottomTeethOpaque.TabIndex = 0;
            this.bottomTeethOpaque.TabStop = true;
            this.bottomTeethOpaque.Text = "Opaque";
            this.bottomTeethOpaque.UseVisualStyleBackColor = true;
            this.bottomTeethOpaque.CheckedChanged += new System.EventHandler(this.bottomTeethOpaque_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.skullInteriorHidden);
            this.groupBox5.Controls.Add(this.skullInteriorTransparent);
            this.groupBox5.Controls.Add(this.skullInteriorOpaque);
            this.groupBox5.Location = new System.Drawing.Point(13, 106);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(125, 91);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Interior";
            // 
            // skullInteriorHidden
            // 
            this.skullInteriorHidden.AutoSize = true;
            this.skullInteriorHidden.Location = new System.Drawing.Point(7, 68);
            this.skullInteriorHidden.Name = "skullInteriorHidden";
            this.skullInteriorHidden.Size = new System.Drawing.Size(59, 17);
            this.skullInteriorHidden.TabIndex = 2;
            this.skullInteriorHidden.TabStop = true;
            this.skullInteriorHidden.Text = "Hidden";
            this.skullInteriorHidden.UseVisualStyleBackColor = true;
            this.skullInteriorHidden.CheckedChanged += new System.EventHandler(this.skullInteriorHidden_CheckedChanged);
            // 
            // skullInteriorTransparent
            // 
            this.skullInteriorTransparent.AutoSize = true;
            this.skullInteriorTransparent.Location = new System.Drawing.Point(7, 44);
            this.skullInteriorTransparent.Name = "skullInteriorTransparent";
            this.skullInteriorTransparent.Size = new System.Drawing.Size(82, 17);
            this.skullInteriorTransparent.TabIndex = 1;
            this.skullInteriorTransparent.TabStop = true;
            this.skullInteriorTransparent.Text = "Transparent";
            this.skullInteriorTransparent.UseVisualStyleBackColor = true;
            this.skullInteriorTransparent.CheckedChanged += new System.EventHandler(this.skullInteriorTransparent_CheckedChanged);
            // 
            // skullInteriorOpaque
            // 
            this.skullInteriorOpaque.AutoSize = true;
            this.skullInteriorOpaque.Location = new System.Drawing.Point(7, 20);
            this.skullInteriorOpaque.Name = "skullInteriorOpaque";
            this.skullInteriorOpaque.Size = new System.Drawing.Size(63, 17);
            this.skullInteriorOpaque.TabIndex = 0;
            this.skullInteriorOpaque.TabStop = true;
            this.skullInteriorOpaque.Text = "Opaque";
            this.skullInteriorOpaque.UseVisualStyleBackColor = true;
            this.skullInteriorOpaque.CheckedChanged += new System.EventHandler(this.skullInteriorOpaque_CheckedChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.eminenceHidden);
            this.groupBox6.Controls.Add(this.eminenceVisible);
            this.groupBox6.Location = new System.Drawing.Point(13, 203);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(125, 66);
            this.groupBox6.TabIndex = 4;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Eminence";
            // 
            // eminenceHidden
            // 
            this.eminenceHidden.AutoSize = true;
            this.eminenceHidden.Location = new System.Drawing.Point(7, 44);
            this.eminenceHidden.Name = "eminenceHidden";
            this.eminenceHidden.Size = new System.Drawing.Size(59, 17);
            this.eminenceHidden.TabIndex = 1;
            this.eminenceHidden.TabStop = true;
            this.eminenceHidden.Text = "Hidden";
            this.eminenceHidden.UseVisualStyleBackColor = true;
            this.eminenceHidden.CheckedChanged += new System.EventHandler(this.eminenceHidden_CheckedChanged);
            // 
            // eminenceVisible
            // 
            this.eminenceVisible.AutoSize = true;
            this.eminenceVisible.Location = new System.Drawing.Point(7, 20);
            this.eminenceVisible.Name = "eminenceVisible";
            this.eminenceVisible.Size = new System.Drawing.Size(55, 17);
            this.eminenceVisible.TabIndex = 0;
            this.eminenceVisible.TabStop = true;
            this.eminenceVisible.Text = "Visible";
            this.eminenceVisible.UseVisualStyleBackColor = true;
            this.eminenceVisible.CheckedChanged += new System.EventHandler(this.eminenceVisible_CheckedChanged);
            // 
            // SimpleLayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(150, 573);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)));
            this.Name = "SimpleLayerControl";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
            this.Text = "Layers";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton skullHidden;
        private System.Windows.Forms.RadioButton skullTransparent;
        private System.Windows.Forms.RadioButton skullOpaque;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton mandibleHidden;
        private System.Windows.Forms.RadioButton mandibleTransparent;
        private System.Windows.Forms.RadioButton mandibleOpaque;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton topTeethHidden;
        private System.Windows.Forms.RadioButton topTeethTransparent;
        private System.Windows.Forms.RadioButton topTeethOpaque;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton bottomTeethHidden;
        private System.Windows.Forms.RadioButton bottomTeethTransparent;
        private System.Windows.Forms.RadioButton bottomTeethOpaque;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton skullInteriorHidden;
        private System.Windows.Forms.RadioButton skullInteriorTransparent;
        private System.Windows.Forms.RadioButton skullInteriorOpaque;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton eminenceHidden;
        private System.Windows.Forms.RadioButton eminenceVisible;

    }
}