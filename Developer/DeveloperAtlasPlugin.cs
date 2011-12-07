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

        public DeveloperAtlasPlugin(StandaloneController standaloneController)
        {

        }

        public void Dispose()
        {
            examViewer.Dispose();
            pluginPublisher.Dispose();
            developerRenderer.Dispose();
        }

        public void createMenuBar(NativeMenuBar menu)
        {

        }

        public void loadGUIResources()
        {
            Gui.Instance.load("Developer.Resources.DeveloperImagesets.xml");
        }

        public void initialize(StandaloneController standaloneController)
        {
            pluginPublishController = new PluginPublishController(standaloneController.AtlasPluginManager);

            GUIManager guiManager = standaloneController.GUIManager;

            examViewer = new ExamViewer(standaloneController.ExamController);
            guiManager.addManagedDialog(examViewer);

            pluginPublisher = new PluginPublisher(pluginPublishController);
            guiManager.addManagedDialog(pluginPublisher);

            developerRenderer = new DeveloperRenderPropertiesDialog(standaloneController.SceneViewController, standaloneController.ImageRenderer, guiManager);
            guiManager.addManagedDialog(developerRenderer);

            //Task Controller
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new MDIDialogOpenTask(examViewer, "Medical.ExamViewer", "Exam Viewer", "ExamIcon", TaskMenuCategories.Patient, 4));

            taskController.addTask(new MDIDialogOpenTask(pluginPublisher, "Developer.PluginPublisher", "Plugin Publisher", "ExamIcon", TaskMenuCategories.Editor));

            taskController.addTask(new MDIDialogOpenTask(developerRenderer, "Developer.DeveloperRender", "Developer Renderer", "RenderIcon", TaskMenuCategories.Tools));
        }

        public void sceneLoaded(SimScene scene)
        {

        }

        public void sceneRevealed()
        {

        }

        public void sceneUnloading(SimScene scene)
        {

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
