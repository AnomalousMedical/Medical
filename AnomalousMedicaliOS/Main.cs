using System;
using System.Collections.Generic;
using System.Linq;
using Medical;
using Foundation;
using UIKit;
using Engine.Platform;
using System.Reflection;
using DentalSim;
using CoreGraphics;
using Medical.Controller;
using Anomalous.OSPlatform;
using System.IO;

#if ALLOW_OVERRIDE
using Medical.Movement;
using Developer;
#endif

namespace AnomalousMedicaliOS
{
	public class Application
	{
		private static TouchMouseGuiForwarder touchForwarder;

		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			NativePlatformPlugin.StaticInitialize();
            OgrePlugin.OgreInterface.CompressedTextureSupport = OgrePlugin.CompressedTextureSupport.None;
            ServerConnection.EnableUnsafeTLS1_0 = true;

			#if DEBUG
			Logging.Log.Default.addLogListener(new Logging.LogConsoleListener());
			#endif

			OtherProcessManager.OpenUrlInBrowserOverride = openUrl;

			AnomalousController anomalous = null;
			try
			{
				anomalous = new AnomalousController()
                    {
                        PrimaryArchive = Path.Combine(FolderFinder.ExecutableFolder, "AnomalousMedicaliOS.dat")
                    };
				anomalous.OnInitCompleted += HandleOnInitCompleted;
				anomalous.AddAdditionalPlugins += HandleAddAdditionalPlugins;
				anomalous.run();
			}
			catch (Exception e)
			{
				Logging.Log.Default.printException(e);
                Logging.Log.Error("{0} occured. Message: {1}", e.GetType().Name, e.Message);
			}
			finally
			{
				if (anomalous != null)
				{
					anomalous.Dispose();
				}
			}
		}

		static void HandleOnInitCompleted (AnomalousController anomalousController, StandaloneController controller)
		{
			touchForwarder = controller.MedicalController.TouchMouseGuiForwarder;
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

			#if ALLOW_OVERRIDE
			controller.AtlasPluginManager.addPlugin(new MovementBodyAtlasPlugin()
			{
				AllowUninstall = false
			});
			controller.AtlasPluginManager.addPlugin(new DeveloperAtlasPlugin(controller)
			{
				AllowUninstall = false
			});
			#endif
		}

		static void openUrl(String url)
		{
			UIViewController currentController = UIApplication.SharedApplication.KeyWindow.RootViewController;
			while(currentController.PresentedViewController != null)
			{
				currentController = currentController.PresentedViewController;
			}

			UIView currentView = currentController.View;

			InAppBrowser browser = new InAppBrowser(currentView, url, touchForwarder);
		}

		void GlNoOp ()
		{
			OpenTK.Graphics.ES20.GL.IsEnabled (OpenTK.Graphics.ES20.EnableCap.Blend);
		}
	}
}
