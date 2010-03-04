using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Medical
{
    public class UpdateManager
    {
        public static void checkForUpdates(Control messageParent, Version currentVersion)
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
                if (currentVersion < newVersion)
                {
                    if (MessageBox.Show(messageParent, String.Format("A new version {0} is avaliable for download. Would you like to download it now?", newVersion.ToString()),
                        "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(downloadURL);
                    }
                }
                else
                {
                    MessageBox.Show(messageParent, "Your current version is up to date.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(messageParent, "Could not find update information, please try again later.", "Cannot find update", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
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
