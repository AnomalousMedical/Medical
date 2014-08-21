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
    class DownloadManagerGUI : AbstractFullscreenGUIPopup
    {
        private InstallPanel installPanel;
        private SingleSelectButtonGrid pluginGrid;
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
            downloadController.AllDownloadsComplete += new Action<DownloadController>(downloadController_AllDownloadsComplete);
            this.downloadServer = downloadServer;
            downloadServer.DownloadFound += new Action<ServerDownloadInfo>(downloadServer_DownloadFound);
            downloadServer.FinishedReadingDownloads += new Action(downloadServer_FinishedReadingDownloads);
            this.notificationManager = standaloneController.NotificationManager;

            pluginGrid = new SingleSelectButtonGrid((ScrollView)widget.findWidget("PluginScrollList"), new ButtonGridListLayout(), new ButtonGridItemNaturalSort());
            pluginGrid.SelectedValueChanged += new EventHandler(pluginGrid_SelectedValueChanged);
            pluginGrid.ItemActivated += pluginGrid_ItemActivated;
            pluginGrid.defineGroup("Downloading");
            pluginGrid.defineGroup("Updates");
            pluginGrid.defineGroup("Not Installed");
            pluginGrid.defineGroup("Pending Uninstall");
            pluginGrid.defineGroup("Pending Install");
            pluginGrid.defineGroup("Installed");
            pluginGrid.defineGroup("Unlicensed");

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

            Button downloadAll = (Button)widget.findWidget("DownloadAll");
            downloadAll.MouseButtonClick += new MyGUIEvent(downloadAll_MouseButtonClick);

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
                if (!addedInstalledPlugins)
                {
                    pluginGrid.SuppressLayout = true;

                    foreach (AtlasPlugin plugin in pluginManager.LoadedPlugins)
                    {
                        if (plugin.AllowUninstall)
                        {
                            addInfoToButtonGrid(new UninstallInfo(plugin), false);
                        }
                    }
                    foreach (AtlasPlugin plugin in pluginManager.UnlicensedPlugins)
                    {
                        if (plugin.AllowUninstall)
                        {
                            plugin.loadGUIResources();
                            addInfoToButtonGrid(new UninstallInfo(plugin, ServerDownloadStatus.Unlicensed), false);
                        }
                    }
                    addedInstalledPlugins = true;

                    if (UpdateController.HasUpdate)
                    {
                        addInfoToButtonGrid(new UpdateInfo("AnomalousMedicalCore/UpdateImage", String.Format("Anomalous Medical version {0}", UpdateController.DownloadedVersion), "Please restart to install the Anomalous Medical update.", ServerDownloadStatus.PendingInstall, true), false);
                    }

                    pluginGrid.SuppressLayout = false;
                    pluginGrid.layout();
                }

                readingServerPluginInfo = true;
                readingInfo.Visible = true;
                downloadServer.readPluginInfoFromServer(pluginManager);
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
            if (download != null)
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
                    case ServerDownloadStatus.Unlicensed:
                        group = "Unlicensed";
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

        void pluginGrid_ItemActivated(ButtonGridItem item)
        {
            ButtonGridItem selectedItem = pluginGrid.SelectedItem;
            ServerDownloadInfo pluginInfo = selectedItem.UserObject as ServerDownloadInfo;
            if (pluginInfo != null && (pluginInfo.Status == ServerDownloadStatus.NotInstalled || pluginInfo.Status == ServerDownloadStatus.Update))
            {
                downloadItem(selectedItem, pluginInfo);
            }
        }

        void downloadAll_MouseButtonClick(Widget source, EventArgs e)
        {
            //Quite possibly the worst way to implement this, read the stuff that can be downloaded out of the button grid.
            List<ServerDownloadInfo> pendingDownloads = new List<ServerDownloadInfo>();
            int itemCount = pluginGrid.Count;
            for (int i = 0; i < pluginGrid.Count; ++i)
            {
                ServerDownloadInfo downloadInfo = pluginGrid.getItem(i).UserObject as ServerDownloadInfo;
                if (downloadInfo != null && (downloadInfo.Status == ServerDownloadStatus.NotInstalled || downloadInfo.Status == ServerDownloadStatus.Update))
                {
                    pendingDownloads.Add(downloadInfo);
                }
            }
            foreach (ServerDownloadInfo dlInfo in pendingDownloads)
            {
                downloadItem(dlInfo.GUIItem, dlInfo);
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
            standaloneController.restartWithWarning("Restarting Anomalous Medical will lose all unsaved data. Are you sure you wish to continue?", info.AutoStartUpdate, false);
        }

        protected override void layoutUpdated()
        {
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
                pluginGrid.SuppressLayout = false;
                pluginGrid.layout();
            }
            unsubscribeFromDownload(downloadInfo);
        }

        void downloadController_AllDownloadsComplete(DownloadController obj)
        {
            if (activeNotDisposed)
            {
                if (displayRestartMessage)
                {
                    notificationManager.showRestartNotification(restartMessage + "\nClick here to do this now.", "AnomalousMedical/Download", autoStartUpdate, false);
                    displayRestartMessage = false;
                    autoStartUpdate = false;
                }
                else if (!Visible)
                {
                    notificationManager.showNotification(String.Format("Your downloads are complete."), "AnomalousMedical/Download");
                }
            }
        }

        void downloadInfo_DownloadFailed(ServerDownloadInfo downloadInfo)
        {
            if (activeNotDisposed)
            {
                ButtonGridItem downloadingItem = downloadInfo.GUIItem;
                bool reselectItem = downloadingItem == pluginGrid.SelectedItem;
                pluginGrid.SuppressLayout = true;
                pluginGrid.removeItem(downloadingItem);
                if (downloadInfo.Download.RequestElevatedRestart)
                {
                    notificationManager.showRestartNotification(String.Format("There was an error downloading {0}.\nYou can restart Anomalous Medical as an Administrator and try to download again.\nClick here to restart as an Administrator.", downloadInfo.Name), MyGUIResourceNames.QuestionIcon, false, true);
                }
                else
                {
                    notificationManager.showNotification(String.Format("There was an error downloading {0}.\nPlease try again later.", downloadInfo.Name), MyGUIResourceNames.ErrorIcon);
                }
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
