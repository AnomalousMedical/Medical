using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Editing;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI
{
    class TextEditorComponentFactory : ViewHostComponentFactory
    {
        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is TextEditorView)
            {
                TextEditorComponent component = new TextEditorComponent(viewHost, (TextEditorView)view);
                return component;
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            
        }
    }
}
