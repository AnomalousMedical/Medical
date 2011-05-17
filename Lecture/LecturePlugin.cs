using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine.ObjectManagement;
using MyGUIPlugin;

namespace Medical
{
    class LecturePlugin : AtlasPlugin
    {
        private CloneWindowDialog cloneWindowDialog;
        private StandaloneController standaloneController;

        public LecturePlugin()
        {

        }

        public void Dispose()
        {
            cloneWindowDialog.Dispose();
        }

        public void initializeGUI(StandaloneController standaloneController, GUIManager guiManager)
        {
            Gui.Instance.load("Medical.Resources.LectureIcons.xml");

            this.standaloneController = standaloneController;
        }

        public void createDialogs(DialogManager dialogManager)
        {
            cloneWindowDialog = new CloneWindowDialog();
        }

        public void addToTaskbar(Taskbar taskbar)
        {
            taskbar.addItem(new CloneWindowTaskbarItem(standaloneController));
        }

        public void finishInitialization()
        {
            
        }

        public void sceneLoaded(SimScene scene)
        {
            
        }

        public void sceneUnloading(SimScene scene)
        {
            
        }

        public void setMainInterfaceEnabled(bool enabled)
        {
            
        }

        public void createMenuBar(NativeMenuBar menu)
        {
            
        }

        public void sceneRevealed()
        {
            
        }
    }
}
