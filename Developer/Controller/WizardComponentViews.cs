using Engine.Editing;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Developer
{
    class WizardComponentViews : ViewHostComponentFactory
    {
        public ViewHostComponent createViewHostComponent(MyGUIView view, Medical.Controller.AnomalousMvc.AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            WizardComponentFactory.makeViewBrowser(browser);
        }
    }
}
