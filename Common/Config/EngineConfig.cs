using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class EngineConfig
    {
        private ConfigSection section;

        public EngineConfig(ConfigFile configFile)
        {
            section = configFile.createOrRetrieveConfigSection("Engine");
        }

        public int MaxFPS
        {
            get
            {
                return section.getValue("MaxFPS", 120);
            }
            set
            {
                section.setValue("MaxFPS", value);
            }
        }
    }
}
