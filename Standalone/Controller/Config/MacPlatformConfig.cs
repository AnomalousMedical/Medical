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
                return false;
            }
        }

        protected override bool HasCustomSSLValidationImpl
        {
            get
            {
                return true;
            }
        }

        protected override bool TrustSSLCertificateImpl(X509Certificate certificate, string hostName)
        {
            return MacOSXFunctions.TrustSSLCertificate(certificate, hostName);
        }

        private String OldUserDocRoot
        {
            get
            {
                return Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Library/Application Support/Anomalous Medical");
            }
        }

        protected override void moveConfigurationIfNeededImpl()
        {
            try
            {
                String configFile = Path.Combine(OldUserDocRoot, "config.ini");
                if (File.Exists(configFile))
                {
                    //Ensure the folder exists
                    if(!Directory.Exists(FolderFinder.LocalUserDocumentsFolder))
                    {
                        Directory.CreateDirectory(FolderFinder.LocalUserDocumentsFolder);
                    }

                    //Move the files
                    File.Move(configFile, Path.Combine(FolderFinder.LocalUserDocumentsFolder, "config.ini"));
					moveDirectory(Path.Combine(OldUserDocRoot, "Users"), Path.Combine(FolderFinder.LocalUserDocumentsFolder, "Users"));
					moveDirectory(Path.Combine(OldUserDocRoot, "SavedFiles"), Path.Combine(FolderFinder.LocalUserDocumentsFolder, "SavedFiles"));
					moveDirectory(Path.Combine(OldUserDocRoot, "Common", "Anomalous Medical", "Plugins"), Path.Combine(FolderFinder.LocalDataFolder, "Plugins"));
					moveDirectory(Path.Combine(OldUserDocRoot, "Common", "Anomalous Medical", "Downloads"), Path.Combine(FolderFinder.LocalDataFolder, "Downloads"));
					moveDirectory(Path.Combine(OldUserDocRoot, "Common", "Anomalous Medical", "Temp"), Path.Combine(FolderFinder.LocalDataFolder, "Temp"));
                }
            }
            catch (Exception ex)
            {
                Logging.Log.Error("{0} copying legacy files from '{1}'. Message: {2}", ex.GetType().ToString(), OldUserDocRoot, ex.Message);
            }
        }

		private void moveDirectory(String src, String dst)
		{
			try
			{
				Directory.Move(src, dst);
			}
			catch (Exception ex)
			{
				Logging.Log.Error("{0} copying legacy files from '{1}'. Message: {2}", ex.GetType().ToString(), OldUserDocRoot, ex.Message);
			}
		}
    }
}
