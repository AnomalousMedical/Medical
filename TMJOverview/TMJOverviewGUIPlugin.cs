﻿using System;
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
        private LicenseManager licenseManager;

        public TMJOverviewGUIPlugin(LicenseManager licenseManager)
        {
            this.licenseManager = licenseManager;
        }

        public void Dispose()
        {
            
        }

        public void initializeGUI(StandaloneController standaloneController, GUIManager guiManager)
        {
            this.standaloneController = standaloneController;
            this.guiManager = guiManager;
            guiManager.ScreenSizeChanged += new ScreenSizeChanged(guiManager_ScreenSizeChanged);

            OgreResourceGroupManager.getInstance().addResourceLocation("GUI/TMJOverview/Imagesets", "EngineArchive", "MyGUI", true);
            Gui.Instance.load("Imagesets.xml");

            standaloneController.TimelineController.PlaybackStarted += new EventHandler(TimelineController_PlaybackStarted);
            standaloneController.TimelineController.PlaybackStopped += new EventHandler(TimelineController_PlaybackStopped);
        }

        void guiManager_ScreenSizeChanged(int width, int height)
        {
            if (intro != null && intro.Visible)
            {
                intro.center();
            }
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
            bool keyValid = licenseManager.KeyValid;
            if (!keyValid)
            {
                licenseManager.KeyEnteredSucessfully += new EventHandler(licenseManager_KeyEnteredSucessfully);
                licenseManager.KeyInvalid += new EventHandler(licenseManager_KeyInvalid);
                setInterfaceEnabled(false);
                licenseManager.showKeyDialog(standaloneController.App.ProductID);
            }
            else
            {
                setInterfaceEnabled(false);
                intro.center();
                intro.open(true);
            }
        }

        void licenseManager_KeyInvalid(object sender, EventArgs e)
        {
            standaloneController.closeMainWindow();
        }

        void licenseManager_KeyEnteredSucessfully(object sender, EventArgs e)
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
