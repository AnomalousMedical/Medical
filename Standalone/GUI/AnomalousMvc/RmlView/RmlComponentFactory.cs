using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    class RmlComponentFactory : ViewHostComponentFactory
    {
        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (typeof(RmlView).IsAssignableFrom(view.GetType()))
            {
                return new RmlWidgetComponent((RmlView)view, context, viewHost);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            BrowserNode rmlNode = new BrowserNode("Rml View", typeof(RmlView));
            browser.addNode("", null, rmlNode);
            browser.DefaultSelection = rmlNode;
        }
    }
}
