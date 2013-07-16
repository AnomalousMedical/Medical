using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
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

        protected override System.Drawing.Color getSecondColorKeyImpl(System.Drawing.Color firstColor)
        {
            //On the Mac likely due to Cairo working a bit different we need to use a color that has been incremented by one. This makes transparency work.
            return System.Drawing.Color.FromArgb(firstColor.ToArgb() + 0x00010101);
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

        protected override String DocumentsFolderImpl
        {
            get
            {
                return Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Library/Application Support");
            }
        }

        protected override String AllUserDocumentsFolderImpl
        {
            get
            {
                return Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Library/Application Support/Anomalous Medical/Common");
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

        protected override bool DefaultEnableMultitouchImpl
        {
            get
            {
                return false;
            }
        }

        protected override bool PreferHardwareSkinningImpl 
		{
			get 
			{
				int major, minor, bugfix;
				SystemInfo_GetOSXVersion (out major, out minor, out bugfix);
				return !(major == 10 && (minor == 6 || (minor == 7 && bugfix < 5)));
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

		#region PInvoke

		[DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern void SystemInfo_GetOSXVersion(out int major, out int minor, out int bugfix);

        [DllImport("OSHelper", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
		private static unsafe extern bool CertificateValidator_ValidateSSLCertificate(byte* certBytes, uint certBytesLength, [MarshalAs(UnmanagedType.LPWStr)] String url);

		#endregion
    }
}
