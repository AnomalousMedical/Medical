using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Medical.GUI;
using Engine;

namespace Medical
{
    class DopplerController : StandaloneApp
    {
        StandaloneController controller;
        bool startupSuceeded = false;

        private static String archiveNameFormat = "Doppler{0}.dat";

        public override bool OnInit()
        {
            startApplication();
            return true;
        }

        public override int OnExit()
        {
            if (controller != null)
            {
                controller.Dispose();
            }
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
            this.addMovementSequenceDirectory("/Doppler");
            CamerasFile = "/GraphicsCameras.cam";
            LayersFile = "/StandaloneLayers.lay";
            controller = new StandaloneController(this);
            controller.GUIManager.addPlugin(new DopplerGUIPlugin());
            controller.go(createBackground(), "GUI/Doppler/SplashScreen");
            controller.TimelineController.ResourceProvider = new TimelineVirtualFSResourceProvider("Timelines/One Minute Doppler");
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
        }

        public override void addHelpDocuments(HtmlHelpController helpController)
        {

        }

        public override string WindowTitle
        {
            get
            {
                return "Doppler Diagnosis by Dr. Mark Piper";
            }
        }

        public override string ProgramFolder
        {
            get
            {
                return "PiperDoppler";
            }
        }

        public override String UpdateURL
        {
            get
            {
                return "http://www.AnomalousMedical.com/Update/PiperDopplerUpdate.xml";
            }
        }

        public override WindowIcons Icon
        {
            get
            {
                return WindowIcons.ICON_DOPPLER;
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

        /// <summary>
        /// Create the background for the version that has been loaded.
        /// </summary>
        private ViewportBackground createBackground()
        {
            OgreWrapper.OgreResourceGroupManager.getInstance().addResourceLocation("GUI/Doppler/Background", "EngineArchive", "Background", false);
            OgreWrapper.OgreResourceGroupManager.getInstance().initializeAllResourceGroups();
            return new ViewportBackground("SourceBackground", "DopplerDiagnosticModuleBackground", 900, 500, 500, 5, 5);
        }
    }
}
