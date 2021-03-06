﻿using Anomalous.OSPlatform;
using Engine.Platform;
using Logging;
using System;

namespace Medical
{
    class AndroidPlatformConfig : PlatformConfig
    {
        public AndroidPlatformConfig()
        {
            Log.ImportantInfo("Platform is Android");
        }

        protected override string formatTitleImpl(string windowText, string subText)
        {
            return String.Format("{0} - {1}", windowText, subText);
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

        protected override string ThemeFileImpl
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
                return true;
            }
        }

        protected override KeyboardButtonCode PanKeyImpl
        {
            get
            {
                return KeyboardButtonCode.KC_LCONTROL;
            }
        }

        protected override string OverrideFileLocationImpl
        {
            get
            {
                return "override.ini";
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
