using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine.Saving;
using Engine.Editing;

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
            
        }

        public void Dispose()
        {
            
        }

        public void initialize(StandaloneController standaloneController)
        {
            TimelineController = standaloneController.TimelineController;
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

        public TimelineController TimelineController { get; private set; }

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
            foreach (DDPluginTask task in tasks)
            {
                task._setPlugin(this);
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("PluginNamespace", pluginNamespace);
            info.ExtractList<DDPluginTask>("Task", tasks);
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
    }
}
