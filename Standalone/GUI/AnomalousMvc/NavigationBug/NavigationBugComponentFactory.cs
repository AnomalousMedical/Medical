using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    class NavigationBugComponentFactory : ViewHostComponentFactory
    {
        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is NavigationBugView)
            {
                return new NavigationBugComponent((NavigationBugView)view, context, viewHost);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            browser.addNode("", null, new GenericBrowserNode<ViewCollection.CreateView>("Navigation Bug View", name =>
            {
                return new NavigationBugView(name);
            }));
        }
    }
}
