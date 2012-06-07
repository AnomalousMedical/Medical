using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI
{
    class EditorInfoBarFactory : ViewHostComponentFactory
    {
        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is EditorInfoBarView)
            {
                return new EditorInfoBarComponent(viewHost, (EditorInfoBarView)view);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            
        }
    }
}
