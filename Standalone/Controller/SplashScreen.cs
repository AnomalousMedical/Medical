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
    public class SplashScreen : IDisposable
    {
        Layout layout;
        Widget mainWidget;
        ProgressBar progressBar;
        OgreWindow ogreWindow;
        TextBox statusText;
        private float smoothShowPosition;
        private bool runningShowTransition; //True to be making the popup visible, false to be hiding.

        /// <summary>
        /// This event is called after the SplashScreen has been hidden completely.
        /// </summary>
        public event EventHandler Hidden;

        public SplashScreen(OgreWindow ogreWindow, uint progressRange, String splashScreenLayoutFile, String splashScreenResourceFile)
        {
            this.ogreWindow = ogreWindow;
            SmoothShow = true;

            Gui gui = Gui.Instance;
            ResourceManager.Instance.load(splashScreenResourceFile);
            layout = LayoutManager.Instance.loadLayout(splashScreenLayoutFile);
            mainWidget = layout.getWidget(0);

            int imageWidth = 1920;
            NumberParser.TryParse(mainWidget.getUserString("ImageWidth"), out imageWidth);
            int imageHeight = 1200;
            NumberParser.TryParse(mainWidget.getUserString("ImageHeight"), out imageHeight);

            int viewWidth = RenderManager.Instance.ViewWidth;
            int viewHeight = RenderManager.Instance.ViewHeight;

            float heightRatio = (float)viewHeight / (float)imageHeight;
            int widgetWidth = (int)(imageWidth * heightRatio);
            int widgetHeight = viewHeight;

            int imageX = (viewWidth - widgetWidth) / 2;
            int imageY = 0;

            //If the newly scaled image is not wide enough for the screen
            if (widgetWidth < viewWidth)
            {
                float widthRatio = (float)viewWidth / (float)imageWidth;
                widgetWidth = viewWidth;
                widgetHeight = (int)(imageHeight * widthRatio);
                imageX = 0;
            }

            progressBar = mainWidget.findWidget("SplashScreen/ProgressBar") as ProgressBar;
            progressBar.Range = progressRange;

            statusText = mainWidget.findWidget("SplashScreen/Status") as TextBox;
            statusText.TextColor = Color.White;

            //Set Sizes
            mainWidget.setPosition(imageX, imageY);
            mainWidget.setSize(widgetWidth, widgetHeight);
            
            Widget widgetPanel = mainWidget.findWidget("WidgetPanel");
            int widgetPanelWidth = widgetPanel.Width;
            int widgetPanelHeight = widgetPanel.Height;
            float panelRatio = viewWidth / (float)widgetPanelWidth;
            float ratio2 = viewHeight / (float)widgetPanelHeight;
            if (ratio2 < panelRatio)
            {
                panelRatio = ratio2;
            }
            for (uint i = 0; i < widgetPanel.ChildCount; ++i)
            {
                Widget widget = widgetPanel.getChildAt(i);
                if (widget.isUserString("ResizeKeepAspectRatio"))
                {
                    widget.setPosition((int)(widget.Left * panelRatio), (int)(widget.Top * panelRatio));
                    widget.setSize((int)(widget.Width * panelRatio), (int)(widget.Height * panelRatio));
                }
            }

            widgetPanel.setPosition(-imageX, imageY);
            widgetPanel.setSize(viewWidth, viewHeight);

            ogreWindow.OgreRenderTarget.update();
        }

        public void Dispose()
        {
            LayoutManager.Instance.unloadLayout(layout);
        }

        public void updateStatus(uint position, String status)
        {
            progressBar.Position = position;
            statusText.Caption = status;
            ogreWindow.OgreRenderTarget.update();
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
