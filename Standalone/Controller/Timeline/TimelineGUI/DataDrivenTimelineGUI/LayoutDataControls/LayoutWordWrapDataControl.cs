using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.LayoutDataControls
{
    class LayoutWordWrapDataControl : LayoutWidgetDataControl
    {
        private Edit edit;
        private String valueName;

        public LayoutWordWrapDataControl(Edit edit, NotesDataField field)
        {
            this.edit = edit;
            valueName = field.Name;
        }

        public override void captureData(DataDrivenExamSection examSection)
        {
            examSection.setValue<String>(valueName, edit.OnlyText);
        }

        public override void displayData(DataDrivenExamSection examSection)
        {
            edit.OnlyText = examSection.getValue<String>(valueName, edit.OnlyText);
        }
    }
}
