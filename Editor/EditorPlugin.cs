﻿using System;
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
        private OpenPropManager openPropManager;
        private ScratchArea scratchArea;
        private DiscControl discControl;

        private TimelineController editorTimelineController;
        private SimObjectMover propMover;
        private ScratchAreaController scratchAreaController;

        private BrowserWindow browserWindow;

        private AspectRatioTask aspectRatioTask;

        public EditorPlugin()
        {
            Log.Info("Editor GUI Loaded");
        }

        public void Dispose()
        {
            TimelineBrowserController.setTimelineController(null);
            discControl.Dispose();
            timelinePropertiesController.Dispose();
            timelineAnalyzer.Dispose();
            movementSequenceEditor.Dispose();
            propTimeline.Dispose();
            openPropManager.Dispose();
            scratchArea.Dispose();
            browserWindow.Dispose();
            aspectRatioTask.Dispose();
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
            editorTimelineController.PlaybackStarted += new EventHandler(editorTimelineController_PlaybackStarted);
            editorTimelineController.PlaybackStopped += new EventHandler(editorTimelineController_PlaybackStopped);
            guiManager.giveGUIsToTimelineController(editorTimelineController);
            TimelineBrowserController.setTimelineController(editorTimelineController);

            standaloneController.TimelineController.PlaybackStarted += new EventHandler(TimelineController_PlaybackStarted);
            standaloneController.TimelineController.PlaybackStopped += new EventHandler(TimelineController_PlaybackStopped);

            //UI Helpers
            browserWindow = new BrowserWindow();
            guiManager.addManagedDialog(browserWindow);

            scratchAreaController = new ScratchAreaController(standaloneController.Clipboard);

            //Dialogs
            propTimeline = new PropTimeline(standaloneController.Clipboard);
            guiManager.addManagedDialog(propTimeline);

            openPropManager = new OpenPropManager();
            guiManager.addManagedDialog(openPropManager);

            timelinePropertiesController = new TimelinePropertiesController(standaloneController, this);
            timelinePropertiesController.CurrentTimelineChanged += new SingleArgumentEvent<TimelinePropertiesController, Timeline>(timelinePropertiesController_CurrentTimelineChanged);
            timelinePropertiesController.MarkerMoved += new Engine.EventDelegate<TimelinePropertiesController, float>(timelinePropertiesController_MarkerMoved);

            timelineAnalyzer = new TimelineAnalyzer(editorTimelineController, timelinePropertiesController);
            guiManager.addManagedDialog(timelineAnalyzer);

            movementSequenceEditor = new MovementSequenceEditor(standaloneController.MovementSequenceController, standaloneController.Clipboard);
            guiManager.addManagedDialog(movementSequenceEditor);

            discControl = new DiscControl();
            guiManager.addManagedDialog(discControl);

            scratchArea = new ScratchArea(scratchAreaController, browserWindow);
            guiManager.addManagedDialog(scratchArea);

            //Tasks Menu
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new TimelineEditorTask(timelinePropertiesController));
            taskController.addTask(new MDIDialogOpenTask(timelineAnalyzer, "Medical.TimelineAnalyzer", "Timeline Analyzer", "TimelineAnalyzerIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(movementSequenceEditor, "Medical.MovementSequenceEditor", "Movement Sequence Editor", "MovementSequenceEditorIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(propTimeline, "Medical.PropTimelineEditor", "Prop Timeline Editor", "PropEditorIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(openPropManager, "Medical.OpenPropManager", "Prop Manager", "PropEditorIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(discControl, "Medical.DiscEditor", "Disc Editor", "DiscEditorIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(scratchArea, "Medical.ScratchArea", "Scratch Area", "ScratchAreaIcon", TaskMenuCategories.Editor));

            aspectRatioTask = new AspectRatioTask(standaloneController.SceneViewController);
            taskController.addTask(aspectRatioTask);
        }

        public void sceneLoaded(SimScene scene)
        {
            propMover.sceneLoaded(scene);
            discControl.sceneLoaded(scene);
        }

        public void sceneUnloading(SimScene scene)
        {
            propMover.sceneUnloading(scene);
            discControl.sceneUnloading();
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

        public OpenPropManager PropManager
        {
            get
            {
                return openPropManager;
            }
        }

        public BrowserWindow BrowserWindow
        {
            get
            {
                return browserWindow;
            }
        }

        public void sceneRevealed()
        {

        }

        private void playbackStarted()
        {
            openPropManager.hideOpenProps();
        }

        private void playbackStopped()
        {
            openPropManager.showOpenProps();
        }

        void TimelineController_PlaybackStopped(object sender, EventArgs e)
        {
            playbackStopped();
        }

        void TimelineController_PlaybackStarted(object sender, EventArgs e)
        {
            playbackStarted();
        }

        void editorTimelineController_PlaybackStopped(object sender, EventArgs e)
        {
            playbackStopped();
        }

        void editorTimelineController_PlaybackStarted(object sender, EventArgs e)
        {
            playbackStarted();
        }

        void timelinePropertiesController_CurrentTimelineChanged(TimelinePropertiesController source, Timeline arg)
        {
            openPropManager.removeAllOpenProps();
        }

        void timelinePropertiesController_MarkerMoved(TimelinePropertiesController source, float arg)
        {
            propTimeline.MarkerTime = arg;
        }
    }
}
