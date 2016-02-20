using System;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using Medical;
using System.Net.Http;
using MonoMac.HttpClient;
using Anomalous.OSPlatform;
using System.IO;
using System.Runtime.InteropServices;
using Anomalous.OSPlatform.Mac;
using DentalSim;
using Lecture;

namespace AnomalousMedicalMac
{
    class MainClass
    {
        static void Main(string[] args)
        {
            NSApplication.Init();

            ServerConnection.HttpClientProvider = () => new HttpClient(new NativeMessageHandler());

            MacRuntimePlatformInfo.Initialize();
            OgrePlugin.OgreInterface.CompressedTextureSupport = OgrePlugin.CompressedTextureSupport.None;

            AnomalousController anomalous = null;
            try
            {
                anomalous = new AnomalousController()
                    {
                        PrimaryArchive = Path.Combine(FolderFinder.ExecutableFolder, "../Resources/AnomalousMedical.dat")
				};
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
                MessageDialog.showErrorDialog(String.Format("{0} occured. Message: {1}.\nPlease see log file for more information" , e.GetType().Name, e.Message), "Anomalous Medical Has Crashed");
                MacRuntimePlatformInfo.AlertCrashing();
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
			controller.AtlasPluginManager.addPlugin (new PremiumBodyAtlasPlugin (controller) {
				AllowUninstall = false
			});

			controller.AtlasPluginManager.addPlugin (new DentalSimPlugin () {
				AllowUninstall = false
			});

			controller.AtlasPluginManager.addPlugin (new LecturePlugin () {
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