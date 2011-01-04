using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Standalone;
using Logging;

namespace Medical
{
    public class EditorGUIPlugin : GUIPlugin
    {
        private TimelineProperties timelineProperties;

        public EditorGUIPlugin()
        {
            Log.Info("Editor GUI Loaded");
        }

        public void Dispose()
        {
            timelineProperties.Dispose();
        }

        public void createDialogs(StandaloneController standaloneController, DialogManager dialogManager)
        {
            timelineProperties = new TimelineProperties(standaloneController.TimelineController);
            dialogManager.addManagedDialog(timelineProperties);
        }

        public void addToTaskbar(Taskbar taskbar)
        {
            taskbar.addItem(new DialogOpenTaskbarItem(timelineProperties, "Timeline", "TimelineIcon"));
        }
    }
}
