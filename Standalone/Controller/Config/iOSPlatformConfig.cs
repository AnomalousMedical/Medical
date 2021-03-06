﻿using System;
using Logging;
using Anomalous.OSPlatform;
using Engine.Platform;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Medical
{
    public class iOSPlatformConfig : PlatformConfig
    {
        public iOSPlatformConfig()
        {
            Log.ImportantInfo("Platform is iOS");
        }

        protected override String formatTitleImpl(String windowText, String subText)
        {
            return subText;
        }

        protected override TouchType TouchTypeImpl
        {
            get
            {
                return TouchType.Screen;
            }
        }

        protected override bool ForwardTouchAsMouseImpl
        {
            get
            {
                return true;
            }
        }

        protected override String ThemeFileImpl
        {
            get
            {
                return MyGUIPlugin.MyGUIInterface.DefaultPureTabletTheme;
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
                return false;
            }
        }

        protected override MouseButtonCode DefaultCameraMouseButtonImpl
        {
            get
            {
                return MouseButtonCode.MB_BUTTON1;
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
                return true;
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
                return false;
            }
        }

        protected override bool AllowDllPluginsToLoadImpl
        {
            get
            {
                return false;
            }
        }

        protected override bool AutoSelectTextImpl
        {
            get
            {
                return false;
            }
        }

        protected override bool AllowCustomSaveLoadPathImpl
        {
            get
            {
                return false;
            }
        }

        protected override bool UseMobileViewsImpl
        {
            get
            {
                return true;
            }
        }
    }
}