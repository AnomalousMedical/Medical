using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class ServerPluginDownloadInfo : ServerDownloadInfo
    {
        public ServerPluginDownloadInfo(int pluginId, String name)
        {
            this.PluginId = pluginId;
            this.Name = name;
        }

        public override void startDownload(DownloadController downloadController, DownloadListener downloadListener, Object callbackObject)
        {
            Download = downloadController.downloadPlugin(PluginId, downloadListener, callbackObject);
        }

        public int PluginId { get; set; }
    }
}
