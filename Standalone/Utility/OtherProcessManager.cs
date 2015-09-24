using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MyGUIPlugin;
using Logging;

namespace Medical
{
    public static class OtherProcessManager
    {
		/// <summary>
		/// Subscribe to this event to override the default openUrlInBrowser function with your own.
		/// </summary>
		public static Action<String> OpenUrlInBrowserOverride;

        public static bool openUrlInBrowser(String url)
        {
            try
            {
                if (OpenUrlInBrowserOverride != null)
                {
                    OpenUrlInBrowserOverride.Invoke(url);
                }
                else
                {
                    Process.Start(url);
                }
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
    }
}
