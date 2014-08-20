using Engine;
using Engine.Resources;
using Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Medical
{
    public class AtlasDependencyManager : IDisposable
    {
        public delegate void DependencyMessageDelegate(String message);
        public event DependencyMessageDelegate LoadError;

        private StandaloneController standaloneController;
        private List<AtlasDependency> uninitializedDependencies = new List<AtlasDependency>();
        private List<AtlasDependency> dependencies = new List<AtlasDependency>();
        private HashSet<Guid> usedDependencyIds = new HashSet<Guid>();
        private String additionalSearchPath;
        private HashSet<String> loadedDependencyNames = new HashSet<string>();
        private ManagePluginInstructions manageDependencyInstructions;

        private DataFileVerifier dataFileVerifier;

        private ResourceManager dependencyResourceManager;

        public AtlasDependencyManager(StandaloneController standaloneController, DataFileVerifier dataFileVerifier)
        {
            this.dataFileVerifier = dataFileVerifier;
            this.standaloneController = standaloneController;
            additionalSearchPath = FolderFinder.ExecutableFolder;
            dependencyResourceManager = PluginManager.Instance.createLiveResourceManager("Dependencies");

            manageDependencyInstructions = new ManagePluginInstructions(MedicalConfig.PluginConfig.DependenciesFolder);
        }

        public void Dispose()
        {
            foreach(var dependency in dependencies)
            {
                dependency.Dispose();
            }
        }

        public void manageInstalledDependencies()
        {
            var loadedInstructions = ManagePluginInstructions.restore(MedicalConfig.PluginConfig.DependenciesFolder);
            if (loadedInstructions != null)
            {
                loadedInstructions.process();
                loadedInstructions.deletePersistantFile();
            }
        }

        public bool addDependency(String path)
        {
            //Do type dll or data driven check here

            return addDataDrivenDependency(path);
        }

        private bool addDataDrivenDependency(String path)
        {
            bool loadedDependency = false;
            String dependencyDirectory = null;
            String fullPath = Path.GetFullPath(path);
            if (!Directory.Exists(fullPath) && !File.Exists(fullPath))
            {
                fullPath = Path.Combine(additionalSearchPath, path);
            }

            //put file load code here
            if (File.Exists(fullPath))
            {
                if (dataFileVerifier.isSafeDataFile(fullPath))
                {
                    String dataFileName = Path.GetFileNameWithoutExtension(fullPath);
                    try
                    {
                        if (!loadedDependencyNames.Contains(dataFileName))
                        {
                            //Add the archive to the VirtualFileSystem if needed
                            if (!VirtualFileSystem.Instance.containsRealAbsolutePath(fullPath))
                            {
                                VirtualFileSystem.Instance.addArchive(fullPath);
                            }
                            dependencyDirectory = String.Format("Dependencies/{0}/", Path.GetFileNameWithoutExtension(path));

                            loadedDependencyNames.Add(dataFileName);
                        }
                        else
                        {
                            Log.Error("Cannot load data file '{0}' from '{1}' because a plugin named '{2}' is already loaded.", path, fullPath, dataFileName);
                        }
                    }
                    catch (ZipAccess.ZipIOException e)
                    {
                        fireLoadError(String.Format("There was an error loading the plugin '{0}'.", dataFileName));
                        Log.Error("Cannot load data file '{0}' from '{1}' because of a zip read error: {2}. Deleting corrupted plugin.", path, fullPath, e.Message);
                        try
                        {
                            File.Delete(fullPath);
                        }
                        catch (Exception deleteEx)
                        {
                            Log.Error("Error deleting data file '{0}' from '{1}' because: {2}.", path, fullPath, deleteEx.Message);
                            manageDependencyInstructions.addFileToDelete(fullPath);
                            manageDependencyInstructions.savePersistantFile();
                        }
                    }
                }
            }
            else if (Directory.Exists(fullPath))
            {
                String directoryName = Path.GetFileName(Path.GetDirectoryName(fullPath));
                if (!loadedDependencyNames.Contains(directoryName))
                {
                    loadedDependencyNames.Add(directoryName);

                    dependencyDirectory = String.Format("Dependencies/{0}/", directoryName);
                    String rootPath = fullPath.Replace("\\", "/");
                    if (!rootPath.EndsWith("/"))
                    {
                        rootPath += "/";
                    }
                    rootPath = rootPath.Replace(dependencyDirectory, "");

                    //Add the archive to the VirtualFileSystem if needed
                    if (!VirtualFileSystem.Instance.containsRealAbsolutePath(rootPath))
                    {
                        VirtualFileSystem.Instance.addArchive(rootPath);
                    }
                }
                else
                {
                    Log.Error("Cannot load data file '{0}' from '{1}' because a dependency named '{2}' is already loaded.", path, fullPath, directoryName);
                }
            }
            else
            {
                Log.Error("Cannot load data file '{0}' from '{0}' or '{1}' because it was not found.", path, fullPath, Path.GetFullPath(path));
            }

            if(dependencyDirectory != null)
            {
                String definitionFile = dependencyDirectory + "Dependency.ddd";

                if (VirtualFileSystem.Instance.exists(definitionFile))
                {
                    using (Stream stream = VirtualFileSystem.Instance.openStream(definitionFile, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                    {
                        try
                        {
                            DDAtlasDependency dependency = SharedXmlSaver.Load<DDAtlasDependency>(stream);
                            if (dependency != null)
                            {
                                dependency.Location = fullPath;
                                dependency.RootFolder = dependencyDirectory;
                                addDependency(dependency);
                                loadedDependency = true;
                            }
                            else
                            {
                                throw new Exception(String.Format("Error loading '{0}' in path '{1}' from '{2}' because it was null.", definitionFile, path, fullPath));
                            }
                        }
                        catch (Exception ex)
                        {
                            fireLoadError(String.Format("There was an error loading the plugin '{0}'.", Path.GetFileName(fullPath)));
                            Log.Error(ex.Message);
                            try
                            {
                                File.Delete(fullPath);
                            }
                            catch (Exception deleteEx)
                            {
                                Log.Error("Error deleting data file '{0}' from '{1}' because: {2}.", path, fullPath, deleteEx.Message);
                                manageDependencyInstructions.addFileToDelete(fullPath);
                                manageDependencyInstructions.savePersistantFile();
                            }
                        }
                    }
                }
                else
                {
                    Log.Error("Error loading '{0}' in path '{1}' from '{2}' because it does not exist.", definitionFile, path, fullPath);
                    manageDependencyInstructions.addFileToDelete(fullPath);
                    manageDependencyInstructions.savePersistantFile();
                }
            }

            return loadedDependency;
        }

        public void addDependency(AtlasDependency dependency)
        {
            if(!usedDependencyIds.Contains(dependency.DependencyId))
            {
                uninitializedDependencies.Add(dependency);
                usedDependencyIds.Add(dependency.DependencyId);
            }
            else
            {
                Log.Error("Attempted to add Dependency '{0}' with id '{1}' that is already in use for another dependency.", dependency.Name, dependency.DependencyId);
            }
        }

        public void initializeDependencies()
        {
            foreach (var status in initializeDependenciesStatus()) { }
        }

        public IEnumerable<PluginLoadStatus> initializeDependenciesStatus()
        {
            PluginLoadStatus status = new PluginLoadStatus()
            {
                Total = uninitializedDependencies.Count,
            };
            foreach(var dependency in uninitializedDependencies)
            {
                dependency.initialize(standaloneController, this);
                dependencies.Add(dependency);
                yield return status;
            }
            uninitializedDependencies.Clear();
        }

        public int UninitalizedCount
        {
            get
            {
                return uninitializedDependencies.Count;
            }
        }

        public ResourceManager DependencyResourceManager
        {
            get
            {
                return dependencyResourceManager;
            }
        }

        private void fireLoadError(String message)
        {
            if (LoadError != null)
            {
                LoadError.Invoke(message);
            }
        }
    }
}
