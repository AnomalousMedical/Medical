using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;
using Logging;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Anomalous.OSPlatform;

namespace Medical
{
    class MacPlatformConfig : PlatformConfig
    {
        public MacPlatformConfig()
        {
			Log.ImportantInfo("Platform is Mac");
        }

        protected override String formatTitleImpl(String windowText, String subText)
        {
            return subText;
        }

        protected override TouchType TouchTypeImpl
        {
            get
            {
                return TouchType.None;
            }
        }

        protected override bool ForwardTouchAsMouseImpl
        {
            get
            {
                return false;
            }
        }

        protected override String ThemeFileImpl
        {
            get
            {
                return MyGUIPlugin.MyGUIInterface.DefaultOSXTheme;
            }
        }

        protected override bool AllowFullscreenImpl
        {
            get
            {
                return false;
            }
        }

        protected override bool AllowFullscreenToggleImpl
        {
            get
            {
                return true;
            }
        }

        protected override MouseButtonCode DefaultCameraMouseButtonImpl
        {
            get
            {
                return MouseButtonCode.MB_BUTTON0;
            }
        }

        protected override bool AllowCloneWindowsImpl
        {
            get
            {
                return false;
            }
        }

        protected override bool CloseMainWindowOnShutdownImpl
        {
            get
            {
                return false;
            }
        }

        protected override KeyboardButtonCode PanKeyImpl
        {
            get
            {
                return KeyboardButtonCode.KC_LWIN;
            }
        }

        protected override String OverrideFileLocationImpl
        {
            get
            {
                return "../../../override.ini";
            }
        }

        protected override bool DefaultEnableMultitouchImpl
        {
            get
            {
                return false;
            }
        }

        protected override int DefaultFPSCapImpl
        {
            get
            {
                return 0;
            }
        }

        protected override bool UnrestrictedEnvironmentImpl
        {
            get
            {
                return true;
            }
        }

        protected override bool AllowDllPluginsToLoadImpl
        {
            get
            {
                return true;
            }
        }

        protected override bool AutoSelectTextImpl
        {
            get
            {
                return true;
            }
        }

        protected override bool AllowCustomSaveLoadPathImpl
        {
            get
            {
                return true;
            }
        }

        protected override bool UseMobileViewsImpl
        {
            get
            {
                return false;
            }
        }
    }
}
