using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    /// <summary>
    /// This class can be a PropertiesForm or PropertiesTable depending on the properties of
    /// the EditInterface currently selected. The PropertiesForm and PropertiesTable should point
    /// to the same scroll view, this class will handle that correctly.
    /// </summary>
    public class ContextualPropertiesEditor : PropertyEditor
    {
        private PropertiesForm form;
        private PropertiesTable table;

        private PropertyEditor currentEditor;

        public ContextualPropertiesEditor(PropertiesForm form, PropertiesTable table)
        {
            this.form = form;
            this.table = table;
            currentEditor = form;
        }

        public PropertyEditor CurrentEditor
        {
            get
            {
                return currentEditor;
            }
        }

        public EditInterface EditInterface
        {
            get
            {
                return currentEditor.EditInterface;
            }
            set
            {
                if(value.canAddRemoveProperties())
                {
                    if(currentEditor != table)
                    {
                        form.EditInterface = null;   
                        currentEditor = table;
                    }
                    table.EditInterface = value;
                }
                else
                {
                    if(currentEditor != form)
                    {
                        table.EditInterface = null;
                        currentEditor = form;
                    }
                    form.EditInterface = value;
                }
            }
        }
    }
}
