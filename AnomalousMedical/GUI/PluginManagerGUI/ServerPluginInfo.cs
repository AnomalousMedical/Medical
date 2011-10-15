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

        protected override void doStartDownload(DownloadController downloadController)
        {
            Download = downloadController.downloadPlugin(PluginId, this, null);
        }

        public int PluginId { get; set; }
    }
}
