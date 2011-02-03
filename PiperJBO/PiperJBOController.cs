using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Medical.GUI;
using Engine;

namespace Medical
{
    class PiperJBOController : StandaloneApp
    {
        UserPermissions permissions;
        StandaloneController controller;

        public override bool OnInit()
        {
            return startApplication();
        }

        public override int OnExit()
        {
            controller.Dispose();
            permissions.Dispose();
            return 0;
        }

        public override bool OnIdle()
        {
            controller.onIdle();
            return MainWindow.Instance.Active;
        }

        public bool startApplication()
        {
            bool connectionLoop = true;
            bool startupSuceeded = false;
#if ENABLE_HASP_PROTECTION
            permissions = new UserPermissions()
#else
            permissions = new UserPermissions(getSimulatedVersion());
#endif
            while (connectionLoop)
            {
                ConnectionResult result = UserPermissions.Instance.checkConnection();
                if (result == ConnectionResult.Ok)
                {
                    connectionLoop = false;
                    if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_MODULE))
                    {
                        determineResourceFiles();
                        controller = new StandaloneController(this);
                        controller.GUIManager.addPlugin(new PiperJBOGUIPlugin());
                        controller.GUIManager.addPlugin("Editor.dll");
                        controller.go(createBackground(), "GUI/PiperJBO/SplashScreen");
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

            SceneViewWindowPresetSet oneWindow = new SceneViewWindowPresetSet("One Window");
            //oneWindow.Image = Resources.OneWindowLayout;
            preset = new SceneViewWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
            oneWindow.addPreset(preset);
            windowPresetController.addPresetSet(oneWindow);

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_CLINICAL) ||
                UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_RADIOGRAPHY_CT) ||
                UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_MRI) ||
                UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_GRAPHICS))
            {
                SceneViewWindowPresetSet twoWindows = new SceneViewWindowPresetSet("Two Windows");
                //twoWindows.Image = Resources.TwoWindowLayout;
                preset = new SceneViewWindowPreset("Camera 1", new Vector3(0.0f, -5.0f, 170.0f), new Vector3(0.0f, -5.0f, 0.0f));
                twoWindows.addPreset(preset);
                preset = new SceneViewWindowPreset("Camera 2", new Vector3(0.0f, -5.0f, -170.0f), new Vector3(0.0f, -5.0f, 0.0f));
                preset.ParentWindow = "Camera 1";
                preset.WindowPosition = WindowAlignment.Right;
                twoWindows.addPreset(preset);
                windowPresetController.addPresetSet(twoWindows);
            }

            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_GRAPHICS))
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

        /// <summary>
        /// Create the background for the version that has been loaded.
        /// </summary>
        private ViewportBackground createBackground()
        {
            ViewportBackground background = null;
            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_GRAPHICS))
            {
                background = new ViewportBackground("SourceBackground", "PiperJBOGraphicsBackground", 900, 500, 500, 5, 5);
            }
            else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_MRI))
            {
                background = new ViewportBackground("SourceBackground", "PiperJBOMRIBackground", 900, 500, 500, 5, 5);
            }
            else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_RADIOGRAPHY_CT))
            {
                background = new ViewportBackground("SourceBackground", "PiperJBORadiographyBackground", 900, 500, 500, 5, 5);
            }
            else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_CLINICAL))
            {
                background = new ViewportBackground("SourceBackground", "PiperJBOClinicalBackground", 900, 500, 500, 5, 5);
            }
            else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_DENTITION_PROFILE))
            {
                background = new ViewportBackground("SourceBackground", "PiperJBODentitionProfileBackground", 900, 500, 500, 5, 5);
            }
            else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_DOPPLER))
            {
                background = new ViewportBackground("SourceBackground", "PiperJBODopplerBackground", 900, 500, 500, 5, 5);
            }
            return background;
        }

        private void determineResourceFiles()
        {
            CamerasFile = null;
            LayersFile = null;
            if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_GRAPHICS))
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
            else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_MRI))
            {
                this.addMovementSequenceDirectory("/MRI");
                this.addMovementSequenceDirectory("/RadiographyCT");
                this.addMovementSequenceDirectory("/Clinical");
                this.addMovementSequenceDirectory("/DentitionProfile");
                this.addMovementSequenceDirectory("/Doppler");
                CamerasFile = "/MRICameras.cam";
                LayersFile = "/MRILayers.lay";
            }
            else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_RADIOGRAPHY_CT))
            {
                this.addMovementSequenceDirectory("/RadiographyCT");
                this.addMovementSequenceDirectory("/Clinical");
                this.addMovementSequenceDirectory("/DentitionProfile");
                this.addMovementSequenceDirectory("/Doppler");
                CamerasFile = "/RadiographyCameras.cam";
                LayersFile = "/RadiographyLayers.lay";
            }
            else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_CLINICAL))
            {
                this.addMovementSequenceDirectory("/Clinical");
                this.addMovementSequenceDirectory("/DentitionProfile");
                this.addMovementSequenceDirectory("/Doppler");
                CamerasFile = "/ClinicalCameras.cam";
                LayersFile = "/ClinicalLayers.lay";
            }
            else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_DENTITION_PROFILE))
            {
                this.addMovementSequenceDirectory("/DentitionProfile");
                this.addMovementSequenceDirectory("/Doppler");
                CamerasFile = "/DentitionProfileCameras.cam";
                LayersFile = "/DentitionProfileLayers.lay";
            }
            else if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_VERSION_DOPPLER))
            {
                this.addMovementSequenceDirectory("/Doppler");
                CamerasFile = "/DopplerCameras.cam";
                LayersFile = "/DopplerLayers.lay";
            }

            //temp, load different layers file
            LayersFile = "/StandaloneLayers.lay";
            //end temp
        }

#if !ENABLE_HASP_PROTECTION
        private static SimulatedVersion getSimulatedVersion()
        {
            String[] args = Environment.GetCommandLineArgs();
            if (args.Length > 2 && args[1] == "-e")
            {
                try
                {
                    SimulatedVersion version = (SimulatedVersion)Enum.Parse(typeof(SimulatedVersion), args[2]);
                    return version;
                }
                catch (Exception)
                {
                    Console.Error.WriteLine(String.Format("The edition specified \'{0}\' is not valid.", args[2]));
                    return SimulatedVersion.Graphics;
                }
            }
            else
            {
                return SimulatedVersion.Graphics;
            }
        }
#endif
    }
}
