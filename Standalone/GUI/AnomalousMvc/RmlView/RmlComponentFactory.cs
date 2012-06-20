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
            if (view is RmlView)
            {
                return new RmlWidgetComponent((RmlView)view, context, viewHost);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            BrowserNode rmlNode = new GenericBrowserNode<ViewCollection.CreateView>("Rml View", name =>
            {
                return new RmlView(name);
            });
            browser.addNode("", null, rmlNode);
            browser.DefaultSelection = rmlNode;
        }
    }
}
