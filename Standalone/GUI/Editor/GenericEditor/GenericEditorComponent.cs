using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine.Saving.XMLSaver;
using System.Xml;
using System.IO;
using Engine.Saving;
using Medical.GUI.AnomalousMvc;
using Medical.Controller.AnomalousMvc;
using Anomalous.GuiFramework.Editor;

namespace Medical.GUI
{
    public class GenericEditorComponent : LayoutComponent, EditInterfaceConsumer, EditMenuProvider
    {
        private Tree tree;
        private EditInterfaceTreeView editTreeView;

        private AddRemoveButtons addRemoveButtons;
        private ScrollView tableScroller;
        private ResizingTable table;
        private PropertiesTable propTable;

        private ObjectEditor objectEditor;

        private String name;
        private int gap;

        private EditorController editorController;
        private EditUICallback uiCallback;

        private Splitter splitter;

        public GenericEditorComponent(MyGUIViewHost viewHost, GenericEditorView view, bool horizontalAlignment = true)
            : base(horizontalAlignment ? "Medical.GUI.Editor.GenericEditor.GenericEditorComponent.layout" : "Medical.GUI.Editor.GenericEditor.GenericEditorVerticalComponent.layout", viewHost)
        {
            this.name = view.Name;
            this.editorController = view.EditorController;
            this.uiCallback = view.EditUICallback;

            tree = new Tree((ScrollView)widget.findWidget("TreeScroller"));
            editTreeView = new EditInterfaceTreeView(tree, view.EditUICallback);

            tableScroller = (ScrollView)widget.findWidget("TableScroller");
            table = new ResizingTable(tableScroller);

            addRemoveButtons = new AddRemoveButtons((Button)widget.findWidget("Add"), (Button)widget.findWidget("Remove"), widget.findWidget("AddRemovePanel"));
            addRemoveButtons.VisibilityChanged += addRemoveButtons_VisibilityChanged;
            propTable = new PropertiesTable(table, view.EditUICallback, addRemoveButtons);

            objectEditor = new ObjectEditor(editTreeView, propTable, view.EditUICallback);

            gap = tableScroller.Bottom - addRemoveButtons.Top;

            EditInterfaceHandler editInterfaceHandler = viewHost.Context.getModel<EditInterfaceHandler>(EditInterfaceHandler.DefaultName);
            if (editInterfaceHandler != null)
            {
                editInterfaceHandler.setEditInterfaceConsumer(this);
            }

            widget.RootKeyChangeFocus += new MyGUIEvent(widget_RootKeyChangeFocus);

            splitter = new Splitter(widget.findWidget("Splitter"));
            splitter.Widget1Resized += split => tree.layout();
            splitter.Widget2Resized += split => table.layout();
        }

        public override void Dispose()
        {
            EditInterfaceHandler editInterfaceHandler = ViewHost.Context.getModel<EditInterfaceHandler>(EditInterfaceHandler.DefaultName);
            if (editInterfaceHandler != null)
            {
                editInterfaceHandler.setEditInterfaceConsumer(null);
            }
            objectEditor.Dispose();
            propTable.Dispose();
            table.Dispose();
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
            splitter.layout();
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

        void addRemoveButtons_VisibilityChanged(AddRemoveButtons source, bool visible)
        {
            if (visible)
            {
                tableScroller.Height = widget.Height - (addRemoveButtons.Height - gap);
            }
            else
            {
                tableScroller.Height = widget.Height - tableScroller.Top;
            }
        }
    }
}
