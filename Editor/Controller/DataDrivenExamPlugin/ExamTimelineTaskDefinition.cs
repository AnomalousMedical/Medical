using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;

namespace Medical
{
    partial class ExamTimelineTaskDefinition : Saveable
    {
        private String uniqueName;

        public ExamTimelineTaskDefinition(String uniqueName)
        {
            this.uniqueName = uniqueName;
        }

        public String UniqueName
        {
            get
            {
                return uniqueName;
            }
        }

        [Editable]
        public String PrettyName { get; set; }

        [Editable]
        public String IconName { get; set; }

        [Editable]
        public String Category { get; set; }

        [Editable]
        public String TimelineProject { get; set; }

        [Editable]
        public String StartupTimeline { get; set; }

        #region Saveable Members

        protected ExamTimelineTaskDefinition(LoadInfo info)
        {
            uniqueName = info.GetString("uniqueName");
            PrettyName = info.GetString("PrettyName");
            IconName = info.GetString("IconName");
            Category = info.GetString("Category");
            TimelineProject = info.GetString("TimelineProject");
            StartupTimeline = info.GetString("StartupTimeline");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("uniqueName", uniqueName);
            info.AddValue("PrettyName", PrettyName);
            info.AddValue("IconName", IconName);
            info.AddValue("Category", Category);
            info.AddValue("TimelineProject", TimelineProject);
            info.AddValue("StartupTimeline", StartupTimeline);
        }

        #endregion
    }

    partial class ExamTimelineTaskDefinition
    {
        private EditInterface editInterface;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, uniqueName + " - Exam Task", null);
                    editInterface.addCommand(new EditInterfaceCommand("Preview", previewExam));
                }
                return editInterface;
            }
        }

        private void previewExam(EditUICallback callback, EditInterfaceCommand caller)
        {
            TimelineController timelineController = null;
            callback.runCustomQuery(DataDrivenExamCustomQueries.GetTimelineController, delegate(Object result, ref string errorPrompt)
            {
                timelineController = result as TimelineController;
                return true;
            });
            if (timelineController != null)
            {
                if (TimelineProject.EndsWith(".tlp"))
                {
                    timelineController.ResourceProvider = new TimelineReadOnlyZipResources(TimelineProject);
                }
                else
                {
                    timelineController.ResourceProvider = new FilesystemTimelineResourceProvider(TimelineProject);
                }
                timelineController.startPlayback(timelineController.openTimeline(StartupTimeline));
            }
            else
            {
                Log.Warning("Could not get TimelineController for ExamTimelineTaskDefinition {0} preview. Is your UICallback setup correctly? It needs a customQuery for DataDrivenExamCustomQueries.GetTimelineController that sends the TimelineController to use.", uniqueName);
            }
        }
    }
}
