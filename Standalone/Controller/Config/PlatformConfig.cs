using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    class PlatformConfig
    {
#if WINDOWS
        private const int ROTATE_FINGER_COUNT = 1;
        private const float ROTATE_DECEL_TIME = 0.5f;
        private const float ROTATE_MIN_MOMENTUM = 0.01f;

        private const float ZOOM_DECEL_TIME = 1.0f;
        private const float ZOOM_MIN_MOMENTUM = 0.01f;

        private const int PAN_FINGER_COUNT = 2;
        private const float PAN_DECEL_TIME = 0.5f;
        private const float PAN_MIN_MOMENTUM = 0.01f;
#elif MAC_OSX
        private const int ROTATE_FINGER_COUNT = 2;
        private const float ROTATE_DECEL_TIME = 0.5f;
        private const float ROTATE_MIN_MOMENTUM = 0.01f;

        private const float ZOOM_DECEL_TIME = 0.5f;
        private const float ZOOM_MIN_MOMENTUM = 0.01f;

        private const int PAN_FINGER_COUNT = 3;
        private const float PAN_DECEL_TIME = 0.5f;
        private const float PAN_MIN_MOMENTUM = 0.01f;
#endif


        public static String formatTitle(String windowText, String subText)
        {
#if WINDOWS
            return String.Format("{0} - {1}", windowText, subText);
#elif MAC_OSX
            return subText;
#endif
        }

        public static System.Drawing.Color getSecondColorKey(System.Drawing.Color firstColor)
        {
#if WINDOWS
            return firstColor;
#elif MAC_OSX
            //On the Mac likely due to Cairo working a bit different we need to use a color that has been incremented by one. This makes transparency work.
            return System.Drawing.Color.FromArgb(firstColor.ToArgb() + 0x00010101);
#endif
        }

        public static Gesture createGuiGesture()
        {
#if WINDOWS
            return new GuiGestures();
#elif MAC_OSX
            return new GUIGestureBlocker();
#endif
        }

        public static MultiFingerScrollGesture createRotateGesture()
        {
            return new MultiFingerScrollGesture(ROTATE_FINGER_COUNT, ROTATE_DECEL_TIME, ROTATE_MIN_MOMENTUM);
        }

        public static MultiFingerScrollGesture createPanGesture()
        {
            return new MultiFingerScrollGesture(PAN_FINGER_COUNT, PAN_DECEL_TIME, PAN_MIN_MOMENTUM);
        }

        public static TwoFingerZoom createZoomGesture()
        {
            return new TwoFingerZoom(ZOOM_DECEL_TIME, ZOOM_MIN_MOMENTUM);
        }

        public static String ThemeFile
        {
            get
            {
#if WINDOWS
                return "core_theme.xml";
#elif MAC_OSX
                return "core_theme_osx.xml";
#endif
            }
        }

        public static bool AllowFullscreen
        {
            get
            {
#if WINDOWS
                return true;
#elif MAC_OSX
                return false;
#endif
            }
        }

        public static MouseButtonCode DefaultCameraMouseButton
        {
            get
            {
#if MAC_OSX
                return MouseButtonCode.MB_BUTTON0;
#else
                return MouseButtonCode.MB_BUTTON1;
#endif
            }
        }
    }
}
