using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Medical.GUI;
using Engine;
using System.IO;
using Engine.ObjectManagement;
using OgrePlugin;

namespace Medical
{
    class BodyAtlasController : StandaloneApp
    {
        LicenseManager licenseManager;
        StandaloneController controller;
        bool startupSuceeded = false;
        AnatomyController anatomyController;
        BookmarksController bookmarksController;
        private SplashScreen splashScreen;

        private static String archiveNameFormat = "BodyAtlas{0}.dat";

        public override bool OnInit()
        {
            return startApplication();
        }

        public override int OnExit()
        {
            anatomyController.Dispose();
            bookmarksController.Dispose();
            controller.Dispose();
            return 0;
        }

        public override bool OnIdle()
        {
            if (startupSuceeded)
            {
                controller.onIdle();
                return MainWindow.Instance.Active;
            }
            else
            {
                return false;
            }
        }

        public bool startApplication()
        {
            //Core
            controller = new StandaloneController(this);
            controller.SceneLoaded += new SceneEvent(standaloneController_SceneLoaded);
            controller.SceneUnloading += new SceneEvent(standaloneController_SceneUnloading);
            controller.BeforeSceneLoadProperties += new SceneEvent(controller_BeforeSceneLoadProperties);
            splashScreen = new SplashScreen(OgreInterface.Instance.OgrePrimaryWindow, 100, "GUI/BodyAtlas/SplashScreen");
            splashScreen.Hidden += new EventHandler(splashScreen_Hidden);

            licenseManager = new LicenseManager("Piper's Joint Based Occlusion", Path.Combine(MedicalConfig.DocRoot, "license.lic"));
            splashScreen.updateStatus(10, "Initializing Core");
            controller.initializeControllers(createBackground());
            anatomyController = new AnatomyController(controller.ImageRenderer);

            //GUI
            splashScreen.updateStatus(20, "Creating GUI");
            WatermarkText = String.Format("Licensed to: {0}", licenseManager.LicenseeName);
            determineResourceFiles();
            bookmarksController = new BookmarksController(controller);
            controller.GUIManager.addPlugin(new BodyAtlasMainPlugin(licenseManager, this));
            if (true)//premium
            {
                controller.GUIManager.addPlugin(new PremiumBodyAtlasPlugin(licenseManager, anatomyController, bookmarksController));
            }
            else
            {
                controller.SceneViewController.AllowRotation = false;
                controller.SceneViewController.AllowZoom = false;
            }
            if (true)//editor
            {
                controller.GUIManager.addPlugin("Editor.dll");
            }
            controller.createGUI();

            //Scene Load
            splashScreen.updateStatus(40, "Loading Scene");
            startupSuceeded = controller.openNewScene(DefaultScene);

            splashScreen.updateStatus(100, "");
            splashScreen.hide();

            return startupSuceeded;
        }

        public void saveCrashLog()
        {
            if (controller != null)
            {
                controller.saveCrashLog();
            }
        }

        void standaloneController_SceneUnloading(SimScene scene)
        {
            anatomyController.sceneUnloading();
        }

        void standaloneController_SceneLoaded(SimScene scene)
        {
            anatomyController.sceneLoaded();
        }

        void controller_BeforeSceneLoadProperties(SimScene scene)
        {
            if (splashScreen != null)
            {
                splashScreen.updateStatus(75, "Loading Scene Properties");
            }
        }

        public override void createWindowPresets(SceneViewWindowPresetController windowPresetController)
        {
            windowPresetController.clearPresetSets();
            SceneViewWindowPresetSet primary = new SceneViewWindowPresetSet("Primary");
            SceneViewWindowPreset preset = new SceneViewWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            primary.addPreset(preset);
            primary.Hidden = true;
            windowPresetController.addPresetSet(primary);

            SceneViewWindowPresetSet oneWindow = new SceneViewWindowPresetSet("One Window");
            //oneWindow.Image = Resources.OneWindowLayout;
            preset = new SceneViewWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            oneWindow.addPreset(preset);
            windowPresetController.addPresetSet(oneWindow);

            SceneViewWindowPresetSet twoWindows = new SceneViewWindowPresetSet("Two Windows");
            //twoWindows.Image = Resources.TwoWindowLayout;
            preset = new SceneViewWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            twoWindows.addPreset(preset);
            preset = new SceneViewWindowPreset("Camera 2", new Vector3(0.0f, -5.0f, -170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            preset.ParentWindow = "Camera 1";
            preset.WindowPosition = WindowAlignment.Right;
            twoWindows.addPreset(preset);
            windowPresetController.addPresetSet(twoWindows);

            SceneViewWindowPresetSet threeWindows = new SceneViewWindowPresetSet("Three Windows");
            //threeWindows.Image = Resources.ThreeWindowLayout;
            preset = new SceneViewWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            threeWindows.addPreset(preset);
            preset = new SceneViewWindowPreset("Camera 2", new Vector3(-170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
            preset.ParentWindow = "Camera 1";
            preset.WindowPosition = WindowAlignment.Left;
            threeWindows.addPreset(preset);
            preset = new SceneViewWindowPreset("Camera 3", new Vector3(170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
            preset.ParentWindow = "Camera 1";
            preset.WindowPosition = WindowAlignment.Right;
            threeWindows.addPreset(preset);
            windowPresetController.addPresetSet(threeWindows);

            SceneViewWindowPresetSet fourWindows = new SceneViewWindowPresetSet("Four Windows");
            //fourWindows.Image = Resources.FourWindowLayout;
            preset = new SceneViewWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            fourWindows.addPreset(preset);
            preset = new SceneViewWindowPreset("Camera 2", new Vector3(0.0f, -5.0f, -170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            preset.ParentWindow = "Camera 1";
            preset.WindowPosition = WindowAlignment.Right;
            fourWindows.addPreset(preset);
            preset = new SceneViewWindowPreset("Camera 3", new Vector3(-170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
            preset.ParentWindow = "Camera 1";
            preset.WindowPosition = WindowAlignment.Bottom;
            fourWindows.addPreset(preset);
            preset = new SceneViewWindowPreset("Camera 4", new Vector3(170.0f, -5.0f, 0.0f), new Vector3(0.0f, -5.0f, 0.0f));
            preset.ParentWindow = "Camera 2";
            preset.WindowPosition = WindowAlignment.Bottom;
            fourWindows.addPreset(preset);
            windowPresetController.addPresetSet(fourWindows);
        }

        public override void addHelpDocuments(HtmlHelpController helpController)
        {
            helpController.AddBook(MedicalConfig.ProgramDirectory + "/Doc/PiperJBOManual.htb");
        }

        public override string WindowTitle
        {
            get 
            {
                return "Anomalous Body Atlas";
            }
        }

        public override string ProgramFolder
        {
            get
            {
                return "BodyAtlas";
            }
        }

        public override WindowIcons Icon
        {
            get
            {
                return WindowIcons.ICON_SKULL;
            }
        }

        public override String PrimaryArchive
        {
            get
            {
                return String.Format(archiveNameFormat, "");
            }
        }

        public override String getPatchArchiveName(int index)
        {
            return String.Format(archiveNameFormat, index);
        }

        public override String DefaultScene
        {
            get
            {
                return MedicalConfig.DefaultScene;
            }
        }

        public override int ProductID
        {
            get
            {
                return 1;
            }
        }

        public override bool IsTrial
        {
            get
            {
                return false;
            }
        }

        public AnatomyController AnatomyController
        {
            get
            {
                return anatomyController;
            }
        }

        /// <summary>
        /// Create the background for the version that has been loaded.
        /// </summary>
        private ViewportBackground createBackground()
        {
            OgreWrapper.OgreResourceGroupManager.getInstance().addResourceLocation("GUI/BodyAtlas/Background", "EngineArchive", "Background", false);
            OgreWrapper.OgreResourceGroupManager.getInstance().initializeAllResourceGroups();
            ViewportBackground background = new ViewportBackground("SourceBackground", "BodyAtlasBackground", 900, 500, 500, 5, 5);
            return background;
        }

        void splashScreen_Hidden(object sender, EventArgs e)
        {
            splashScreen.Dispose();
            splashScreen = null;
            bookmarksController.loadSavedBookmarks();
        }

        private void determineResourceFiles()
        {
            this.addMovementSequenceDirectory("/Graphics");
            this.addMovementSequenceDirectory("/MRI");
            this.addMovementSequenceDirectory("/RadiographyCT");
            this.addMovementSequenceDirectory("/Clinical");
            this.addMovementSequenceDirectory("/DentitionProfile");
            this.addMovementSequenceDirectory("/Doppler");
        }
    }
}
