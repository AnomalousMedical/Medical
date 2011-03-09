using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Medical.GUI;
using Engine;
using System.IO;

namespace Medical
{
    class PiperJBOController : StandaloneApp
    {
        LicenseManager licenseManager;
        StandaloneController controller;
        bool startupSuceeded = false;

        private static String archiveNameFormat = "PiperJBO{0}.dat";

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
            controller = new StandaloneController(this);
            licenseManager = new LicenseManager("Piper's Joint Based Occlusion", Path.Combine(MedicalConfig.DocRoot, "license.lic"));
            determineResourceFiles();
            controller.GUIManager.addPlugin(new PiperJBOGUIPlugin(licenseManager, this));
            if (licenseManager.allowFeature((int)Features.PIPER_JBO_VERSION_GRAPHICS))
            {
                controller.GUIManager.addPlugin("Editor.dll");
            }
            controller.go(createBackground(), "GUI/PiperJBO/SplashScreen");
            startupSuceeded = true;
            return startupSuceeded;
        }

        public void saveCrashLog()
        {
            if (controller != null)
            {
                controller.saveCrashLog();
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

            if (licenseManager.allowFeature((int)Features.PIPER_JBO_VERSION_GRAPHICS))
            {
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
        }

        public override void addHelpDocuments(HtmlHelpController helpController)
        {
            helpController.AddBook(MedicalConfig.ProgramDirectory + "/Doc/PiperJBOManual.htb");
        }

        public override string WindowTitle
        {
            get 
            {
                return "Piper's Joint Based Occlusion";
            }
        }

        public override string ProgramFolder
        {
            get
            {
                return "PiperJBO";
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
            OgreWrapper.OgreResourceGroupManager.getInstance().addResourceLocation("GUI/PiperJBO/Background", "EngineArchive", "Background", false);
            OgreWrapper.OgreResourceGroupManager.getInstance().initializeAllResourceGroups();
            ViewportBackground background = null;
            if (licenseManager.allowFeature((int)Features.PIPER_JBO_VERSION_GRAPHICS))
            {
                background = new ViewportBackground("SourceBackground", "PiperJBOGraphicsBackground", 900, 500, 500, 5, 5);
            }
            else if (licenseManager.allowFeature((int)Features.PIPER_JBO_VERSION_MRI) ||
                licenseManager.allowFeature((int)Features.PIPER_JBO_VERSION_RADIOGRAPHY_CT))
            {
                background = new ViewportBackground("SourceBackground", "PiperJBOMRIBackground", 900, 500, 500, 5, 5);
            }
            else
            {
                background = new ViewportBackground("SourceBackground", "PiperJBOClinicalBackground", 900, 500, 500, 5, 5);
            }
            return background;
        }

        private void determineResourceFiles()
        {
            CamerasFile = null;
            LayersFile = null;
            if (licenseManager.allowFeature((int)Features.PIPER_JBO_VERSION_GRAPHICS))
            {
                this.addMovementSequenceDirectory("/Graphics");
                this.addMovementSequenceDirectory("/MRI");
                this.addMovementSequenceDirectory("/RadiographyCT");
                this.addMovementSequenceDirectory("/Clinical");
                this.addMovementSequenceDirectory("/DentitionProfile");
                this.addMovementSequenceDirectory("/Doppler");
                CamerasFile = "/GraphicsCameras.cam";
                LayersFile = "/GraphicsLayers.lay";
            }
            else if (licenseManager.allowFeature((int)Features.PIPER_JBO_VERSION_MRI) ||
                licenseManager.allowFeature((int)Features.PIPER_JBO_VERSION_RADIOGRAPHY_CT))
            {
                this.addMovementSequenceDirectory("/MRI");
                this.addMovementSequenceDirectory("/RadiographyCT");
                this.addMovementSequenceDirectory("/Clinical");
                this.addMovementSequenceDirectory("/DentitionProfile");
                this.addMovementSequenceDirectory("/Doppler");
                CamerasFile = "/MRICameras.cam";
                LayersFile = "/MRILayers.lay";
            }
            else
            {
                this.addMovementSequenceDirectory("/Clinical");
                this.addMovementSequenceDirectory("/DentitionProfile");
                this.addMovementSequenceDirectory("/Doppler");
                CamerasFile = "/ClinicalCameras.cam";
                LayersFile = "/ClinicalLayers.lay";
            }

            //temp, load different layers file
            LayersFile = "/StandaloneLayers.lay";
            //end temp
        }
    }
}
