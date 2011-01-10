using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using wx;
using System.Drawing;

namespace Medical.GUI
{
    public class MainWindow : Frame
    {
        private WxUpdateTimer updateTimer;
        private String windowDefaultText;

        public static MainWindow Instance { get; private set; }

        private Window mainDrawControl = null;

        public MainWindow(bool fullscreen)
            :base("Piper's Joint Based Occlusion", wxDefaultPosition, new Size(800, 600))
        {
            Instance = this;
            this.BackgroundColour = Colour.wxBLACK;

#if MAC_OSX
            //OSX needs a panel to change mouse cursors.
            Panel panel = new Panel(this);
            mainDrawControl = panel;
#else
            mainDrawControl = this;
#endif

            RenderWindow = new WxOSWindow(mainDrawControl);
            InputWindow = new WxOSWindow(mainDrawControl);

            Center();
        }

        public void setTimer(WxUpdateTimer updateTimer)
        {
            this.updateTimer = updateTimer;
            this.EVT_IDLE(updateTimer.IdleListener);
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

#if WINDOWS
            Title = String.Format("{0} - {1}", windowDefaultText, subName);
#elif MAC_OSX
            Title = subName;
#endif
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

        public OSWindow RenderWindow { get; private set; }

        public OSWindow InputWindow { get; private set; }

        public Window MainDrawControl
        {
            get
            {
                return mainDrawControl;
            }
        }
    }
}
