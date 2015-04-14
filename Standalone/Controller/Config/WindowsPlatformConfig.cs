﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Logging;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Anomalous.OSPlatform;
using System.IO;

namespace Medical
{
    class WindowsPlatformConfig : PlatformConfig
    {
        public WindowsPlatformConfig()
        {
            Log.ImportantInfo("Platform is Windows");
        }

        protected override String formatTitleImpl(String windowText, String subText)
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
                return false;
            }
        }

        protected override String ThemeFileImpl
        {
            get
            {
                return MyGUIPlugin.MyGUIInterface.DefaultWindowsTheme;
            }
        }

        protected override bool AllowFullscreenImpl
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
                return MouseButtonCode.MB_BUTTON1;
            }
        }

        protected override bool AllowCloneWindowsImpl
        {
            get
            {
                return true;
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

        protected override String OverrideFileLocationImpl
        {
            get
            {
                return "override.ini";
            }
        }

        protected override ProcessStartInfo RestartProcInfoImpl
        {
            get
            {
                String[] args = Environment.GetCommandLineArgs();
                return new ProcessStartInfo(args[0]);
            }
        }

        protected override ProcessStartInfo RestartAdminProcInfoImpl
        {
            get
            {
                var startInfo = RestartProcInfoImpl;
                startInfo.Verb = "runas";
                startInfo.UseShellExecute = true;

                return startInfo;
            }
        }

        protected override bool DefaultEnableMultitouchImpl
        {
            get
            {
                return true;
            }
        }

        protected override bool HasCustomSSLValidationImpl
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
                return 60;
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

        protected override bool TrustSSLCertificateImpl(X509Certificate certificate, string hostName)
        {
            throw new NotImplementedException();
		}

        protected override void moveConfigurationIfNeededImpl()
        {
            
        }
    }
}
