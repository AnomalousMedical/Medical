using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;
using libRocketPlugin;
using Engine;

namespace Medical
{
    public class RmlTimelineGUI : GenericTimelineGUI<RmlTimelineGUIData>
    {
        private RocketWidget rocketWidget;
        private ImageBox rmlImage;
        private int imageHeight;
        RmlTimelineGUIEventController eventController;

        //Action queue stuff
        private bool queuedCloseGui = false;
        private String queuedTimeline = null;

        public RmlTimelineGUI()
            :base("Medical.Controller.Timeline.TimelineGUI.RmlTimelineGUI.RmlTimelineGUI.layout")
        {
            rmlImage = (ImageBox)widget.findWidget("RmlImage");
            rocketWidget = new RocketWidget("RmlGUI", rmlImage);
            imageHeight = rmlImage.Height;

            Button closeButton = (Button)widget.findWidget("Close");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            layoutContainer.LayoutChanged += new Action(layoutContainer_LayoutChanged);

            eventController = new RmlTimelineGUIEventController(this);
        }

        public override void Dispose()
        {
            rocketWidget.Dispose();
            base.Dispose();
        }

        protected override void onShown()
        {
            RocketEventListenerInstancer.setEventController(eventController);
            using (ElementDocument document = rocketWidget.Context.LoadDocument(getFullPath(GUIData.RmlFile)))
            {
                if (document != null)
                {
                    document.Show();
                }
            }
            RocketEventListenerInstancer.resetEventController();
        }

        public void runAction(string name)
        {
            queuedCloseGui = false;
            queuedTimeline = null;
            GUIData.ActionManager.runAction(name, this);
            if (queuedCloseGui)
            {
                if (queuedTimeline == null)
                {
                    closeAndReturnToMainGUI();
                }
                else
                {
                    closeAndPlayTimeline(queuedTimeline);
                }
            }
            else if(queuedTimeline != null)
            {
                playExampleTimeline(queuedTimeline);
            }
        }

        public void queueClose()
        {
            queuedCloseGui = true;
        }

        public void queueTimeline(String timeline)
        {
            queuedTimeline = timeline;
        }

        void layoutContainer_LayoutChanged()
        {
            if (widget.Height != imageHeight)
            {
                rocketWidget.resized();
                imageHeight = widget.Height;
            }
        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            closeAndReturnToMainGUI();
        }
    }
}
