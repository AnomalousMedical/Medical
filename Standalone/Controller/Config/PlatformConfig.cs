using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace Medical
{
    public enum OperatingSystem
    {
        Windows,
        Mac,
    }

    public abstract class PlatformConfig
    {
        private static PlatformConfig currentConfig;

        static PlatformConfig()
        {
            OsId = PlatformConfig_getPlatform();
            switch (OsId)
            {
                case OperatingSystem.Windows:
                    currentConfig = new WindowsPlatformConfig();
                    break;
                case OperatingSystem.Mac:
                    currentConfig = new MacPlatformConfig();
                    break;
                default:
                    throw new Exception("Could not find platform configuration.");
            }
        }

        public static String formatTitle(String windowText, String subText)
        {
            return currentConfig.formatTitleImpl(windowText, subText);
        }

        public static Gesture createGuiGesture()
        {
            return currentConfig.createGuiGestureImpl();
        }

        public static MultiFingerScrollGesture createRotateGesture()
        {
            return currentConfig.createRotateGestureImpl();
        }

        public static MultiFingerScrollGesture createPanGesture()
        {
            return currentConfig.createPanGestureImpl();
        }

        public static TwoFingerZoom createZoomGesture()
        {
            return currentConfig.createZoomGestureImpl();
        }

        public static String ThemeFile
        {
            get
            {
                return currentConfig.ThemeFileImpl;
            }
        }

        public static bool AllowFullscreen
        {
            get
            {
                return currentConfig.AllowFullscreenImpl;
            }
        }

        public static MouseButtonCode DefaultCameraMouseButton
        {
            get
            {
                return currentConfig.DefaultCameraMouseButtonImpl;
            }
        }

        public static bool AllowCloneWindows
        {
            get
            {
                return currentConfig.AllowCloneWindowsImpl;
            }
        }

        public static String LocalUserDocumentsFolder
        {
            get
            {
                return currentConfig.LocalUserDocumentsFolderImpl;
            }
        }

        public static String LocalDataFolder
        {
            get
            {
                return currentConfig.LocalDataFolderImpl;
            }
        }

        public static String LocalPrivateDataFolder
        {
            get
            {
                return currentConfig.LocalPrivateDataFolderImpl;
            }
        }

        public static bool CloseMainWindowOnShutdown
        {
            get
            {
                return currentConfig.CloseMainWindowOnShutdownImpl;
            }
        }

        public static KeyboardButtonCode PanKey
        {
            get
            {
                return currentConfig.PanKeyImpl;
            }
        }

        public static String OverrideFileLocation
        {
            get
            {
                return currentConfig.OverrideFileLocationImpl;
            }
        }

        public static ProcessStartInfo RestartProcInfo
        {
            get
            {
                return currentConfig.RestartProcInfoImpl;
            }
        }

        public static ProcessStartInfo RestartAdminProcInfo
        {
            get
            {
                return currentConfig.RestartAdminProcInfoImpl;
            }
        }

        public static OperatingSystem OsId { get; private set; }

        public static bool DefaultEnableMultitouch
        {
            get
            {
                return currentConfig.DefaultEnableMultitouchImpl;
            }
        }

        public static bool HasCustomSSLValidation
        {
            get
            {
                return currentConfig.HasCustomSSLValidationImpl;
            }
        }

        public static bool TrustSSLCertificate(X509Certificate certificate, String hostName)
        {
            return currentConfig.TrustSSLCertificateImpl(certificate, hostName);
        }

        //Subclass
        protected abstract String formatTitleImpl(String windowText, String subText);

        protected abstract Gesture createGuiGestureImpl();

        protected abstract MultiFingerScrollGesture createRotateGestureImpl();

        protected abstract MultiFingerScrollGesture createPanGestureImpl();

        protected abstract TwoFingerZoom createZoomGestureImpl();

        protected abstract String ThemeFileImpl { get; }

        protected abstract bool AllowFullscreenImpl { get; }

        protected abstract MouseButtonCode DefaultCameraMouseButtonImpl { get; }

        protected abstract bool AllowCloneWindowsImpl { get; }

        protected abstract String LocalUserDocumentsFolderImpl { get; }

        protected abstract String LocalDataFolderImpl { get; }

        protected abstract String LocalPrivateDataFolderImpl { get; }

        protected abstract bool CloseMainWindowOnShutdownImpl { get; }

        protected abstract KeyboardButtonCode PanKeyImpl { get; }

        protected abstract String OverrideFileLocationImpl { get; }

        protected abstract ProcessStartInfo RestartProcInfoImpl { get; }

        protected abstract bool DefaultEnableMultitouchImpl { get; }

        protected abstract bool HasCustomSSLValidationImpl { get; }

        protected abstract bool TrustSSLCertificateImpl(X509Certificate certificate, String hostName);

        protected abstract ProcessStartInfo RestartAdminProcInfoImpl { get; }

        #region PInvoke

        [DllImport("OSHelper", CallingConvention=CallingConvention.Cdecl)]
        private static extern OperatingSystem PlatformConfig_getPlatform();

        #endregion
    }
}
