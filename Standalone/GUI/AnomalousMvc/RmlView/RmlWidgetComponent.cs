using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;
using libRocketPlugin;

namespace Medical.GUI.AnomalousMvc
{
    public class RmlWidgetComponent : LayoutComponent
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

            if (view.RmlFile != null)
            {
                RocketEventListenerInstancer.setEventController(new RmlMvcEventController(context, ViewHost));
                using (ElementDocument document = rocketWidget.Context.LoadDocument(view.RmlFile))
                {
                    if (document != null)
                    {
                        document.Show();
                        rocketWidget.removeFocus();
                        rocketWidget.renderOnNextFrame();
                    }
                }
                RocketEventListenerInstancer.resetEventController();
            }

            view._fireComponentCreated(this);
        }

        public override void Dispose()
        {
            rocketWidget.Dispose();
            base.Dispose();
        }

        public override void closing()
        {
            rocketWidget.RenderingEnabled = false;
            rocketWidget.InputEnabled = false;
            base.closing();
        }

        public override void populateViewData(IDataProvider dataProvider)
        {
            base.populateViewData(dataProvider);
            ElementDocument document = rocketWidget.Context.GetDocument(0);
            foreach (Element form in document.GetElementsByTagName("form"))
            {
                foreach (Element input in form.GetElementsByTagName("input"))
                {
                    String name = input.GetAttributeString("name");
                    if (dataProvider.hasValue(name))
                    {
                        input.SetAttribute("value", dataProvider.getValue(name));
                    }
                }
            }
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

        public void reloadDocument(String documentName)
        {
            RocketGuiManager.clearAllCaches();

            rocketWidget.Context.UnloadAllDocuments();

            if (documentName != null)
            {
                RocketEventListenerInstancer.setEventController(new RmlMvcEventController(context, ViewHost));
                using (ElementDocument document = rocketWidget.Context.LoadDocument(documentName))
                {
                    if (document != null)
                    {
                        document.Show();
                        rocketWidget.removeFocus();
                        rocketWidget.renderOnNextFrame();
                    }
                }
                RocketEventListenerInstancer.resetEventController();
            }
        }
    }
}
