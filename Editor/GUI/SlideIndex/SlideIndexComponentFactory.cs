using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI
{
    class SlideIndexComponentFactory : ViewHostComponentFactory
    {
        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is SlideIndexView)
            {
                SlideIndexView slideIndexView = (SlideIndexView)view;
                SlideIndex component = new SlideIndex(viewHost, slideIndexView);
                slideIndexView._fireComponentCreated(component);
                return component;
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {

        }
    }
}
