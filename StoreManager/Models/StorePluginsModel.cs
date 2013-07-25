using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anomalous.Medical.StoreManager.Models
{
    public class StorePluginsModel : Saveable
    {
        private List<PluginModel> plugins = new List<PluginModel>();

        public StorePluginsModel()
        {

        }

        public StorePluginsModel(IEnumerable<PluginModel> plugins)
        {
            this.plugins.AddRange(plugins);
        }

        protected StorePluginsModel(LoadInfo info)
        {
            info.RebuildList("Plugin", plugins);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList("Plugin", plugins);
        }

        public IEnumerable<PluginModel> Plugins
        {
            get
            {
                return plugins;
            }
        }
    }
}
