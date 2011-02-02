using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;

namespace Medical
{
    class DopplerController : App
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
            return true;
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
                        String cameraFile, layersFile;
                        List<String> sequenceDirectories = new List<string>();
                        determineResourceFiles(out layersFile, out cameraFile, sequenceDirectories);
                        controller = new StandaloneController(this, "/Anomalous Medical/PiperDoppler", cameraFile, layersFile, sequenceDirectories);
                        controller.GUIManager.addPlugin(new DopplerGUIPlugin());
                        controller.go(createBackground(), "GUI/Doppler/SplashScreen");
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

        /// <summary>
        /// Create the background for the version that has been loaded.
        /// </summary>
        private ViewportBackground createBackground()
        {
            return new ViewportBackground("SourceBackground", "PiperJBODopplerBackground", 900, 500, 500, 5, 5);
        }

        private void determineResourceFiles(out String layersFile, out String cameraFile, List<String> sequenceDirectories)
        {
            sequenceDirectories.Add("/Doppler");
            cameraFile = "/DopplerCameras.cam";
            layersFile = "/DopplerLayers.lay";
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
                return SimulatedVersion.Doppler;
            }
        }
#endif
    }
}
