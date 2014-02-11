using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine;

namespace Medical.GUI
{
    class PropertiesFormUInt16 : ConstrainableFormComponent
    {
        private UInt16NumericEdit num;
        private bool allowValueChanges = true;

        public PropertiesFormUInt16(EditableProperty property, Widget parent)
            : base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormTextBox.layout")
        {
            widget.ForwardMouseWheelToParent = true;

            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);
            textBox.ForwardMouseWheelToParent = true;

            num = new UInt16NumericEdit((EditBox)widget.findWidget("EditBox"));
            num.Value = (UInt16)property.getRealValue(1);
            num.ValueChanged += new MyGUIEvent(editBox_ValueChanged);
        }

        public override void refreshData()
        {
            num.Value = (UInt16)Property.getRealValue(1);
        }

        public override void setConstraints(ReflectedMinMaxEditableProperty minMaxProp)
        {
            num.MinValue = minMaxProp.MinValue.AsUInt16;
            num.MaxValue = minMaxProp.MaxValue.AsUInt16;
            num.Increment = minMaxProp.Increment.AsUInt16;
        }

        void editBox_ValueChanged(Widget source, EventArgs e)
        {
            setValue();
        }

        private void setValue()
        {
            if (allowValueChanges)
            {
                allowValueChanges = false;

                Property.setValue(1, num.Value);
                allowValueChanges = true;
            }
        }
    }
}