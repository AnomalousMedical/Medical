using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class ServerPluginInfo
    {
        public ServerPluginInfo(int pluginId, String name)
        {
            this.PluginId = pluginId;
            this.Name = name;
        }

        public String Name { get; set; }

        public int PluginId { get; set; }

        public Download Download { get; set; }
    }
}
