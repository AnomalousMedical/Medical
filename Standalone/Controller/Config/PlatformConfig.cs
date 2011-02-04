using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    abstract class PlatformConfig
    {
        private static PlatformConfig currentConfig = new WindowsPlatformConfig();

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


        //Subclass
        protected abstract String formatTitleImpl(String windowText, String subText);

        protected abstract System.Drawing.Color getSecondColorKeyImpl(System.Drawing.Color firstColor);

        protected abstract Gesture createGuiGestureImpl();

        protected abstract MultiFingerScrollGesture createRotateGestureImpl();

        protected abstract MultiFingerScrollGesture createPanGestureImpl();

        protected abstract TwoFingerZoom createZoomGestureImpl();

        protected abstract String ThemeFileImpl
        {
            get;
        }

        protected abstract bool AllowFullscreenImpl
        {
            get;
        }

        protected abstract MouseButtonCode DefaultCameraMouseButtonImpl
        {
            get;
        }

        protected abstract bool CreateMenuImpl { get; }
    }
}
