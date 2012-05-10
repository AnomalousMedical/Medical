using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    class DecoratorComponentFactory : ViewHostComponentFactory
    {
        private List<ViewHostComponentFactory> concreteComponentFactories = new List<ViewHostComponentFactory>();

        public void addFactory(ViewHostComponentFactory factory)
        {
            concreteComponentFactories.Add(factory);
        }

        public ViewHostComponent createViewHostComponent(View view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            foreach (ViewHostComponentFactory factory in concreteComponentFactories)
            {
                ViewHostComponent component = factory.createViewHostComponent(view, context, viewHost);
                if (component != null)
                {
                    return component;
                }
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            foreach (ViewHostComponentFactory factory in concreteComponentFactories)
            {
                factory.createViewBrowser(browser);
            }
        }
    }
}
