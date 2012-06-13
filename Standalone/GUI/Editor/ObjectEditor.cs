using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class ObjectEditor : IDisposable
    {
        private EditInterfaceTreeView treeView;
        private PropertyEditor propTable;
        private EditInterface parentEditInterface;
        private EditInterface selectedEditInterface;
        private MedicalUICallback uiCallback;

        public ObjectEditor(EditInterfaceTreeView treeView, PropertyEditor propTable, MedicalUICallback uiCallback)
        {
            this.treeView = treeView;
            treeView.EditInterfaceSelectionChanged += treeView_EditInterfaceSelectionChanged;
            this.propTable = propTable;
            this.uiCallback = uiCallback;
        }

        public void Dispose()
        {
            treeView.EditInterfaceSelectionChanged -= treeView_EditInterfaceSelectionChanged;
        }

        public EditInterface EditInterface
        {
            get
            {
                return parentEditInterface;
            }
            set
            {
                parentEditInterface = value;
                treeView.EditInterface = value;
                propTable.EditInterface = value;
            }
        }

        public void clear()
        {
            EditInterface = null;
        }

        void treeView_EditInterfaceSelectionChanged(EditInterfaceViewEvent evt)
        {
            selectedEditInterface = evt.EditInterface;
            propTable.EditInterface = selectedEditInterface;
            uiCallback.SelectedEditInterface = selectedEditInterface;
        }
    }
}
