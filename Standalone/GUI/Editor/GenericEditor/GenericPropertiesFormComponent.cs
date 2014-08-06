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

        public GenericPropertiesFormComponent(MyGUIViewHost viewHost, GenericPropertiesFormView genericEditorView)
            : base(genericEditorView.HorizontalAlignment ? "Medical.GUI.Editor.GenericEditor.GenericEditorComponent.layout" : "Medical.GUI.Editor.GenericEditor.GenericEditorVerticalComponent.layout", viewHost)
        {
            Widget window = this.widget;

            this.name = genericEditorView.Name;
            this.editorController = genericEditorView.EditorController;

            tree = new Tree((ScrollView)window.findWidget("TreeScroller"));
            editTreeView = new EditInterfaceTreeView(tree, genericEditorView.EditUICallback);

            //This class does not use the add / remove buttons from the layout, so hide them
            Widget addRemovePanel = widget.findWidget("AddRemovePanel");
            addRemovePanel.Visible = false;
            ScrollView tableScroller = (ScrollView)widget.findWidget("TableScroller");
            tableScroller.Height = widget.Height - tableScroller.Top;

            propertiesForm = new ScrollablePropertiesForm(tableScroller, genericEditorView.EditUICallback);

            objectEditor = new ObjectEditor(editTreeView, propertiesForm, genericEditorView.EditUICallback);

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
                EditMenuManager editMenuManager = ViewHost.Context.getModel<EditMenuManager>(EditMenuManager.DefaultName);
                if (editMenuManager != null)
                {
                    editMenuManager.setMenuProvider(this);
                }
            }
        }
    }
}
