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
        private const int ROTATE_FINGER_COUNT = 2;
        private const float ROTATE_DECEL_TIME = 0.5f;
        private const float ROTATE_MIN_MOMENTUM = 0.01f;

        private const float ZOOM_DECEL_TIME = 0.5f;
        private const float ZOOM_MIN_MOMENTUM = 0.01f;

        private const int PAN_FINGER_COUNT = 3;
        private const float PAN_DECEL_TIME = 0.5f;
        private const float PAN_MIN_MOMENTUM = 0.01f;

        public MacPlatformConfig()
        {
			Log.ImportantInfo("Platform is Mac");
        }

        protected override String formatTitleImpl(String windowText, String subText)
        {
            return subText;
        }

        protected override Gesture createGuiGestureImpl()
        {
            return new GUIGestureBlocker();
        }

        protected override MultiFingerScrollGesture createRotateGestureImpl()
        {
            return new MultiFingerScrollGesture(ROTATE_FINGER_COUNT, ROTATE_DECEL_TIME, ROTATE_MIN_MOMENTUM);
        }

        protected override MultiFingerScrollGesture createPanGestureImpl()
        {
            return new MultiFingerScrollGesture(PAN_FINGER_COUNT, PAN_DECEL_TIME, PAN_MIN_MOMENTUM);
        }

        protected override TwoFingerZoom createZoomGestureImpl()
        {
            return new TwoFingerZoom(ZOOM_DECEL_TIME, ZOOM_MIN_MOMENTUM);
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
				StringRetriever sr = new StringRetriever();
				MacPlatformConfig_getLocalUserDocumentsFolder(sr.StringCallback);
				return sr.retrieveString();
            }
        }

        protected override String LocalDataFolderImpl
        {
            get
            {
				StringRetriever sr = new StringRetriever();
				MacPlatformConfig_getLocalDataFolder(sr.StringCallback);
				return sr.retrieveString();
            }
        }

        protected override String LocalPrivateDataFolderImpl
        {
            get
            {
				StringRetriever sr = new StringRetriever();
				MacPlatformConfig_getLocalPrivateDataFolder(sr.StringCallback);
				return sr.retrieveString();
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

        private object sslTrustLock = new object();

        protected override bool TrustSSLCertificateImpl(X509Certificate certificate, string hostName)
        {
            unsafe
            {
                //Apple says that the functions used on the native side to check validity are potentially not thread safe, so lock here when checking.
                lock (sslTrustLock)
                {
                    byte[] certBytes = certificate.Export(X509ContentType.Cert);
                    fixed (byte* certBytesPtr = &certBytes[0])
                    {
                        return CertificateValidator_ValidateSSLCertificate(certBytesPtr, (uint)certBytes.Length, hostName);
                    }
                }
            }
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

		#region PInvoke

        [DllImport("OSHelper", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
		private static unsafe extern bool CertificateValidator_ValidateSSLCertificate(byte* certBytes, uint certBytesLength, [MarshalAs(UnmanagedType.LPWStr)] String url);

		[DllImport("OSHelper", CallingConvention = CallingConvention.Cdecl)]
		private static unsafe extern void MacPlatformConfig_getLocalUserDocumentsFolder (StringRetriever.Callback retrieve);

		[DllImport("OSHelper", CallingConvention = CallingConvention.Cdecl)]
		private static unsafe extern void MacPlatformConfig_getLocalDataFolder(StringRetriever.Callback retrieve);

		[DllImport("OSHelper", CallingConvention = CallingConvention.Cdecl)]
		private static unsafe extern void MacPlatformConfig_getLocalPrivateDataFolder(StringRetriever.Callback retrieve);

		#endregion
    }
}
