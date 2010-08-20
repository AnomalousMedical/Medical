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
                        //MessageBox.Show("Your dongle does not allow the use of Piper's Joint Based Occlusion.", "Dongle Connection Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (result == ConnectionResult.TooManyUsers)
                {
                    connectionLoop = false;// MessageBox.Show("Too many users currently connected. Please shut down the program on another workstation.", "Network Dongle Connection Failure", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) == DialogResult.Retry;
                }
                else if (result == ConnectionResult.NoDongle)
                {
                    connectionLoop = false;// MessageBox.Show("Please connect your dongle.", "Dongle Connection Failure", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) == DialogResult.Retry;
                }
            }
            return startupSuceeded;
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
                catch (Exception e)
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
            try
            {
                StandaloneApp app = new StandaloneApp();
                app.Run();
            }
            catch (Exception e)
            {
                Logging.Log.Default.printException(e);
                String errorMessage = e.Message + "\n" + e.StackTrace;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    errorMessage += "\n" + e.Message + "\n" + e.StackTrace;
                }
                //MessageBox.Show(errorMessage, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
