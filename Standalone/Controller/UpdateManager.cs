using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MyGUIPlugin;

namespace Medical
{
    public class UpdateManager
    {
        private static string downloadURL = "";

        public static void checkForUpdates(Version currentVersion)
        {
            Version newVersion = null;
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
                    MessageBox.show(String.Format("A new version {0} is avaliable for download. Would you like to download it now?", newVersion.ToString()), "Update", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, updateMessageClosed);
                }
                else
                {
                    MessageBox.show("Your current version is up to date.", "Update", MessageBoxStyle.Ok | MessageBoxStyle.IconInfo);
                }
            }
            catch (Exception e)
            {
                MessageBox.show("Could not find update information, please try again later.", "Cannot find update", MessageBoxStyle.Ok | MessageBoxStyle.IconWarning);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        private static void updateMessageClosed(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                System.Diagnostics.Process.Start(downloadURL);
            }
        }
    }
}
