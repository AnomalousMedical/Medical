using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Logging;
using Engine.ObjectManagement;

namespace Medical
{
    public class EditorGUIPlugin : GUIPlugin
    {
        private StandaloneController standaloneController;

        private PropTimeline propTimeline;
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
            propTimeline.Dispose();
        }

        public void initializeGUI(StandaloneController standaloneController, GUIManager guiManager)
        {
            this.standaloneController = standaloneController;
        }

        public void createDialogs(DialogManager dialogManager)
        {
            propTimeline = new PropTimeline();
            dialogManager.addManagedDialog(propTimeline);

            timelineProperties = new TimelineProperties(standaloneController.TimelineController, this);
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

        public void sceneLoaded(SimScene scene)
        {
            advancedLayerControl.sceneLoaded(scene);
        }

        public void sceneUnloading(SimScene scene)
        {
            advancedLayerControl.sceneUnloading();
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            
        }

        public void createMenuBar(MenuBar menu)
        {

        }

        public PropTimeline PropTimeline
        {
            get
            {
                return propTimeline;
            }
        }
    }
}
