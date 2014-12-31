using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgrePlugin;
using MyGUIPlugin;
using Engine;
using Engine.Platform;

namespace Medical.Controller
{
    public class SplashScreen : IDisposable
    {
        private Layout layout;
        private Widget mainWidget;
        private ProgressBar progressBar;
        private OgreWindow ogreWindow;
        private OSWindow window;
        private TextBox statusText;
        Widget widgetPanel;
        private float smoothShowPosition;
        private bool runningShowTransition; //True to be making the popup visible, false to be hiding.
        private int imageWidth = 1920;
        private int imageHeight = 1200;
        private List<Tuple<Widget, IntCoord>> resizingWidgets;
        private int widgetPanelWidth;
        private int widgetPanelHeight;

        /// <summary>
        /// This event is called after the SplashScreen has been hidden completely.
        /// </summary>
        public event EventHandler Hidden;

        public SplashScreen(OSWindow window, OgreWindow ogreWindow, uint progressRange, String splashScreenLayoutFile, String splashScreenResourceFile)
        {
            this.window = window;
            this.ogreWindow = ogreWindow;
            window.Resized += window_Resized;
            SmoothShow = true;

            Gui gui = Gui.Instance;
            ResourceManager.Instance.load(splashScreenResourceFile);
            layout = LayoutManager.Instance.loadLayout(splashScreenLayoutFile);
            mainWidget = layout.getWidget(0);

            NumberParser.TryParse(mainWidget.getUserString("ImageWidth"), out imageWidth);
            NumberParser.TryParse(mainWidget.getUserString("ImageHeight"), out imageHeight);

            progressBar = mainWidget.findWidget("SplashScreen/ProgressBar") as ProgressBar;
            progressBar.Range = progressRange;

            statusText = mainWidget.findWidget("SplashScreen/Status") as TextBox;
            statusText.TextColor = Color.White;

            widgetPanel = mainWidget.findWidget("WidgetPanel");
            widgetPanelWidth = Math.Max(widgetPanel.Width, 1);
            widgetPanelHeight = Math.Max(widgetPanel.Height, 1);

            resizingWidgets = new List<Tuple<Widget, IntCoord>>((int)widgetPanel.ChildCount);
            resizingWidgets.AddRange(widgetPanel.Children.Where(c => c.isUserString("ResizeKeepAspectRatio")).Select(c => Tuple.Create(c, c.Coord)));

            resized();

            ogreWindow.OgreRenderTarget.update();
        }

        public void Dispose()
        {
            window.Resized -= window_Resized;
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

        public uint Position
        {
            get
            {
                return progressBar.Position;
            }
        }

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

        private void resized()
        {
            int viewWidth = Math.Max(window.WindowWidth, 1);
            int viewHeight = Math.Max(window.WindowHeight, 1);

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

            //Set Sizes
            mainWidget.setCoord(imageX, imageY, widgetWidth, widgetHeight);

            float panelRatio = viewWidth /  (float)widgetPanelWidth;
            float ratio2 = viewHeight /  (float)widgetPanelHeight;
            if (ratio2 < panelRatio)
            {
                panelRatio = ratio2;
            }
            foreach(var resizingInfo in resizingWidgets)
            {
                IntCoord coord = resizingInfo.Item2;
                resizingInfo.Item1.setCoord((int)(coord.left * panelRatio), 
                                            (int)(coord.top * panelRatio), 
                                            (int)(coord.width * panelRatio), 
                                            (int)(coord.height * panelRatio));
            }

            widgetPanel.setCoord(-imageX, imageY, viewWidth, viewHeight);
        }

        void window_Resized(OSWindow window)
        {
            resized();
        }
    }
}
