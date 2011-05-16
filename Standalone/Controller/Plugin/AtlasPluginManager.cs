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

        public AtlasPluginManager(StandaloneController standaloneController)
        {
            this.standaloneController = standaloneController;
            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;
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
            if (File.Exists(fullPath))
            {
                Assembly assembly = Assembly.LoadFile(fullPath);
                object[] attributes = assembly.GetCustomAttributes(typeof(AtlasPluginEntryPointAttribute), true);
                if (attributes.Length > 0)
                {
                    AtlasPluginEntryPointAttribute entryPointAttribute = (AtlasPluginEntryPointAttribute)attributes[0];
                    entryPointAttribute.createPlugin(standaloneController);
                }
                else
                {
                    Log.Error("Cannot find AtlasPluginEntryPointAttribute in assembly {0}. Please add this property to the assembly.", assembly.FullName);
                }
            }
            else
            {
                Log.Error("Cannot find Assembly {0}.", fullPath);
            }
        }

        public void addPlugin(AtlasPlugin plugin)
        {
            OgreResourceGroupManager.getInstance().addResourceLocation(plugin.GetType().AssemblyQualifiedName, "EmbeddedResource", "MyGUI", true);
            plugins.Add(plugin);
        }

        internal void initializeGUI(GUIManager guiManager)
        {
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.initializeGUI(standaloneController, guiManager);
            }
        }

        internal void createDialogs(DialogManager dialogManager)
        {
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.createDialogs(dialogManager);
            }
        }

        internal void addToTaskbar(Taskbar taskbar)
        {
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.addToTaskbar(taskbar);
            }
        }

        internal void finishInitialization()
        {
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.finishInitialization();
            }
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
        
        private void standaloneController_SceneUnloading(SimScene scene)
        {
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.sceneUnloading(scene);
            }
        }

        private void standaloneController_SceneLoaded(SimScene scene)
        {
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.sceneLoaded(scene);
            }
        }
    }
}