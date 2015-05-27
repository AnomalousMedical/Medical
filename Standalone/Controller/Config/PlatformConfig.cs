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
                case RuntimeOperatingSystem.Android:
                    currentConfig = new AndroidPlatformConfig();
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
#if ALLOW_OVERRIDE
                if (MedicalConfig.HasThemeFileOverride)
                {
                    return MedicalConfig.ThemeFileOverride;
                }
                else
                {
                    return currentConfig.ThemeFileImpl;
                }
#else
                return currentConfig.ThemeFileImpl;
#endif
            }
        }

        /// <summary>
        /// Allow the software to go into a fullscreen exclusive mode on this platform.
        /// </summary>
        public static bool AllowFullscreen
        {
            get
            {
                return currentConfig.AllowFullscreenImpl;
            }
        }

        /// <summary>
        /// Allow the toggle of fullscreen mode.
        /// </summary>
        public static bool AllowFullscreenToggle
        {
            get
            {
                return currentConfig.AllowFullscreenToggleImpl;
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

        public static bool AllowDllPluginsToLoad
        {
            get
            {
                return currentConfig.AllowDllPluginsToLoadImpl;
            }
        }

        public static bool AutoSelectText
        {
            get
            {
                return currentConfig.AutoSelectTextImpl;
            }
        }

        /// <summary>
        /// Determines if custom save/load paths should be used. In sandboxed environments this should
        /// be disabled so we always write files to the correct place.
        /// </summary>
        public static bool AllowCustomSaveLoadPath
        {
            get
            {
                return currentConfig.AllowCustomSaveLoadPathImpl;
            }
        }

        /// <summary>
        /// Determines if the program should prefer mobile views where they are available.
        /// </summary>
        /// <value><c>true</c> if use mobile views; otherwise, <c>false</c>.</value>
        public static bool UseMobileViews
        {
            get
            {
                return currentConfig.UseMobileViewsImpl;
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

        protected abstract bool AllowFullscreenToggleImpl { get; }

        protected abstract MouseButtonCode DefaultCameraMouseButtonImpl { get; }

        protected abstract bool AllowCloneWindowsImpl { get; }

        protected abstract bool CloseMainWindowOnShutdownImpl { get; }

        protected abstract KeyboardButtonCode PanKeyImpl { get; }

        protected abstract String OverrideFileLocationImpl { get; }

        protected abstract ProcessStartInfo RestartProcInfoImpl { get; }

        protected abstract bool DefaultEnableMultitouchImpl { get; }

        protected abstract bool HasCustomSSLValidationImpl { get; }

        protected abstract int DefaultFPSCapImpl { get; }

        protected abstract bool UnrestrictedEnvironmentImpl { get; }

        protected abstract bool TrustSSLCertificateImpl(X509Certificate certificate, String hostName);

        protected abstract ProcessStartInfo RestartAdminProcInfoImpl { get; }

        protected abstract bool AllowDllPluginsToLoadImpl { get; }

        protected abstract void moveConfigurationIfNeededImpl();

        protected abstract bool AutoSelectTextImpl { get; }

        protected abstract bool AllowCustomSaveLoadPathImpl { get; }

        protected abstract bool UseMobileViewsImpl{ get; }
    }
}
