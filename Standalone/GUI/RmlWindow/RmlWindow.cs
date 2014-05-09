using Engine;
using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class RmlWindow : MDIDialog
    {
        ImageBox rmlImage;
        RocketWidget rocketWidget;

        public RmlWindow()
            : base("Medical.GUI.RmlWindow.RmlWindow.layout")
        {
            rmlImage = (ImageBox)window.findWidget("RmlImage");
            rocketWidget = new RocketWidget(rmlImage, false);

            window.WindowChangedCoord += window_WindowChangedCoord;
        }

        public override void Dispose()
        {
            rocketWidget.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// You can use this to just do body using a default head.
        /// </summary>
        public void setBodyMarkup(String markup)
        {
            rocketWidget.Context.UnloadAllDocuments();

            StringBuilder htmlString = new StringBuilder();
            htmlString.Append(@"<rml>  <head>
    <link type=""text/rcss"" href=""/libRocketPlugin.Resources.rkt.rcss""/>
    <link type=""text/rcss"" href=""libRocketPlugin.Resources.Anomalous.rcss""/>
    </head><body><div id=""Content"" class=""ScrollArea"">");
            htmlString.Append(markup);
            htmlString.Append("</div></body></rml>");
            setFullMarkup(htmlString.ToString());
        }

        /// <summary>
        /// Use this to control the full markup for the window.
        /// </summary>
        public void setFullMarkup(String markup)
        {
            rocketWidget.Context.UnloadAllDocuments();

            using (ElementDocument document = rocketWidget.Context.LoadDocumentFromMemory(markup))
            {
                if (document != null)
                {
                    document.Show();
                    rocketWidget.removeFocus();
                    rocketWidget.renderOnNextFrame();
                }
            }
        }

        public void setFile(String file)
        {
            rocketWidget.Context.UnloadAllDocuments();

            using (ElementDocument document = rocketWidget.Context.LoadDocument(file))
            {
                if (document != null)
                {
                    document.Show();
                    rocketWidget.removeFocus();
                    rocketWidget.renderOnNextFrame();
                }
            }
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            rocketWidget.resized();
        }
    }
}
