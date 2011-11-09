using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Medical.GUI
{
    class ServerPluginDownloadInfo : ServerDownloadInfo
    {
        private DownloadManagerServer server;

        public ServerPluginDownloadInfo(DownloadManagerServer server, int pluginId, String name, ServerDownloadStatus status)
            :base(status)
        {
            this.server = server;
            this.PluginId = pluginId;
            this.Name = name;
        }

        protected override void doStartDownload(DownloadController downloadController)
        {
            Download = downloadController.downloadPlugin(PluginId, this);
        }

        public override void downloadCompleted(Download download)
        {
            PluginDownload pluginDownload = (PluginDownload)download;
            //The plugin has been installed, so we need to remove it from the download list.
            if (pluginDownload.LoadedSucessfully)
            {
                server.removeDetectedPlugin(this);
            }
            else if(!download.Cancel)
            {
                requestRestart("A plugin you have downloaded requires Anomalous Medical to restart.", false);
            }
            base.downloadCompleted(download);
        }

        public override DownloadGUIInfo createClientDownloadInfo()
        {
            if (Download.DownloadedToSafeLocation)
            {
                return new UpdateInfo(ImageKey, Name, String.Format("You must restart Anomalous Medical to finish installing {0}. You may install more things by selecting them and clicking install before restarting.", Name), ServerDownloadStatus.PendingInstall, false);
            }
            AtlasPlugin plugin = ((PluginDownload)Download).Plugin;
            if (plugin != null)
            {
                return new UninstallInfo(plugin);
            }
            return null;
        }

        public int PluginId { get; set; }

        public override string MoreInfoURL
        {
            get
            {
                return String.Format(MedicalConfig.ProductPageBaseURL, PluginId);
            }
        }
    }
}
