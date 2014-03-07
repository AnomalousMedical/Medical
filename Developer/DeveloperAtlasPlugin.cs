using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Engine.ObjectManagement;
using Medical.GUI;
using Developer.GUI;
using MyGUIPlugin;
using libRocketPlugin;

namespace Developer
{
    class DeveloperAtlasPlugin : AtlasPlugin
    {
        private ExamViewer examViewer;
        private PluginPublisher pluginPublisher;
        private PluginPublishController pluginPublishController;
        private DeveloperRenderPropertiesDialog developerRenderer;
        private DiscControl discControl;
        private AdvancedMandibleMovementDialog advancedMandibleMovement;
        private GridPropertiesDialog gridProperties;
        private PerformanceGui performanceGui;
        private DisablePhysicsTask disablePhysics;
        private MeasurementGUI measurementGUI;
        private ShowLibRocketDebugger libRocketDebugger;
        private ChangeRenderingMode changeRenderingMode;

        public DeveloperAtlasPlugin(StandaloneController standaloneController)
        {

        }

        public void Dispose()
        {
            changeRenderingMode.Dispose();
            libRocketDebugger.Dispose();
            measurementGUI.Dispose();
            advancedMandibleMovement.Dispose();
            examViewer.Dispose();
            pluginPublisher.Dispose();
            developerRenderer.Dispose();
            discControl.Dispose();
            gridProperties.Dispose();
            performanceGui.Dispose();
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
            gridProperties = new GridPropertiesDialog(standaloneController.MeasurementGrid);
            guiManager.addManagedDialog(gridProperties);

            examViewer = new ExamViewer(standaloneController.ExamController);
            guiManager.addManagedDialog(examViewer);

            pluginPublisher = new PluginPublisher(pluginPublishController);
            guiManager.addManagedDialog(pluginPublisher);

            developerRenderer = new DeveloperRenderPropertiesDialog(standaloneController.SceneViewController, standaloneController.ImageRenderer, guiManager, standaloneController.NotificationManager);
            guiManager.addManagedDialog(developerRenderer);

            discControl = new DiscControl();
            guiManager.addManagedDialog(discControl);

            advancedMandibleMovement = new AdvancedMandibleMovementDialog(standaloneController.MovementSequenceController);
            guiManager.addManagedDialog(advancedMandibleMovement);

            performanceGui = new PerformanceGui(standaloneController);
            guiManager.addManagedDialog(performanceGui);

            measurementGUI = new MeasurementGUI(standaloneController);
            guiManager.addManagedDialog(measurementGUI);

            libRocketDebugger = new ShowLibRocketDebugger(guiManager);

            RocketInterface.Instance.FileInterface.addExtension(new RocketAssemblyResourceLoader(this.GetType().Assembly));

            //Task Controller
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new MDIDialogOpenTask(examViewer, "Medical.ExamViewer", "Exam Viewer", "Developer.ExamIcon", TaskMenuCategories.Developer, 4));

            taskController.addTask(new MDIDialogOpenTask(pluginPublisher, "Developer.PluginPublisher", "Plugin Publisher", "Developer.PublisherIcon", TaskMenuCategories.Developer));

            taskController.addTask(new MDIDialogOpenTask(developerRenderer, "Developer.DeveloperRender", "Developer Renderer", "Developer.RenderIcon", TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(discControl, "Medical.DiscEditor", "Disc Editor", "Developer.DiscEditorIcon", TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(advancedMandibleMovement, "Medical.AdvancedMandibleMovement", "Advanced Mandible Movement", "Developer.MovementIcon", TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(gridProperties, "Medical.GridProperties", "Grid", "Developer.GridIcon", TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(performanceGui, "Medical.Performance", "Performance", "Developer.StatisticsIcon", TaskMenuCategories.Developer));
            taskController.addTask(new MDIDialogOpenTask(measurementGUI, "Developer.Measurement", "Measurements", "Developer.Measurements", TaskMenuCategories.Developer));
            taskController.addTask(libRocketDebugger);
            taskController.addTask(new SaveMicrocodeCacheTask());
            changeRenderingMode = new ChangeRenderingMode(standaloneController.SceneViewController);
            taskController.addTask(changeRenderingMode);

            disablePhysics = new DisablePhysicsTask(int.MaxValue);
            taskController.addTask(disablePhysics);

            standaloneController.ViewHostFactory.addFactory(new WizardComponentViews());
        }

        public void sceneLoaded(SimScene scene)
        {
            advancedMandibleMovement.sceneLoaded(scene);
            discControl.sceneLoaded(scene);
            disablePhysics.sceneChanged(scene);
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

        public bool AllowUninstall
        {
            get
            {
                return true;
            }
        }
    }
}
