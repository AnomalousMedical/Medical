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
        private SimScene currentScene;
        private String additionalSearchPath;
        private HashSet<String> loadedPluginNames = new HashSet<string>();
        private XmlSaver xmlSaver = new XmlSaver();
        private bool addedPluginsToMyGUIResourceGroup = false;

        private static RSACryptoServiceProvider rsaProvider;
        private static SHA1CryptoServiceProvider sha1Provider;

        static AtlasPluginManager()
        {
            rsaProvider = new RSACryptoServiceProvider();
            rsaProvider.FromXmlString("<RSAKeyValue><Modulus>sFBRdwESLKFtqMjfjMLrZiueRyeaNd+bbK4CFnC3tvZEnqDDs3OLajebXYSDD+MABD1DRJ+XJgKZO1XmBUW2BpK415CwHj+6cFf0/Vz4eBknoruJRJEhMyQJ4k/RTmpiSl+WCrtpPV9EuBTBnlmAFGWEX53c/v/ihqooV/DpVWE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
            sha1Provider = new SHA1CryptoServiceProvider();
        }

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
                    try
                    {
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

                        loadedPluginNames.Add(dllFileName);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Cannot load dll '{0}' from '{1}' because: {2}. Deleting corrupted plugin.", dllName, fullPath, e.Message);
                        try
                        {
                            File.Delete(fullPath);
                        }
                        catch (Exception deleteEx)
                        {
                            Log.Error("Error deleting dll file '{0}' from '{1}' because: {2}.", dllName, fullPath, deleteEx.Message);
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
                try
                {
                    String dataFileName = Path.GetFileNameWithoutExtension(fullPath);
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
                        DDAtlasPlugin plugin = loadPlugin(pluginStream);
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

        /// <summary>
        /// Load a plugin from a stream. The stream will NOT be closed by this method.
        /// </summary>
        /// <param name="stream">The stream to read the plugin from.</param>
        /// <returns>A DDAtlasPlugin object or null if an error occured.</returns>
        public DDAtlasPlugin loadPlugin(Stream stream)
        {
            //Read the file
            byte[] fileContents = new byte[stream.Length];
            stream.Read(fileContents, 0, fileContents.Length);

            //Read the plugin as a signed plugin
            try
            {
                byte[] hashedData;
                byte[] realData;
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(fileContents)))
                {
                    char[] magicLetters = binaryReader.ReadChars(4);
                    if (magicLetters[0] == 'S' &&
                        magicLetters[1] == 'D' &&
                        magicLetters[2] == 'D' &&
                        magicLetters[3] == 'P')
                    {
                        hashedData = new byte[binaryReader.ReadInt32()];
                        binaryReader.Read(hashedData, 0, hashedData.Length);
                        realData = new byte[binaryReader.ReadInt32()];
                        binaryReader.Read(realData, 0, realData.Length);

                        if (rsaProvider.VerifyData(realData, sha1Provider, hashedData))
                        {
                            using (XmlTextReader xmlReader = new XmlTextReader(new MemoryStream(realData)))
                            {
                                DDAtlasPlugin plugin = xmlSaver.restoreObject(xmlReader) as DDAtlasPlugin;
                                return plugin;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

#if ALLOW_OVERRIDE
            //If we allow overrides try to read the plugin as unsigned
            try
            {
                using (XmlTextReader xmlReader = new XmlTextReader(new MemoryStream(fileContents)))
                {
                    DDAtlasPlugin plugin = xmlSaver.restoreObject(xmlReader) as DDAtlasPlugin;
                    return plugin;
                }
            }
            catch (Exception)
            {

            }
#endif

            return null;
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

        public void initialzePlugins()
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

        public IEnumerable<AtlasPlugin> LoadedPlugins
        {
            get
            {
                return plugins;
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