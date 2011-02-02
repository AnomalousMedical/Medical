using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgrePlugin;
using MyGUIPlugin;
using OgreWrapper;
using Engine;

namespace Medical.Controller
{
    class SplashScreen : IDisposable
    {
        Layout layout;
        Widget mainWidget;
        Progress progressBar;
        OgreWindow ogreWindow;
        StaticText statusText;
        private float smoothShowPosition;
        private bool runningShowTransition; //True to be making the popup visible, false to be hiding.

        /// <summary>
        /// This event is called after the SplashScreen has been hidden completely.
        /// </summary>
        public event EventHandler Hidden;

        public SplashScreen(OgreWindow ogreWindow, uint progressRange, String splashScreenLocation)
        {
            this.ogreWindow = ogreWindow;
            SmoothShow = true;

            Gui gui = Gui.Instance;
            OgreResourceGroupManager.getInstance().addResourceLocation(splashScreenLocation, "EngineArchive", "MyGUI", true);
            gui.load("SplashScreen.xml");
            layout = LayoutManager.Instance.loadLayout("SplashScreen.layout");
            mainWidget = layout.getWidget(0);
            mainWidget.setPosition(0, 0);
            mainWidget.setSize(gui.getViewWidth(), gui.getViewHeight());

            progressBar = mainWidget.findWidget("SplashScreen/ProgressBar") as Progress;
            progressBar.Range = progressRange;

            statusText = mainWidget.findWidget("SplashScreen/Status") as StaticText;
            statusText.TextColor = Color.White;

            ogreWindow.OgreRenderWindow.update();
        }

        public void Dispose()
        {
            LayoutManager.Instance.unloadLayout(layout);
        }

        public void updateStatus(uint position, String status)
        {
            progressBar.Position = position;
            statusText.Caption = status;
            ogreWindow.OgreRenderWindow.update();
            WindowFunctions.pumpMessages();
        }

        public void show(int left, int top)
        {
            if (!Visible)
            {
                LayerManager.Instance.upLayerItem(mainWidget);
                int guiWidth = Gui.Instance.getViewWidth();
                int guiHeight = Gui.Instance.getViewHeight();

                int right = left + mainWidget.Width;
                int bottom = top + mainWidget.Height;

                if (right > guiWidth)
                {
                    left -= right - guiWidth;
                    if (left < 0)
                    {
                        left = 0;
                    }
                }

                if (bottom > guiHeight)
                {
                    top -= bottom - guiHeight;
                    if (top < 0)
                    {
                        top = 0;
                    }
                }

                mainWidget.setPosition(left, top);
                Visible = true;
                if (SmoothShow)
                {
                    mainWidget.Alpha = 0.0f;
                    smoothShowPosition = 0.0f;
                    subscribeToUpdate();
                    runningShowTransition = true;
                }
            }
        }

        public void hide()
        {
            if (Visible)
            {
                Visible = false;
                if (SmoothShow)
                {
                    smoothShowPosition = 0.0f;
                    subscribeToUpdate();
                    runningShowTransition = false;
                    mainWidget.Visible = true;
                }
                else
                {
                    if (Hidden != null)
                    {
                        Hidden.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        public bool Visible
        {
            get
            {
                return mainWidget.Visible;
            }
            private set
            {
                if (mainWidget.Visible != value)
                {
                    mainWidget.Visible = value;
                    if(!value)
                    {
                        if (!SmoothShow) //Unsubscribe if not smooth showing.
                        {
                            unsubscribeFromUpdate();
                        }
                    }
                }
            }
        }

        public bool SmoothShow { get; set; }

        private bool subscribedToUpdate = false;

        private void subscribeToUpdate()
        {
            if (!subscribedToUpdate)
            {
                subscribedToUpdate = true;
                Gui.Instance.Update += update;
            }
        }

        private void unsubscribeFromUpdate()
        {
            if (subscribedToUpdate)
            {
                subscribedToUpdate = false;
                Gui.Instance.Update -= update;
            }
        }

        void update(float updateTime)
        {
            smoothShowPosition += updateTime;
            if (runningShowTransition)
            {
                if (smoothShowPosition > MyGUIInterface.SmoothShowDuration)
                {
                    smoothShowPosition = MyGUIInterface.SmoothShowDuration;
                    unsubscribeFromUpdate();
                }
                mainWidget.Alpha = smoothShowPosition / MyGUIInterface.SmoothShowDuration;
            }
            else
            {
                if (smoothShowPosition > MyGUIInterface.SmoothShowDuration)
                {
                    smoothShowPosition = MyGUIInterface.SmoothShowDuration;
                    unsubscribeFromUpdate();
                    mainWidget.Visible = false;
                    mainWidget.Alpha = 0.0f;
                    if (Hidden != null)
                    {
                        Hidden.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    mainWidget.Alpha = 1 - smoothShowPosition / MyGUIInterface.SmoothShowDuration;
                }
            }
        }
    }
}
