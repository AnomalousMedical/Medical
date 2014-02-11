using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine;

namespace Medical.GUI
{
    class PropertiesFormSByte : ConstrainableFormComponent
    {
        private SByteNumericEdit num;
        private bool allowValueChanges = true;

        public PropertiesFormSByte(EditableProperty property, Widget parent)
            : base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormTextBox.layout")
        {
            widget.ForwardMouseWheelToParent = true;

            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);
            textBox.ForwardMouseWheelToParent = true;

            num = new SByteNumericEdit((EditBox)widget.findWidget("EditBox"));
            num.Value = (SByte)property.getRealValue(1);
            num.ValueChanged += new MyGUIEvent(editBox_ValueChanged);
        }

        public override void refreshData()
        {
            num.Value = (SByte)Property.getRealValue(1);
        }

        public override void setConstraints(ReflectedMinMaxEditableProperty minMaxProp)
        {
            num.MinValue = minMaxProp.MinValue.AsSByte;
            num.MaxValue = minMaxProp.MaxValue.AsSByte;
            num.Increment = minMaxProp.Increment.AsSByte;
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