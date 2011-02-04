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
        UserPermissions permissions;
        StandaloneController controller;
        bool startupSuceeded = false;

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
            if (permissions != null)
            {
                permissions.Dispose();
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
            bool connectionLoop = true;
#if ENABLE_HASP_PROTECTION
            permissions = new UserPermissions();
#else
            permissions = new UserPermissions(SimulatedVersion.Doppler);
#endif
            while (connectionLoop)
            {
                ConnectionResult result = UserPermissions.Instance.checkConnection();
                if (result == ConnectionResult.Ok)
                {
                    connectionLoop = false;
                    if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_MODULE))
                    {
                        this.addMovementSequenceDirectory("/Doppler");
                        CamerasFile = "/GraphicsCameras.cam";
                        LayersFile = "/StandaloneLayers.lay";
                        controller = new StandaloneController(this);
                        controller.GUIManager.addPlugin(new DopplerGUIPlugin());
                        controller.go(createBackground(), "GUI/Doppler/SplashScreen");
                        controller.TimelineController.ResourceProvider = new TimelineReadOnlyZipResources("S:/export/Timelines/One Minute Doppler.tlp");
                        startupSuceeded = true;
                    }
                    else
                    {
                        MessageDialog.showErrorDialog("Your dongle does not allow the use of Piper's Joint Based Occlusion.", "Dongle Connection Failure");
                    }
                }
                else if (result == ConnectionResult.TooManyUsers)
                {
                    connectionLoop = MessageDialog.showQuestionDialog("Too many users currently connected. Please shut down the program on another workstation. Would you like to try to connect again?", "Network Dongle Connection Failure") == NativeDialogResult.YES;
                }
                else if (result == ConnectionResult.NoDongle)
                {
                    connectionLoop = MessageDialog.showQuestionDialog("Please connect your dongle. Would you like to try to connect again?", "Dongle Connection Failure") == NativeDialogResult.YES;
                }
            }
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
                return "Dr. Piper's Chair Side Consultation: Doppler";
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

        /// <summary>
        /// Create the background for the version that has been loaded.
        /// </summary>
        private ViewportBackground createBackground()
        {
            return new ViewportBackground("SourceBackground", "PiperJBODopplerBackground", 900, 500, 500, 5, 5);
        }
    }
}
