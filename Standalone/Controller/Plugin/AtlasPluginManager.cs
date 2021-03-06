﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using OgrePlugin;
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
using System.Security.Cryptography;

namespace Medical
{
    public class AtlasPluginManager : IDisposable
    {
        private StandaloneController standaloneController;
        private List<AtlasPlugin> plugins = new List<AtlasPlugin>();
        private List<AtlasPlugin> unlicensedPlugins = new List<AtlasPlugin>();
        private Dictionary<String, Assembly> artworkPluginAssemblies = new Dictionary<string, Assembly>();
        private SimScene currentScene;
        private String additionalSearchPath;
        private HashSet<String> loadedPluginNames = new HashSet<string>();
        private bool addedPluginsToMyGUIResourceGroup = false;
        private ManagePluginInstructions managePluginInstructions;
        private HashSet<long> usedPluginIds = new HashSet<long>();
        private HashSet<long> loadedDependencyPluginIds = new HashSet<long>();
        private Version requiredAssemblyVersion;

        public delegate void PluginMessageDelegate(String message);
        public event PluginMessageDelegate PluginLoadError;
        public event Action<AtlasPlugin> PluginUnloading;

        /// <summary>
        /// This event is fired if the dependencies for a given plugin are requested to be downloaded.
        /// </summary>
        public event Action<AtlasPlugin> RequestDependencyDownload;

        private Engine.Resources.ResourceManager resourceManager;

        public AtlasPluginManager(StandaloneController standaloneController)
        {
            requiredAssemblyVersion = standaloneController.GetType().Assembly.GetName().Version;
            this.standaloneController = standaloneController;
            standaloneController.SceneLoaded += standaloneController_SceneLoaded;
            standaloneController.SceneUnloading += standaloneController_SceneUnloading;

            additionalSearchPath = FolderFinder.ExecutableFolder;

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(FindArtworkPluginAssembly);

            managePluginInstructions = new ManagePluginInstructions(MedicalConfig.PluginConfig.PluginsFolder);

            resourceManager = PluginManager.Instance.createLiveResourceManager("Plugins");
        }

        public void Dispose()
        {
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.unload(standaloneController, false, true);
                plugin.Dispose();
            }

            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.SceneUnloading -= standaloneController_SceneUnloading;
        }

        public bool isPluginPathSafeToLoad(String path)
        {
            String fullPath = Path.GetFullPath(path);
            if (File.Exists(fullPath))
            {
                return fullPath.EndsWith(".dat", StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                return true;
            }
        }

        public void manageInstalledPlugins()
        {
            var loadedInstructions = ManagePluginInstructions.restore(MedicalConfig.PluginConfig.PluginsFolder);
            if (loadedInstructions != null)
            {
                loadedInstructions.process();
                loadedInstructions.deletePersistantFile();
            }
        }

        public void addPluginToMove(String path)
        {
            managePluginInstructions.addFileToMove(path);
            managePluginInstructions.savePersistantFile();
        }

        /// <summary>
        /// Uninstall a plugin, will return true if the program needs to restart to finish uninstalling.
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        public bool uninstallPlugin(AtlasPlugin plugin, bool willReload)
        {
            bool needsRestart = false;
            String location = plugin.Location;
            if (plugin.AllowUninstall)
            {
                if (plugin.AllowRuntimeUninstall)
                {
                    if (PluginUnloading != null)
                    {
                        PluginUnloading.Invoke(plugin);
                    }
                    unloadPlugin(plugin, willReload);
                    try
                    {
                        File.Delete(location);
                    }
                    catch (Exception deleteEx)
                    {
                        Log.Error("Error deleting data file '{0}' from '{1}' because: {2}.", location, location, deleteEx.Message);
                        managePluginInstructions.addFileToDelete(location);
                        managePluginInstructions.savePersistantFile();
                        needsRestart = true;
                    }
                }
                else
                {
                    managePluginInstructions.addFileToDelete(location);
                    managePluginInstructions.savePersistantFile();
                    needsRestart = true;
                }
            }
            return needsRestart;
        }

        /// <summary>
        /// Add a plugin from a path, you must pass if the plugin is safe to load or not, which
        /// can be determined by calling isPluginSafeToLoad, which will check signatures on the file.
        /// For performance you might want to call isPluginSafeToLoad on a background thread and then
        /// call this function with the result on the main thread (must load plugins on the main thread). 
        /// It is not reccomended to force this setting to true or false.
        /// </summary>
        /// <param name="pluginPath">The path to load.</param>
        /// <param name="isSafeToLoad">True if the plugin is safe to load and false otherwise.</param>
        /// <returns>True if the plugin is loaded sucessfully.</returns>
        public bool addPlugin(String pluginPath, bool isSafeToLoad)
        {
            if (isSafeToLoad)
            {
                if (pluginPath.EndsWith(".dat", true, CultureInfo.InvariantCulture))
                {
                    return addDataDrivenPlugin(pluginPath);
                }
            }
            return false;
        }

        private bool addDataDrivenPlugin(String path)
        {
            bool loadedPlugin = false;
            String pluginDirectory = null;
            String fullPath;
            //Try to load from virtual file system first
            if (VirtualFileSystem.Instance.directoryExists(path))
            {
                fullPath = path;
                String directoryName = Path.GetFileName(Path.GetDirectoryName(path));
                if (!loadedPluginNames.Contains(directoryName))
                {
                    loadedPluginNames.Add(directoryName);

                    pluginDirectory = String.Format("Plugins/{0}/", directoryName);
                }
                else
                {
                    Log.Error("Cannot plugin file '{0}' from virtual file system because a plugin named '{1}' is already loaded.", path, directoryName);
                }
            }
            else
            {

                fullPath = Path.GetFullPath(path);
                if (!Directory.Exists(fullPath) && !File.Exists(fullPath))
                {
                    fullPath = Path.Combine(additionalSearchPath, path);
                }

                if (File.Exists(fullPath))
                {
                    String dataFileName = Path.GetFileNameWithoutExtension(fullPath);
                    try
                    {
                        if (!loadedPluginNames.Contains(dataFileName))
                        {
                            //Add the archive to the VirtualFileSystem if needed
                            if (!VirtualFileSystem.Instance.containsRealAbsolutePath(fullPath))
                            {
                                VirtualFileSystem.Instance.addArchive(fullPath);
                            }
                            pluginDirectory = String.Format("Plugins/{0}/", Path.GetFileNameWithoutExtension(path));

                            loadedPluginNames.Add(dataFileName);
                        }
                        else
                        {
                            Log.Error("Cannot load data file '{0}' from '{1}' because a plugin named '{2}' is already loaded.", path, fullPath, dataFileName);
                        }
                    }
                    catch (ZipAccess.ZipIOException e)
                    {
                        firePluginLoadError(String.Format("There was an error loading the plugin '{0}'.", dataFileName));
                        Log.Error("Cannot load data file '{0}' from '{1}' because of a zip read error: {2}. Deleting corrupted plugin.", path, fullPath, e.Message);
                        try
                        {
                            File.Delete(fullPath);
                        }
                        catch (Exception deleteEx)
                        {
                            Log.Error("Error deleting data file '{0}' from '{1}' because: {2}.", path, fullPath, deleteEx.Message);
                        }
                    }
                }
                else if (Directory.Exists(fullPath))
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
            }

            if (pluginDirectory != null)
            {
                String pluginDefinitionFile = pluginDirectory + "Plugin.ddp";

                if (VirtualFileSystem.Instance.exists(pluginDefinitionFile))
                {
                    using (Stream pluginStream = VirtualFileSystem.Instance.openStream(pluginDefinitionFile, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                    {
                        try
                        {
                            DDAtlasPlugin plugin = SharedXmlSaver.Load<DDAtlasPlugin>(pluginStream);
                            if (plugin != null)
                            {
                                plugin.Location = fullPath;
                                plugin.PluginRootFolder = pluginDirectory;
                                addPlugin(plugin, false);
                                loadedPlugin = true;
                            }
                            else
                            {
                                throw new Exception(String.Format("Error loading '{0}' in path '{1}' from '{2}' because it was null.", pluginDefinitionFile, path, fullPath));
                            }
                        }
                        catch (Exception ex)
                        {
                            firePluginLoadError(String.Format("There was an error loading the plugin '{0}'.", Path.GetFileName(fullPath)));
                            Log.Error(ex.Message);
                            try
                            {
                                File.Delete(fullPath);
                            }
                            catch (Exception deleteEx)
                            {
                                Log.Error("Error deleting data file '{0}' from '{1}' because: {2}.", path, fullPath, deleteEx.Message);
                                managePluginInstructions.addFileToDelete(fullPath);
                                managePluginInstructions.savePersistantFile();
                            }
                        }
                    }
                }

                if (!loadedPlugin)
                {
                    String dependencyDefinitionFile = pluginDirectory + "Dependency.ddd";
                    if (VirtualFileSystem.Instance.fileExists(dependencyDefinitionFile))
                    {
                        using (Stream stream = VirtualFileSystem.Instance.openStream(dependencyDefinitionFile, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                        {
                            try
                            {
                                DDAtlasDependency dependency = SharedXmlSaver.Load<DDAtlasDependency>(stream);
                                if (dependency != null)
                                {
                                    dependency.Location = fullPath;
                                    dependency.RootFolder = pluginDirectory;
                                    addDependency(dependency);
                                    loadedPlugin = true;
                                }
                                else
                                {
                                    throw new Exception(String.Format("Error loading '{0}' in path '{1}' from '{2}' because it was null.", dependencyDefinitionFile, path, fullPath));
                                }
                            }
                            catch (Exception ex)
                            {
                                firePluginLoadError(String.Format("There was an error loading the plugin '{0}'.", Path.GetFileName(fullPath)));
                                Log.Error(ex.Message);
                                try
                                {
                                    File.Delete(fullPath);
                                }
                                catch (Exception deleteEx)
                                {
                                    Log.Error("Error deleting data file '{0}' from '{1}' because: {2}.", path, fullPath, deleteEx.Message);
                                    managePluginInstructions.addFileToDelete(fullPath);
                                    managePluginInstructions.savePersistantFile();
                                }
                            }
                        }
                    }
                    else
                    {
                        Log.Error("Error loading '{0}' or '{1}' in path '{2}' from '{3}' because it does not exist.", pluginDefinitionFile, dependencyDefinitionFile, path, fullPath);
                    }
                }
            }

            return loadedPlugin;
        }

        public void addPlugin(AtlasPlugin plugin)
        {
            addPlugin(plugin, true);
            String dllFileName = Path.GetFileNameWithoutExtension(plugin.GetType().Assembly.Location);
            loadedPluginNames.Add(dllFileName);
        }

        private void addPlugin(AtlasPlugin plugin, bool addAssemblyResources)
        {
            bool initialize = false;
            if (standaloneController.LicenseManager.allowFeature(plugin.PluginId) && !usedPluginIds.Contains(plugin.PluginId))
            {
                if (plugin.PluginId != -1)
                {
                    usedPluginIds.Add(plugin.PluginId);
                }
                initialize = true;
            }
            else
            {
                unlicensedPlugins.Add(plugin);
            }
            if (addAssemblyResources)
            {
                Type pluginType = plugin.GetType();
                Assembly assembly = pluginType.Assembly;
                artworkPluginAssemblies.Add(assembly.FullName, assembly);
                MyGUIInterface.Instance.CommonResourceGroup.addResource(pluginType.AssemblyQualifiedName, "EmbeddedScalableResource", true);
            }
            if (initialize)
            {
                initializePlugin(plugin);
            }
        }

        private void unloadPlugin(AtlasPlugin plugin, bool willReload)
        {
            String dataFileName = Path.GetFileNameWithoutExtension(plugin.Location);

            plugin.unload(standaloneController, willReload, false);

            loadedPluginNames.Remove(dataFileName);
            usedPluginIds.Remove(plugin.PluginId);
            unlicensedPlugins.Remove(plugin);
            plugins.Remove(plugin);
            loadedDependencyPluginIds.Remove(plugin.PluginId); //Remove from loaded dependencies list, will only make a change if the plugin was a dependency plugin

            //Remove the archive, only supports non-dll right now
            if (!plugin.Location.EndsWith(".dll"))
            {
                VirtualFileSystem.Instance.removeArchive(plugin.Location);
            }
            //Else, we don't unload dll plugins, but if we did remove the assembly resources here

            //Remove plugins folder if it no longer exists
            if (!VirtualFileSystem.Instance.exists("Plugins"))
            {
                MyGUIInterface.Instance.CommonResourceGroup.removeResource("Plugins");
                addedPluginsToMyGUIResourceGroup = false;
            }
        }

        private void addDependency(AtlasDependency dependency)
        {
            if (!loadedDependencyPluginIds.Contains(dependency.PluginId))
            {
                loadedDependencyPluginIds.Add(dependency.PluginId);
                initializePlugin(dependency);
            }
        }

        public AtlasPlugin getPlugin(long pluginId)
        {
            foreach (AtlasPlugin plugin in plugins)
            {
                if (plugin.PluginId == pluginId)
                {
                    return plugin;
                }
            }
            return null;
        }

        public void initializePlugin(AtlasPlugin plugin)
        {
            //If a plugins folder exists in the virtual file system and it has not been found already add it to the MyGUI group.
            if (!addedPluginsToMyGUIResourceGroup && VirtualFileSystem.Instance.exists("Plugins"))
            {
                MyGUIInterface.Instance.CommonResourceGroup.addResource("Plugins", "ScalableEngineArchive", false); //Does not have to be recursive because we don't have to index
                addedPluginsToMyGUIResourceGroup = true;
            }

            try
            {
                plugin.loadGUIResources();
                plugin.initialize(standaloneController);
                if (currentScene != null)
                {
                    plugin.sceneLoaded(currentScene);
                }
                plugins.Add(plugin);
            }
            catch (Exception e)
            {
                firePluginLoadError(String.Format("There was an error loading the plugin '{0}'.", plugin.PluginName));
                Log.Error("Cannot load plugin '{0}' from '{1}' because: {2}. Deleting corrupted plugin.", plugin.PluginName, plugin.Location, e.Message);
                Log.Default.printException(e);
                try
                {
                    File.Delete(plugin.Location);
                }
                catch (Exception deleteEx)
                {
                    Log.Error("Error deleting dll file '{0}' from '{1}' because: {2}.", plugin.PluginName, plugin.Location, deleteEx.Message);
                    managePluginInstructions.addFileToDelete(plugin.Location);
                    managePluginInstructions.savePersistantFile();
                }
            }
        }

        public bool allDependenciesLoaded()
        {
            foreach (AtlasPlugin plugin in plugins)
            {
                if (!allDependenciesLoadedFor(plugin))
                {
                    return false;
                }
            }
            return true;
        }

        public bool allDependenciesLoadedFor(AtlasPlugin plugin)
        {
            foreach (long dependencyPluginId in plugin.DependencyPluginIds)
            {
                if (!loadedDependencyPluginIds.Contains(dependencyPluginId))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Fire a request to download the dependencies for a given plugin.
        /// </summary>
        /// <param name="plugin">The plugin to download dependencies for.</param>
        public void requestDependencyDownloadFor(AtlasPlugin plugin)
        {
            if (RequestDependencyDownload != null)
            {
                RequestDependencyDownload(plugin);
            }
        }

        public IEnumerable<AtlasPlugin> LoadedPlugins
        {
            get
            {
                return plugins;
            }
        }

        public IEnumerable<AtlasPlugin> UnlicensedPlugins
        {
            get
            {
                return unlicensedPlugins;
            }
        }

        public Engine.Resources.ResourceManager ResourceManager
        {
            get
            {
                return resourceManager;
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

        private Assembly FindArtworkPluginAssembly(object sender, ResolveEventArgs args)
        {
            Assembly assembly = null;
            artworkPluginAssemblies.TryGetValue(args.Name, out assembly);
            return assembly;
        }

        private void firePluginLoadError(String message)
        {
            if (PluginLoadError != null)
            {
                PluginLoadError.Invoke(message);
            }
        }
    }
}