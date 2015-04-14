using Anomalous.OSPlatform;
using Engine.Platform;
using Logging;
using Mono.Security.X509;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        protected override ProcessStartInfo RestartProcInfoImpl
        {
            get
            {
                //ANROID_FIXLATER: probably not right
                String[] args = Environment.GetCommandLineArgs();
                return new ProcessStartInfo(args[0]);
            }
        }

        protected override ProcessStartInfo RestartAdminProcInfoImpl
        {
            get
            {
                //ANROID_FIXLATER: probably not right
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

        protected override bool TrustSSLCertificateImpl(System.Security.Cryptography.X509Certificates.X509Certificate certificate, string hostName)
        {
            //The built in mono ssl validation appears to work on android, so like windows throw an not implemented exception if this is called.
            throw new NotImplementedException();
        }

        protected override bool AllowDllPluginsToLoadImpl
        {
            get
            {
                return true;
            }
        }

        protected override void moveConfigurationIfNeededImpl()
        {
            
        }
    }
}
