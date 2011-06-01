using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Logging;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO;

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
            //ServicePointManager.ServerCertificateValidationCallback = checkValidationResult;
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
                return "core_theme_osx.xml";
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

        protected override bool CreateMenuImpl
        {
            get
            {
                return true;
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

        private bool checkValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        protected override bool CloseMainWindowOnShutdownImpl
        {
            get
            {
                return false;
            }
        }
    }
}
