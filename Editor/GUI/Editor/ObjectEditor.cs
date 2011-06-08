using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical.GUI
{
    public class ObjectEditor
    {
        private EditInterfaceTreeView treeView;
        private PropertiesTable propTable;
        private EditInterface currentEditInterface;

        public ObjectEditor(EditInterfaceTreeView treeView, PropertiesTable propTable)
        {
            this.treeView = treeView;
            this.propTable = propTable;
        }

        public EditInterface EditInterface
        {
            get
            {
                return currentEditInterface;
            }
            set
            {
                currentEditInterface = value;
                treeView.EditInterface = value;
                propTable.EditInterface = value;
            }
        }

        public void clear()
        {
            EditInterface = null;
        }
    }
}
