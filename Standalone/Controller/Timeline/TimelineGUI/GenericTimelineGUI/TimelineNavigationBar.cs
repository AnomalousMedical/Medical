using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical
{
    class TimelineNavigationBar : Component
    {
        private ScrollView iconScrollView;
        private FlowLayoutContainer flowLayout = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Horizontal, 10.0f, new Vector2(4.0f, 10.0f));
        private Dictionary<String, TimelineButtonContainer> panels = new Dictionary<String, TimelineButtonContainer>();
        private TimelineButtonContainer selectedButton;
        private AbstractTimelineGUI currentGUI = null;

        public TimelineNavigationBar()
            :base("Medical.Controller.Timeline.TimelineGUI.GenericTimelineGUI.TimelineNavigationBar.layout")
        {
            Layout = new MyGUILayoutContainer(widget);

            iconScrollView = widget.findWidget("WizardIconPanel/ScrollView") as ScrollView;
            Size2 size = iconScrollView.CanvasSize;
            size.Width = 10;
            iconScrollView.CanvasSize = size;
        }

        public override void Dispose()
        {
            clearPanels();
            base.Dispose();
        }

        public LayoutContainer Layout { get; private set; }

        public void addPanel(String timelineFile, String text, String imageKey)
        {
            TimelineButtonContainer timelineButton;
            if (!panels.TryGetValue(timelineFile, out timelineButton))
            {
                Button button = iconScrollView.createWidgetT("Button", "RibbonButton", 0, 0, 78, 64, Align.Default, "") as Button;
                button.Caption = text;
                button.ForwardMouseWheelToParent = true;
                int captionWidth = (int)button.getTextSize().Width;
                button.setSize(captionWidth + 10, button.Height);
                button.StaticImage.setItemResource(imageKey);
                timelineButton = new TimelineButtonContainer(button);
                timelineButton.Clicked += new EventDelegate<TimelineButtonContainer>(timelineButton_Clicked);
            }
            timelineButton.TimelineFile = timelineFile;
            timelineButton.Visible = true;
            flowLayout.addChild(timelineButton.Layout);
            panels.Add(timelineFile, timelineButton);

            //Adjust scroll area size
            Size2 size = iconScrollView.CanvasSize;
            size.Width = flowLayout.DesiredSize.Width;
            iconScrollView.CanvasSize = size;
        }

        public void clearPanels()
        {
            flowLayout.clearChildren();
            foreach (TimelineButtonContainer wizardButton in panels.Values)
            {
                wizardButton.Dispose();
            }
            panels.Clear();
            selectedButton = null;
        }

        internal void activeGUIChanged(AbstractTimelineGUI gui)
        {
            currentGUI = gui;

            if (selectedButton != null)
            {
                selectedButton.StateCheck = false;
            }
            if (panels.TryGetValue(gui.TimelineFile, out selectedButton))
            {
                selectedButton.StateCheck = true;
            }
            else
            {
                selectedButton = null;
                Logging.Log.Warning("Could not find a button for the timeline {0}", gui.TimelineFile);
            }
        }

        public bool SuppressLayout
        {
            get
            {
                return flowLayout.SuppressLayout;
            }
            set
            {
                flowLayout.SuppressLayout = value;
            }
        }

        public void invalidate()
        {
            flowLayout.invalidate();
        }

        void timelineButton_Clicked(TimelineButtonContainer source)
        {
            if (currentGUI != null)
            {
                currentGUI._alertNavigationBarTimelineChange(source.TimelineFile);
                currentGUI.closeAndPlayTimeline(source.TimelineFile);
            }
            else
            {
                Logging.Log.Warning("Navigation bar could not start next timeline '{0}', it does not have a current gui.", source.TimelineFile);
            }
        }

        public bool Enabled
        {
            get
            {
                return widget.Enabled;
            }
            set
            {
                widget.Enabled = value;
            }
        }

        class TimelineButtonContainer : IDisposable
        {
            private Button button;

            public event EventDelegate<TimelineButtonContainer> Clicked;

            public TimelineButtonContainer(Button button)
            {
                this.button = button;
                button.MouseButtonClick += new MyGUIEvent(button_MouseButtonClick);
                button.UserObject = this;
                Layout = new MyGUILayoutContainer(button);
                TimelineFile = null;
            }

            void button_MouseButtonClick(Widget source, EventArgs e)
            {
                if (Clicked != null)
                {
                    Clicked.Invoke(this);
                }
            }

            public void Dispose()
            {
                Gui.Instance.destroyWidget(button);
            }

            public LayoutContainer Layout { get; set; }

            public String TimelineFile { get; set; }

            public bool Visible
            {
                get
                {
                    return button.Visible;
                }
                set
                {
                    button.Visible = value;
                }
            }

            public bool StateCheck
            {
                get
                {
                    return button.StateCheck;
                }
                set
                {
                    button.StateCheck = value;
                }
            }

            public int RightEdge
            {
                get
                {
                    return button.Right;
                }
            }
        }
    }
}
