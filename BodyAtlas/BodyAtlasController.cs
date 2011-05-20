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
using OgreWrapper;
using MyGUIPlugin;
using System.Diagnostics;

namespace Medical
{
    class BodyAtlasController : StandaloneApp
    {
        StandaloneController controller;
        bool startupSuceeded = false;
        private SplashScreen splashScreen;

        private static String archiveNameFormat = "BodyAtlas{0}.dat";

        public override bool OnInit()
        {
            return startApplication();
        }

        public override int OnExit()
        {
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
            controller.BeforeSceneLoadProperties += new SceneEvent(controller_BeforeSceneLoadProperties);
            OgreResourceGroupManager.getInstance().addResourceLocation(this.GetType().AssemblyQualifiedName, "EmbeddedResource", "MyGUI", true);
            splashScreen = new SplashScreen(OgreInterface.Instance.OgrePrimaryWindow, 100, "Medical.Resources.SplashScreen.SplashScreen.layout", "Medical.Resources.SplashScreen.SplashScreen.xml");
            splashScreen.Hidden += new EventHandler(splashScreen_Hidden);

            LicenseManager = new LicenseManager("Anomalous Body Atlas", Path.Combine(MedicalConfig.DocRoot, "license.lic"));
            WatermarkText = String.Format("Licensed to: {0}", LicenseManager.LicenseeName);
            splashScreen.updateStatus(10, "Initializing Core");
            controller.initializeControllers(createBackground());

            //GUI
            splashScreen.updateStatus(20, "Creating GUI");
            controller.createGUI();

            //Scene Load
            splashScreen.updateStatus(30, "Loading Scene");
            startupSuceeded = controller.openNewScene(DefaultScene);

            splashScreen.updateStatus(85, "Loading Plugins");
            finishInitialization();

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

        /// <summary>
        /// Create the background for the version that has been loaded.
        /// </summary>
        private ViewportBackground createBackground()
        {
            OgreResourceGroupManager.getInstance().addResourceLocation(this.GetType().AssemblyQualifiedName, "EmbeddedResource", "Background", true);
            OgreWrapper.OgreResourceGroupManager.getInstance().initializeAllResourceGroups();
            ViewportBackground background = new ViewportBackground("SourceBackground", "BodyAtlasBackground", 900, 500, 500, 5, 5);
            return background;
        }

        void splashScreen_Hidden(object sender, EventArgs e)
        {
            splashScreen.Dispose();
            splashScreen = null;
            controller.sceneRevealed();
        }

        private void addPlugins()
        {
            controller.AtlasPluginManager.addPlugin(new BodyAtlasMainPlugin(LicenseManager, this));
            if (true)//PiperJBO
            {
                controller.AtlasPluginManager.addPlugin("PiperJBO.dll");
            }
            if (true)//premium
            {
                controller.AtlasPluginManager.addPlugin("Premium.dll");
            }
            else
            {
                controller.SceneViewController.AllowRotation = false;
                controller.SceneViewController.AllowZoom = false;
            }
            if (true)//Lecture
            {
                controller.AtlasPluginManager.addPlugin("Lecture.dll");
            }
            if (true)//editor
            {
                controller.AtlasPluginManager.addPlugin("Editor.dll");
            }
        }

        #region License

        public void finishInitialization()
        {
            if (LicenseManager.KeyValid)
            {
                addPlugins();
                controller.GUIManager.createGUIPlugins();
            }
            else
            {
                controller.GUIManager.setMainInterfaceEnabled(false);
                startKeyDialog();
            }
        }

        private void startKeyDialog()
        {
            LicenseManager.KeyEnteredSucessfully += new EventHandler(licenseManager_KeyEnteredSucessfully);
            LicenseManager.KeyInvalid += new EventHandler(licenseManager_KeyInvalid);
            LicenseManager.showKeyDialog(ProductID);
        }

        void licenseManager_KeyInvalid(object sender, EventArgs e)
        {
            controller.closeMainWindow();
        }

        void licenseManager_KeyEnteredSucessfully(object sender, EventArgs e)
        {
            controller.GUIManager.setMainInterfaceEnabled(true);
            addPlugins();
            controller.GUIManager.createGUIPlugins();
            //MessageBox.show("Please restart to apply your license changes.", "Restart required", MessageBoxStyle.Ok | MessageBoxStyle.IconInfo, restartMessageClosed);
        }

        #endregion
    }
}
