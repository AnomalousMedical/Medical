using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Medical
{
    public class WindowsInfo
    {
        /// <summary>
        /// Determine if composition (Aero) is enabled.
        /// </summary>
        public static bool CompositionEnabled
        {
            get
            {
                try
                {
                    if (Environment.OSVersion.Version.Major < 6)
                    {
                        return false;
                    }
                    int enabled = 0;
                    DwmIsCompositionEnabled(ref enabled);
                    if (enabled == 0)
                    {
                        return false;
                    }
                }
                catch 
                { 
                    return false; 
                }
                return true;
            }
        }

        [DllImport("dwmapi.dll")]
        private extern static int DwmIsCompositionEnabled(ref int enabled);
    }
}
