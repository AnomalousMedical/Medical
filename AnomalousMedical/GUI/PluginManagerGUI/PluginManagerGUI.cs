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
    class PluginManagerGUI : AbstractFullscreenGUIPopup, DownloadUIDisplay
    {
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
        private bool displayRestartMessage = false;
        private bool allowRestartMessageDisplay = true;
        private PluginDownloadServer downloadServer;
        
        public PluginManagerGUI(AtlasPluginManager pluginManager, LicenseManager licenseManager, DownloadController downloadController, GUIManager guiManager)
            :base("Medical.GUI.PluginManagerGUI.PluginManagerGUI.layout", guiManager)
        {
            this.pluginManager = pluginManager;
            this.licenseManager = licenseManager;
            this.downloadController = downloadController;

            downloadServer = new PluginDownloadServer(licenseManager);

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
            downloadServer.Dispose();
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

                readingServerPluginInfo = true;
                readingInfo.Visible = true;
                downloadServer.readPluginInfoFromServer(detectedPluginIds, addNotInstalledPlugins);
            }
        }

        private void addNotInstalledPlugins(List<ServerDownloadInfo> downloadInfo)
        {
            if (activeNotDisposed)
            {
                pluginGrid.SuppressLayout = true;

                foreach (ServerDownloadInfo download in downloadInfo)
                {
                    addDownloadToButtonGrid(download);
                }

                pluginGrid.SuppressLayout = false;
                pluginGrid.layout();

                readingServerPluginInfo = false;
                readingInfo.Visible = false;
            }
        }

        private void addDownloadToButtonGrid(ServerDownloadInfo download)
        {
            String group = "";
            switch (download.Status)
            {
                case ServerDownloadStatus.NotInstalled:
                    group = "Not Installed";
                    break;
                case ServerDownloadStatus.Update:
                    group = "Update";
                    break;
            }
            ButtonGridItem item = pluginGrid.addItem(group, download.Name, download.ImageKey);
            item.UserObject = download;
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
            ServerDownloadInfo pluginInfo = selectedItem.UserObject as ServerDownloadInfo;
            if (pluginInfo != null)
            {
                downloadItem(selectedItem, pluginInfo);
            }
        }

        void pluginGrid_ItemActivated(object sender, EventArgs e)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            ServerDownloadInfo pluginInfo = selectedItem.UserObject as ServerDownloadInfo;
            if (pluginInfo != null && pluginInfo.Download == null)
            {
                downloadItem(selectedItem, pluginInfo);
            }
        }

        private void downloadItem(ButtonGridItem selectedItem, ServerDownloadInfo downloadInfo)
        {
            pluginGrid.SuppressLayout = true;
            pluginGrid.removeItem(selectedItem);
            ButtonGridItem downloadingItem = pluginGrid.addItem("Downloading", String.Format("{0} - {1}", downloadInfo.Name, "Starting Download"), downloadInfo.ImageKey);
            downloadingItem.UserObject = downloadInfo;
            pluginGrid.SuppressLayout = false;
            pluginGrid.layout();

            downloadInfo.UserObject = downloadingItem;
            downloadInfo.startDownload(downloadController, this);
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            ServerDownloadInfo pluginInfo = selectedItem.UserObject as ServerDownloadInfo;
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
                ServerDownloadInfo pluginInfo = downloadingItem.UserObject as ServerDownloadInfo;
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(downloadingItem);
                if (download.Successful)
                {
                    pluginGrid.addItem("Installed", pluginInfo.Name, pluginInfo.ImageKey);
                    //PluginDownload pluginDownload = (PluginDownload)download;
                    //if (pluginDownload.LoadedSucessfully)
                    //{
                    //    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //    //Need to remove the plugin from the download list.
                    //    //detectedServerPlugins.Remove(pluginInfo);
                    //}
                    //else
                    //{
                    //    displayRestartMessage = true;
                    //}
                    //if (!downloadController.Downloading && displayRestartMessage)
                    //{
                    //    displayRestartMessage = false;
                    //    if (allowRestartMessageDisplay)
                    //    {
                    //        allowRestartMessageDisplay = false;
                    //        MessageBox.show("You must restart Anomalous Medical in order to use some of the plugins you have downloaded.", "Restart Required", MessageBoxStyle.IconInfo | MessageBoxStyle.Ok, new MessageBox.MessageClosedDelegate(delegate(MessageBoxStyle result)
                    //        {
                    //            allowRestartMessageDisplay = true;
                    //        }));
                    //    }
                    //}
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
                pluginGrid.SuppressLayout = false;
                pluginGrid.layout();
            }
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

        #region DownloadUIDisplay Members

        public void updateStatus(ServerDownloadInfo downloadInfo, string status)
        {
            if (activeNotDisposed)
            {
                ButtonGridItem downloadingItem = (ButtonGridItem)downloadInfo.UserObject;
                downloadingItem.Caption = status;
            }
        }

        public void downloadSuccessful(ServerDownloadInfo downloadInfo)
        {
            if (activeNotDisposed)
            {
                ButtonGridItem downloadingItem = (ButtonGridItem)downloadInfo.UserObject;
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(downloadingItem);
                pluginGrid.addItem("Installed", downloadInfo.Name, downloadInfo.ImageKey);
                //PluginDownload pluginDownload = (PluginDownload)download;
                //if (pluginDownload.LoadedSucessfully)
                //{
                //    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //    //Need to remove the plugin from the download list.
                //    //detectedServerPlugins.Remove(pluginInfo);
                //}
                //else
                //{
                //    displayRestartMessage = true;
                //}
                //if (!downloadController.Downloading && displayRestartMessage)
                //{
                //    displayRestartMessage = false;
                //    if (allowRestartMessageDisplay)
                //    {
                //        allowRestartMessageDisplay = false;
                //        MessageBox.show("You must restart Anomalous Medical in order to use some of the plugins you have downloaded.", "Restart Required", MessageBoxStyle.IconInfo | MessageBoxStyle.Ok, new MessageBox.MessageClosedDelegate(delegate(MessageBoxStyle result)
                //        {
                //            allowRestartMessageDisplay = true;
                //        }));
                //    }
                //}
                pluginGrid.SuppressLayout = false;
                pluginGrid.layout();
            }
        }

        public void downloadFailed(ServerDownloadInfo downloadInfo)
        {
            if (activeNotDisposed)
            {
                ButtonGridItem downloadingItem = (ButtonGridItem)downloadInfo.UserObject;
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(downloadingItem);
                MessageBox.show(String.Format("There was an error downloading {0}.\nPlease try again later.", downloadInfo.Name), "Download Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
                addDownloadToButtonGrid(downloadInfo);
                pluginGrid.SuppressLayout = false;
                pluginGrid.layout();
            }
        }

        public void downloadCanceled(ServerDownloadInfo downloadInfo)
        {
            if (activeNotDisposed)
            {
                ButtonGridItem downloadingItem = (ButtonGridItem)downloadInfo.UserObject;
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(downloadingItem);
                addDownloadToButtonGrid(downloadInfo);
                pluginGrid.SuppressLayout = false;
                pluginGrid.layout();
            }
        }

        #endregion
    }
}
