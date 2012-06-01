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

        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            ViewHostComponent component = null;
            foreach (ViewHostComponentFactory factory in concreteComponentFactories)
            {
                component = factory.createViewHostComponent(view, context, viewHost);
                if (component != null)
                {
                    break;
                }
            }

            if(component == null)
            {
                return component;
            }

            switch (view.ViewLocation)
            {
                case ViewLocations.Left:
                    component = new SidePanelDecorator(component, view.Buttons);
                    break;
                case ViewLocations.Right:
                    component = new SidePanelDecorator(component, view.Buttons);
                    break;
                case ViewLocations.Top:
                    component = new TopBottomPanelDecorator(component, view.Buttons);
                    break;
                case ViewLocations.Bottom:
                    component = new TopBottomPanelDecorator(component, view.Buttons);
                    break;
                case ViewLocations.Window:
                    component = new WindowDecorator(component, view.Buttons);
                    break;
            }

            return component;
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
