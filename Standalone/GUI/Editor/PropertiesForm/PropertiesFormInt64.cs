using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine;

namespace Medical.GUI
{
    class PropertiesFormInt64 : ConstrainableFormComponent
    {
        private Int64NumericEdit num;
        private bool allowValueChanges = true;

        public PropertiesFormInt64(EditableProperty property, Widget parent)
            : base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormTextBox.layout")
        {
            widget.ForwardMouseWheelToParent = true;

            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);
            textBox.ForwardMouseWheelToParent = true;

            num = new Int64NumericEdit((EditBox)widget.findWidget("EditBox"));
            num.Value = (Int64)property.getRealValue(1);
            num.ValueChanged += new MyGUIEvent(editBox_ValueChanged);
        }

        public override void refreshData()
        {
            num.Value = (Int64)Property.getRealValue(1);
        }

        public override void setConstraints(ReflectedMinMaxEditableProperty minMaxProp)
        {
            num.MinValue = minMaxProp.MinValue.AsInt64;
            num.MaxValue = minMaxProp.MaxValue.AsInt64;
            num.Increment = minMaxProp.Increment.AsInt64;
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