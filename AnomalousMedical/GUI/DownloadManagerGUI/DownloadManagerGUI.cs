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
    class DownloadManagerGUI : AbstractFullscreenGUIPopup
    {
        private InstallPanel installPanel;
        private ButtonGrid pluginGrid;
        private DownloadingPanel downloadPanel;
        private Widget readingInfo;

        private AtlasPluginManager pluginManager;
        private DownloadController downloadController;
        private bool activeNotDisposed = true;
        private bool addedInstalledPlugins = false;
        private bool readingServerPluginInfo = false;
        private bool displayRestartMessage = false;
        private bool autoStartUpdate = false;
        private String restartMessage = "";
        private DownloadManagerServer downloadServer;
        private NotificationGUIManager notificationManager;
        private StandaloneController standaloneController;

        public DownloadManagerGUI(StandaloneController standaloneController, DownloadManagerServer downloadServer)
            : base("Medical.GUI.DownloadManagerGUI.DownloadManagerGUI.layout", standaloneController.GUIManager)
        {
            this.standaloneController = standaloneController;
            this.pluginManager = standaloneController.AtlasPluginManager;
            this.downloadController = standaloneController.DownloadController;
            this.downloadServer = downloadServer;
            downloadServer.DownloadFound += new Action<ServerDownloadInfo>(downloadServer_DownloadFound);
            downloadServer.FinishedReadingDownloads += new Action(downloadServer_FinishedReadingDownloads);
            this.notificationManager = standaloneController.GUIManager.NotificationManager;

            pluginGrid = new ButtonGrid((ScrollView)widget.findWidget("PluginScrollList"), new ButtonGridListLayout(), new ButtonGridItemNaturalSort());
            pluginGrid.SelectedValueChanged += new EventHandler(pluginGrid_SelectedValueChanged);
            pluginGrid.ItemActivated += new EventHandler(pluginGrid_ItemActivated);
            pluginGrid.defineGroup("Downloading");
            pluginGrid.defineGroup("Updates");
            pluginGrid.defineGroup("Not Installed");
            pluginGrid.defineGroup("Pending Uninstall");
            pluginGrid.defineGroup("Pending Install");
            pluginGrid.defineGroup("Installed");

            installPanel = new InstallPanel(widget.findWidget("InstallPanel"));
            installPanel.InstallItem += installPanel_InstallItem;
            installPanel.UninstallItem += installPanel_UninstallItem;
            installPanel.Restart += installPanel_Restart;
            installPanel.Visible = false;

            downloadPanel = new DownloadingPanel(widget.findWidget("DownloadingPanel"));
            downloadPanel.CancelDownload += new EventHandler(downloadPanel_CancelDownload);
            downloadPanel.Visible = false;

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
                List<AtlasPlugin> installedPluginsList = new List<AtlasPlugin>();

                if (addedInstalledPlugins)
                {
                    foreach (AtlasPlugin plugin in pluginManager.LoadedPlugins)
                    {
                        if (plugin.PluginId != -1)
                        {
                            installedPluginsList.Add(plugin);
                        }
                    }
                }
                else
                {
                    pluginGrid.SuppressLayout = true;

                    foreach (AtlasPlugin plugin in pluginManager.LoadedPlugins)
                    {
                        if (plugin.PluginId != -1)
                        {
                            addInfoToButtonGrid(new UninstallInfo(plugin), false);
                            installedPluginsList.Add(plugin);
                        }
                        //
                        //ADD STUFF THAT ALSO HAS PLUGIN ID -1, BUT NOT AS AN UNINSTALL INFO
                        //
                    }
                    addedInstalledPlugins = true;

                    pluginGrid.SuppressLayout = false;
                    pluginGrid.layout();
                }

                readingServerPluginInfo = true;
                readingInfo.Visible = true;
                downloadServer.readPluginInfoFromServer(installedPluginsList);
            }
        }

        void downloadServer_DownloadFound(ServerDownloadInfo download)
        {
            if (activeNotDisposed)
            {
                addInfoToButtonGrid(download, false);
            }
        }

        void downloadServer_FinishedReadingDownloads()
        {
            if (activeNotDisposed)
            {
                readingServerPluginInfo = false;
                readingInfo.Visible = false;
            }
        }

        private void addInfoToButtonGrid(DownloadGUIInfo download, bool selectNewItem)
        {
            String group = "";
            switch (download.Status)
            {
                case ServerDownloadStatus.NotInstalled:
                    group = "Not Installed";
                    break;
                case ServerDownloadStatus.Update:
                    group = "Updates";
                    break;
                case ServerDownloadStatus.Downloading:
                    group = "Downloading";
                    break;
                case ServerDownloadStatus.PendingUninstall:
                    group = "Pending Uninstall";
                    break;
                case ServerDownloadStatus.PendingInstall:
                    group = "Pending Install";
                    break;
                case ServerDownloadStatus.Installed:
                    group = "Installed";
                    break;
            }
            ButtonGridItem item = pluginGrid.addItem(group, download.Name, download.ImageKey);
            item.UserObject = download;
            download.GUIItem = item;
            if (selectNewItem)
            {
                pluginGrid.SelectedItem = item;
            }
        }

        private void togglePanelVisibility()
        {
            installPanel.Visible = false;
            downloadPanel.Visible = false;
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            if (selectedItem != null)
            {
                DownloadGUIInfo downloadInfo = (DownloadGUIInfo)selectedItem.UserObject;
                if (downloadInfo != null)
                {
                    switch (downloadInfo.Status)
                    {
                        case ServerDownloadStatus.Downloading:
                            downloadPanel.Visible = true;
                            downloadPanel.setDownloadInfo((ServerDownloadInfo)selectedItem.UserObject);
                            break;
                        default:
                            installPanel.Visible = true;
                            installPanel.setDownloadInfo((DownloadGUIInfo)selectedItem.UserObject);
                            break;
                    }
                }
            }
        }

        void pluginGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            togglePanelVisibility();
        }

        void installPanel_InstallItem(DownloadGUIInfo info)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            ServerDownloadInfo pluginInfo = selectedItem.UserObject as ServerDownloadInfo;
            if (pluginInfo != null)
            {
                downloadItem(selectedItem, pluginInfo);
            }
        }

        void installPanel_UninstallItem(DownloadGUIInfo info)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            UninstallInfo pluginInfo = selectedItem.UserObject as UninstallInfo;
            if (pluginInfo != null)
            {
                pluginInfo.uninstall(pluginManager);
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(selectedItem);
                pluginGrid.SuppressLayout = false;
                addInfoToButtonGrid(pluginInfo, true);
            }
        }

        void pluginGrid_ItemActivated(object sender, EventArgs e)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            ServerDownloadInfo pluginInfo = selectedItem.UserObject as ServerDownloadInfo;
            if (pluginInfo != null && (pluginInfo.Download == null || pluginInfo.Download.Cancel))
            {
                downloadItem(selectedItem, pluginInfo);
            }
        }

        private void downloadItem(ButtonGridItem selectedItem, ServerDownloadInfo downloadInfo)
        {
            downloadInfo.startDownload(downloadController);

            pluginGrid.SuppressLayout = true;
            pluginGrid.removeItem(selectedItem);
            addInfoToButtonGrid(downloadInfo, true);
            pluginGrid.SuppressLayout = false;
            pluginGrid.layout();

            subscribeToDownload(downloadInfo);
        }

        void downloadPanel_CancelDownload(object sender, EventArgs e)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            ServerDownloadInfo pluginInfo = selectedItem.UserObject as ServerDownloadInfo;
            if (pluginInfo != null && pluginInfo.Download != null)
            {
                pluginInfo.Download.cancelDownload(DownloadPostAction.DeleteFile);
            }
        }

        void installPanel_Restart(DownloadGUIInfo info)
        {
            MessageBox.show("Restarting Anomalous Medical will loose all unsaved data. Are you sure you wish to continue?", "Restart", MessageBoxStyle.IconInfo | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
            {
                if (result == MessageBoxStyle.Yes)
                {
                    UpdateController.AutoStartUpdate = info.AutoStartUpdate;
                    standaloneController.restart();
                }
            });
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

        void downloadInfo_UpdateStatus(ServerDownloadInfo downloadInfo, string status)
        {
            if (activeNotDisposed)
            {
                downloadInfo.GUIItem.Caption = status;
            }
        }

        void downloadInfo_DownloadSuccessful(ServerDownloadInfo downloadInfo)
        {
            if (activeNotDisposed)
            {
                ButtonGridItem downloadingItem = downloadInfo.GUIItem;
                bool reselectItem = downloadingItem == pluginGrid.SelectedItem;
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(downloadingItem);
                addInfoToButtonGrid(downloadInfo.createClientDownloadInfo(), reselectItem);

                if (!downloadController.Downloading && displayRestartMessage)
                {
                    if (!widget.Visible || !reselectItem)
                    {
                        notificationManager.showRestartNotification(restartMessage + "\nClick here to do this now.", "AnomalousMedical/Download", autoStartUpdate);
                    }
                    displayRestartMessage = false;
                    autoStartUpdate = false;
                }
                else if (!Visible)
                {
                    notificationManager.showNotification(String.Format("{0} has finished downloading.", downloadInfo.Name), "AnomalousMedical/Download");
                }
                pluginGrid.SuppressLayout = false;
                pluginGrid.layout();
            }
            unsubscribeFromDownload(downloadInfo);
        }

        void downloadInfo_DownloadFailed(ServerDownloadInfo downloadInfo)
        {
            if (activeNotDisposed)
            {
                ButtonGridItem downloadingItem = downloadInfo.GUIItem;
                bool reselectItem = downloadingItem == pluginGrid.SelectedItem;
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(downloadingItem);
                MessageBox.show(String.Format("There was an error downloading {0}.\nPlease try again later.", downloadInfo.Name), "Download Error", MessageBoxStyle.IconWarning | MessageBoxStyle.Ok);
                addInfoToButtonGrid(downloadInfo, reselectItem);
                pluginGrid.SuppressLayout = false;
                pluginGrid.layout();
            }
            unsubscribeFromDownload(downloadInfo);
        }

        void downloadInfo_DownloadCanceled(ServerDownloadInfo downloadInfo)
        {
            if (activeNotDisposed)
            {
                ButtonGridItem downloadingItem = downloadInfo.GUIItem;
                bool reselectItem = downloadingItem == pluginGrid.SelectedItem;
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(downloadingItem);
                addInfoToButtonGrid(downloadInfo, reselectItem);
                pluginGrid.SuppressLayout = false;
                pluginGrid.layout();
            }
            unsubscribeFromDownload(downloadInfo);
        }

        void downloadInfo_RequestRestart(ServerDownloadInfo downloadInfo, string restartMessage, bool startPlatformUpdate)
        {
            if (!autoStartUpdate)
            {
                autoStartUpdate = startPlatformUpdate;
                displayRestartMessage = true;
                this.restartMessage = restartMessage;
            }
        }

        private void subscribeToDownload(ServerDownloadInfo downloadInfo)
        {
            downloadInfo.DownloadCanceled += downloadInfo_DownloadCanceled;
            downloadInfo.DownloadFailed += downloadInfo_DownloadFailed;
            downloadInfo.DownloadSuccessful += downloadInfo_DownloadSuccessful;
            downloadInfo.RequestRestart += downloadInfo_RequestRestart;
            downloadInfo.UpdateStatus += downloadInfo_UpdateStatus;
        }

        private void unsubscribeFromDownload(ServerDownloadInfo downloadInfo)
        {
            downloadInfo.DownloadCanceled -= downloadInfo_DownloadCanceled;
            downloadInfo.DownloadFailed -= downloadInfo_DownloadFailed;
            downloadInfo.DownloadSuccessful -= downloadInfo_DownloadSuccessful;
            downloadInfo.RequestRestart -= downloadInfo_RequestRestart;
            downloadInfo.UpdateStatus -= downloadInfo_UpdateStatus;
        }

        #endregion
    }
}
