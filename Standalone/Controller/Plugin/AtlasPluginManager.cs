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
using System.Security.Cryptography;

namespace Medical
{
    public class AtlasPluginManager : IDisposable
    {
        private StandaloneController standaloneController;
        private List<AtlasPlugin> plugins = new List<AtlasPlugin>();
        private List<AtlasPlugin> uninitializedPlugins = new List<AtlasPlugin>();
        private List<AtlasPlugin> unlicensedPlugins = new List<AtlasPlugin>();
        private Dictionary<String, Assembly> artworkPluginAssemblies = new Dictionary<string, Assembly>();
        private SimScene currentScene;
        private String additionalSearchPath;
        private HashSet<String> loadedPluginNames = new HashSet<string>();
        private bool addedPluginsToMyGUIResourceGroup = false;
        private ManagePluginInstructions managePluginInstructions;
        private HashSet<long> usedPluginIds = new HashSet<long>();
        private HashSet<long> loadedDependencyPluginIds = new HashSet<long>();

        public delegate void PluginMessageDelegate(String message);
        public event PluginMessageDelegate PluginLoadError;

        /// <summary>
        /// This event is fired if the dependencies for a given plugin are requested to be downloaded.
        /// </summary>
        public event Action<AtlasPlugin> RequestDependencyDownload;

        private DataFileVerifier dataFileVerifier;

        private Engine.Resources.ResourceManager resourceManager;

        public AtlasPluginManager(StandaloneController standaloneController, DataFileVerifier dataFileVerifier)
        {
            this.dataFileVerifier = dataFileVerifier;
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
                plugin.Dispose();
            }

            standaloneController.SceneLoaded -= standaloneController_SceneLoaded;
            standaloneController.SceneUnloading -= standaloneController_SceneUnloading;
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

        public void addPluginToUninstall(AtlasPlugin plugin)
        {
            if (plugin.AllowUninstall)
            {
                managePluginInstructions.addFileToDelete(plugin.Location);
                managePluginInstructions.savePersistantFile();
            }
        }

        public bool addPlugin(String pluginPath)
        {
            if (pluginPath.EndsWith(".dll", true, CultureInfo.InvariantCulture))
            {
                return addDllPlugin(pluginPath);
            }
            else
            {
                return addDataDrivenPlugin(pluginPath);
            }
        }

        private bool addDllPlugin(String dllName)
        {
            bool loadedPlugin = false;
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
                    if (dataFileVerifier.isSafeDll(fullPath))
                    {
                        try
                        {
                            Assembly assembly = Assembly.LoadFile(fullPath);

                            //Always set dlls as loaded even if they are corrupted. If we get this far the dll is valid, but might not actually work.
                            loadedPluginNames.Add(dllFileName);

                            AtlasPluginEntryPointAttribute[] attributes = (AtlasPluginEntryPointAttribute[])assembly.GetCustomAttributes(typeof(AtlasPluginEntryPointAttribute), true);
                            if (attributes.Length > 0)
                            {
                                foreach (AtlasPluginEntryPointAttribute entryPointAttribute in attributes)
                                {
                                    entryPointAttribute.createPlugin(standaloneController);
                                }
                                loadedPlugin = true;
                            }
                            else
                            {
                                String errorMessage = String.Format("Cannot find AtlasPluginEntryPointAttribute in assembly '{0}'. Please add this property to the assembly.", assembly.FullName);
                                firePluginLoadError(errorMessage);
                                Log.Error(errorMessage);
                            }
                        }
                        catch (Exception e)
                        {
                            firePluginLoadError(String.Format("There was an error loading the plugin '{0}'.", dllFileName));
                            Log.Error("Cannot load dll '{0}' from '{1}' because: {2}. Deleting corrupted plugin.", dllName, fullPath, e.Message);
                            try
                            {
                                File.Delete(fullPath);
                            }
                            catch (Exception deleteEx)
                            {
                                Log.Error("Error deleting dll file '{0}' from '{1}' because: {2}.", dllName, fullPath, deleteEx.Message);
                                managePluginInstructions.addFileToDelete(fullPath);
                                managePluginInstructions.savePersistantFile();
                            }
                        }
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
            return loadedPlugin;
        }

        private bool addDataDrivenPlugin(String path)
        {
            bool loadedPlugin = false;
            String pluginDirectory = null;
            String fullPath = Path.GetFullPath(path);
            if (!Directory.Exists(fullPath) && !File.Exists(fullPath))
            {
                fullPath = Path.Combine(additionalSearchPath, path);
            }

            if (File.Exists(fullPath))
            {
                if (dataFileVerifier.isSafeDataFile(fullPath))
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
        }

        private void addPlugin(AtlasPlugin plugin, bool addAssemblyResources)
        {
            if (standaloneController.App.LicenseManager.allowFeature(plugin.PluginId) && !usedPluginIds.Contains(plugin.PluginId))
            {
                uninitializedPlugins.Add(plugin);
#if ALLOW_OVERRIDE
                if (plugin.PluginId != -1)
#endif
                {
                    usedPluginIds.Add(plugin.PluginId);
                }
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
        }

        private void addDependency(AtlasDependency dependency)
        {
            uninitializedPlugins.Insert(0, dependency);
            if(!loadedDependencyPluginIds.Contains(dependency.PluginId))
            {
                loadedDependencyPluginIds.Add(dependency.PluginId);
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

        public void initializePlugins()
        {
            foreach (var status in initializePluginsStatus()) { }
        }

        public IEnumerable<PluginLoadStatus> initializePluginsStatus()
        {
            PluginLoadStatus loadStatus = new PluginLoadStatus()
            {
                Total = uninitializedPlugins.Count
            };

            //If we already added the plugins folder to MyGUI, remove it.
            if (addedPluginsToMyGUIResourceGroup)
            {
                MyGUIInterface.Instance.CommonResourceGroup.removeResource("Plugins");
            }

            //If a plugins folder exists in the virtual file system add it to the MyGUI group.
            if (VirtualFileSystem.Instance.exists("Plugins"))
            {
                MyGUIInterface.Instance.CommonResourceGroup.addResource("Plugins", "ScalableEngineArchive", true);
                addedPluginsToMyGUIResourceGroup = true;
            }

            foreach (AtlasPlugin plugin in uninitializedPlugins)
            {
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

                loadStatus.Current++;
                yield return loadStatus;
            }
            uninitializedPlugins.Clear();
        }

        public bool allDependenciesLoaded()
        {
            foreach(AtlasPlugin plugin in plugins)
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

        internal void setMainInterfaceEnabled(bool enabled)
        {
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.setMainInterfaceEnabled(enabled);
            }
        }

        internal void sceneRevealed()
        {
            foreach (AtlasPlugin plugin in plugins)
            {
                plugin.sceneRevealed();
            }
        }

        /// <summary>
        /// Fire a request to download the dependencies for a given plugin.
        /// </summary>
        /// <param name="plugin">The plugin to download dependencies for.</param>
        public void requestDependencyDownloadFor(AtlasPlugin plugin)
        {
            if(RequestDependencyDownload != null)
            {
                RequestDependencyDownload(plugin);
            }
        }

        public int UninitalizedCount
        {
            get
            {
                return uninitializedPlugins.Count;
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