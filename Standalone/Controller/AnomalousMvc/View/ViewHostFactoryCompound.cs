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
                try
                {
                    return factory.createViewHost(view, context);
                }
                catch (Exception)
                {

                }
            }
            throw new Exception(String.Format("No ViewHost defined for {0}", view));
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
