using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Logging;
using Engine.ObjectManagement;
using MyGUIPlugin;

namespace Medical
{
    public class EditorPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;

        private PropTimeline propTimeline;
        private TimelineProperties timelineProperties;
        private MovementSequenceEditor movementSequenceEditor;
        private TimelineAnalyzer timelineAnalyzer;

        public EditorPlugin()
        {
            Log.Info("Editor GUI Loaded");
        }

        public void Dispose()
        {
            timelineAnalyzer.Dispose();
            movementSequenceEditor.Dispose();
            timelineProperties.Dispose();
            propTimeline.Dispose();
        }

        public void initializeGUI(StandaloneController standaloneController, GUIManager guiManager)
        {
            Gui.Instance.load("Medical.Resources.EditorImagesets.xml");

            this.standaloneController = standaloneController;
        }

        public void createDialogs(DialogManager dialogManager)
        {
            propTimeline = new PropTimeline();
            dialogManager.addManagedDialog(propTimeline);

            timelineProperties = new TimelineProperties(standaloneController.TimelineController, this, dialogManager);
            dialogManager.addManagedDialog(timelineProperties);

            timelineAnalyzer = new TimelineAnalyzer(standaloneController.TimelineController, timelineProperties);
            dialogManager.addManagedDialog(timelineAnalyzer);

            movementSequenceEditor = new MovementSequenceEditor(standaloneController.MovementSequenceController);
            dialogManager.addManagedDialog(movementSequenceEditor);
        }

        public void addToTaskbar(Taskbar taskbar)
        {
            taskbar.addItem(new DialogOpenTaskbarItem(timelineProperties, "Timeline", "TimelineEditorIcon"));
            taskbar.addItem(new DialogOpenTaskbarItem(timelineAnalyzer, "Timeline Analyzer", "TimelineAnalyzerIcon"));
            taskbar.addItem(new DialogOpenTaskbarItem(movementSequenceEditor, "Movement Sequence Editor", "MovementSequenceEditorIcon"));
        }

        public void finishInitialization()
        {

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

        public PropTimeline PropTimeline
        {
            get
            {
                return propTimeline;
            }
        }

        public void sceneRevealed()
        {

        }
    }
}
