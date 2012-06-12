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

        private MovementSequenceEditor movementSequenceEditor;
        private OpenPropManager openPropManager;
        private ScratchArea scratchArea;

        private TimelineController editorTimelineController;
        private SimObjectMover propMover;
        private ScratchAreaController scratchAreaController;

        private AspectRatioTask aspectRatioTask;
        private GenericEditor mvcEditor;
        private ProjectExplorer projectExplorer;
        private DDAtlasPluginEditor pluginEditor;

        private EditorController editorController;
        private MedicalUICallback medicalUICallback;
        private PropEditController propEditController;

        public EditorPlugin()
        {
            Log.Info("Editor GUI Loaded");
        }

        public void Dispose()
        {
            pluginEditor.Dispose();
            projectExplorer.Dispose();
            mvcEditor.Dispose();
            movementSequenceEditor.Dispose();
            openPropManager.Dispose();
            scratchArea.Dispose();
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
            medicalUICallback = new MedicalUICallback();

            scratchAreaController = new ScratchAreaController(standaloneController.Clipboard);

            //Controller
            editorController = new EditorController(this, standaloneController);
            propEditController = new PropEditController(propMover);

            //Dialogs
            openPropManager = new OpenPropManager(propEditController);
            guiManager.addManagedDialog(openPropManager);

            movementSequenceEditor = new MovementSequenceEditor(standaloneController.MovementSequenceController, standaloneController.Clipboard, editorController);
            guiManager.addManagedDialog(movementSequenceEditor);

            scratchArea = new ScratchArea(scratchAreaController, medicalUICallback);
            guiManager.addManagedDialog(scratchArea);

            mvcEditor = new GenericEditor("Medical.GUI.MvcEditor", "MVC Context", medicalUICallback, editorController, false);
            guiManager.addManagedDialog(mvcEditor);

            projectExplorer = new ProjectExplorer(editorController);
            guiManager.addManagedDialog(projectExplorer);

            pluginEditor = new DDAtlasPluginEditor(medicalUICallback, standaloneController.AtlasPluginManager, editorController);
            guiManager.addManagedDialog(pluginEditor);

            //Tasks Menu
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new MDIDialogOpenTask(movementSequenceEditor, "Medical.MovementSequenceEditor", "Movement Sequence Editor", "MovementSequenceEditorIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(openPropManager, "Medical.OpenPropManager", "Prop Manager", "PropManagerIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(scratchArea, "Medical.ScratchArea", "Scratch Area", "ScratchAreaIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(mvcEditor, "Medical.MvcEditor", "MVC Editor", "PropManagerIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(projectExplorer, "Medical.ProjectExplorer", "Project Explorer", "ScratchAreaIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(pluginEditor, "Medical.DDPluginEditor", "Plugin Editor", "PlugInEditorIcon", TaskMenuCategories.Editor));

            editorController.addTypeController(new RmlTypeController(editorController, guiManager));
            editorController.addTypeController(new RcssTypeController(editorController, guiManager));
            editorController.addTypeController(new MvcTypeController(mvcEditor, editorController));
            editorController.addTypeController(new PluginTypeController(pluginEditor, editorController));
            editorController.addTypeController(new MovementSequenceTypeController(movementSequenceEditor, editorController));
            TimelineTypeController timelineTypeController = new TimelineTypeController(editorController, propEditController);
            timelineTypeController.TimelineChanged += new TimelineTypeEvent(timelineTypeController_TimelineChanged);
            editorController.addTypeController(timelineTypeController);

            aspectRatioTask = new AspectRatioTask(standaloneController.SceneViewController);
            taskController.addTask(aspectRatioTask);

            standaloneController.ViewHostFactory.addFactory(new TimelineComponentFactory(editorTimelineController, editorController, standaloneController.Clipboard, this));
            standaloneController.ViewHostFactory.addFactory(new GenericEditorComponentFactory(medicalUICallback, editorController));
            standaloneController.ViewHostFactory.addFactory(new EditorInfoBarFactory());
            standaloneController.ViewHostFactory.addFactory(new TextEditorComponentFactory());
            standaloneController.ViewHostFactory.addFactory(new PropTimelineFactory(standaloneController.Clipboard, propEditController));
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

        public PropEditController PropEditController
        {
            get
            {
                return propEditController;
            }
        }

        public void sceneRevealed()
        {

        }

        void guiManager_MainGUIHidden()
        {
            propEditController.hideOpenProps();
        }

        void guiManager_MainGUIShown()
        {
            propEditController.showOpenProps();
        }

        void timelineTypeController_TimelineChanged(TimelineTypeController typeController, Timeline timeline)
        {
            propEditController.removeAllOpenProps();
        }
    }
}
