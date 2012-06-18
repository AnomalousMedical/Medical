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
        MedicalUICallback uiCallback;
        EditorController editorController;

        public GenericEditorComponentFactory(MedicalUICallback uiCallback, EditorController editorController)
        {
            this.uiCallback = uiCallback;
            this.editorController = editorController;
        }

        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is GenericEditorView)
            {
                GenericEditorView genericEditorView = (GenericEditorView)view;
                GenericEditorComponent component = new GenericEditorComponent(viewHost, view.Name, uiCallback, editorController, genericEditorView.HorizontalAlignment);
                component.CurrentEditInterface = genericEditorView.EditInterface;
                return component;
            }
            if (view is GenericPropertiesFormView)
            {
                GenericPropertiesFormComponent component = new GenericPropertiesFormComponent(viewHost, (GenericPropertiesFormView)view, uiCallback, editorController);
                return component;
            }
            return null;
        }

        public void createViewBrowser(Browser browser)
        {
            
        }
    }
}
