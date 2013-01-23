using Engine.Editing;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class DragAndDropFactory : ViewHostComponentFactory
    {
        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is DragAndDropViewBase)
            {
                return new DragAndDropComponent(viewHost, (DragAndDropViewBase)view);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {

        }
    }
}
