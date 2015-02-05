using System;
using System.Collections.Generic;
using System.Linq;
using Medical;
using Foundation;
using UIKit;
using Engine.Platform;
using System.Reflection;
using DentalSim;
using Medical.Movement;
using Developer;

namespace AnomalousMedicaliOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			Logging.Log.Default.addLogListener(new Logging.LogConsoleListener());

			StartupManager.SetupDllDirectories();

			AnomalousController anomalous = null;
			try
			{
				anomalous = new AnomalousController();
				anomalous.AddAdditionalPlugins += HandleAddAdditionalPlugins;
				anomalous.run();
			}
			catch (Exception e)
			{
//				Logging.Log.Default.printException(e);
//				if (anomalous != null)
//				{
//					anomalous.saveCrashLog();
//				}
//				String errorMessage = e.Message + "\n" + e.StackTrace;
//				while (e.InnerException != null)
//				{
//					e = e.InnerException;
//					errorMessage += "\n" + e.Message + "\n" + e.StackTrace;
//				}
//				MessageDialog.showErrorDialog(errorMessage, "Exception");
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
			controller.AtlasPluginManager.addPlugin(new PremiumBodyAtlasPlugin(controller));
			controller.AtlasPluginManager.addPlugin(new DentalSimPlugin());
			controller.AtlasPluginManager.addPlugin(new MovementBodyAtlasPlugin());
			controller.AtlasPluginManager.addPlugin(new DeveloperAtlasPlugin(controller));
		}

		void GlNoOp ()
		{
			OpenTK.Graphics.ES20.GL.IsEnabled (OpenTK.Graphics.ES20.EnableCap.Blend);
		}
	}
}
