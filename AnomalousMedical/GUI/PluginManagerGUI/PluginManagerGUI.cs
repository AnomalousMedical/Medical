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
        private DownloadController downloadController;
        
        public PluginManagerGUI(AtlasPluginManager pluginManager, LicenseManager licenseManager, DownloadController downloadController, GUIManager guiManager)
            :base("Medical.GUI.PluginManagerGUI.PluginManagerGUI.layout", guiManager)
        {
            this.pluginManager = pluginManager;
            this.licenseManager = licenseManager;
            this.downloadController = downloadController;

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

            pluginGrid.SuppressLayout = true;
            pluginGrid.clear();
            pluginGrid.defineGroup("Downloading");
            pluginGrid.defineGroup("Not Installed");
            pluginGrid.defineGroup("Installed");

            List<int> installedPluginIds = new List<int>();

            foreach (AtlasPlugin plugin in pluginManager.LoadedPlugins)
            {
                ButtonGridItem item = pluginGrid.addItem("Installed", plugin.PluginName);
                installedPluginIds.Add((int)plugin.PluginId);
            }

            pluginGrid.SuppressLayout = false;
            pluginGrid.layout();

            readPluginInfoFromServer(installedPluginIds);
        }

        void readPluginInfoFromServer(List<int> installedPluginIds)
        {
            Thread serverReadThread = new Thread(delegate()
            {                
                StringBuilder sb = new StringBuilder();
                foreach (int pluginId in installedPluginIds)
                {
                    sb.Append(pluginId.ToString());
                    sb.Append(",");
                }
                if (sb.Length > 0)
                {
                    String installedPluginsList = sb.ToString(0, sb.Length - 1);
                    List<ServerPluginInfo> pluginInfo = readServerPluginInfo(installedPluginsList);
                    ThreadManager.invoke(new Action<List<ServerPluginInfo>>(setNotInstalledPluginDataOnGUI), pluginInfo);
                }
            });
            serverReadThread.Start();
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

                downloadController.downloadPlugin(pluginInfo.PluginId, updateDownloadStatus, downloadCompleted, downloadingItem);
            }
        }

        void downloadCompleted(Download download)
        {
            ButtonGridItem downloadingItem = (ButtonGridItem)download.UserObject;
            if (download.Successful)
            {
                ServerPluginInfo pluginInfo = downloadingItem.UserObject as ServerPluginInfo;
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(downloadingItem);
                pluginGrid.addItem("Installed", pluginInfo.Name);
                pluginGrid.SuppressLayout = false;
                pluginGrid.layout();
            }
            else
            {
                MessageBox.show("There was an error downloading this plugin. Please try again later.", "Plugin Download Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
                ServerPluginInfo pluginInfo = downloadingItem.UserObject as ServerPluginInfo;
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(downloadingItem);
                ButtonGridItem item = pluginGrid.addItem("Not Installed", pluginInfo.Name);
                item.UserObject = pluginInfo;
                pluginGrid.SuppressLayout = false;
                pluginGrid.layout();
            }
        }

        void updateDownloadStatus(Download download)
        {
            ButtonGridItem downloadingItem = (ButtonGridItem)download.UserObject;
            ServerPluginInfo pluginInfo = downloadingItem.UserObject as ServerPluginInfo;
            downloadingItem.Caption = String.Format("{0} - {1}%", pluginInfo.Name, (int)((float)download.TotalRead / download.TotalSize * 100.0f));
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
