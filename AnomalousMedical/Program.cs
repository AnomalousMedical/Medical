using Anomalous.OSPlatform;
using Anomalous.OSPlatform.Win32;
using System;
using System.Net;

namespace Medical
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
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
    }
}
