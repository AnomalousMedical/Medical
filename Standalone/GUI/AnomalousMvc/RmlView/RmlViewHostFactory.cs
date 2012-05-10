using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    class RmlViewHostFactory : ViewHostComponentFactory
    {
        public ViewHostComponent createViewHostComponent(View view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (typeof(RmlView).IsAssignableFrom(view.GetType()))
            {
                return new RmlWidgetViewHost((RmlView)view, context, viewHost);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            browser.addNode("", null, new BrowserNode("Rml View", typeof(RmlView)));
        }
    }
}
