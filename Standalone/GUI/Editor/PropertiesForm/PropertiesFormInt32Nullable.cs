using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine;

namespace Medical.GUI
{
    class PropertiesFormInt32Nullable : ConstrainableFormComponent
    {
        private Int32NullableNumericEdit num;
        private bool allowValueChanges = true;

        public PropertiesFormInt32Nullable(EditableProperty property, Widget parent)
            : base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormTextBox.layout")
        {
            widget.ForwardMouseWheelToParent = true;

            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);
            textBox.ForwardMouseWheelToParent = true;

            num = new Int32NullableNumericEdit((EditBox)widget.findWidget("EditBox"));
            num.Value = (Int32?)property.getRealValue(1);
            num.ValueChanged += new MyGUIEvent(editBox_ValueChanged);
        }

        public override void refreshData()
        {
            num.Value = (Int32?)Property.getRealValue(1);
        }

        public override void setConstraints(ReflectedMinMaxEditableProperty minMaxProp)
        {
            num.MinValue = minMaxProp.MinValue.AsInt32;
            num.MaxValue = minMaxProp.MaxValue.AsInt32;
            num.Increment = minMaxProp.Increment.AsInt32;
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