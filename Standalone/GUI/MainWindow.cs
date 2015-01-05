using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using System.Runtime.InteropServices;
using Engine;
using MyGUIPlugin;
using Anomalous.OSPlatform;

namespace Medical.GUI
{
    public class MainWindow : NativeOSWindow
    {
        private String windowDefaultText;
        private String windowTitle;

        public static MainWindow Instance { get; private set; }

        public MainWindow(String windowTitle)
            : base(windowTitle, new IntVector2(-1, -1), new IntSize2(MedicalConfig.EngineConfig.HorizontalRes, MedicalConfig.EngineConfig.VerticalRes))
        {
            Instance = this;
            this.windowTitle = windowTitle;
        }

        /// <summary>
        /// Update the title of the window to reflect a current filename or other info.
        /// </summary>
        /// <param name="subName">A name to place as a secondary name in the title.</param>
        public void updateWindowTitle(String subName)
        {
            if (windowDefaultText == null)
            {
                windowDefaultText = this.Title;
            }

            Title = PlatformConfig.formatTitle(windowDefaultText, subName);
        }

        /// <summary>
        /// Clear the window title back to the default text.
        /// </summary>
        public void clearWindowTitle()
        {
            if (windowDefaultText != null)
            {
                Title = windowDefaultText;
            }
        }
    }
}
