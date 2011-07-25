using System;
using System.Collections.Generic;

namespace Medical
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            BodyAtlasController bodyAtlas = null;
            try
            {
                bodyAtlas = new BodyAtlasController();
                bodyAtlas.run();
            }
            catch (Exception e)
            {
                Logging.Log.Default.printException(e);
                if (bodyAtlas != null)
                {
                    bodyAtlas.saveCrashLog();
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
