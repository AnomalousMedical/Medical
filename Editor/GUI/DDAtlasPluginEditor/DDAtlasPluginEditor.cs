using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine.Saving.XMLSaver;
using System.Xml;
using System.IO;

namespace Medical.GUI
{
    public class DDAtlasPluginEditor : MDIDialog
    {
        private Tree tree;
        private EditInterfaceTreeView editTreeView;

        private ResizingTable table;
        private PropertiesTable propTable;

        private ObjectEditor objectEditor;

        private DDAtlasPlugin currentPlugin;

        private AtlasPluginManager pluginManager;
        private EditorController editorController;

        public DDAtlasPluginEditor(MedicalUICallback uiCallback, AtlasPluginManager pluginManager, EditorController editorController)
            : base("Medical.GUI.DDAtlasPluginEditor.DDAtlasPluginEditor.layout")
        {
            this.pluginManager = pluginManager;
            this.editorController = editorController;

            tree = new Tree((ScrollView)window.findWidget("TreeScroller"));
            editTreeView = new EditInterfaceTreeView(tree, uiCallback);

            table = new ResizingTable((ScrollView)window.findWidget("TableScroller"));
            propTable = new PropertiesTable(table);

            objectEditor = new ObjectEditor(editTreeView, propTable, uiCallback);

            this.Resized += new EventHandler(DDAtlasPluginEditor_Resized);
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

        public void updateCaption(String file)
        {
            if (file != null)
            {
                window.Caption = String.Format("Plugin Editor - {0}", file);
            }
            else
            {
                window.Caption = String.Format("Plugin Editor");
            }
        }

        public DDAtlasPlugin CurrentPlugin
        {
            get
            {
                return currentPlugin;
            }
            set
            {
                currentPlugin = value;
                objectEditor.EditInterface = currentPlugin.EditInterface;
            }
        }

        void DDAtlasPluginEditor_Resized(object sender, EventArgs e)
        {
            tree.layout();
            table.layout();
        }
    }
}
