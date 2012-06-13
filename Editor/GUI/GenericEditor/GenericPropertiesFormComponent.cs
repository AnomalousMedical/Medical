using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.GUI.AnomalousMvc;
using Medical.Controller.AnomalousMvc;
using Engine.Editing;

namespace Medical.GUI
{
    class GenericPropertiesFormComponent : LayoutComponent, EditInterfaceConsumer
    {
        private Tree tree;
        private EditInterfaceTreeView editTreeView;

        private PropertiesForm propertiesForm;

        private ObjectEditor objectEditor;

        private String name;

        private EditorController editorController;

        public GenericPropertiesFormComponent(MyGUIViewHost viewHost, String name, MedicalUICallback uiCallback, EditorController editorController, bool horizontalAlignment = true)
            : base(horizontalAlignment ? "Medical.GUI.GenericEditor.GenericEditorComponent.layout" : "Medical.GUI.GenericEditor.GenericEditorVerticalComponent.layout", viewHost)
        {
            Widget window = this.widget;

            this.name = name;
            this.editorController = editorController;
            //window.Caption = String.Format("{0} Editor", name);

            tree = new Tree((ScrollView)window.findWidget("TreeScroller"));
            editTreeView = new EditInterfaceTreeView(tree, uiCallback);

            propertiesForm = new PropertiesForm((ScrollView)window.findWidget("TableScroller"), uiCallback);

            objectEditor = new ObjectEditor(editTreeView, propertiesForm, uiCallback);

            //this.Resized += new EventHandler(GenericEditor_Resized);

            EditInterfaceHandler editInterfaceHandler = viewHost.Context.getModel<EditInterfaceHandler>(EditInterfaceHandler.DefaultName);
            if (editInterfaceHandler != null)
            {
                editInterfaceHandler.setEditInterfaceConsumer(this);
            }
        }

        public override void Dispose()
        {
            EditInterfaceHandler editInterfaceHandler = ViewHost.Context.getModel<EditInterfaceHandler>(EditInterfaceHandler.DefaultName);
            if (editInterfaceHandler != null)
            {
                editInterfaceHandler.setEditInterfaceConsumer(null);
            }
            objectEditor.Dispose();
            propertiesForm.Dispose();
            editTreeView.Dispose();
            tree.Dispose();
            base.Dispose();
        }

        public void changeCaption(String file)
        {
            if (file != null)
            {
                //window.Caption = String.Format("{0} Editor - {1}", name, file);
            }
            else
            {
                //window.Caption = String.Format("{0} Editor", name);
            }
        }

        public EditInterface CurrentEditInterface
        {
            get
            {
                return objectEditor.EditInterface;
            }
            set
            {
                objectEditor.EditInterface = value;
            }
        }

        public override void topLevelResized()
        {
            base.topLevelResized();
            tree.layout();
            propertiesForm.layout();
        }
    }
}
