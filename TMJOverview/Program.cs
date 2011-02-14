using System;
using System.Collections.Generic;
using System.Linq;

namespace Medical
{
    static class Medical
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            TMJOverviewController controller = null;
            try
            {
                controller = new TMJOverviewController();
                controller.run();
            }
            catch (Exception e)
            {
                Logging.Log.Default.printException(e);
                if (controller != null)
                {
                    controller.saveCrashLog();
                }
                String errorMessage = e.Message + "\n" + e.StackTrace;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    errorMessage += "\n" + e.Message + "\n" + e.StackTrace;
                }
                MessageDialog.showErrorDialog(errorMessage, "Exception");
            }
        }
    }
}
