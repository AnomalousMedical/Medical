using Engine.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class Main
    {
        public static void Run()
        {
            StartupManager.SetupDllDirectories();

            AnomalousController anomalous = null;
            try
            {
                anomalous = new AnomalousController();
                anomalous.run();
            }
            catch (Exception e)
            {
                Logging.Log.Default.printException(e);
                if (anomalous != null)
                {
                    anomalous.saveCrashLog();
                }
                String errorMessage = e.Message + "\n" + e.StackTrace;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    errorMessage += "\n" + e.Message + "\n" + e.StackTrace;
                }
                MessageDialog.showErrorDialog(errorMessage, "Exception");
            }
            finally
            {
                if (anomalous != null)
                {
                    anomalous.Dispose();
                }
            }
        }
    }
}
