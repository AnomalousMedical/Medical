namespace Medical.GUI
{
    partial class ClinicalAbout
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
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.openSourcePanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.freeimageLinkLabel = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this.freetypeLinkLabel = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this.zliblinklabel = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this.zziplibLinkLabel = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this.oisLinkLabel = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this.bulletLinkLabel = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this.ogreLinkLabel = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this.openSourceToNamesButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.namesPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.namesLabel = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.namesToOpenSourceButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.copyright = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.closeButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.anomalousSoftwareLabel = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this.versionLabel = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.openSourcePanel)).BeginInit();
            this.openSourcePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.namesPanel)).BeginInit();
            this.namesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.copyright);
            this.kryptonPanel1.Controls.Add(this.closeButton);
            this.kryptonPanel1.Controls.Add(this.anomalousSoftwareLabel);
            this.kryptonPanel1.Controls.Add(this.versionLabel);
            this.kryptonPanel1.Controls.Add(this.openSourcePanel);
            this.kryptonPanel1.Controls.Add(this.namesPanel);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(349, 417);
            this.kryptonPanel1.StateCommon.Image = global::Medical.Properties.Resources.AboutImage;
            this.kryptonPanel1.StateCommon.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.TopLeft;
            this.kryptonPanel1.TabIndex = 0;
            // 
            // openSourcePanel
            // 
            this.openSourcePanel.Controls.Add(this.kryptonLabel1);
            this.openSourcePanel.Controls.Add(this.freeimageLinkLabel);
            this.openSourcePanel.Controls.Add(this.freetypeLinkLabel);
            this.openSourcePanel.Controls.Add(this.zliblinklabel);
            this.openSourcePanel.Controls.Add(this.zziplibLinkLabel);
            this.openSourcePanel.Controls.Add(this.oisLinkLabel);
            this.openSourcePanel.Controls.Add(this.bulletLinkLabel);
            this.openSourcePanel.Controls.Add(this.ogreLinkLabel);
            this.openSourcePanel.Controls.Add(this.openSourceToNamesButton);
            this.openSourcePanel.Location = new System.Drawing.Point(13, 165);
            this.openSourcePanel.Name = "openSourcePanel";
            this.openSourcePanel.Size = new System.Drawing.Size(324, 218);
            this.openSourcePanel.TabIndex = 5;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(4, 5);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(121, 19);
            this.kryptonLabel1.TabIndex = 9;
            this.kryptonLabel1.Values.Text = "Open Source Projects";
            // 
            // freeimageLinkLabel
            // 
            this.freeimageLinkLabel.Location = new System.Drawing.Point(4, 133);
            this.freeimageLinkLabel.Name = "freeimageLinkLabel";
            this.freeimageLinkLabel.Size = new System.Drawing.Size(64, 19);
            this.freeimageLinkLabel.TabIndex = 8;
            this.freeimageLinkLabel.Values.Text = "Freeimage";
            // 
            // freetypeLinkLabel
            // 
            this.freetypeLinkLabel.Location = new System.Drawing.Point(4, 115);
            this.freetypeLinkLabel.Name = "freetypeLinkLabel";
            this.freetypeLinkLabel.Size = new System.Drawing.Size(55, 19);
            this.freetypeLinkLabel.TabIndex = 7;
            this.freetypeLinkLabel.Values.Text = "Freetype";
            // 
            // zliblinklabel
            // 
            this.zliblinklabel.Location = new System.Drawing.Point(4, 97);
            this.zliblinklabel.Name = "zliblinklabel";
            this.zliblinklabel.Size = new System.Drawing.Size(29, 19);
            this.zliblinklabel.TabIndex = 6;
            this.zliblinklabel.Values.Text = "Zlib";
            // 
            // zziplibLinkLabel
            // 
            this.zziplibLinkLabel.Location = new System.Drawing.Point(4, 79);
            this.zziplibLinkLabel.Name = "zziplibLinkLabel";
            this.zziplibLinkLabel.Size = new System.Drawing.Size(44, 19);
            this.zziplibLinkLabel.TabIndex = 5;
            this.zziplibLinkLabel.Values.Text = "Zziplib";
            // 
            // oisLinkLabel
            // 
            this.oisLinkLabel.Location = new System.Drawing.Point(4, 61);
            this.oisLinkLabel.Name = "oisLinkLabel";
            this.oisLinkLabel.Size = new System.Drawing.Size(28, 19);
            this.oisLinkLabel.TabIndex = 4;
            this.oisLinkLabel.Values.Text = "OIS";
            // 
            // bulletLinkLabel
            // 
            this.bulletLinkLabel.Location = new System.Drawing.Point(4, 43);
            this.bulletLinkLabel.Name = "bulletLinkLabel";
            this.bulletLinkLabel.Size = new System.Drawing.Size(79, 19);
            this.bulletLinkLabel.TabIndex = 3;
            this.bulletLinkLabel.Values.Text = "Bullet Physics";
            // 
            // ogreLinkLabel
            // 
            this.ogreLinkLabel.Location = new System.Drawing.Point(4, 25);
            this.ogreLinkLabel.Name = "ogreLinkLabel";
            this.ogreLinkLabel.Size = new System.Drawing.Size(54, 19);
            this.ogreLinkLabel.TabIndex = 2;
            this.ogreLinkLabel.Values.Text = "Ogre 3D";
            // 
            // openSourceToNamesButton
            // 
            this.openSourceToNamesButton.Location = new System.Drawing.Point(231, 190);
            this.openSourceToNamesButton.Name = "openSourceToNamesButton";
            this.openSourceToNamesButton.Size = new System.Drawing.Size(90, 25);
            this.openSourceToNamesButton.TabIndex = 1;
            this.openSourceToNamesButton.Values.Text = "More";
            this.openSourceToNamesButton.Click += new System.EventHandler(this.openSourceToNamesButton_Click);
            // 
            // namesPanel
            // 
            this.namesPanel.Controls.Add(this.namesLabel);
            this.namesPanel.Controls.Add(this.kryptonLabel2);
            this.namesPanel.Controls.Add(this.namesToOpenSourceButton);
            this.namesPanel.Location = new System.Drawing.Point(13, 165);
            this.namesPanel.Name = "namesPanel";
            this.namesPanel.Size = new System.Drawing.Size(324, 218);
            this.namesPanel.TabIndex = 3;
            // 
            // namesLabel
            // 
            this.namesLabel.Location = new System.Drawing.Point(4, 30);
            this.namesLabel.Name = "namesLabel";
            this.namesLabel.Size = new System.Drawing.Size(49, 19);
            this.namesLabel.TabIndex = 3;
            this.namesLabel.Values.Text = "NAMES";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(4, 5);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(68, 19);
            this.kryptonLabel2.TabIndex = 2;
            this.kryptonLabel2.Values.Text = "Developers";
            // 
            // namesToOpenSourceButton
            // 
            this.namesToOpenSourceButton.Location = new System.Drawing.Point(231, 190);
            this.namesToOpenSourceButton.Name = "namesToOpenSourceButton";
            this.namesToOpenSourceButton.Size = new System.Drawing.Size(90, 25);
            this.namesToOpenSourceButton.TabIndex = 1;
            this.namesToOpenSourceButton.Values.Text = "More";
            this.namesToOpenSourceButton.Click += new System.EventHandler(this.namesToOpenSourceButton_Click);
            // 
            // copyright
            // 
            this.copyright.Location = new System.Drawing.Point(54, 140);
            this.copyright.Name = "copyright";
            this.copyright.Size = new System.Drawing.Size(235, 19);
            this.copyright.TabIndex = 4;
            this.copyright.Values.Text = "Copyright ©2009-2010 Anomalous Medical";
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(4, 389);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(90, 25);
            this.closeButton.TabIndex = 2;
            this.closeButton.Values.Text = "Close";
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // anomalousSoftwareLabel
            // 
            this.anomalousSoftwareLabel.Location = new System.Drawing.Point(229, 395);
            this.anomalousSoftwareLabel.Name = "anomalousSoftwareLabel";
            this.anomalousSoftwareLabel.Size = new System.Drawing.Size(117, 19);
            this.anomalousSoftwareLabel.TabIndex = 1;
            this.anomalousSoftwareLabel.Values.Text = "Anomalous Software";
            // 
            // versionLabel
            // 
            this.versionLabel.Location = new System.Drawing.Point(136, 73);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(49, 19);
            this.versionLabel.StateCommon.LongText.Color1 = System.Drawing.Color.White;
            this.versionLabel.StateCommon.LongText.Color2 = System.Drawing.Color.White;
            this.versionLabel.StateCommon.ShortText.Color1 = System.Drawing.Color.White;
            this.versionLabel.StateCommon.ShortText.Color2 = System.Drawing.Color.White;
            this.versionLabel.TabIndex = 0;
            this.versionLabel.Values.Text = "Version ";
            // 
            // ClinicalAbout
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 417);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "ClinicalAbout";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "About Articulometrics";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.openSourcePanel)).EndInit();
            this.openSourcePanel.ResumeLayout(false);
            this.openSourcePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.namesPanel)).EndInit();
            this.namesPanel.ResumeLayout(false);
            this.namesPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel versionLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel anomalousSoftwareLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonButton closeButton;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel namesPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel copyright;
        private ComponentFactory.Krypton.Toolkit.KryptonButton namesToOpenSourceButton;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel openSourcePanel;
        private ComponentFactory.Krypton.Toolkit.KryptonButton openSourceToNamesButton;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel oisLinkLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel bulletLinkLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel ogreLinkLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel freeimageLinkLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel freetypeLinkLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel zliblinklabel;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel zziplibLinkLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel namesLabel;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
    }
}