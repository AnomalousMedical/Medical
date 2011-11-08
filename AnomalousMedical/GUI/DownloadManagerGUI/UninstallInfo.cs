using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class UninstallInfo
    {
        private AtlasPlugin plugin;

        public UninstallInfo(AtlasPlugin plugin)
        {
            this.plugin = plugin;
        }

        public void uninstall(AtlasPluginManager pluginManager)
        {
            pluginManager.addPluginToUninstall(plugin);
        }

        public String ImageKey
        {
            get
            {
                return plugin.BrandingImageKey;
            }
        }

        public String Name
        {
            get
            {
                return plugin.PluginName;
            }
        }
    }
}
