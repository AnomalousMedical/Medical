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
    class GenericPropertiesFormComponent : LayoutComponent, EditInterfaceConsumer, EditMenuProvider
    {
        private Tree tree;
        private EditInterfaceTreeView editTreeView;

        private PropertiesForm propertiesForm;

        private ObjectEditor objectEditor;

        private String name;

        private EditorController editorController;

        public GenericPropertiesFormComponent(MyGUIViewHost viewHost, GenericPropertiesFormView genericEditorView, MedicalUICallback uiCallback, EditorController editorController)
            : base(genericEditorView.HorizontalAlignment ? "Medical.GUI.GenericEditor.GenericEditorComponent.layout" : "Medical.GUI.GenericEditor.GenericEditorVerticalComponent.layout", viewHost)
        {
            Widget window = this.widget;

            this.name = genericEditorView.Name;
            this.editorController = editorController;

            tree = new Tree((ScrollView)window.findWidget("TreeScroller"));
            editTreeView = new EditInterfaceTreeView(tree, uiCallback);

            propertiesForm = new ScrollablePropertiesForm((ScrollView)window.findWidget("TableScroller"), uiCallback);

            objectEditor = new ObjectEditor(editTreeView, propertiesForm, uiCallback);

            EditInterfaceHandler editInterfaceHandler = viewHost.Context.getModel<EditInterfaceHandler>(EditInterfaceHandler.DefaultName);
            if (editInterfaceHandler != null)
            {
                editInterfaceHandler.setEditInterfaceConsumer(this);
            }

            widget.RootKeyChangeFocus += new MyGUIEvent(widget_RootKeyChangeFocus);

            CurrentEditInterface = genericEditorView.EditInterface;
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

        public void cut()
        {

        }

        public void copy()
        {

        }

        public void paste()
        {

        }

        public void selectAll()
        {

        }

        void widget_RootKeyChangeFocus(Widget source, EventArgs e)
        {
            RootFocusEventArgs rfea = (RootFocusEventArgs)e;
            if (rfea.Focus)
            {
                ViewHost.Context.getModel<EditMenuManager>(EditMenuManager.DefaultName).setMenuProvider(this);
            }
        }
    }
}
