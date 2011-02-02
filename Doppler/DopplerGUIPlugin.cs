using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine.ObjectManagement;

namespace Medical
{
    class DopplerGUIPlugin : GUIPlugin
    {
        private StandaloneController standaloneController;
        private GUIManager guiManager;

        private DopplerAppMenu appMenu;

        public DopplerGUIPlugin()
        {

        }

        public void Dispose()
        {
            
        }

        public void initializeGUI(StandaloneController standaloneController, GUIManager guiManager)
        {
            this.standaloneController = standaloneController;
            this.guiManager = guiManager;
            appMenu = new DopplerAppMenu(this, standaloneController);
            guiManager.setAppMenu(appMenu);
        }

        public void createDialogs(DialogManager dialogManager)
        {
            
        }

        public void addToTaskbar(Taskbar taskbar)
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
    }
}
