using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using wx;
using PCPlatform;
using System.Drawing;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class MainWindow : Frame
    {
        private WxUpdateTimer updateTimer;
        private Panel renderPanel;

        public MainWindow(bool fullscreen)
            :base("Piper's Joint Based Occlusion", wxDefaultPosition, new Size(800, 600))
        {
            this.BackgroundColour = Colour.wxBLACK;

            if (fullscreen)
            {
                RenderWindow = new WxOSWindow(this);
                InputWindow = new WxOSWindow(this);
            }
            else
            {
                renderPanel = new Panel(this, -1);
                RenderWindow = new WxOSWindow(renderPanel);
                InputWindow = new WxOSWindow(renderPanel);
            }

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
