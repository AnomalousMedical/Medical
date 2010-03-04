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
using System.Xml;

namespace Medical.GUI
{
    public partial class ClinicalAbout : KryptonForm
    {
        public ClinicalAbout(String featureLevelString)
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
            featureLevelLabel.Text = featureLevelString;
            serialLabel.Text += UserPermissions.Instance.getId();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            openSourcePanel.Visible = false;
            namesPanel.Visible = true;
        }

        void anomalousSoftwareLabel_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.anomalousmedical.com");
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

        private void updateButton_Click(object sender, EventArgs e)
        {
            checkForUpdates();
        }

        private void checkForUpdates()
        {
            Version newVersion = null;
            string downloadURL = "";
            XmlTextReader reader = null;
            try
            {
                string xmlURL = MedicalConfig.UpdateURL;
                reader = new XmlTextReader(xmlURL);
                reader.MoveToContent();
                string elementName = "";
                if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "update"))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            elementName = reader.Name;
                        }
                        else
                        { 
                            if ((reader.NodeType == XmlNodeType.Text) && (reader.HasValue))
                            {
                                switch (elementName)
                                {
                                    case "version":
                                        newVersion = new Version(reader.Value);
                                        break;
                                    case "url":
                                        downloadURL = reader.Value;
                                        break;
                                }
                            }
                        }
                    }
                }
                if (Assembly.GetAssembly(typeof(ClinicalAbout)).GetName().Version < newVersion)
                {
                    if(MessageBox.Show(this, String.Format("A new version {0} is avaliable for download. Would you like to download it now?", newVersion.ToString()), 
                        "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(downloadURL);
                    }
                }
                else
                {
                    MessageBox.Show(this, "Your current version is up to date.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(this, "Could not find update information, please try again later.", "Cannot find update", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
    }
}
