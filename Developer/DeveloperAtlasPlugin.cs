using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Engine.ObjectManagement;
using Medical.GUI;
using Developer.GUI;
using MyGUIPlugin;

namespace Developer
{
    class DeveloperAtlasPlugin : AtlasPlugin
    {
        private ExamViewer examViewer;
        private PluginPublisher pluginPublisher;
        private PluginPublishController pluginPublishController;
        private DeveloperRenderPropertiesDialog developerRenderer;
        private DiscControl discControl;
        private DDAtlasPluginEditor pluginEditor;
        private BrowserWindow browserWindow;
        private AdvancedMandibleMovementDialog advancedMandibleMovement;
        private GridPropertiesDialog gridProperties;

        public DeveloperAtlasPlugin(StandaloneController standaloneController)
        {

        }

        public void Dispose()
        {
            advancedMandibleMovement.Dispose();
            examViewer.Dispose();
            pluginPublisher.Dispose();
            developerRenderer.Dispose();
            discControl.Dispose();
            pluginEditor.Dispose();
            browserWindow.Dispose();
            gridProperties.Dispose();
        }

        public void createMenuBar(NativeMenuBar menu)
        {

        }

        public void loadGUIResources()
        {
            ResourceManager.Instance.load("Developer.Resources.DeveloperImagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            pluginPublishController = new PluginPublishController(standaloneController.AtlasPluginManager);

            GUIManager guiManager = standaloneController.GUIManager;

            //UI Helpers
            browserWindow = new BrowserWindow();
            guiManager.addManagedDialog(browserWindow);

            gridProperties = new GridPropertiesDialog(standaloneController.MeasurementGrid);
            guiManager.addManagedDialog(gridProperties);

            examViewer = new ExamViewer(standaloneController.ExamController);
            guiManager.addManagedDialog(examViewer);

            pluginPublisher = new PluginPublisher(pluginPublishController);
            guiManager.addManagedDialog(pluginPublisher);

            developerRenderer = new DeveloperRenderPropertiesDialog(standaloneController.SceneViewController, standaloneController.ImageRenderer, guiManager);
            guiManager.addManagedDialog(developerRenderer);

            discControl = new DiscControl();
            guiManager.addManagedDialog(discControl);

            pluginEditor = new DDAtlasPluginEditor(browserWindow, standaloneController.TimelineController, standaloneController.AtlasPluginManager);
            guiManager.addManagedDialog(pluginEditor);

            advancedMandibleMovement = new AdvancedMandibleMovementDialog(standaloneController.MovementSequenceController);
            guiManager.addManagedDialog(advancedMandibleMovement);

            //Task Controller
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new MDIDialogOpenTask(examViewer, "Medical.ExamViewer", "Exam Viewer", "Developer.ExamIcon", TaskMenuCategories.Developer, 4));

            taskController.addTask(new MDIDialogOpenTask(pluginPublisher, "Developer.PluginPublisher", "Plugin Publisher", "Developer.PublisherIcon", TaskMenuCategories.Developer));

            taskController.addTask(new MDIDialogOpenTask(developerRenderer, "Developer.DeveloperRender", "Developer Renderer", "Developer.RenderIcon", TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(discControl, "Medical.DiscEditor", "Disc Editor", "Developer.DiscEditorIcon", TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(pluginEditor, "Medical.DDPluginEditor", "Plugin Editor", "Developer.PlugInEditorIcon", TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(advancedMandibleMovement, "Medical.AdvancedMandibleMovement", "Advanced Mandible Movement", "Developer.MovementIcon", TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(gridProperties, "Medical.GridProperties", "Grid", "Developer.GridIcon", TaskMenuCategories.Developer));
        }

        public void sceneLoaded(SimScene scene)
        {
            advancedMandibleMovement.sceneLoaded(scene);
            discControl.sceneLoaded(scene);
        }

        public void sceneRevealed()
        {

        }

        public void sceneUnloading(SimScene scene)
        {
            advancedMandibleMovement.sceneUnloading(scene);
            discControl.sceneUnloading();
        }

        public void setMainInterfaceEnabled(bool enabled)
        {

        }

        public long PluginId
        {
            get
            {
                return 8;
            }
        }

        public String PluginName
        {
            get
            {
                return "Developer Plugin";
            }
        }

        public String BrandingImageKey
        {
            get
            {
                return "Developer/BrandingImage";
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
    }
}
