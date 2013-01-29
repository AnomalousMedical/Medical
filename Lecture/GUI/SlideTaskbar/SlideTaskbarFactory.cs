using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;
using Medical;

namespace Lecture.GUI
{
    class SlideTaskbarFactory : ViewHostComponentFactory
    {
        public SlideTaskbarFactory()
        {
            
        }

        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is SlideTaskbarView)
            {
                return new SlideTaskbar((SlideTaskbarView)view, viewHost);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            
        }
    }
}
