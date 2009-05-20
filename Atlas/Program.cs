using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Medical;
using Medical.Controller;
using Logging;

namespace Atlas
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (MedicalController controller = new MedicalController())
            {
                try
                {
                    controller.intialize();
                    controller.setMedicalInterface(new HeadController());
                    controller.start();
                }
                catch (Exception e)
                {
                    Log.Default.sendMessage("Exception: {0}.\n{1}\n{2}.", LogLevel.Error, "Anomaly", e.GetType().Name, e.Message, e.StackTrace);
                    while (e.InnerException != null)
                    {
                        e = e.InnerException;
                        Log.Default.sendMessage("--Inner exception: {0}.\n{1}\n{2}.", LogLevel.Error, "Anomaly", e.GetType().Name, e.Message, e.StackTrace);
                    }
                    MessageBox.Show(e.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
