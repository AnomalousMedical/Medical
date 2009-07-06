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
            this.teethHidden = new System.Windows.Forms.RadioButton();
            this.teethTransparent = new System.Windows.Forms.RadioButton();
            this.teethOpaque = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
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
            this.groupBox2.Location = new System.Drawing.Point(12, 110);
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
            this.groupBox3.Controls.Add(this.teethHidden);
            this.groupBox3.Controls.Add(this.teethTransparent);
            this.groupBox3.Controls.Add(this.teethOpaque);
            this.groupBox3.Location = new System.Drawing.Point(13, 210);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(125, 91);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Teeth";
            // 
            // teethHidden
            // 
            this.teethHidden.AutoSize = true;
            this.teethHidden.Location = new System.Drawing.Point(7, 68);
            this.teethHidden.Name = "teethHidden";
            this.teethHidden.Size = new System.Drawing.Size(59, 17);
            this.teethHidden.TabIndex = 2;
            this.teethHidden.TabStop = true;
            this.teethHidden.Text = "Hidden";
            this.teethHidden.UseVisualStyleBackColor = true;
            this.teethHidden.CheckedChanged += new System.EventHandler(this.teethHidden_CheckedChanged);
            // 
            // teethTransparent
            // 
            this.teethTransparent.AutoSize = true;
            this.teethTransparent.Location = new System.Drawing.Point(7, 44);
            this.teethTransparent.Name = "teethTransparent";
            this.teethTransparent.Size = new System.Drawing.Size(82, 17);
            this.teethTransparent.TabIndex = 1;
            this.teethTransparent.TabStop = true;
            this.teethTransparent.Text = "Transparent";
            this.teethTransparent.UseVisualStyleBackColor = true;
            this.teethTransparent.CheckedChanged += new System.EventHandler(this.teethTransparent_CheckedChanged);
            // 
            // teethOpaque
            // 
            this.teethOpaque.AutoSize = true;
            this.teethOpaque.Location = new System.Drawing.Point(7, 20);
            this.teethOpaque.Name = "teethOpaque";
            this.teethOpaque.Size = new System.Drawing.Size(63, 17);
            this.teethOpaque.TabIndex = 0;
            this.teethOpaque.TabStop = true;
            this.teethOpaque.Text = "Opaque";
            this.teethOpaque.UseVisualStyleBackColor = true;
            this.teethOpaque.CheckedChanged += new System.EventHandler(this.teethOpaque_CheckedChanged);
            // 
            // SimpleLayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(150, 309);
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
        private System.Windows.Forms.RadioButton teethHidden;
        private System.Windows.Forms.RadioButton teethTransparent;
        private System.Windows.Forms.RadioButton teethOpaque;

    }
}