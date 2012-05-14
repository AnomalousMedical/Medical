﻿using System;
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

            if (view.Buttons.Count > 0)
            {
                component = new ButtonDecorator(component, view.Buttons);
            }

            switch (view.ViewLocation)
            {
                case ViewLocations.Left:
                    component = new SidePanelDecorator(component);
                    break;
                case ViewLocations.Right:
                    component = new SidePanelDecorator(component);
                    break;
                case ViewLocations.Top:
                    component = new TopBottomPanelDecorator(component);
                    break;
                case ViewLocations.Bottom:
                    component = new TopBottomPanelDecorator(component);
                    break;
                case ViewLocations.Floating:
                    component = new WindowDecorator(component);
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
