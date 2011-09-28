using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MyGUIPlugin;
using System.Net;
using System.IO;
using System.Globalization;

namespace Medical
{
    public class UpdateManager
    {
        private UpdateManager() { }

        private static string downloadURL = "";

        public static void checkForUpdates(Version currentVersion)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(MedicalConfig.UpdateURL));
                request.Timeout = 15000;
                //request.Method = "POST";
                //String postData = String.Format(CultureInfo.InvariantCulture, "productID={0}", productId);
                //byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
                //request.ContentType = "application/x-www-form-urlencoded";

                //request.ContentLength = byteArray.Length;
                //using (Stream dataStream = request.GetRequestStream())
                //{
                //    dataStream.Write(byteArray, 0, byteArray.Length);
                //}

                // Get the response.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                {
                    using (XmlTextReader reader = new XmlTextReader(response.GetResponseStream()))
                    {
                        updateLogic(currentVersion, reader);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.show("Could not find update information, please try again later.", "Cannot find update", MessageBoxStyle.Ok | MessageBoxStyle.IconWarning);
            }
        }

        private static void updateLogic(Version currentVersion, XmlTextReader reader)
        {
            Version newVersion = new Version("1.0.0.0");
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

        private static void updateMessageClosed(MessageBoxStyle result)
        {
            if (result == MessageBoxStyle.Yes)
            {
                System.Diagnostics.Process.Start(downloadURL);
            }
        }
    }
}
