using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;
using libRocketPlugin;

namespace Medical.GUI.AnomalousMvc
{
    class RmlWidgetViewHost : LayoutComponent
    {
        private RocketWidget rocketWidget;
        private ImageBox rmlImage;
        private int imageHeight;

        private AnomalousMvcContext context;

        public RmlWidgetViewHost(RmlView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
            :base("Medical.GUI.AnomalousMvc.RmlView.RmlWidgetViewHost.layout", viewHost)
        {
            this.context = context;

            rmlImage = (ImageBox)widget.findWidget("RmlImage");
            rocketWidget = new RocketWidget(rmlImage);
            imageHeight = rmlImage.Height;

            RocketEventListenerInstancer.setEventController(new RmlMvcEventController(context, ViewHost));
            using (ElementDocument document = rocketWidget.Context.LoadDocument(context.getFullPath(view.RmlFile)))
            {
                if (document != null)
                {
                    document.Show();
                }
            }
            RocketEventListenerInstancer.resetEventController();
        }

        public override void Dispose()
        {
            rocketWidget.Dispose();
            base.Dispose();
        }

        public override void topLevelResized()
        {
            if (widget.Height != imageHeight)
            {
                rocketWidget.resized();
                imageHeight = widget.Height;
            }
            base.topLevelResized();
        }
    }
}
