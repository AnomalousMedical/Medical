using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Logging;
using Engine.ObjectManagement;
using MyGUIPlugin;
using Medical.Controller;
using Medical.Editor;

namespace Medical
{
    public class EditorPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;

        private PropTimeline propTimeline;
        private MovementSequenceEditor movementSequenceEditor;
        private OpenPropManager openPropManager;
        private ScratchArea scratchArea;

        private TimelineController editorTimelineController;
        private SimObjectMover propMover;
        private ScratchAreaController scratchAreaController;

        private BrowserWindow browserWindow;
        private InputBrowserWindow inputBrowserWindow;

        private AspectRatioTask aspectRatioTask;
        private RmlViewer rmlViewer;
        private GenericEditor mvcEditor;
        private GenericEditor timelinePropertiesEditor;
        private ProjectExplorer projectExplorer;
        private DDAtlasPluginEditor pluginEditor;
        private TimelineEditor timelineEditor;

        private EditorController editorController;
        private MedicalUICallback medicalUICallback;

        public EditorPlugin()
        {
            Log.Info("Editor GUI Loaded");
        }

        public void Dispose()
        {
            timelinePropertiesEditor.Dispose();
            timelineEditor.Dispose();
            pluginEditor.Dispose();
            projectExplorer.Dispose();
            mvcEditor.Dispose();
            rmlViewer.Dispose();
            movementSequenceEditor.Dispose();
            propTimeline.Dispose();
            openPropManager.Dispose();
            scratchArea.Dispose();
            inputBrowserWindow.Dispose();
            browserWindow.Dispose();
            aspectRatioTask.Dispose();
            editorController.Dispose();
        }

        public void loadGUIResources()
        {
            ResourceManager.Instance.load("Medical.Resources.EditorImagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            GUIManager guiManager = standaloneController.GUIManager;
            guiManager.MainGUIShown += new Action(guiManager_MainGUIShown);
            guiManager.MainGUIHidden += new Action(guiManager_MainGUIHidden);

            //Prop Mover
            MedicalController medicalController = standaloneController.MedicalController;
            propMover = new SimObjectMover("Props", medicalController.PluginManager, medicalController.EventManager);
            medicalController.FixedLoopUpdate += propMover.update;

            this.standaloneController = standaloneController;
            editorTimelineController = new TimelineController(standaloneController);
            guiManager.giveGUIsToTimelineController(editorTimelineController);

            //UI Helpers
            browserWindow = new BrowserWindow("Editor");
            guiManager.addManagedDialog(browserWindow);
            inputBrowserWindow = new InputBrowserWindow("Editor");
            guiManager.addManagedDialog(inputBrowserWindow);
            medicalUICallback = new MedicalUICallback(browserWindow, inputBrowserWindow);

            scratchAreaController = new ScratchAreaController(standaloneController.Clipboard);

            //Controller
            editorController = new EditorController(this, standaloneController);

            //Dialogs
            propTimeline = new PropTimeline(standaloneController.Clipboard);
            guiManager.addManagedDialog(propTimeline);

            openPropManager = new OpenPropManager();
            guiManager.addManagedDialog(openPropManager);

            movementSequenceEditor = new MovementSequenceEditor(standaloneController.MovementSequenceController, standaloneController.Clipboard, editorController);
            guiManager.addManagedDialog(movementSequenceEditor);

            scratchArea = new ScratchArea(scratchAreaController, medicalUICallback);
            guiManager.addManagedDialog(scratchArea);

            rmlViewer = new RmlViewer(editorController);
            guiManager.addManagedDialog(rmlViewer);

            mvcEditor = new GenericEditor("Medical.GUI.MvcEditor", "MVC Context", medicalUICallback, editorController, false);
            guiManager.addManagedDialog(mvcEditor);

            projectExplorer = new ProjectExplorer(editorController);
            guiManager.addManagedDialog(projectExplorer);

            pluginEditor = new DDAtlasPluginEditor(medicalUICallback, standaloneController.AtlasPluginManager, editorController);
            guiManager.addManagedDialog(pluginEditor);

            timelineEditor = new TimelineEditor(editorTimelineController, editorController, standaloneController.Clipboard, this);
            timelineEditor.MarkerMoved += new Engine.EventDelegate<GUI.TimelineEditor, float>(timelineEditor_MarkerMoved);
            guiManager.addManagedDialog(timelineEditor);

            timelinePropertiesEditor = new GenericEditor("Medical.GUI.TimelinePropertiesEditor", "Timeline Properties", medicalUICallback, editorController, false);
            guiManager.addManagedDialog(timelinePropertiesEditor);

            //Tasks Menu
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new MDIDialogOpenTask(movementSequenceEditor, "Medical.MovementSequenceEditor", "Movement Sequence Editor", "MovementSequenceEditorIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(propTimeline, "Medical.PropTimelineEditor", "Prop Timeline Editor", "PropEditorIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(openPropManager, "Medical.OpenPropManager", "Prop Manager", "PropManagerIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(scratchArea, "Medical.ScratchArea", "Scratch Area", "ScratchAreaIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(rmlViewer, "Medical.RmlViewer", "RML Viewer", "TimelineAnalyzerIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(mvcEditor, "Medical.MvcEditor", "MVC Editor", "PropManagerIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(projectExplorer, "Medical.ProjectExplorer", "Project Explorer", "ScratchAreaIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(pluginEditor, "Medical.DDPluginEditor", "Plugin Editor", "PlugInEditorIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(timelineEditor, "Medical.NewTimelineEditor", "Timeline Editor", "TimelineEditorIcon", TaskMenuCategories.Editor));

            editorController.addTypeController(new RmlTypeController(rmlViewer, editorController));
            editorController.addTypeController(new MvcTypeController(mvcEditor, editorController));
            editorController.addTypeController(new PluginTypeController(pluginEditor, editorController));
            editorController.addTypeController(new MovementSequenceTypeController(movementSequenceEditor, editorController));
            TimelineTypeController timelineTypeController = new TimelineTypeController(timelineEditor, timelinePropertiesEditor, editorController);
            timelineTypeController.TimelineChanged += new TimelineTypeEvent(timelineTypeController_TimelineChanged);
            editorController.addTypeController(timelineTypeController);

            aspectRatioTask = new AspectRatioTask(standaloneController.SceneViewController);
            taskController.addTask(aspectRatioTask);
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

        public long PluginId
        {
            get
            {
                return 6;
            }
        }

        public String PluginName
        {
            get
            {
                return "Editor Tools";
            }
        }

        public String BrandingImageKey
        {
            get
            {
                return "Editor/BrandingImage";
            }
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

        public String Location
        {
            get
            {
                return GetType().Assembly.Location;
            }
        }

        public Version Version
        {
            get
            {
                return GetType().Assembly.GetName().Version;
            }
        }

        public RmlViewer RmlViewer
        {
            get
            {
                return rmlViewer;
            }
        }

        public GenericEditor MvcEditor
        {
            get
            {
                return mvcEditor;
            }
        }

        public DDAtlasPluginEditor AtlasPluginEditor
        {
            get
            {
                return pluginEditor;
            }
        }

        public MovementSequenceEditor MovementSequenceEditor
        {
            get
            {
                return movementSequenceEditor;
            }
        }

        public TimelineEditor TimelineEditor
        {
            get
            {
                return timelineEditor;
            }
        }

        public ProjectExplorer ProjectExplorer
        {
            get
            {
                return projectExplorer;
            }
        }

        public MedicalUICallback MedicalUICallback
        {
            get
            {
                return medicalUICallback;
            }
        }

        public void sceneRevealed()
        {

        }

        void guiManager_MainGUIHidden()
        {
            openPropManager.hideOpenProps();
        }

        void guiManager_MainGUIShown()
        {
            openPropManager.showOpenProps();
        }

        void timelineTypeController_TimelineChanged(TimelineTypeController typeController, Timeline timeline)
        {
            openPropManager.removeAllOpenProps();
        }

        void timelineEditor_MarkerMoved(TimelineEditor source, float arg)
        {
            propTimeline.MarkerTime = arg;
        }
    }
}
