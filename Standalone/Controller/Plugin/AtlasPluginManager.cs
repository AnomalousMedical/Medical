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

namespace Medical
{
    public class AtlasPluginManager : IDisposable
    {
        private StandaloneController standaloneController;
        private List<AtlasPlugin> plugins = new List<AtlasPlugin>();
        private List<AtlasPlugin> uninitializedPlugins = new List<AtlasPlugin>();
        private SimScene currentScene;
        private String additionalSearchPath;
        private HashSet<String> loadedPluginDlls = new HashSet<string>();

        public AtlasPluginManager(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;

            additionalSearchPath = MedicalConfig.ProgramDirectory;

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

        public void addPlugin(String dllName)
        {
            String fullPath = Path.GetFullPath(dllName);
            if (!File.Exists(fullPath))
            {
                fullPath = Path.Combine(additionalSearchPath, dllName);
            }

            if (File.Exists(fullPath))
            {
                if (!loadedPluginDlls.Contains(fullPath))
                {
                    loadedPluginDlls.Add(fullPath);
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
                        Log.Error("Cannot find AtlasPluginEntryPointAttribute in assembly {0}. Please add this property to the assembly.", assembly.FullName);
                    }
                }
                else
                {
                    Log.Error("Cannot load Assembly {0} from {1} because it is already loaded.", dllName, fullPath);
                }
            }
            else
            {
                Log.Error("Cannot load Assembly {0} from {0} or {1}.", dllName, fullPath, Path.GetFullPath(dllName));
            }
        }

        public void addPlugin(AtlasPlugin plugin)
        {
            uninitializedPlugins.Add(plugin);
            OgreResourceGroupManager.getInstance().addResourceLocation(plugin.GetType().AssemblyQualifiedName, "EmbeddedResource", "MyGUI", true);
        }

        internal void initialzePlugins()
        {
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