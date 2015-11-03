using System;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using Medical;
using System.Net.Http;
using MonoMac.HttpClient;
using Anomalous.OSPlatform;
using System.IO;

namespace AnomalousMedicalMac
{
    class MainClass
    {
        static void Main(string[] args)
        {
            NSApplication.Init();

            ServerConnection.HttpClientProvider = () => new HttpClient(new NativeMessageHandler());

            NativePlatformPlugin.StaticInitialize();
            OgrePlugin.OgreInterface.CompressedTextureSupport = OgrePlugin.CompressedTextureSupport.None;

            AnomalousController anomalous = null;
            try
            {
                anomalous = new AnomalousController()
                    {
                        PrimaryArchive = Path.Combine(FolderFinder.ExecutableFolder, "../Resources/AnomalousMedical.dat")
                    };
                anomalous.run();
            }
            catch (Exception e)
            {
                Logging.Log.Default.printException(e);
                if (anomalous != null)
                {
                    anomalous.saveCrashLog();
                }
                MessageDialog.showErrorDialog(String.Format("{0} occured. Message: {1}.\nPlease see log file for more information" , e.GetType().Name, e.Message), "Anomalous Medical Has Crashed");
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