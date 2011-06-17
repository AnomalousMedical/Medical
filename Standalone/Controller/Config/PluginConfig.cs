using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class PluginConfig
    {
        private List<String> additionalPlugins = new List<string>();

        public PluginConfig()
        {

        }

        public void readPlugins(ConfigFile configFile)
        {
            String[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i] == "-plugin")
                {
                    if (++i < args.Length)
                    {
                        additionalPlugins.Add(args[i]);
                    }
                }
            }
            ConfigSection section = configFile.createOrRetrieveConfigSection("Plugins");
            ConfigIterator iter = new ConfigIterator(section, "Plugin");
            while (iter.hasNext())
            {
                additionalPlugins.Add(iter.next());
            }
        }

        public IEnumerable<String> AdditionalPlugins
        {
            get
            {
                return additionalPlugins;
            }
        }
    }
}
