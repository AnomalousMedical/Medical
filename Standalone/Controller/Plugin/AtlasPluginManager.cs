using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using OgreWrapper;
using Engine.ObjectManagement;
using Medical.Controller;
using Logging;
using Engine.Platform;
using Engine;
using System.Reflection;
using System.IO;
using Medical.GUI;
using Engine.Saving.XMLSaver;
using System.Xml;
using System.Globalization;

namespace Medical
{
    public class AtlasPluginManager : IDisposable
    {
        private StandaloneController standaloneController;
        private List<AtlasPlugin> plugins = new List<AtlasPlugin>();
        private List<AtlasPlugin> uninitializedPlugins = new List<AtlasPlugin>();
        private SimScene currentScene;
        private String additionalSearchPath;
        private HashSet<String> loadedPluginNames = new HashSet<string>();
        private XmlSaver xmlSaver = new XmlSaver();
        private bool addedPluginsToMyGUIResourceGroup = false;

        public AtlasPluginManager(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;

            additionalSearchPath = FolderFinder.ExecutableFolder;

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            String assemblyName = args.Name;
            foreach (AtlasPlugin plugin in plugins)
            {
                Assembly assembly = plugin.GetType().Assembly;
                if (assembly.FullName == assemblyName)
                {
                    return assembly;
                }
            }
            foreach (AtlasPlugin plugin in uninitializedPlugins)
            {
                Assembly assembly = plugin.GetType().Assembly;
                if (assembly.FullName == assemblyName)
                {
                    return assembly;
                }
            }
            return null;
        }

        public void Dispose()
        {
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.Dispose();
            }

            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.SceneUnloading -= standaloneController_SceneUnloading;
        }

        public void addPlugin(String pluginPath)
        {
            if (pluginPath.EndsWith(".dll", true, CultureInfo.InvariantCulture))
            {
                addDllPlugin(pluginPath);
            }
            else
            {
                addDataDrivenPlugin(pluginPath);
            }
        }

        public void addDllPlugin(String dllName)
        {
            String fullPath = Path.GetFullPath(dllName);
            if (!File.Exists(fullPath))
            {
                fullPath = Path.Combine(additionalSearchPath, dllName);
            }

            if (File.Exists(fullPath))
            {
                String dllFileName = Path.GetFileNameWithoutExtension(fullPath);
                if (!loadedPluginNames.Contains(dllFileName))
                {
                    loadedPluginNames.Add(dllFileName);
                    Assembly assembly = Assembly.LoadFile(fullPath);
                    AtlasPluginEntryPointAttribute[] attributes = (AtlasPluginEntryPointAttribute[])assembly.GetCustomAttributes(typeof(AtlasPluginEntryPointAttribute), true);
                    if (attributes.Length > 0)
                    {
                        foreach (AtlasPluginEntryPointAttribute entryPointAttribute in attributes)
                        {
                            entryPointAttribute.createPlugin(standaloneController);
                        }
                    }
                    else
                    {
                        Log.Error("Cannot find AtlasPluginEntryPointAttribute in assembly '{0}'. Please add this property to the assembly.", assembly.FullName);
                    }
                }
                else
                {
                    Log.Error("Cannot load Assembly '{0}' from '{1}' because a plugin named '{2}' is already loaded.", dllName, fullPath, dllFileName);
                }
            }
            else
            {
                Log.Error("Cannot load Assembly '{0}' from '{0}' or '{1}' because it was not found.", dllName, fullPath, Path.GetFullPath(dllName));
            }
        }

        public void addDataDrivenPlugin(String path)
        {
            String pluginDirectory = null;
            String fullPath = Path.GetFullPath(path);
            if (!Directory.Exists(fullPath) && !File.Exists(fullPath))
            {
                fullPath = Path.Combine(additionalSearchPath, path);
            }

            if (File.Exists(fullPath))
            {
                String dataFileName = Path.GetFileNameWithoutExtension(fullPath);
                if (!loadedPluginNames.Contains(dataFileName))
                {
                    loadedPluginNames.Add(dataFileName);

                    //Add the archive to the VirtualFileSystem if needed
                    if (!VirtualFileSystem.Instance.containsRealAbsolutePath(fullPath))
                    {
                        VirtualFileSystem.Instance.addArchive(fullPath);
                    }
                    pluginDirectory = String.Format("Plugins/{0}/", Path.GetFileNameWithoutExtension(path));
                }
                else
                {
                    Log.Error("Cannot load data file '{0}' from '{1}' because a plugin named '{2}' is already loaded.", path, fullPath, dataFileName);
                }
            }
            else if(Directory.Exists(fullPath))
            {
                String directoryName = Path.GetFileName(Path.GetDirectoryName(fullPath));
                if (!loadedPluginNames.Contains(directoryName))
                {
                    loadedPluginNames.Add(directoryName);

                    pluginDirectory = String.Format("Plugins/{0}/", directoryName);
                    String rootPluginPath = fullPath.Replace("\\", "/");
                    if (!rootPluginPath.EndsWith("/"))
                    {
                        rootPluginPath += "/";
                    }
                    rootPluginPath = rootPluginPath.Replace(pluginDirectory, "");

                    //Add the archive to the VirtualFileSystem if needed
                    if (!VirtualFileSystem.Instance.containsRealAbsolutePath(rootPluginPath))
                    {
                        VirtualFileSystem.Instance.addArchive(rootPluginPath);
                    }
                }
                else
                {
                    Log.Error("Cannot load data file '{0}' from '{1}' because a plugin named '{2}' is already loaded.", path, fullPath, directoryName);
                }
            }
            else
            {
                Log.Error("Cannot load data file '{0}' from '{0}' or '{1}' because it was not found.", path, fullPath, Path.GetFullPath(path));
            }

            if (pluginDirectory != null)
            {
                String pluginDefinitionFile = pluginDirectory + "Plugin.ddp";

                if (VirtualFileSystem.Instance.exists(pluginDefinitionFile))
                {
                    using (XmlTextReader xmlReader = new XmlTextReader(VirtualFileSystem.Instance.openStream(pluginDefinitionFile, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read)))
                    {
                        DDAtlasPlugin plugin = xmlSaver.restoreObject(xmlReader) as DDAtlasPlugin;
                        if (plugin != null)
                        {
                            plugin.PluginRootFolder = pluginDirectory;
                            addPlugin(plugin, false);
                        }
                        else
                        {
                            Log.Error("Error loading '{0}' in path '{1}' from '{2}' because it was null.", pluginDefinitionFile, path, fullPath);
                        }
                    }
                }
                else
                {
                    Log.Error("Error loading '{0}' in path '{1}' from '{2}' because it does not exist.", pluginDefinitionFile, path, fullPath);
                }
            }
        }

        public void addPlugin(AtlasPlugin plugin)
        {
            addPlugin(plugin, true);
        }

        public void addPlugin(AtlasPlugin plugin, bool addAssemblyResources)
        {
            uninitializedPlugins.Add(plugin);
            if (addAssemblyResources)
            {
                OgreResourceGroupManager.getInstance().addResourceLocation(plugin.GetType().AssemblyQualifiedName, "EmbeddedResource", "MyGUI", true);
            }
        }

        internal void initialzePlugins()
        {
            //If we already added the plugins folder to MyGUI, remove it.
            if (addedPluginsToMyGUIResourceGroup)
            {
                OgreResourceGroupManager.getInstance().removeResourceLocation("Plugins", "MyGUI");
            }

            //If a plugins folder exists in the virtual file system add it to the MyGUI group.
            if (VirtualFileSystem.Instance.exists("Plugins"))
            {
                OgreResourceGroupManager.getInstance().addResourceLocation("Plugins", "EngineArchive", "MyGUI", true);
                addedPluginsToMyGUIResourceGroup = true;
            }

            foreach (AtlasPlugin plugin in uninitializedPlugins)
            {
                plugin.initialize(standaloneController);
                if (currentScene != null)
                {
                    plugin.sceneLoaded(currentScene);
                }
                plugins.Add(plugin);
            }
            uninitializedPlugins.Clear();
        }

        internal void setMainInterfaceEnabled(bool enabled)
        {
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.setMainInterfaceEnabled(enabled);
            }
        }

        internal void createMenus(NativeMenuBar menu)
        {
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.createMenuBar(menu);
            }
        }

        internal void sceneRevealed()
        {
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.sceneRevealed();
            }
        }
        
        private void standaloneController_SceneUnloading(SimScene scene)
        {
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.sceneUnloading(scene);
            }
            currentScene = null;
        }

        private void standaloneController_SceneLoaded(SimScene scene)
        {
            currentScene = scene;
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.sceneLoaded(scene);
            }
        }
    }
}