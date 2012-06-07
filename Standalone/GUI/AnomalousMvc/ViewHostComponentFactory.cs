using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    public interface ViewHostComponentFactory
    {
        ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost);

        void createViewBrowser(Browser browser);
    }
}
