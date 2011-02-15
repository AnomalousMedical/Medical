using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine.ObjectManagement;
using MyGUIPlugin;
using OgreWrapper;
using Engine;

namespace Medical
{
    class TMJOverviewGUIPlugin : GUIPlugin
    {
        private StandaloneController standaloneController;
        private GUIManager guiManager;

        private Intro intro;
        private SystemMenu systemMenu;

        public TMJOverviewGUIPlugin()
        {
            
        }

        public void Dispose()
        {
            
        }

        public void initializeGUI(StandaloneController standaloneController, GUIManager guiManager)
        {
            this.standaloneController = standaloneController;
            this.guiManager = guiManager;

            OgreResourceGroupManager.getInstance().addResourceLocation("GUI/TMJOverview/Imagesets", "EngineArchive", "MyGUI", true);
            Gui.Instance.load("Imagesets.xml");

            standaloneController.TimelineController.PlaybackStarted += new EventHandler(TimelineController_PlaybackStarted);
            standaloneController.TimelineController.PlaybackStopped += new EventHandler(TimelineController_PlaybackStopped);
        }

        public void createDialogs(DialogManager dialogManager)
        {
            intro = new Intro(standaloneController.App.WindowTitle, this);
        }

        public void addToTaskbar(Taskbar taskbar)
        {

        }

        public void finishInitialization()
        {
            setInterfaceEnabled(false);
            intro.center();
            intro.open(true);
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
            systemMenu = new SystemMenu(menu, this, standaloneController);
            systemMenu.FileMenuEnabled = false;
        }

        public void runTMJOverview()
        {
            standaloneController.MedicalStateController.clearStates();
            standaloneController.MedicalStateController.createNormalStateFromScene();
            Timeline tl = standaloneController.TimelineController.openTimeline("Startup.tl");
            standaloneController.TimelineController.startPlayback(tl);
        }

        void options_VideoOptionsChanged(object sender, EventArgs e)
        {
            standaloneController.recreateMainWindow();
        }

        void TimelineController_PlaybackStopped(object sender, EventArgs e)
        {
            standaloneController.closeMainWindow();
        }

        void TimelineController_PlaybackStarted(object sender, EventArgs e)
        {
            setInterfaceEnabled(false);
        }

        void setInterfaceEnabled(bool enable)
        {
            guiManager.setMainInterfaceEnabled(enable);
            if(systemMenu != null)
            {
                systemMenu.FileMenuEnabled = enable;
            }
        }
    }
}
