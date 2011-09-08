using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class DataDrivenExamEditor : MDIDialog
    {
        private MedicalUICallback uiCallback;
        private Tree tree;
        private EditInterfaceTreeView editTreeView;

        private ResizingTable table;
        private PropertiesTable propTable;

        private ObjectEditor objectEditor;

        private DataDrivenExamPluginDefinition currentDefinition;

        public DataDrivenExamEditor(BrowserWindow browserWindow)
            :base("Medical.GUI.DataDrivenExamEditor.DataDrivenExamEditor.layout")
        {
            uiCallback = new MedicalUICallback(browserWindow);

            tree = new Tree((ScrollView)window.findWidget("TreeScroller"));
            editTreeView = new EditInterfaceTreeView(tree, uiCallback);

            table = new ResizingTable((ScrollView)window.findWidget("TableScroller"));
            propTable = new PropertiesTable(table);

            objectEditor = new ObjectEditor(editTreeView, propTable, uiCallback);

            createNewExamDefinition();

            this.Resized += new EventHandler(DataDrivenExamEditor_Resized);
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

        public void createNewExamDefinition()
        {
            currentDefinition = new DataDrivenExamPluginDefinition();
            currentDefinitionChanged();
        }

        void DataDrivenExamEditor_Resized(object sender, EventArgs e)
        {
            tree.layout();
            table.layout();
        }

        private void currentDefinitionChanged()
        {
            editTreeView.EditInterface = currentDefinition.EditInterface;
        }
    }
}
