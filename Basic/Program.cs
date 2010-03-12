using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Logging;
using Medical.GUI;
using Medical.Controller;

namespace Medical
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// 
        /// The edition of the program to run can be specified on the command
        /// line by typing -e followed by one of the following: Doppler,
        /// DentitionAndProfile, Clinical, Radiography, MRI or Graphics. That will
        /// cause the given edition to open instead of the Graphics edition.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool connectionLoop = true;
#if ENABLE_HASP_PROTECTION
            using (UserPermissions permissions = new UserPermissions())
#else
            using (UserPermissions permissions = new UserPermissions(getSimulatedVersion()))
#endif
            {
                while (connectionLoop)
                {
                    ConnectionResult result = UserPermissions.Instance.checkConnection();
                    if (result == ConnectionResult.Ok)
                    {
                        connectionLoop = false;
                        if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_MODULE))
                        {
                            using (BasicController controller = new BasicController())
                            {
                                try
                                {
                                    controller.go();
                                }
                                catch (Exception e)
                                {
                                    Log.Default.printException(e);
                                    controller.saveCrashLog();
                                    MessageBox.Show(e.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Your dongle does not allow the use of Piper's Joint Based Occlusion.", "Dongle Connection Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else if (result == ConnectionResult.TooManyUsers)
                    {
                        connectionLoop = MessageBox.Show("Too many users currently connected. Please shut down the program on another workstation.", "Network Dongle Connection Failure", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) == DialogResult.Retry;
                    }
                    else if (result == ConnectionResult.NoDongle)
                    {
                        connectionLoop = MessageBox.Show("Please connect your dongle.", "Dongle Connection Failure", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) == DialogResult.Retry;
                    }
                }
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
}
