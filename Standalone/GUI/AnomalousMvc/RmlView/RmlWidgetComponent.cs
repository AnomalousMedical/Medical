using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;
using libRocketPlugin;

namespace Medical.GUI.AnomalousMvc
{
    class RmlWidgetComponent : LayoutComponent
    {
        private RocketWidget rocketWidget;
        private ImageBox rmlImage;
        private int imageHeight;
        private int imageWidth;

        private AnomalousMvcContext context;

        public RmlWidgetComponent(RmlView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
            :base("Medical.GUI.AnomalousMvc.RmlView.RmlWidgetComponent.layout", viewHost)
        {
            this.context = context;

            rmlImage = (ImageBox)widget;
            rocketWidget = new RocketWidget(rmlImage);
            imageHeight = rmlImage.Height;

            RocketEventListenerInstancer.setEventController(new RmlMvcEventController(context, ViewHost));
            using (ElementDocument document = rocketWidget.Context.LoadDocument(context.getFullPath(view.RmlFile)))
            {
                if (document != null)
                {
                    document.Show();
                    rocketWidget.removeFocus();
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
            if (widget.Height != imageHeight || widget.Width != imageWidth)
            {
                rocketWidget.resized();
                imageHeight = widget.Height;
                imageWidth = widget.Width;
            }
            base.topLevelResized();
        }
    }
}
