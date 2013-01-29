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
using Engine;

namespace Medical.GUI
{
    public class ExpandingGenericEditor : LayoutComponent, EditInterfaceConsumer, EditMenuProvider
    {
        private ScrollingExpandingEditInterfaceViewer expandingView;
        private String name;
        private EditorController editorController;

        public ExpandingGenericEditor(MyGUIViewHost viewHost, ExpandingGenericEditorView view)
            : base("Medical.GUI.Editor.GenericEditor.ExpandingGenericEditor.layout", viewHost)
        {
            this.name = view.Name;
            this.editorController = view.EditorController;

            EditInterfaceHandler editInterfaceHandler = viewHost.Context.getModel<EditInterfaceHandler>(EditInterfaceHandler.DefaultName);
            if (editInterfaceHandler != null)
            {
                editInterfaceHandler.setEditInterfaceConsumer(this);
            }

            expandingView = new ScrollingExpandingEditInterfaceViewer((ScrollView)widget.findWidget("Scroller"), view.EditUICallback);

            widget.RootKeyChangeFocus += new MyGUIEvent(widget_RootKeyChangeFocus);

            CurrentEditInterface = view.EditInterface;
        }

        public override void Dispose()
        {
            EditInterfaceHandler editInterfaceHandler = ViewHost.Context.getModel<EditInterfaceHandler>(EditInterfaceHandler.DefaultName);
            if (editInterfaceHandler != null)
            {
                editInterfaceHandler.setEditInterfaceConsumer(null);
            }
            expandingView.Dispose();
            base.Dispose();
        }

        public EditInterface CurrentEditInterface
        {
            get
            {
                return expandingView.EditInterface;
            }
            set
            {
                expandingView.EditInterface = value;
                topLevelResized();
            }
        }

        public override void topLevelResized()
        {
            base.topLevelResized();
            expandingView.layout();
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
