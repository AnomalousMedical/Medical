using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;
using libRocketPlugin;

namespace Medical.GUI.AnomalousMvc
{
    class RmlWidgetViewHost : MyGUIViewHost
    {
        private RocketWidget rocketWidget;
        private ImageBox rmlImage;
        private int imageHeight;

        private AnomalousMvcContext context;

        public RmlWidgetViewHost(RmlView view, AnomalousMvcContext context)
            :base("Medical.GUI.AnomalousMvc.RmlWidgetViewHost.layout")
        {
            this.context = context;

            rmlImage = (ImageBox)widget.findWidget("RmlImage");
            rocketWidget = new RocketWidget("RmlGUI", rmlImage);
            imageHeight = rmlImage.Height;

            Button closeButton = (Button)widget.findWidget("Close");
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            layoutContainer.LayoutChanged += new Action(layoutContainer_LayoutChanged);

            //RocketEventListenerInstancer.setEventController(eventController);
            using (ElementDocument document = rocketWidget.Context.LoadDocument(context.getFullPath(view.RmlFile)))
            {
                if (document != null)
                {
                    document.Show();
                }
            }
            //RocketEventListenerInstancer.resetEventController();

            //eventController = new RmlTimelineGUIEventController(this);
        }

        public override void Dispose()
        {
            rocketWidget.Dispose();
            base.Dispose();
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
            context.queueClose();
        }
    }
}
