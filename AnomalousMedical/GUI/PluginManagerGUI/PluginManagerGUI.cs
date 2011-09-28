using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.Net;
using System.IO;
using System.Globalization;
using Logging;
using Engine;
using System.Threading;
using Medical.Controller;

namespace Medical.GUI
{
    class PluginManagerGUI : AbstractFullscreenGUIPopup
    {
        private Widget installPanel;
        private ButtonGrid pluginGrid;

        private AtlasPluginManager pluginManager;
        private LicenseManager licenseManager;
        
        public PluginManagerGUI(AtlasPluginManager pluginManager, LicenseManager licenseManager, GUIManager guiManager)
            :base("Medical.GUI.PluginManagerGUI.PluginManagerGUI.layout", guiManager)
        {
            this.pluginManager = pluginManager;
            this.licenseManager = licenseManager;

            pluginGrid = new ButtonGrid((ScrollView)widget.findWidget("PluginScrollList"), new ButtonGridListLayout());
            pluginGrid.SelectedValueChanged += new EventHandler(pluginGrid_SelectedValueChanged);

            installPanel = widget.findWidget("InstallPanel");
            installPanel.Visible = false;

            Button installButton = (Button)widget.findWidget("InstallButton");
            installButton.MouseButtonClick += new MyGUIEvent(installButton_MouseButtonClick);

            Button closeButton = (Button)widget.findWidget("CloseButton");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            this.Showing += new EventHandler(PluginManagerGUI_Showing);
        }

        void PluginManagerGUI_Showing(object sender, EventArgs e)
        {
            installPanel.Visible = false;
            pluginGrid.clear();
            pluginGrid.defineGroup("Downloading");
            pluginGrid.defineGroup("Not Installed");
            pluginGrid.defineGroup("Installed");

            readPluginInfoFromServer();
        }

        void readPluginInfoFromServer()
        {
            Thread serverReadThread = new Thread(delegate()
            {
                List<int> serverPlugins = readServerPlugins();
                ThreadManager.invokeAndWait(new Action<List<int>>(setInstalledPluginDataOnGUI), serverPlugins);
                
                StringBuilder sb = new StringBuilder();
                foreach (int pluginId in serverPlugins)
                {
                    sb.Append(pluginId.ToString());
                    sb.Append(",");
                }
                if (sb.Length > 0)
                {
                    String uninstalledPluginsList = sb.ToString(0, sb.Length - 1);
                    List<ServerPluginInfo> pluginInfo = readServerPluginInfo(uninstalledPluginsList);
                    ThreadManager.invoke(new Action<List<ServerPluginInfo>>(setNotInstalledPluginDataOnGUI), pluginInfo);
                }
            });
            serverReadThread.Start();
        }

        void setInstalledPluginDataOnGUI(List<int> serverPlugins)
        {
            pluginGrid.SuppressLayout = true;

            foreach (AtlasPlugin plugin in pluginManager.LoadedPlugins)
            {
                ButtonGridItem item = pluginGrid.addItem("Installed", plugin.PluginName);
                serverPlugins.Remove((int)plugin.PluginId);
            }

            pluginGrid.SuppressLayout = false;
            pluginGrid.layout();
        }

        void setNotInstalledPluginDataOnGUI(List<ServerPluginInfo> pluginInfo)
        {
            pluginGrid.SuppressLayout = true;

            foreach (ServerPluginInfo plugin in pluginInfo)
            {
                ButtonGridItem item = pluginGrid.addItem("Not Installed", plugin.Name);
                item.UserObject = plugin;
            }

            pluginGrid.SuppressLayout = false;
            pluginGrid.layout();
        }

        void pluginGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            installPanel.Visible = selectedItem != null && selectedItem.UserObject != null;
        }

        void installButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            ServerPluginInfo pluginInfo = selectedItem.UserObject as ServerPluginInfo;
            if (pluginInfo != null)
            {
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(selectedItem);
                ButtonGridItem downloadingItem = pluginGrid.addItem("Downloading", String.Format("{0} - {1}", pluginInfo.Name, "Starting Download"));
                downloadingItem.UserObject = pluginInfo;
                pluginGrid.SuppressLayout = false;
                pluginGrid.layout();

                downloadPlugin(pluginInfo.PluginId, downloadingItem);
            }
        }

        void downloadSuccess(ButtonGridItem downloadingItem)
        {
            ServerPluginInfo pluginInfo = downloadingItem.UserObject as ServerPluginInfo;
            pluginGrid.SuppressLayout = true;
            pluginGrid.removeItem(downloadingItem);
            pluginGrid.addItem("Installed", pluginInfo.Name);
            pluginGrid.SuppressLayout = false;
            pluginGrid.layout();
        }

        void downloadFailure(ButtonGridItem downloadingItem)
        {
            MessageBox.show("There was an error downloading this plugin. Please try again later.", "Plugin Download Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
            ServerPluginInfo pluginInfo = downloadingItem.UserObject as ServerPluginInfo;
            pluginGrid.SuppressLayout = true;
            pluginGrid.removeItem(downloadingItem);
            pluginGrid.addItem("Not Installed", pluginInfo.Name);
            pluginGrid.SuppressLayout = false;
            pluginGrid.layout();
        }

        void updateDownloadStatus(ButtonGridItem downloadingItem, int progress)
        {
            ServerPluginInfo pluginInfo = downloadingItem.UserObject as ServerPluginInfo;
            downloadingItem.Caption = String.Format("{0} - {1}%", pluginInfo.Name, progress);
        }

        void licenseServerReadFail()
        {
            MessageBox.show("There was an problem getting a new license. Please restart the program to use your new plugin.", "License Download Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
        }

        List<int> readServerPlugins()
        {
            List<int> pluginIdList = new List<int>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(MedicalConfig.UserPluginListURL));
                request.Timeout = 10000;
                request.Method = "POST";
                String postData = String.Format(CultureInfo.InvariantCulture, "user={0}&pass={1}", licenseManager.User, licenseManager.MachinePassword);
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/x-www-form-urlencoded";

                request.ContentLength = byteArray.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                // Get the response.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                {
                    using (Stream serverDataStream = response.GetResponseStream())
                    {
                        using (Stream localDataStream = new MemoryStream())
                        {
                            byte[] buffer = new byte[8 * 1024];
                            int len;
                            while ((len = serverDataStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                localDataStream.Write(buffer, 0, len);
                            }
                            localDataStream.Seek(0, SeekOrigin.Begin);
                            using (BinaryReader binReader = new BinaryReader(localDataStream))
                            {
                                while (binReader.BaseStream.Position < binReader.BaseStream.Length)
                                {
                                    pluginIdList.Add(binReader.ReadInt32());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error reading plugin data from the server: {0}", e.Message);
            }

            return pluginIdList;
        }

        List<ServerPluginInfo> readServerPluginInfo(String commaSeparatedPluginList)
        {
            List<ServerPluginInfo> pluginInfoList = new List<ServerPluginInfo>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(MedicalConfig.PluginInfoURL));
                request.Timeout = 10000;
                request.Method = "POST";
                String postData = String.Format(CultureInfo.InvariantCulture, "user={0}&pass={1}&list={2}", licenseManager.User, licenseManager.MachinePassword, commaSeparatedPluginList);
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/x-www-form-urlencoded";

                request.ContentLength = byteArray.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                // Get the response.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                {
                    using (Stream serverDataStream = response.GetResponseStream())
                    {
                        using (Stream localDataStream = new MemoryStream())
                        {
                            byte[] buffer = new byte[8 * 1024];
                            int len;
                            while ((len = serverDataStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                localDataStream.Write(buffer, 0, len);
                            }
                            localDataStream.Seek(0, SeekOrigin.Begin);
                            using (StreamReader streamReader = new StreamReader(localDataStream))
                            {
                                while (!streamReader.EndOfStream)
                                {
                                    pluginInfoList.Add(new ServerPluginInfo(NumberParser.ParseInt(streamReader.ReadLine()), streamReader.ReadLine()));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error reading plugin data from the server: {0}", e.Message);
            }

            return pluginInfoList;
        }

        void downloadPlugin(int pluginId, ButtonGridItem downloadingItem)
        {
            Thread downloadThread = new Thread(delegate()
                {
                    try
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(MedicalConfig.PluginDownloadURL));
                        request.Timeout = 10000;
                        request.Method = "POST";
                        String postData = String.Format(CultureInfo.InvariantCulture, "user={0}&pass={1}&type=Plugin&pluginId={2}", licenseManager.User, licenseManager.MachinePassword, pluginId);
                        byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
                        request.ContentType = "application/x-www-form-urlencoded";

                        request.ContentLength = byteArray.Length;
                        using (Stream dataStream = request.GetRequestStream())
                        {
                            dataStream.Write(byteArray, 0, byteArray.Length);
                        }

                        // Get the response.
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                        {
                            using (Stream serverDataStream = response.GetResponseStream())
                            {
                                String filename = response.Headers["content-disposition"].Substring(21);
                                String sizeStr = response.Headers["Content-Length"];
                                float fileSize = NumberParser.ParseFloat(sizeStr);
                                String pluginFileLocation = Path.Combine(MedicalConfig.PluginConfig.PluginsFolder, filename);
                                using (Stream localDataStream = new FileStream(pluginFileLocation, FileMode.Create, FileAccess.Write, FileShare.None))
                                {
                                    byte[] buffer = new byte[8 * 1024];
                                    int len;
                                    int totalRead = 0;
                                    while ((len = serverDataStream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        totalRead += len;
                                        localDataStream.Write(buffer, 0, len);
                                        ThreadManager.invoke(new Action<ButtonGridItem, int>(updateDownloadStatus), downloadingItem, (int)(totalRead / fileSize * 100.0f));
                                    }
                                }

                                if (!licenseManager.allowFeature(pluginId) && !licenseManager.getNewLicense())
                                {
                                    ThreadManager.invoke(new Action(licenseServerReadFail));
                                }
                                else
                                {
                                    pluginManager.addPlugin(pluginFileLocation);
                                    pluginManager.initialzePlugins();
                                }

                                //If we got here the plugin installed correctly
                                ThreadManager.invoke(new Action<ButtonGridItem>(downloadSuccess), downloadingItem);
                                return;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //Log.Error("Error reading plugin data from the server: {0}", e.Message);
                    }
                    ThreadManager.invoke(new Action<ButtonGridItem>(downloadFailure), downloadingItem);
                });
            downloadThread.Start();
        }

        public override void setSize(int width, int height)
        {
            base.setSize(width, height);
            pluginGrid.layout();
        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }
    }
}
