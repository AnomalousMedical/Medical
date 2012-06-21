using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;
using libRocketPlugin;
using Medical.GUI.AnomalousMvc;

namespace Medical.GUI
{
    public class RmlWysiwygComponent : LayoutComponent
    {
        private RocketWidget rocketWidget;
        private ImageBox rmlImage;
        private int imageHeight;
        private int imageWidth;

        private AnomalousMvcContext context;

        public RmlWysiwygComponent(RmlWysiwygView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
            : base("Medical.GUI.RmlWysiwyg.RmlWysiwygComponent.layout", viewHost)
        {
            this.context = context;

            rmlImage = (ImageBox)widget;
            rocketWidget = new RocketWidget(rmlImage);
            rocketWidget.MouseButtonClick += new MyGUIEvent(rocketWidget_MouseButtonClick);
            imageHeight = rmlImage.Height;

            if (view.RmlFile != null)
            {
                //RocketEventListenerInstancer.setEventController(new RmlMvcEventController(context, ViewHost));
                using (ElementDocument document = rocketWidget.Context.LoadDocument(context.getFullPath(view.RmlFile)))
                {
                    if (document != null)
                    {
                        document.Show();
                        rocketWidget.removeFocus();
                        rocketWidget.renderOnNextFrame();
                    }
                }
                //RocketEventListenerInstancer.resetEventController();
            }

            view._fireComponentCreated(this);
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

        public void reloadDocument(String documentName)
        {
            RocketOgreTextureManager.refreshTextures();

            Factory.ClearStyleSheetCache();
            rocketWidget.Context.UnloadAllDocuments();

            if (documentName != null)
            {
                //RocketEventListenerInstancer.setEventController(new RmlMvcEventController(context, ViewHost));
                using (ElementDocument document = rocketWidget.Context.LoadDocument(context.getFullPath(documentName)))
                {
                    if (document != null)
                    {
                        document.Show();
                        rocketWidget.removeFocus();
                        rocketWidget.renderOnNextFrame();
                    }
                }
                //RocketEventListenerInstancer.resetEventController();
            }
        }

        void rocketWidget_MouseButtonClick(Widget source, EventArgs e)
        {
            Element element = rocketWidget.Context.GetFocusElement();
            if (element != null)
            {
                //MouseEventArgs me = ((MouseEventArgs)e);
                RmlElementEditor editor = RmlElementEditor.openTextEditor((int)element.AbsoluteLeft + rocketWidget.AbsoluteLeft, (int)element.AbsoluteTop + rocketWidget.AbsoluteTop);
                editor.Text = element.InnerRml;
                editor.Hiding += (src, evt) =>
                {
                    element.InnerRml = editor.Text;
                    rocketWidget.renderOnNextFrame();
                };

                //Logging.Log.Debug(element.TagName);
                //Logging.Log.Debug(element.InnerRml);

                //int index = 0;
                //String name = null;
                //String value = null;
                //while (element.IterateAttributes(ref index, ref name, ref value))
                //{
                //    Logging.Log.Debug("Attr: {0} - {1}", name, value);
                //}
            }

            //Element rootElement = rocketWidget.Context.GetRootElement();
            //Logging.Log.Debug(rootElement.TagName);
            //Logging.Log.Debug(rootElement.InnerRml);
        }
    }
}
