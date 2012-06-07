using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    class NavigationComponentFactory : ViewHostComponentFactory
    {
        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is NavigationView)
            {
                return new NavigationComponent((NavigationView)view, context, viewHost);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            browser.addNode("", null, new BrowserNode("Navigation View", typeof(NavigationView)));
        }
    }
}
