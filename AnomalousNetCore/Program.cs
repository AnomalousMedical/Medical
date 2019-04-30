using Anomalous.OSPlatform;
using Anomalous.OSPlatform.Win32;
using DentalSim;
using Lecture;
using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Medical
{
    public static class Program
    {
        /// <summary>
        /// Calling this function will set the dpi awareness. It should be done first.
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetProcessDPIAware();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            SetProcessDPIAware();

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12; //Enable TLS 1.1 and 1.2

            var osVersion = Environment.OSVersion.Version;
            if (!(osVersion.Major == 6 && osVersion.Minor == 0)) //Only disable if not on vista (6.0) we don't worry about earlier versions since this won't even run on xp and vista's days are numbered.
            {
                ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls11;
            }

            WindowsRuntimePlatformInfo.Initialize();
            OgrePlugin.OgreInterface.CompressedTextureSupport = OgrePlugin.CompressedTextureSupport.None;

            AnomalousController anomalous = null;
            try
            {
                anomalous = new AnomalousController();
                anomalous.AddAdditionalPlugins += HandleAddAdditionalPlugins;
                anomalous.run();
            }
            catch (Exception e)
            {
                Logging.Log.Default.printException(e);
                if (anomalous != null)
                {
                    anomalous.saveCrashLog();
                }
                String errorMessage = e.Message + "\n" + e.StackTrace;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    errorMessage += "\n" + e.Message + "\n" + e.StackTrace;
                }
                MessageDialog.showErrorDialog(errorMessage, "Exception");
            }
            finally
            {
                if (anomalous != null)
                {
                    anomalous.Dispose();
                }
            }
        }

        static void HandleAddAdditionalPlugins(AnomalousController anomalousController, StandaloneController controller)
        {
            controller.AtlasPluginManager.addPlugin(new PremiumBodyAtlasPlugin(controller)
            {
                AllowUninstall = false
            });

            controller.AtlasPluginManager.addPlugin(new DentalSimPlugin()
            {
                AllowUninstall = false
            });

            controller.AtlasPluginManager.addPlugin(new LecturePlugin()
            {
                AllowUninstall = false
            });

#if ALLOW_OVERRIDE
            controller.AtlasPluginManager.addPlugin(new Movement.MovementBodyAtlasPlugin()
            {
                AllowUninstall = false
            });
            controller.AtlasPluginManager.addPlugin(new Developer.DeveloperAtlasPlugin(controller)
            {
                AllowUninstall = false
            });
            controller.AtlasPluginManager.addPlugin(new EditorPlugin()
            {
                AllowUninstall = false
            });
#endif
        }
    }
}
