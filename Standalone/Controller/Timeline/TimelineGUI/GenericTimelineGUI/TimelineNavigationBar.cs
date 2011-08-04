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
            TimelineButtonContainer container;
            if (!panels.TryGetValue(timelineFile, out container))
            {
                Button button = iconScrollView.createWidgetT("Button", "RibbonButton", 0, 0, 78, 64, Align.Default, "") as Button;
                
                button.Caption = text;
                button.MouseButtonClick += iconClicked;
                int captionWidth = (int)button.getTextSize().Width;
                button.setSize(captionWidth + 10, button.Height);
                button.StaticImage.setItemResource(imageKey);
                container = new TimelineButtonContainer(button);
            }
            container.TimelineFile = timelineFile;
            container.Visible = true;
            flowLayout.addChild(container.Layout);
            panels.Add(timelineFile, container);

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

        internal void setCurrentTimeline(string timelineFile)
        {
            if (selectedButton != null)
            {
                selectedButton.StateCheck = false;
            }
            if (panels.TryGetValue(timelineFile, out selectedButton))
            {
                selectedButton.StateCheck = true;
            }
            else
            {
                selectedButton = null;
                Logging.Log.Warning("Could not find a button for the timeline {0}.", timelineFile);
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

        void iconClicked(Widget source, EventArgs e)
        {
            //WizardButtonContainer buttonContainer = source.UserObject as WizardButtonContainer;
            //if (buttonContainer != null)
            //{
            //    if (ModeChanged != null)
            //    {
            //        ModeChanged.Invoke(buttonContainer.ModeIndex);
            //    }
            //}
            //else
            //{
            //    Log.Error("Somehow got a bad WizardButtonContainer. This error should never occur.");
            //}
        }


        class TimelineButtonContainer : IDisposable
        {
            private Button button;

            public TimelineButtonContainer(Button button)
            {
                this.button = button;
                button.UserObject = this;
                Layout = new MyGUILayoutContainer(button);
                TimelineFile = null;
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
