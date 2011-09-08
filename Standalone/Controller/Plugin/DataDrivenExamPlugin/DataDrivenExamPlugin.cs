using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine.Saving;

namespace Medical
{
    class DataDrivenExamPlugin : AtlasPlugin, Saveable
    {
        private List<StartExamTimelineTask> examTasks = new List<StartExamTimelineTask>();

        public DataDrivenExamPlugin()
        {

        }

        public void Dispose()
        {
            
        }

        public void initialize(StandaloneController standaloneController)
        {
            foreach (StartExamTimelineTask task in examTasks)
            {
                standaloneController.TaskController.addTask(task);
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

        protected DataDrivenExamPlugin(LoadInfo info)
        {
            info.RebuildList<StartExamTimelineTask>("ExamTask", examTasks);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList<StartExamTimelineTask>("ExamTask", examTasks);
        }
    }
}
