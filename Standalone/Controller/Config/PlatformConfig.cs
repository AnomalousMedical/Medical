using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Anomalous.OSPlatform;

namespace Medical
{
    public abstract class PlatformConfig
    {
        private static PlatformConfig currentConfig;

        static PlatformConfig()
        {
            OsId = SystemInfo.RuntimeOS;
            switch (OsId)
            {
                case RuntimeOperatingSystem.Windows:
                    currentConfig = new WindowsPlatformConfig();
                    break;
                case RuntimeOperatingSystem.Mac:
                    currentConfig = new MacPlatformConfig();
                    break;
                case RuntimeOperatingSystem.iOS:
                    currentConfig = new iOSPlatformConfig();
                    break;
                default:
                    throw new Exception("Could not find platform configuration.");
            }
        }

        public static String formatTitle(String windowText, String subText)
        {
            return currentConfig.formatTitleImpl(windowText, subText);
        }

        public static TouchType TouchType
        {
            get
            {
                return currentConfig.TouchTypeImpl;
            }
        }

        public static bool ForwardTouchAsMouse
        {
            get
            {
                return currentConfig.ForwardTouchAsMouseImpl;
            }
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

        /// <summary>
        /// The documents folder to put user settings and documents into.
        /// </summary>
        public static String LocalUserDocumentsFolder
        {
            get
            {
                return currentConfig.LocalUserDocumentsFolderImpl;
            }
        }

        /// <summary>
        /// A local folder to put data files shared by all users of the program into.
        /// </summary>
        public static String LocalDataFolder
        {
            get
            {
                return currentConfig.LocalDataFolderImpl;
            }
        }

        /// <summary>
        /// A folder that will not get shared that specifys where private data (like the license file) goes.
        /// Can overlap with LocalDataFolder.
        /// </summary>
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

        public static RuntimeOperatingSystem OsId { get; private set; }

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

        public static String ExecutablePath
        {
            get
            {
                return currentConfig.ExecutablePathImpl;
            }
        }

        public static int DefaultFPSCap
        {
            get
            {
                return currentConfig.DefaultFPSCapImpl;
            }
        }

        /// <summary>
        /// This will be true if we can use all features of anomalous medical, false if not. This pretty
        /// much means disable features we can't use on ios (like in app ads, exit and other features).
        /// Sometime in the future we will probably have to break this apart, but for now one property is ok.
        /// </summary>
        public static bool UnrestrictedEnvironment
        {
            get
            {
#if ALLOW_OVERRIDE
                //Override the unrestricted environement setting if overwritten in the medical config, since thie is anded
                //if the platform is already restricted it will stay restricted.
                return currentConfig.UnrestrictedEnvironmentImpl && MedicalConfig.UnrestrictedEnvironmentOverride;
#else
                return currentConfig.UnrestrictedEnvironmentImpl;
#endif
            }
        }

        public static bool TrustSSLCertificate(X509Certificate certificate, String hostName)
        {
            return currentConfig.TrustSSLCertificateImpl(certificate, hostName);
        }

        /// <summary>
        /// This function moves the configuration files for a specific os if they need to move.
        /// We can remove this at some point in the future when we no longer need to check if files need
        /// to be moved.
        /// </summary>
        public static void MoveConfigurationIfNeeded()
        {
            currentConfig.moveConfigurationIfNeededImpl();
        }

        //Subclass
        protected abstract String formatTitleImpl(String windowText, String subText);

        protected abstract TouchType TouchTypeImpl { get; }

        protected abstract bool ForwardTouchAsMouseImpl { get; }

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

        protected abstract string ExecutablePathImpl { get; }

        protected abstract int DefaultFPSCapImpl { get; }

        protected abstract bool UnrestrictedEnvironmentImpl { get; }

        protected abstract bool TrustSSLCertificateImpl(X509Certificate certificate, String hostName);

        protected abstract ProcessStartInfo RestartAdminProcInfoImpl { get; }

        protected abstract void moveConfigurationIfNeededImpl();
    }
}
