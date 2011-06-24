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
        private MovementSequenceEditor movementSequenceEditor;
        private TimelineAnalyzer timelineAnalyzer;
        private TimelinePropertiesController timelinePropertiesController;

        private TimelineController editorTimelineController;
        private SimObjectMover propMover;

        public EditorPlugin()
        {
            Log.Info("Editor GUI Loaded");
        }

        public void Dispose()
        {
            timelinePropertiesController.Dispose();
            timelineAnalyzer.Dispose();
            movementSequenceEditor.Dispose();
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

            timelinePropertiesController = new TimelinePropertiesController(standaloneController, this);

            timelineAnalyzer = new TimelineAnalyzer(editorTimelineController, timelinePropertiesController);
            guiManager.addManagedDialog(timelineAnalyzer);

            movementSequenceEditor = new MovementSequenceEditor(standaloneController.MovementSequenceController);
            guiManager.addManagedDialog(movementSequenceEditor);

            //Taskbar
            Taskbar taskbar = guiManager.Taskbar;
            taskbar.addItem(new TimelineEditorTaskbarItem(timelinePropertiesController));
            taskbar.addItem(new MDIDialogOpenTaskbarItem(timelineAnalyzer, "Timeline Analyzer", "TimelineAnalyzerIcon"));
            taskbar.addItem(new MDIDialogOpenTaskbarItem(movementSequenceEditor, "Movement Sequence Editor", "MovementSequenceEditorIcon"));
            taskbar.addItem(new MDIDialogOpenTaskbarItem(propTimeline, "Prop Timeline Editor", "PropEditorIcon"));
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

        public TimelineController TimelineController
        {
            get
            {
                return editorTimelineController;
            }
        }

        public void sceneRevealed()
        {

        }
    }
}
