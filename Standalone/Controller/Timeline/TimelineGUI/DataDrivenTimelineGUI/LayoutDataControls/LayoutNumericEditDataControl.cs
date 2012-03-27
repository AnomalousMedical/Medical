using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.LayoutDataControls
{
    class LayoutNumericEditDataControl : LayoutWidgetDataControl
    {
        private NumericEdit numberEdit;
        private String valueName;

        public LayoutNumericEditDataControl(EditBox edit, NumericDataField numericField)
        {
            numberEdit = new NumericEdit(edit);
            numberEdit.AllowFloat = numericField.AllowDecimalPlaces;
            numberEdit.MinValue = (float)numericField.MinValue;
            numberEdit.MaxValue = (float)numericField.MaxValue;
            numberEdit.DecimalValue = numericField.StartingValue;

            valueName = numericField.Name;
        }

        public override void captureData(DataDrivenExamSection examSection)
        {
            examSection.setValue<decimal>(valueName, numberEdit.DecimalValue);
        }

        public override void displayData(DataDrivenExamSection examSection)
        {
            numberEdit.DecimalValue = examSection.getValue<decimal>(valueName, numberEdit.DecimalValue);
        }
    }
}
