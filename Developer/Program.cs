using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Medical.Controller;
using Logging;

namespace Developer
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

            using (DeveloperController controller = new DeveloperController())
            {
                try
                {
                    controller.go();
                }
                catch (Exception e)
                {
                    Log.Default.printException(e);
                    String errorMessage = e.Message + "\n" + e.StackTrace;
                    while (e.InnerException != null)
                    {
                        e = e.InnerException;
                        errorMessage += "\n" + e.Message + "\n" + e.StackTrace;
                    }
                    MessageBox.Show(errorMessage, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
