using System;
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

        protected override String LocalUserDocumentsFolderImpl
        {
            get
            {
                return MacOSXFunctions.LocalUserDocumentsFolder;
            }
        }

        protected override String LocalDataFolderImpl
        {
            get
            {
                return MacOSXFunctions.LocalDataFolder;
            }
        }

        protected override String LocalPrivateDataFolderImpl
        {
            get
            {
                return MacOSXFunctions.LocalPrivateDataFolder;
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

        protected override ProcessStartInfo RestartProcInfoImpl
        {
            get
            {
                String appBundle = Path.GetFullPath("../../");
                if (appBundle.Length > 1)
                {
                    appBundle = appBundle.Substring(0, appBundle.Length - 1);
                }
                ProcessStartInfo startInfo = new ProcessStartInfo("open", String.Format("-a '{0}' -n", appBundle));
                startInfo.UseShellExecute = false;
                return startInfo;
            }
        }

        protected override ProcessStartInfo RestartAdminProcInfoImpl
        {
            get
            {
                return RestartProcInfoImpl;
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
                return true;
            }
        }

        protected override string ExecutablePathImpl
        {
            get
            {
                return Path.GetFullPath(".");
            }
        }

        protected override int DefaultFPSCapImpl
        {
            get
            {
                return 0;
            }
        }

        protected override bool TrustSSLCertificateImpl(X509Certificate certificate, string hostName)
        {
			if(hostName.Equals("anomalousmedicalweb.blob.core.windows.net", StringComparison.InvariantCultureIgnoreCase))
			{
				return true; //IOS_FIXLATER - always trusting blob storage
			}
            return MacOSXFunctions.TrustSSLCertificate(certificate, hostName);
        }

        protected override void moveConfigurationIfNeededImpl()
        {

        }
    }
}