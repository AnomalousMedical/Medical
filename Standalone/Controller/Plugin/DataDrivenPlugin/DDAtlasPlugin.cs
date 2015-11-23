using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine.Saving;
using Engine.Editing;
using Medical.GUI;
using Engine;
using MyGUIPlugin;
using Medical.Controller;
using System.IO;
using Medical.Controller.AnomalousMvc;
using Anomalous.GuiFramework;

namespace Medical
{
    public partial class DDAtlasPlugin : AtlasPlugin, Saveable
    {
        private List<DDPluginTask> tasks = new List<DDPluginTask>();
        private String pluginNamespace;
        private List<long> dependencyIds = new List<long>();
        private List<VirtualFSMovementSequenceInfo> loadedSequences;

        public DDAtlasPlugin()
        {
            IconResourceFile = "Resources/Imagesets.xml";
            PluginId = -1;
            VersionString = "1.0.0.0";
            AllowUninstall = true;
        }

        public void Dispose()
        {
            
        }

        public void loadGUIResources()
        {
            ResourceManager.Instance.load(System.IO.Path.Combine(PluginRootFolder, IconResourceFile));
        }

        public void initialize(StandaloneController standaloneController)
        {
            TimelineController = standaloneController.TimelineController;
            AtlasPluginManager = standaloneController.AtlasPluginManager;
            MvcCore = standaloneController.MvcCore;
            GuiManager = standaloneController.GUIManager;

            TaskController taskController = standaloneController.TaskController;
            foreach (DDPluginTask task in tasks)
            {
                task._setPlugin(this);
                taskController.addTask(task);
                standaloneController.AnatomyTaskManager.addTask(task.TaggedAnatomy, task);
            }

            //Load sequences
            if (!String.IsNullOrEmpty(SequencesDirectory))
            {
                String fullSequencesDirectory = Path.Combine(PluginRootFolder, SequencesDirectory);
                VirtualFileSystem archive = VirtualFileSystem.Instance;
                if (archive.exists(fullSequencesDirectory))
                {
                    loadedSequences = new List<VirtualFSMovementSequenceInfo>();
                    MovementSequenceController movementSequenceController = standaloneController.MovementSequenceController;
                    foreach (String directory in archive.listDirectories(fullSequencesDirectory, false, false))
                    {
                        String groupName = archive.getFileInfo(directory).Name;
                        foreach (String file in archive.listFiles(directory, false))
                        {
                            VirtualFileInfo fileInfo = archive.getFileInfo(file);
                            String fileName = fileInfo.Name;
                            if (fileName.EndsWith(".seq"))
                            {
                                VirtualFSMovementSequenceInfo info = new VirtualFSMovementSequenceInfo();
                                info.GroupName = groupName;
                                info.Name = fileName.Substring(0, fileName.Length - 4);
                                info.FileName = fileInfo.FullName;
                                movementSequenceController.addMovementSequence(info.GroupName, info);
                                loadedSequences.Add(info);
                            }
                        }
                    }
                }
            }
        }

        public void unload(StandaloneController standaloneController, bool willReload, bool shuttingDown)
        {
            //Unload sequences
            if(loadedSequences != null)
            {
                MovementSequenceController movementSequenceController = standaloneController.MovementSequenceController;
                foreach(var info in loadedSequences)
                {
                    movementSequenceController.removeMovementSequence(info.GroupName, info);
                }
            }

            //Also need to unload icon resources, this isn't the best way, but unload by removing the branding image key and all task icons manually (done in task unload).
            ResourceManager.Instance.destroyAllTexturesForResource(BrandingImageKey);
            ResourceManager.Instance.removeByName(BrandingImageKey);

            if (!shuttingDown)
            {
                TaskController taskController = standaloneController.TaskController;
                foreach (DDPluginTask task in tasks)
                {
                    standaloneController.AnatomyTaskManager.removeTask(task.TaggedAnatomy, task);
                    taskController.removeTask(task, willReload);
                    ResourceManager.Instance.destroyAllTexturesForResource(task.IconName);
                    ResourceManager.Instance.removeByName(task.IconName);
                }
            }
        }

        public void sceneLoaded(SimScene scene)
        {
            
        }

        public void sceneUnloading(SimScene scene)
        {
            
        }

        public void addTask(DDPluginTask task)
        {
            tasks.Add(task);
            onTaskAdded(task);
        }

        public void removeTask(DDPluginTask task)
        {
            tasks.Remove(task);
            onTaskRemoved(task);
        }

        public bool hasTask(String uniqueName)
        {
            foreach (DDPluginTask task in tasks)
            {
                if (task.UniqueName == uniqueName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Set the DependencyIds for this plugin, this will clear out any existing ids so be sure to include
        /// them all in your enumerator.
        /// </summary>
        /// <param name="dependencyIds">An enumerator over all the dependency ids for this plugin.</param>
        public void setDependencyIds(IEnumerable<long> dependencyIds)
        {
            this.dependencyIds.Clear();
            this.dependencyIds.AddRange(dependencyIds.Distinct());
        }

        public IEnumerable<DDPluginTask> Tasks
        {
            get
            {
                return tasks;
            }
        }

        [Editable]
        public String IconResourceFile { get; set; }

        [Editable]
        public long PluginId { get; set; }

        [Editable]
        public String PluginName { get; set; }

        [Editable]
        public String BrandingImageKey { get; set; }

        [Editable]
        public String SequencesDirectory { get; set; }

        public TimelineController TimelineController { get; private set; }

        public AtlasPluginManager AtlasPluginManager { get; private set; }

        public AnomalousMvcCore MvcCore { get; private set; }

        public GUIManager GuiManager { get; private set; }

        public String PluginRootFolder { get; set; }

        /// <summary>
        /// This will be set by the AtlasPluginManager. It will be the location the plugin was loaded from.
        /// </summary>
        public String Location { get; internal set; }

        /// <summary>
        /// The verison of the plugin.
        /// </summary>
        public Version Version { get; private set; }

        [Editable]
        public String VersionString
        {
            get
            {
                return Version.ToString();
            }
            set
            {
                try
                {
                    Version = new Version(value);
                }
                catch (Exception) { }
            }
        }

        [Editable]
        public String PluginNamespace
        {
            get
            {
                return pluginNamespace;
            }
            set
            {
                pluginNamespace = value;
            }
        }

        [Editable]
        public bool AllowUninstall { get; set; }

        public bool AllowRuntimeUninstall
        {
            get
            {
                return true;
            }
        }

        public IEnumerable<long> DependencyPluginIds
        {
            get
            {
                return dependencyIds;
            }
        }

        #region Saveable Members

        protected DDAtlasPlugin(LoadInfo info)
        {
            pluginNamespace = info.GetString("PluginNamespace");
            info.RebuildList<DDPluginTask>("Task", tasks);
            IconResourceFile = info.GetString("IconResourceFile");
            PluginId = info.GetInt64("PluginID");
            PluginName = info.GetString("PluginName");
            BrandingImageKey = info.GetString("BrandingImageKey");
            VersionString = info.GetString("Version", "1.0.0.0");
            SequencesDirectory = info.GetString("SequencesDirectory", null);
            AllowUninstall = info.GetBoolean("AllowUninstall", true);
            info.RebuildList("DependencyId", dependencyIds);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("PluginNamespace", pluginNamespace);
            info.ExtractList<DDPluginTask>("Task", tasks);
            info.AddValue("IconResourceFile", IconResourceFile);
            info.AddValue("PluginID", PluginId);
            info.AddValue("PluginName", PluginName);
            info.AddValue("BrandingImageKey", BrandingImageKey);
            info.AddValue("Version", VersionString);
            info.AddValue("SequencesDirectory", SequencesDirectory);
            info.AddValue("AllowUninstall", AllowUninstall);
            info.ExtractList("DependencyId", dependencyIds);
        }

        #endregion
    }

    partial class DDAtlasPlugin
    {
        private EditInterface editInterface;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, "DDAtlasPlugin", null);
                    editInterface.addCommand(new EditInterfaceCommand("Add Start Mvc Context Task", addStartMvcContextTask));
                    var taskManager = editInterface.createEditInterfaceManager<DDPluginTask>();
                    taskManager.addCommand(new EditInterfaceCommand("Remove", removeDDPluginTask));
                    foreach (DDPluginTask task in tasks)
                    {
                        onTaskAdded(task);
                    }
                }
                return editInterface;
            }
        }

        private void addStartMvcContextTask(EditUICallback callback)
        {
            callback.getInputString("Enter a name for this task.", delegate(String result, ref string errorPrompt)
            {
                if (!hasTask(result))
                {
                    addTask(new StartAnomalousMvcTask(result, "", "", ""));
                    return true;
                }
                errorPrompt = String.Format("A task named {0} already exists. Please enter another.", result);
                return false;
            });
        }

        private void onTaskAdded(DDPluginTask task)
        {
            if (editInterface != null)
            {
                editInterface.addSubInterface(task, task.EditInterface);
            }
        }

        private void removeDDPluginTask(EditUICallback callback)
        {
            DDPluginTask task = editInterface.resolveSourceObject<DDPluginTask>(callback.getSelectedEditInterface());
            removeTask(task);
        }

        private void onTaskRemoved(DDPluginTask task)
        {
            if (editInterface != null)
            {
                editInterface.removeSubInterface(task);
            }
        }
    }
}
