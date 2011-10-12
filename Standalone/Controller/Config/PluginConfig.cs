using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.IO;

namespace Medical
{
    public class PluginConfig
    {
        private List<String> plugins = new List<string>();
        private String defaultPluginsFolder;

        public PluginConfig(String defaultPluginsFolder)
        {
            this.defaultPluginsFolder = defaultPluginsFolder;
            if (!Directory.Exists(defaultPluginsFolder))
            {
                Directory.CreateDirectory(defaultPluginsFolder);
            }
            ManagePluginsFile = Path.Combine(defaultPluginsFolder, "ManagePlugins.xml");
        }

        public void findRegularPlugins()
        {
            String[] pluginFiles = Directory.GetFiles(defaultPluginsFolder, "*.dat", SearchOption.TopDirectoryOnly);
            foreach (String plugin in pluginFiles)
            {
                plugins.Add(plugin);
            }
            pluginFiles = Directory.GetFiles(defaultPluginsFolder, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (String plugin in pluginFiles)
            {
                plugins.Add(plugin);
            }
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
                        plugins.Add(args[i]);
                    }
                }
            }
            ConfigSection section = configFile.createOrRetrieveConfigSection("Plugins");
            ConfigIterator iter = new ConfigIterator(section, "Plugin");
            while (iter.hasNext())
            {
                plugins.Add(iter.next());
            }
        }

        public IEnumerable<String> Plugins
        {
            get
            {
                return plugins;
            }
        }

        public String PluginsFolder
        {
            get
            {
                return defaultPluginsFolder;
            }
        }

        public String ManagePluginsFile { get; private set; }
    }
}
