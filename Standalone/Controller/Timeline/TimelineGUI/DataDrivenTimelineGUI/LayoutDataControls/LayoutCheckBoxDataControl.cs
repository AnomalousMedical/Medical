using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;

namespace Medical.LayoutDataControls
{
    class LayoutCheckBoxDataControl : LayoutWidgetDataControl
    {
        private CheckButton checkButton;
        private String valueName;

        public LayoutCheckBoxDataControl(Button button, BooleanDataField field)
        {
            this.checkButton = new CheckButton(button);
            valueName = field.Name;
        }

        public override void captureData(DataDrivenExamSection examSection)
        {
            examSection.setValue<bool>(valueName, checkButton.Checked);
        }

        public override void displayData(DataDrivenExamSection examSection)
        {
            checkButton.Checked = examSection.getValue<bool>(valueName, checkButton.Checked);
        }
    }
}
