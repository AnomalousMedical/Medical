using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Logging;

namespace Medical
{
    class WindowsPlatformConfig : PlatformConfig
    {
        private const int ROTATE_FINGER_COUNT = 1;
        private const float ROTATE_DECEL_TIME = 0.5f;
        private const float ROTATE_MIN_MOMENTUM = 0.01f;

        private const float ZOOM_DECEL_TIME = 1.0f;
        private const float ZOOM_MIN_MOMENTUM = 0.01f;

        private const int PAN_FINGER_COUNT = 2;
        private const float PAN_DECEL_TIME = 0.5f;
        private const float PAN_MIN_MOMENTUM = 0.01f;

        public WindowsPlatformConfig()
        {
            Log.ImportantInfo("Platform is Windows");
        }

        protected override String formatTitleImpl(String windowText, String subText)
        {
            return String.Format("{0} - {1}", windowText, subText);
        }

        protected override System.Drawing.Color getSecondColorKeyImpl(System.Drawing.Color firstColor)
        {
            return firstColor;
        }

        protected override Gesture createGuiGestureImpl()
        {
            return new GuiGestures();
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
                return "core_theme.xml";
            }
        }

        protected override bool AllowFullscreenImpl
        {
            get
            {
                return true;
            }
        }

        protected override MouseButtonCode DefaultCameraMouseButtonImpl
        {
            get
            {
                return MouseButtonCode.MB_BUTTON1;
            }
        }

        protected override bool CreateMenuImpl
        {
            get
            {
                return false;
            }
        }

        protected override bool AllowCloneWindowsImpl
        {
            get
            {
                return true;
            }
        }

        protected override String DocumentsFolderImpl
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }
    }
}
