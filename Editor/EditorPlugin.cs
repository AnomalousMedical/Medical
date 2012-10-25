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
using System.Reflection;
using System.IO;

namespace Medical
{
    public class EditorPlugin : AtlasPlugin
    {
        private StandaloneController standaloneController;

        private ScratchArea scratchArea;

        private TimelineController editorTimelineController;
        private SimObjectMover propMover;
        private ScratchAreaController scratchAreaController;

        private AspectRatioTask aspectRatioTask;
        private ProjectExplorer projectExplorer;

        private EditorController editorController;
        private EditorUICallback editorUICallback;
        private PropEditController propEditController;
        private TypeControllerManager typeControllerManager;

        public EditorPlugin()
        {
            Log.Info("Editor GUI Loaded");

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                {
                    String assemblyFileName = new AssemblyName(args.Name).Name;
                    String embeddedResourceName = String.Format("Medical.Resources.{0}.dll", assemblyFileName);
                    try
                    {
                        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResourceName))
                        {
                            if (stream != null)
                            {
                                Byte[] assemblyData = new Byte[stream.Length];
                                stream.Read(assemblyData, 0, assemblyData.Length);
                                return Assembly.Load(assemblyData);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                    return null;
                };
        }

        public void Dispose()
        {
            EditorConfig.save();
            projectExplorer.Dispose();
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

            scratchAreaController = new ScratchAreaController(standaloneController.Clipboard);

            //Controller
            editorController = new EditorController(standaloneController, editorTimelineController);
            standaloneController.DocumentController.addDocumentHandler(new ProjectDocumentHandler(editorController));
            propEditController = new PropEditController(propMover);

            //UI Helpers
            editorUICallback = new EditorUICallback(standaloneController, editorController, propEditController);

            //Dialogs
            scratchArea = new ScratchArea(scratchAreaController, editorUICallback);
            guiManager.addManagedDialog(scratchArea);

            projectExplorer = new ProjectExplorer(editorController);
            guiManager.addManagedDialog(projectExplorer);

            //Tasks Menu
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new MDIDialogOpenTask(scratchArea, "Medical.ScratchArea", "Scratch Area", "ScratchAreaIcon", TaskMenuCategories.Editor));
            taskController.addTask(new MDIDialogOpenTask(projectExplorer, "Medical.ProjectExplorer", "Project Explorer", "Editor/ProjectExplorerIcon", TaskMenuCategories.Editor));

            typeControllerManager = new TypeControllerManager(standaloneController, this);

            aspectRatioTask = new AspectRatioTask(standaloneController.SceneViewController);
            taskController.addTask(aspectRatioTask);

            standaloneController.ViewHostFactory.addFactory(new RmlWysiwygComponentFactory());

            standaloneController.ViewHostFactory.addFactory(new TimelineComponentFactory(editorTimelineController, editorController, standaloneController.Clipboard, this));
            standaloneController.ViewHostFactory.addFactory(new GenericEditorComponentFactory(editorUICallback, editorController));
            standaloneController.ViewHostFactory.addFactory(new EditorInfoBarFactory());
            standaloneController.ViewHostFactory.addFactory(new TextEditorComponentFactory());
            standaloneController.ViewHostFactory.addFactory(new PropTimelineFactory(standaloneController.Clipboard, propEditController));
            standaloneController.ViewHostFactory.addFactory(new EditorTaskbarFactory(editorController));
            standaloneController.ViewHostFactory.addFactory(new MovementSequenceEditorFactory(standaloneController.MovementSequenceController, editorController, standaloneController.Clipboard));

            editorController.ProjectChanged += editorController_ProjectChanged;
        }

        void editorController_ProjectChanged(EditorController editorController, String defaultFile)
        {
            if (editorController.ResourceProvider != null)
            {
                if (!projectExplorer.Visible)
                {
                    projectExplorer.Visible = true;
                }
            }
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

        public ProjectExplorer ProjectExplorer
        {
            get
            {
                return projectExplorer;
            }
        }

        public EditorUICallback UICallback
        {
            get
            {
                return editorUICallback;
            }
        }

        public PropEditController PropEditController
        {
            get
            {
                return propEditController;
            }
        }

        public EditorController EditorController
        {
            get
            {
                return editorController;
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
    }
}
