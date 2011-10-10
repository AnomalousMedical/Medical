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
using System.Drawing;

namespace Medical.GUI
{
    class PluginManagerGUI : AbstractFullscreenGUIPopup, DownloadListener
    {
        private const float BYTES_TO_MEGABYTES = 9.53674316e-7f;

        private Widget installPanel;
        private ButtonGrid pluginGrid;
        private Widget downloadPanel;
        private Widget readingInfo;

        private AtlasPluginManager pluginManager;
        private LicenseManager licenseManager;
        private DownloadController downloadController;
        private bool activeNotDisposed = true;
        private bool addedInstalledPlugins = false;
        private bool readingServerPluginInfo = false;
        private ImageAtlas serverImages = new ImageAtlas("PluginManagerServerImages", new Size2(100, 100), new Size2(1024, 1024));

        List<ServerPluginInfo> detectedServerPlugins = new List<ServerPluginInfo>();
        
        public PluginManagerGUI(AtlasPluginManager pluginManager, LicenseManager licenseManager, DownloadController downloadController, GUIManager guiManager)
            :base("Medical.GUI.PluginManagerGUI.PluginManagerGUI.layout", guiManager)
        {
            this.pluginManager = pluginManager;
            this.licenseManager = licenseManager;
            this.downloadController = downloadController;

            pluginGrid = new ButtonGrid((ScrollView)widget.findWidget("PluginScrollList"), new ButtonGridListLayout());
            pluginGrid.SelectedValueChanged += new EventHandler(pluginGrid_SelectedValueChanged);
            pluginGrid.ItemActivated += new EventHandler(pluginGrid_ItemActivated);
            pluginGrid.defineGroup("Downloading");
            pluginGrid.defineGroup("Not Installed");
            pluginGrid.defineGroup("Installed");

            installPanel = widget.findWidget("InstallPanel");
            installPanel.Visible = false;

            Button installButton = (Button)widget.findWidget("InstallButton");
            installButton.MouseButtonClick += new MyGUIEvent(installButton_MouseButtonClick);

            downloadPanel = widget.findWidget("DownloadingPanel");
            downloadPanel.Visible = false;

            Button cancelButton = (Button)widget.findWidget("CancelButton");
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            Button closeButton = (Button)widget.findWidget("CloseButton");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            readingInfo = widget.findWidget("ReadingInfo");
            readingInfo.Visible = false;

            this.Showing += new EventHandler(PluginManagerGUI_Showing);
        }

        public override void Dispose()
        {
            serverImages.Dispose();
            activeNotDisposed = false;
            base.Dispose();
        }

        void PluginManagerGUI_Showing(object sender, EventArgs e)
        {
            if (!readingServerPluginInfo)
            {
                List<int> detectedPluginIds = new List<int>();

                if (addedInstalledPlugins)
                {
                    foreach (AtlasPlugin plugin in pluginManager.LoadedPlugins)
                    {
                        if (plugin.PluginId != -1)
                        {
                            detectedPluginIds.Add((int)plugin.PluginId);
                        }
                    }
                    foreach (ServerPluginInfo plugin in detectedServerPlugins)
                    {
                        detectedPluginIds.Add(plugin.PluginId);
                    }
                }
                else
                {
                    pluginGrid.SuppressLayout = true;

                    foreach (AtlasPlugin plugin in pluginManager.LoadedPlugins)
                    {
                        ButtonGridItem item = pluginGrid.addItem("Installed", plugin.PluginName, plugin.BrandingImageKey);
                        if (plugin.PluginId != -1)
                        {
                            detectedPluginIds.Add((int)plugin.PluginId);
                        }
                    }
                    addedInstalledPlugins = true;

                    pluginGrid.SuppressLayout = false;
                    pluginGrid.layout();
                }

                readPluginInfoFromServer(detectedPluginIds);
            }
        }

        void readPluginInfoFromServer(List<int> installedPluginIds)
        {
            readingServerPluginInfo = true;
            readingInfo.Visible = true;
            Thread serverReadThread = new Thread(delegate()
            {
                StringBuilder sb = new StringBuilder();
                foreach (int pluginId in installedPluginIds)
                {
                    sb.Append(pluginId.ToString());
                    sb.Append(",");
                }
                String installedPluginsList = String.Empty;
                if (sb.Length > 0)
                {
                    installedPluginsList = sb.ToString(0, sb.Length - 1);
                }
                List<ServerPluginInfo> pluginInfo = readServerPluginInfo(installedPluginsList);
                ThreadManager.invoke(new Action(delegate()
                {
                    if (activeNotDisposed)
                    {
                        pluginGrid.SuppressLayout = true;

                        foreach (ServerPluginInfo plugin in pluginInfo)
                        {
                            ButtonGridItem item = pluginGrid.addItem("Not Installed", plugin.Name, plugin.ImageKey);
                            item.UserObject = plugin;
                            detectedServerPlugins.Add(plugin);
                        }

                        pluginGrid.SuppressLayout = false;
                        pluginGrid.layout();

                        readingServerPluginInfo = false;
                        readingInfo.Visible = false;
                    }
                }));
            });
            serverReadThread.Start();
        }

        private void togglePanelVisibility()
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            if (selectedItem != null)
            {
                installPanel.Visible = selectedItem.GroupName == "Not Installed";
                downloadPanel.Visible = selectedItem.GroupName == "Downloading";
            }
            else
            {
                installPanel.Visible = false;
                downloadPanel.Visible = false;
            }
        }

        void pluginGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            togglePanelVisibility();
        }

        void installButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            ServerPluginInfo pluginInfo = selectedItem.UserObject as ServerPluginInfo;
            if (pluginInfo != null)
            {
                downloadItem(selectedItem, pluginInfo);
            }
        }

        void pluginGrid_ItemActivated(object sender, EventArgs e)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            ServerPluginInfo pluginInfo = selectedItem.UserObject as ServerPluginInfo;
            if (pluginInfo != null && pluginInfo.Download == null)
            {
                downloadItem(selectedItem, pluginInfo);
            }
        }

        private void downloadItem(ButtonGridItem selectedItem, ServerPluginInfo pluginInfo)
        {
            pluginGrid.SuppressLayout = true;
            pluginGrid.removeItem(selectedItem);
            ButtonGridItem downloadingItem = pluginGrid.addItem("Downloading", String.Format("{0} - {1}", pluginInfo.Name, "Starting Download"), pluginInfo.ImageKey);
            downloadingItem.UserObject = pluginInfo;
            pluginGrid.SuppressLayout = false;
            pluginGrid.layout();

            pluginInfo.Download = downloadController.downloadPlugin(pluginInfo.PluginId, this, downloadingItem);
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            ServerPluginInfo pluginInfo = selectedItem.UserObject as ServerPluginInfo;
            if (pluginInfo != null && pluginInfo.Download != null)
            {
                pluginInfo.Download.cancelDownload(DownloadPostAction.DeleteFile);
            }
        }

        public void downloadCompleted(Download download)
        {
            if (activeNotDisposed)
            {
                ButtonGridItem downloadingItem = (ButtonGridItem)download.UserObject;
                ServerPluginInfo pluginInfo = downloadingItem.UserObject as ServerPluginInfo;
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(downloadingItem);
                if (download.Successful)
                {
                    pluginGrid.addItem("Installed", pluginInfo.Name, pluginInfo.ImageKey);
                    detectedServerPlugins.Remove(pluginInfo);
                }
                else
                {
                    if (!download.Cancel)
                    {
                        MessageBox.show("There was an error downloading this plugin. Please try again later.", "Plugin Download Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
                    }
                    ButtonGridItem item = pluginGrid.addItem("Not Installed", pluginInfo.Name, pluginInfo.ImageKey);
                    item.UserObject = pluginInfo;
                }
                pluginInfo.Download = null;
                pluginGrid.SuppressLayout = false;
                pluginGrid.layout();
            }
        }

        public void updateStatus(Download download)
        {
            if (activeNotDisposed)
            {
                ButtonGridItem downloadingItem = (ButtonGridItem)download.UserObject;
                ServerPluginInfo pluginInfo = downloadingItem.UserObject as ServerPluginInfo;
                downloadingItem.Caption = String.Format("{0} - {1}%\n{2} of {3} (MB)", pluginInfo.Name, (int)((float)download.TotalRead / download.TotalSize * 100.0f), (download.TotalRead * BYTES_TO_MEGABYTES).ToString("N2"), (download.TotalSize * BYTES_TO_MEGABYTES).ToString("N2"));
            }
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
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream serverDataStream = response.GetResponseStream())
                        {
                            //----------------------
                            ///Modify this to read directly
                            ///-----------------------
                            using (MemoryStream localDataStream = new MemoryStream())
                            {
                                byte[] buffer = new byte[8 * 1024];
                                int len;
                                while ((len = serverDataStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    localDataStream.Write(buffer, 0, len);
                                }
                                localDataStream.Seek(0, SeekOrigin.Begin);
                                try
                                {
                                    using (BinaryReader streamReader = new BinaryReader(localDataStream))
                                    {
                                        while (streamReader.PeekChar() != -1)
                                        {
                                            ServerPluginInfo pluginInfo = new ServerPluginInfo(streamReader.ReadInt32(), streamReader.ReadString());
                                            pluginInfoList.Add(pluginInfo);
                                            String imageURL = streamReader.ReadString();
                                            if (!String.IsNullOrEmpty(imageURL))
                                            {
                                                using (Bitmap image = loadImageFromURL(imageURL))
                                                {
                                                    if (image != null)
                                                    {
                                                        ThreadManager.invokeAndWait(new Action(delegate()
                                                        {
                                                            pluginInfo.ImageKey = serverImages.addImage(pluginInfo, image);
                                                        }));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (EndOfStreamException)
                                {
                                    //At end of stream
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ThreadManager.invoke(new Action(delegate()
                {
                    MessageBox.show(String.Format("Error reading plugin data from the server. Please try again later.\nReason: {0}", e.Message), "Server Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                }));
            }

            return pluginInfoList;
        }

        //Runs on background thread
        private Bitmap loadImageFromURL(String url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(MedicalConfig.WebsiteImagesBaseURL + url));
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            return new Bitmap(responseStream);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ThreadManager.invoke(new Action(delegate()
                {
                    Log.Error("Could not load image from {0} because {1}.", url, e.Message);
                }));
            }
            return null;
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
