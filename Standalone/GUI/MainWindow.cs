using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using wx;
using System.Drawing;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class MainWindow : Frame
    {
        private WxUpdateTimer updateTimer;

        public static MainWindow Instance { get; private set; }

        public MainWindow(bool fullscreen)
            :base("Piper's Joint Based Occlusion", wxDefaultPosition, new Size(800, 600))
        {
            Instance = this;
            this.BackgroundColour = Colour.wxBLACK;

            RenderWindow = new WxOSWindow(this);
            InputWindow = new WxOSWindow(this);

            Center();
        }

        public void setTimer(WxUpdateTimer updateTimer)
        {
            this.updateTimer = updateTimer;
            this.EVT_IDLE(updateTimer.IdleListener);
        }

        public OSWindow RenderWindow { get; private set; }

        public OSWindow InputWindow { get; private set; }
    }
}
