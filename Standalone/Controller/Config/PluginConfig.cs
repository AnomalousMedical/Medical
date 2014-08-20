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
        }

        public int findPlugins()
        {
            plugins.AddRange(findLoadableFiles(defaultPluginsFolder));
            return plugins.Count;
        }

        public void findConfiguredPlugins(ConfigFile configFile)
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

        public void addAdditionalPluginFile(String file)
        {
            plugins.Add(file);
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

        private IEnumerable<String> findLoadableFiles(String path)
        {
            foreach (String result in Directory.EnumerateFiles(path, "*.dat", SearchOption.TopDirectoryOnly))
            {
                yield return result;
            }
            foreach (String result in Directory.EnumerateFiles(path, "*.dll", SearchOption.TopDirectoryOnly))
            {
                yield return result;
            }
        }
    }
}
