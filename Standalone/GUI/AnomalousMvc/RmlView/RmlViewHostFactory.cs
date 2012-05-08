using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    class RmlViewHostFactory : ViewHostFactory
    {
        public ViewHost createViewHost(View view, AnomalousMvcContext context)
        {
            if (view.GetType().IsAssignableFrom(typeof(RmlView)))
            {
                return new RmlWidgetViewHost((RmlView)view, context);
            }
            throw new Exception(String.Format("No ViewHost defined for {0}", view));
        }

        public void createViewBrowser(Browser browser)
        {
            browser.addNode("", null, new BrowserNode("Rml View", typeof(RmlView)));
        }
    }
}
