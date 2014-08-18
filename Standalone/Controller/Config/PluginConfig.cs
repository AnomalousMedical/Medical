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

        private List<String> dependencies = new List<string>();
        private String defaultDependenciesFolder;

        public PluginConfig(String defaultPluginsFolder, String defaultDependenciesFolder)
        {
            this.defaultPluginsFolder = defaultPluginsFolder;
            if (!Directory.Exists(defaultPluginsFolder))
            {
                Directory.CreateDirectory(defaultPluginsFolder);
            }

            this.defaultDependenciesFolder = defaultDependenciesFolder;
            if(!Directory.Exists(defaultDependenciesFolder))
            {
                Directory.CreateDirectory(defaultDependenciesFolder);
            }

            ManagePluginsFile = Path.Combine(defaultPluginsFolder, "ManagePlugins.xml");
        }

        public int findRegularPluginsAndDependencies()
        {
            plugins.AddRange(findLoadableFiles(defaultPluginsFolder));
            dependencies.AddRange(findLoadableFiles(defaultDependenciesFolder));

            return plugins.Count + dependencies.Count;
        }

        public void readPluginsAndDependencies(ConfigFile configFile)
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

            section = configFile.createOrRetrieveConfigSection("Dependencies");
            iter = new ConfigIterator(section, "Dependency");
            while (iter.hasNext())
            {
                dependencies.Add(iter.next());
            }
        }

        public void addAdditionalDependencyFile(String file)
        {
            dependencies.Add(file);
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

        public IEnumerable<String> Dependencies
        {
            get
            {
                return dependencies;
            }
        }

        public String PluginsFolder
        {
            get
            {
                return defaultPluginsFolder;
            }
        }

        public String DependenciesFolder
        {
            get
            {
                return defaultDependenciesFolder;
            }
        }

        public String ManagePluginsFile { get; private set; }

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
