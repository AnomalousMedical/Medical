using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI
{
    class EditorTaskbarFactory : ViewHostComponentFactory
    {
        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is EditorTaskbarView)
            {
                return new EditorTaskbar((EditorTaskbarView)view, viewHost);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            
        }
    }
}
