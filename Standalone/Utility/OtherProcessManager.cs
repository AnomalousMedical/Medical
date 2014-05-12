using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MyGUIPlugin;
using Logging;

namespace Medical
{
    public class OtherProcessManager
    {
        private OtherProcessManager()
        {

        }

        public static bool openUrlInBrowser(String url)
        {
            try
            {
                Process.Start(url);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.show(String.Format("There was a problem opening '{0}' in your browser. You can open you browser and directly put in the url\n{0}\nto view the content you requested.", url), "Browser Error", MessageBoxStyle.Ok | MessageBoxStyle.IconWarning);
                Log.Error("Could not open url {0} because: {1}", url, e.Message);
                return false;
            }
        }

        public static bool openLocalURL(String url)
        {
            try
            {
                Process.Start(url);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.show(String.Format("There was a problem opening '{0}'.\nReason: {1}", url, e.Message), "Open Error", MessageBoxStyle.Ok | MessageBoxStyle.IconWarning);
                Log.Error("Could not open url {0} because: {1}", url, e.Message);
                return false;
            }
        }

        public static void restart()
        {
            try
            {
                Process.Start(PlatformConfig.RestartProcInfo);
            }
            catch (Exception e)
            {
                MessageDialog.showErrorDialog(String.Format("Could not restart program automatically. Please restart it manually by clicking its icon.\nReason:\n{0}", e.Message), "Restart Error");
                Log.Error("Could not restart program because {0}", e.Message);
            }
        }

        public static void restartAdmin()
        {
            try
            {
                Process.Start(PlatformConfig.RestartAdminProcInfo);
            }
            catch (Exception e)
            {
                MessageDialog.showErrorDialog(String.Format("Could not restart program automatically. Please restart it manually by clicking its icon.\nReason:\n{0}", e.Message), "Restart Error");
                Log.Error("Could not restart program because {0}", e.Message);
            }
        }
    }
}
