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

namespace Medical
{
    public enum DDAtlasPluginCustomQueries
    {
        GetTimelineController
    }

    public partial class DDAtlasPlugin : AtlasPlugin, Saveable
    {
        private List<DDPluginTask> tasks = new List<DDPluginTask>();
        private String pluginNamespace;

        public DDAtlasPlugin()
        {
            IconResourceFile = "Resources/Imagesets.xml";
            PluginId = -1;
        }

        public void Dispose()
        {
            
        }

        public void initialize(StandaloneController standaloneController)
        {
            if (PluginId == -1 || standaloneController.App.LicenseManager.allowFeature(PluginId))
            {
                TimelineController = standaloneController.TimelineController;

                Gui.Instance.load(System.IO.Path.Combine(PluginRootFolder, IconResourceFile));

                TaskController taskController = standaloneController.TaskController;
                foreach (DDPluginTask task in tasks)
                {
                    task._setPlugin(this);
                    taskController.addTask(task);
                }
            }
        }

        public void sceneLoaded(SimScene scene)
        {
            
        }

        public void sceneUnloading(SimScene scene)
        {
            
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            
        }

        public void createMenuBar(NativeMenuBar menu)
        {
            
        }

        public void sceneRevealed()
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

        [Editable]
        public String IconResourceFile { get; set; }

        [Editable]
        public long PluginId { get; set; }

        [Editable]
        public String PluginName { get; set; }

        [Editable]
        public String BrandingImageKey { get; set; }

        public TimelineController TimelineController { get; private set; }

        public String PluginRootFolder { get; set; }

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

        #region Saveable Members

        protected DDAtlasPlugin(LoadInfo info)
        {
            pluginNamespace = info.GetString("PluginNamespace");
            info.RebuildList<DDPluginTask>("Task", tasks);
            IconResourceFile = info.GetString("IconResourceFile");
            PluginId = info.GetInt64("PluginID");
            PluginName = info.GetString("PluginName");
            BrandingImageKey = info.GetString("BrandingImageKey");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("PluginNamespace", pluginNamespace);
            info.ExtractList<DDPluginTask>("Task", tasks);
            info.AddValue("IconResourceFile", IconResourceFile);
            info.AddValue("PluginID", PluginId);
            info.AddValue("PluginName", PluginName);
            info.AddValue("BrandingImageKey", BrandingImageKey);
        }

        #endregion
    }

    partial class DDAtlasPlugin
    {
        private EditInterface editInterface;
        private EditInterfaceManager<DDPluginTask> taskManager;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, "DDAtlasPlugin", null);
                    editInterface.addCommand(new EditInterfaceCommand("Add Start Timeline Task", addStartTimelineTask));
                    taskManager = new EditInterfaceManager<DDPluginTask>(editInterface);
                    taskManager.addCommand(new EditInterfaceCommand("Remove", removeDDPluginTask));
                    foreach (DDPluginTask task in tasks)
                    {
                        onTaskAdded(task);
                    }
                }
                return editInterface;
            }
        }

        private void addStartTimelineTask(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.getInputString("Enter a name for this exam", delegate(String result, ref string errorPrompt)
            {
                if (!hasTask(result))
                {
                    addTask(new StartDDPluginTimelineTask(result, "", "", ""));
                    return true;
                }
                errorPrompt = String.Format("A task named {0} already exists. Please enter another.", result);
                return false;
            });
        }

        private void onTaskAdded(DDPluginTask task)
        {
            if (taskManager != null)
            {
                taskManager.addSubInterface(task, task.EditInterface);
            }
        }

        private void removeDDPluginTask(EditUICallback callback, EditInterfaceCommand caller)
        {
            DDPluginTask task = taskManager.resolveSourceObject(callback.getSelectedEditInterface());
            removeTask(task);
        }

        private void onTaskRemoved(DDPluginTask task)
        {
            if (taskManager != null)
            {
                taskManager.removeSubInterface(task);
            }
        }
    }
}
