using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class UninstallInfo : DownloadGUIInfo
    {
        private AtlasPlugin plugin;

        public UninstallInfo(AtlasPlugin plugin)
            :this(plugin, ServerDownloadStatus.Installed)
        {
            
        }

        public UninstallInfo(AtlasPlugin plugin, ServerDownloadStatus status)
            : base(status)
        {
            this.plugin = plugin;
            this.ImageKey = plugin.BrandingImageKey;
            this.Name = plugin.PluginName;
        }

        public void uninstall(AtlasPluginManager pluginManager)
        {
            pluginManager.addPluginToUninstall(plugin);
            Status = ServerDownloadStatus.PendingUninstall;
        }

        public override void getDescription(DescriptionFoundCallback descriptionFoundCallback)
        {
            switch (Status)
            {
                case ServerDownloadStatus.Installed:
                    descriptionFoundCallback.Invoke(String.Format("Version: {0}\nLocation: {1}", plugin.Version, plugin.Location), this);
                    break;
                case ServerDownloadStatus.PendingUninstall:
                    descriptionFoundCallback.Invoke(String.Format("You must restart Anomalous Medical to finish uninstalling {0}. You may uninstall more things by selecting them and clicking uninstall before restarting.", plugin.PluginName), this);
                    break;
                case ServerDownloadStatus.Unlicensed:
                    descriptionFoundCallback.Invoke(String.Format("You do not own {0}. You may purchase it in the Anomalous Medical Store.", plugin.PluginName), this);
                    break;
            }
        }

        public override string MoreInfoURL
        {
            get
            {
                return String.Format(MedicalConfig.ProductPageBaseURL, plugin.PluginId);
            }
        }
    }
}
