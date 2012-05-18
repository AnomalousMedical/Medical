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

namespace Medical.GUI
{
    public class GenericEditor : MDIDialog
    {
        private Tree tree;
        private EditInterfaceTreeView editTreeView;

        private ResizingTable table;
        private PropertiesTable propTable;

        private ObjectEditor objectEditor;

        private String name;

        private EditorController editorController;

        public GenericEditor(String persistName, String name, MedicalUICallback uiCallback, EditorController editorController, bool horizontalAlignment = true)
            : base(horizontalAlignment ? "Medical.GUI.GenericEditor.GenericEditor.layout" : "Medical.GUI.GenericEditor.GenericEditorVertical.layout", persistName)
        {
            this.name = name;
            this.editorController = editorController;
            window.Caption = String.Format("{0} Editor", name);

            tree = new Tree((ScrollView)window.findWidget("TreeScroller"));
            editTreeView = new EditInterfaceTreeView(tree, uiCallback);

            table = new ResizingTable((ScrollView)window.findWidget("TableScroller"));
            propTable = new PropertiesTable(table, uiCallback);

            objectEditor = new ObjectEditor(editTreeView, propTable, uiCallback);

            this.Resized += new EventHandler(GenericEditor_Resized);
        }

        public override void Dispose()
        {
            objectEditor.Dispose();
            propTable.Dispose();
            table.Dispose();
            editTreeView.Dispose();
            tree.Dispose();
            base.Dispose();
        }

        public void changeCaption(String file)
        {
            if (file != null)
            {
                window.Caption = String.Format("{0} Editor - {1}", name, file);
            }
            else
            {
                window.Caption = String.Format("{0} Editor", name);
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

        void GenericEditor_Resized(object sender, EventArgs e)
        {
            tree.layout();
            table.layout();
        }
    }
}
