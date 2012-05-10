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
        public ViewHostComponent createViewHostComponent(View view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (typeof(NavigationView).IsAssignableFrom(view.GetType()))
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
