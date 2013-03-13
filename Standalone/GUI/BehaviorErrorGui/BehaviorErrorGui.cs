using Engine;
using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class BehaviorErrorGui : MDIDialog
    {
        ImageBox rmlImage;
        RocketWidget rocketWidget;

        public BehaviorErrorGui(BehaviorErrorManager behaviorErrorManager)
            : base("Medical.GUI.BehaviorErrorGui.BehaviorErrorGui.layout")
        {
            rmlImage = (ImageBox)window.findWidget("RmlImage");
            rocketWidget = new RocketWidget(rmlImage);

            window.WindowChangedCoord += window_WindowChangedCoord;

            StringBuilder htmlString = new StringBuilder();
            htmlString.Append(@"<rml>  <head>
    <link type=""text/rcss"" href=""/libRocketPlugin.Resources.rkt.rcss""/>
    <link type=""text/rcss"" href=""libRocketPlugin.Resources.Anomalous.rcss""/>
    </head><body><div id=""Content"" class=""ScrollArea"">");
            foreach (BehaviorBlacklistEventArgs blacklist in behaviorErrorManager.BlacklistEvents)
            {
                if (blacklist.Behavior != null)
                {
                    htmlString.AppendFormat("<p>Behavior {0}, type='{1}', SimObject='{3}' blacklisted.  Reason: {2}</p>", blacklist.Behavior.Name, blacklist.Behavior.GetType().Name, blacklist.Message, blacklist.Behavior.Owner != null ? blacklist.Behavior.Owner.Name : "NullOwner");
                }
                else
                {
                    htmlString.AppendFormat("<p>Null Behavior blacklisted.  Reason: {0}</p>", blacklist.Message);
                }
            }
            htmlString.Append("</div></body></rml>");
            using (ElementDocument document = rocketWidget.Context.LoadDocumentFromMemory(htmlString.ToString()))
            {
                if (document != null)
                {
                    document.Show();
                    rocketWidget.removeFocus();
                    rocketWidget.renderOnNextFrame();
                }
            }
        }

        public override void Dispose()
        {
            rocketWidget.Dispose();
            base.Dispose();
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            rocketWidget.resized();
        }
    }
}
