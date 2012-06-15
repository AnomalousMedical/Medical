using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine;

namespace Medical.GUI
{
    class PropertiesFormVector3 : ConstrainableFormComponent
    {
        private SingleNumericEdit x;
        private SingleNumericEdit y;
        private SingleNumericEdit z;
        private bool allowValueChanges = true;

        public PropertiesFormVector3(EditableProperty property, Widget parent)
            : base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormXYZ.layout")
        {
            widget.ForwardMouseWheelToParent = true;

            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);
            textBox.ForwardMouseWheelToParent = true;

            Vector3 value = (Vector3)property.getRealValue(1);

            x = new SingleNumericEdit((EditBox)widget.findWidget("X"));
            x.Value = value.x;
            x.ValueChanged += new MyGUIEvent(editBox_ValueChanged);

            y = new SingleNumericEdit((EditBox)widget.findWidget("Y"));
            y.Value = value.y;
            y.ValueChanged += new MyGUIEvent(editBox_ValueChanged);

            z = new SingleNumericEdit((EditBox)widget.findWidget("Z"));
            z.Value = value.z;
            z.ValueChanged += new MyGUIEvent(editBox_ValueChanged);
        }

        public override void refreshData()
        {
            Vector3 value = (Vector3)Property.getRealValue(1);
            x.Value = value.x;
            y.Value = value.y;
            z.Value = value.z;
        }

        public override void setConstraints(ReflectedMinMaxEditableProperty minMaxProp)
        {
            x.MinValue = minMaxProp.MinValue;
            x.MaxValue = minMaxProp.MaxValue;
            x.Increment = minMaxProp.Increment;

            y.MinValue = minMaxProp.MinValue;
            y.MaxValue = minMaxProp.MaxValue;
            y.Increment = minMaxProp.Increment;

            z.MinValue = minMaxProp.MinValue;
            z.MaxValue = minMaxProp.MaxValue;
            z.Increment = minMaxProp.Increment;
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

                Property.setValue(1, new Vector3(x.Value, y.Value, z.Value));
                allowValueChanges = true;
            }
        }
    }
}
