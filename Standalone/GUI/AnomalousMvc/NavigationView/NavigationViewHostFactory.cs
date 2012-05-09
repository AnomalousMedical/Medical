using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    class NavigationViewHostFactory : ViewHostFactory
    {
        public ViewHost createViewHost(View view, AnomalousMvcContext context)
        {
            if (typeof(NavigationView).IsAssignableFrom(view.GetType()))
            {
                return new NavigationGui((NavigationView)view, context);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            browser.addNode("", null, new BrowserNode("Navigation View", typeof(NavigationView)));
        }
    }
}
