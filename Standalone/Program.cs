using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;
using Engine;
using OgrePlugin;
using BulletPlugin;
using Engine.Platform;
using Engine.Renderer;
using Medical;
using System.Xml;
using Engine.ObjectManagement;
using System.IO;
using Engine.Saving.XMLSaver;
using OgreWrapper;
using Medical.Controller;
using wx;
using System.Drawing;

namespace Standalone
{
    class StandaloneApp : App
    {
        UserPermissions permissions;
        StandaloneController controller;

        public override bool OnInit()
        {
            wx.Image.InitAllHandlers();
            return startApplication();
        }

        public override int OnExit()
        {
            controller.Dispose();
            permissions.Dispose();
            return base.OnExit();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
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
                        controller = new StandaloneController();
                        controller.go();
                        startupSuceeded = true;
                    }
                    else
                    {
                        wx.MessageDialog.ShowModal("Your dongle does not allow the use of Piper's Joint Based Occlusion.", "Dongle Connection Failure", WindowStyles.DIALOG_OK | WindowStyles.ICON_ERROR);
                    }
                }
                else if (result == ConnectionResult.TooManyUsers)
                {
                    connectionLoop = wx.MessageDialog.ShowModal("Too many users currently connected. Please shut down the program on another workstation. Would you like to try to connect again?", "Network Dongle Connection Failure", WindowStyles.DIALOG_YES_NO | WindowStyles.ICON_QUESTION) == ShowModalResult.YES;
                }
                else if (result == ConnectionResult.NoDongle)
                {
                    connectionLoop = wx.MessageDialog.ShowModal("Please connect your dongle. Would you like to try to connect again?", "Dongle Connection Failure", WindowStyles.DIALOG_YES_NO | WindowStyles.ICON_QUESTION) == ShowModalResult.YES;
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

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            StandaloneApp app = null;
            try
            {
                app = new StandaloneApp();
                app.Run();
            }
            catch (Exception e)
            {
                Logging.Log.Default.printException(e);
                if (app != null)
                {
                    app.saveCrashLog();
                }
                String errorMessage = e.Message + "\n" + e.StackTrace;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    errorMessage += "\n" + e.Message + "\n" + e.StackTrace;
                }
                wx.MessageDialog.ShowModal(errorMessage, "Exception", WindowStyles.DIALOG_OK | WindowStyles.ICON_ERROR);
            }
        }
    }
}
