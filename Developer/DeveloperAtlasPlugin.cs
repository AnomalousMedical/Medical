using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Engine.ObjectManagement;
using Medical.GUI;
using Developer.GUI;

namespace Developer
{
    class DeveloperAtlasPlugin : AtlasPlugin
    {
        private ExamViewer examViewer;
        private PluginPublisher pluginPublisher;
        private PluginPublishController pluginPublishController;

        public DeveloperAtlasPlugin(StandaloneController standaloneController)
        {

        }

        public void Dispose()
        {
            examViewer.Dispose();
            pluginPublisher.Dispose();
        }

        public void createMenuBar(NativeMenuBar menu)
        {

        }

        public void initialize(StandaloneController standaloneController)
        {
            pluginPublishController = new PluginPublishController(standaloneController.AtlasPluginManager);

            GUIManager guiManager = standaloneController.GUIManager;

            examViewer = new ExamViewer(standaloneController.ExamController);
            guiManager.addManagedDialog(examViewer);

            pluginPublisher = new PluginPublisher(pluginPublishController);
            guiManager.addManagedDialog(pluginPublisher);

            //Task Controller
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new MDIDialogOpenTask(examViewer, "Medical.ExamViewer", "Exam Viewer", "ExamIcon", TaskMenuCategories.Patient, 4));

            taskController.addTask(new MDIDialogOpenTask(pluginPublisher, "Developer.PluginPublisher", "Plugin Publisher", "ExamIcon", TaskMenuCategories.Editor));
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
                return -1;
            }
        }
    }
}
