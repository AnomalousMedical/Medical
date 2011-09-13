using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical;
using Engine.ObjectManagement;
using Medical.GUI;

namespace Developer
{
    class DeveloperAtlasPlugin : AtlasPlugin
    {
        private ExamViewer examViewer;

        public DeveloperAtlasPlugin(StandaloneController standaloneController)
        {

        }

        public void Dispose()
        {
            examViewer.Dispose();
        }

        public void createMenuBar(NativeMenuBar menu)
        {

        }

        public void initialize(StandaloneController standaloneController)
        {
            GUIManager guiManager = standaloneController.GUIManager;

            examViewer = new ExamViewer(standaloneController.ExamController);
            guiManager.addManagedDialog(examViewer);

            //Task Controller
            TaskController taskController = standaloneController.TaskController;

            taskController.addTask(new MDIDialogOpenTask(examViewer, "Medical.ExamViewer", "Exam Viewer", "ExamIcon", TaskMenuCategories.Patient, 4));
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
    }
}
