using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI
{
    class OffsetSequenceEditorFactory : ViewHostComponentFactory
    {
        SaveableClipboard clipboard;

        public OffsetSequenceEditorFactory(SaveableClipboard clipboard)
        {
            this.clipboard = clipboard;
        }

        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is OffsetSequenceEditorView)
            {
                return new OffsetSequenceEditor(clipboard, viewHost, (OffsetSequenceEditorView)view);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            
        }
    }
}
