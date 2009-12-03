using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using System.Reflection;

namespace Medical.GUI
{
    public partial class ClinicalAbout : KryptonForm
    {
        public ClinicalAbout()
        {
            InitializeComponent();
            this.AllowFormChrome = !WindowsInfo.CompositionEnabled;
            versionLabel.Text = versionLabel.Text + Assembly.GetAssembly(typeof(ClinicalAbout)).GetName().Version;
            namesLabel.Text = "Dr. Mark Piper DMD MD\nAndrew Piper\nChase Donald\nGreg Chance";
            anomalousSoftwareLabel.LinkClicked += new EventHandler(anomalousSoftwareLabel_LinkClicked);

            ogreLinkLabel.LinkClicked += new EventHandler(ogreLinkLabel_LinkClicked);
            bulletLinkLabel.LinkClicked += new EventHandler(bulletLinkLabel_LinkClicked);
            oisLinkLabel.LinkClicked += new EventHandler(oisLinkLabel_LinkClicked);
            zziplibLinkLabel.LinkClicked += new EventHandler(zziplibLinkLabel_LinkClicked);
            zliblinklabel.LinkClicked += new EventHandler(zliblinklabel_LinkClicked);
            freeimageLinkLabel.LinkClicked += new EventHandler(freeimageLinkLabel_LinkClicked);
            freetypeLinkLabel.LinkClicked += new EventHandler(freetypeLinkLabel_LinkClicked);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            openSourcePanel.Visible = false;
        }

        void anomalousSoftwareLabel_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.anomaloussoftware.com");
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openSourceToNamesButton_Click(object sender, EventArgs e)
        {
            openSourcePanel.Visible = false;
            namesPanel.Visible = true;
        }

        private void namesToOpenSourceButton_Click(object sender, EventArgs e)
        {
            namesPanel.Visible = false;
            openSourcePanel.Visible = true;
        }

        void freetypeLinkLabel_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.freetype.org/");
        }

        void freeimageLinkLabel_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://freeimage.sourceforge.net/");
        }

        void zliblinklabel_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.zlib.net/");
        }

        void zziplibLinkLabel_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://zziplib.sourceforge.net/");
        }

        void oisLinkLabel_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.wreckedgames.com/");
        }

        void bulletLinkLabel_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.bulletphysics.org/");
        }

        void ogreLinkLabel_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.ogre3d.org/");
        }
    }
}
