using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Standalone;
using Logging;
using Engine.ObjectManagement;

namespace Medical
{
    public class EditorGUIPlugin : GUIPlugin
    {
        private TimelineProperties timelineProperties;
        private AdvancedLayerControl advancedLayerControl;
        private MovementSequenceEditor movementSequenceEditor;

        public EditorGUIPlugin()
        {
            Log.Info("Editor GUI Loaded");
        }

        public void Dispose()
        {
            movementSequenceEditor.Dispose();
            advancedLayerControl.Dispose();
            timelineProperties.Dispose();
        }

        public void createDialogs(StandaloneController standaloneController, DialogManager dialogManager)
        {
            timelineProperties = new TimelineProperties(standaloneController.TimelineController);
            dialogManager.addManagedDialog(timelineProperties);

            advancedLayerControl = new AdvancedLayerControl();
            dialogManager.addManagedDialog(advancedLayerControl);

            movementSequenceEditor = new MovementSequenceEditor(standaloneController.MovementSequenceController);
            dialogManager.addManagedDialog(movementSequenceEditor);
        }

        public void addToTaskbar(Taskbar taskbar)
        {
            taskbar.addItem(new DialogOpenTaskbarItem(timelineProperties, "Timeline", "TimelineIcon"));
            taskbar.addItem(new DialogOpenTaskbarItem(advancedLayerControl, "Advanced Layers", "ManualObject"));
            taskbar.addItem(new DialogOpenTaskbarItem(movementSequenceEditor, "Movement Sequence Editor", "View/LayersMuscleLarge"));
        }

        #region GUIPlugin Members


        public void sceneLoaded(SimScene scene)
        {
            advancedLayerControl.sceneLoaded(scene);
        }

        public void sceneUnloading(SimScene scene)
        {
            advancedLayerControl.sceneUnloading();
        }

        #endregion
    }
}
