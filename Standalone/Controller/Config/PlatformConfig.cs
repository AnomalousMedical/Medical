using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using System.Runtime.InteropServices;

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
            switch (PlatformConfig_getPlatform())
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

        public static System.Drawing.Color getSecondColorKey(System.Drawing.Color firstColor)
        {
            return currentConfig.getSecondColorKeyImpl(firstColor);
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

        public static bool CreateMenu
        {
            get
            {
                return currentConfig.CreateMenuImpl;
            }
        }

        public static bool AllowCloneWindows
        {
            get
            {
                return currentConfig.AllowCloneWindowsImpl;
            }
        }

        public static String DocumentsFolder
        {
            get
            {
                return currentConfig.DocumentsFolderImpl;
            }
        }

        public static String AllUserDocumentsFolder
        {
            get
            {
                return currentConfig.AllUserDocumentsFolderImpl;
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


        //Subclass
        protected abstract String formatTitleImpl(String windowText, String subText);

        protected abstract System.Drawing.Color getSecondColorKeyImpl(System.Drawing.Color firstColor);

        protected abstract Gesture createGuiGestureImpl();

        protected abstract MultiFingerScrollGesture createRotateGestureImpl();

        protected abstract MultiFingerScrollGesture createPanGestureImpl();

        protected abstract TwoFingerZoom createZoomGestureImpl();

        protected abstract String ThemeFileImpl { get; }

        protected abstract bool AllowFullscreenImpl { get; }

        protected abstract MouseButtonCode DefaultCameraMouseButtonImpl { get; }

        protected abstract bool CreateMenuImpl { get; }

        protected abstract bool AllowCloneWindowsImpl { get; }

        protected abstract String DocumentsFolderImpl { get; }

        protected abstract String AllUserDocumentsFolderImpl { get; }

        protected abstract bool CloseMainWindowOnShutdownImpl { get; }

        protected abstract KeyboardButtonCode PanKeyImpl { get; }

        #region PInvoke

        [DllImport("OSHelper")]
        private static extern OperatingSystem PlatformConfig_getPlatform();

        #endregion
    }
}
