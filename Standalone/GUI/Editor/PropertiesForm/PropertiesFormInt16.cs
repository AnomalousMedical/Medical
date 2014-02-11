using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine;

namespace Medical.GUI
{
    class PropertiesFormInt16 : ConstrainableFormComponent
    {
        private Int16NumericEdit num;
        private bool allowValueChanges = true;

        public PropertiesFormInt16(EditableProperty property, Widget parent)
            : base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormTextBox.layout")
        {
            widget.ForwardMouseWheelToParent = true;

            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);
            textBox.ForwardMouseWheelToParent = true;

            num = new Int16NumericEdit((EditBox)widget.findWidget("EditBox"));
            num.Value = (Int16)property.getRealValue(1);
            num.ValueChanged += new MyGUIEvent(editBox_ValueChanged);
        }

        public override void refreshData()
        {
            num.Value = (Int16)Property.getRealValue(1);
        }

        public override void setConstraints(ReflectedMinMaxEditableProperty minMaxProp)
        {
            num.MinValue = minMaxProp.MinValue.AsInt16;
            num.MaxValue = minMaxProp.MaxValue.AsInt16;
            num.Increment = minMaxProp.Increment.AsInt16;
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