using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    class ViewHostFactoryCompound : ViewHostFactory
    {
        private List<ViewHostFactory> subFactories = new List<ViewHostFactory>();

        public ViewHost createViewHost(View view, AnomalousMvcContext context)
        {
            foreach (ViewHostFactory factory in subFactories)
            {
                ViewHost host = factory.createViewHost(view, context);
                if (host != null)
                {
                    return host;
                }
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            foreach (ViewHostFactory factory in subFactories)
            {
                factory.createViewBrowser(browser);
            }
        }

        public void addSubFactory(ViewHostFactory factory)
        {
            subFactories.Add(factory);
        }
    }
}
