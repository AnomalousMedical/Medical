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
            :base(DownloadGUIPanel.UninstallPanel, ServerDownloadStatus.Installed)
        {
            this.plugin = plugin;
            this.ImageKey = plugin.BrandingImageKey;
            this.Name = plugin.PluginName;
        }

        public void uninstall(AtlasPluginManager pluginManager)
        {
            pluginManager.addPluginToUninstall(plugin);
            Status = ServerDownloadStatus.PendingUninstall;
            Panel = DownloadGUIPanel.RestartPanel;
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
            }
        }
    }
}
