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

        private TimelineController editorTimelineController;
        private SimObjectMover propMover;

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

        public void initialize(StandaloneController standaloneController)
        {
            GUIManager guiManager = standaloneController.GUIManager;
            Gui.Instance.load("Medical.Resources.EditorImagesets.xml");

            //Prop Mover
            MedicalController medicalController = standaloneController.MedicalController;
            propMover = new SimObjectMover("Props", medicalController.PluginManager, medicalController.EventManager);
            medicalController.FixedLoopUpdate += propMover.update;

            this.standaloneController = standaloneController;
            editorTimelineController = new TimelineController(standaloneController);
            guiManager.giveGUIsToTimelineController(editorTimelineController);

            //Dialogs
            propTimeline = new PropTimeline();
            guiManager.addManagedDialog(propTimeline);

            timelineProperties = new TimelineProperties(editorTimelineController, this, guiManager, standaloneController.DocumentController);
            guiManager.addManagedDialog(timelineProperties);

            timelineAnalyzer = new TimelineAnalyzer(editorTimelineController, timelineProperties);
            guiManager.addManagedDialog(timelineAnalyzer);

            movementSequenceEditor = new MovementSequenceEditor(standaloneController.MovementSequenceController);
            guiManager.addManagedDialog(movementSequenceEditor);

            //Taskbar
            Taskbar taskbar = guiManager.Taskbar;
            taskbar.addItem(new DialogOpenTaskbarItem(timelineProperties, "Timeline", "TimelineEditorIcon"));
            taskbar.addItem(new DialogOpenTaskbarItem(timelineAnalyzer, "Timeline Analyzer", "TimelineAnalyzerIcon"));
            taskbar.addItem(new DialogOpenTaskbarItem(movementSequenceEditor, "Movement Sequence Editor", "MovementSequenceEditorIcon"));
        }

        public void sceneLoaded(SimScene scene)
        {
            propMover.sceneLoaded(scene);
        }

        public void sceneUnloading(SimScene scene)
        {
            propMover.sceneUnloading(scene);
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

        public SimObjectMover SimObjectMover
        {
            get
            {
                return propMover;
            }
        }

        public void sceneRevealed()
        {

        }
    }
}
