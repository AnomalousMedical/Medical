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
        MedicalController medicalController;

        public OffsetSequenceEditorFactory(MedicalController medicalController, SaveableClipboard clipboard)
        {
            this.clipboard = clipboard;
            this.medicalController = medicalController;
        }

        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is OffsetSequenceEditorView)
            {
                return new OffsetSequenceEditor(clipboard, viewHost, (OffsetSequenceEditorView)view, medicalController);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            
        }
    }
}
