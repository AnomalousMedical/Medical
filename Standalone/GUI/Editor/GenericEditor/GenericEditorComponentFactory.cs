using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Editing;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI
{
    class GenericEditorComponentFactory : ViewHostComponentFactory
    {
        public GenericEditorComponentFactory()
        {
            
        }

        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is GenericEditorView)
            {
                GenericEditorView genericEditorView = (GenericEditorView)view;
                GenericEditorComponent component = new GenericEditorComponent(viewHost, genericEditorView, genericEditorView.HorizontalAlignment);
                component.CurrentEditInterface = genericEditorView.EditInterface;
                return component;
            }
            if (view is GenericPropertiesFormView)
            {
                GenericPropertiesFormComponent component = new GenericPropertiesFormComponent(viewHost, (GenericPropertiesFormView)view);
                return component;
            }
            if (view is ExpandingGenericEditorView)
            {
                return new ExpandingGenericEditor(viewHost, (ExpandingGenericEditorView)view);
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            
        }
    }
}
