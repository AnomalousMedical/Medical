using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    enum DataDrivenExamCustomQueries
    {
        GetTimelineController
    }

    partial class DataDrivenExamPluginDefinition : Saveable
    {
        private List<ExamTimelineTaskDefinition> examTasks = new List<ExamTimelineTaskDefinition>();

        public DataDrivenExamPluginDefinition()
        {
            
        }

        public void addTask(ExamTimelineTaskDefinition examTask)
        {
            examTasks.Add(examTask);
            onExamTaskAdded(examTask);
        }

        public bool hasTask(String uniqueName)
        {
            foreach (ExamTimelineTaskDefinition examTask in examTasks)
            {
                if (examTask.UniqueName == uniqueName)
                {
                    return true;
                }
            }
            return false;
        }

        [Editable]
        public String OutputFile { get; set; }

        [Editable]
        public String PluginName { get; set; }

        protected DataDrivenExamPluginDefinition(LoadInfo info)
        {
            OutputFile = info.GetString("OutputFile");
            PluginName = info.GetString("PluginName");
            info.RebuildList<ExamTimelineTaskDefinition>("ExamTask", examTasks);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("OutputFile", OutputFile);
            info.AddValue("PluginName", PluginName);
            info.ExtractList<ExamTimelineTaskDefinition>("ExamTask", examTasks);
        }
    }

    partial class DataDrivenExamPluginDefinition
    {
        private EditInterface editInterface;
        private EditInterfaceManager<ExamTimelineTaskDefinition> examTaskEditManager;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, "Exam Plugin", null);
                    editInterface.addCommand(new EditInterfaceCommand("Add Exam Task", addExamTask));
                    examTaskEditManager = new EditInterfaceManager<ExamTimelineTaskDefinition>(editInterface);
                    examTaskEditManager.addCommand(new EditInterfaceCommand("Remove", removeExamTask));
                    foreach (ExamTimelineTaskDefinition taskDef in examTasks)
                    {
                        onExamTaskAdded(taskDef);
                    }
                }
                return editInterface;
            }
        }

        private void addExamTask(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.getInputString("Enter a name for this exam", delegate(String result, ref string errorPrompt)
            {
                if (!hasTask(result))
                {
                    addTask(new ExamTimelineTaskDefinition(result));
                    return true;
                }
                errorPrompt = String.Format("A task named {0} already exists. Please enter another.", result);
                return false;
            });
        }

        private void onExamTaskAdded(ExamTimelineTaskDefinition taskDefinition)
        {
            if (editInterface != null)
            {
                examTaskEditManager.addSubInterface(taskDefinition, taskDefinition.EditInterface);
            }
        }

        private void removeExamTask(EditUICallback callback, EditInterfaceCommand caller)
        {

        }
    }
}
