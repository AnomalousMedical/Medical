using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anomalous.Medical.StoreManager.Models
{
    public class PluginModel : Saveable
    {
        public PluginModel()
        {

        }

        public String Name { get; set; }

        public String UniqueName { get; set; }

        public String Version { get; set; }

        protected PluginModel(LoadInfo info)
        {
            Name = info.GetString("Name");
            UniqueName = info.GetString("UniqueName");
            Version = info.GetString("Version");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Name", Name);
            info.AddValue("UniqueName", UniqueName);
            info.AddValue("Version", Version);
        }
    }
}
