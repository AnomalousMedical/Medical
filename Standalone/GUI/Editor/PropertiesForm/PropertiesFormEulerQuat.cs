using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine;

namespace Medical.GUI
{
    class PropertiesFormEulerQuat : PropertiesFormLayoutComponent
    {
        private NumericEdit x;
        private NumericEdit y;
        private NumericEdit z;
        private bool allowValueChanges = true;

        public PropertiesFormEulerQuat(EditableProperty property, Widget parent)
            : base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormXYZ.layout")
        {
            widget.ForwardMouseWheelToParent = true;

            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);
            textBox.ForwardMouseWheelToParent = true;

            Quaternion quat = (Quaternion)property.getRealValue(1);
            Vector3 value = quat.getEuler();
            value *= 57.2957795f;

            x = new NumericEdit((EditBox)widget.findWidget("X"));
            x.MinValue = float.MinValue;
            x.MaxValue = float.MaxValue;
            x.AllowFloat = true;
            x.FloatValue = value.x;
            x.ValueChanged += new MyGUIEvent(editBox_ValueChanged);

            y = new NumericEdit((EditBox)widget.findWidget("Y"));
            y.MinValue = float.MinValue;
            y.MaxValue = float.MaxValue;
            y.AllowFloat = true;
            y.FloatValue = value.y;
            y.ValueChanged += new MyGUIEvent(editBox_ValueChanged);

            z = new NumericEdit((EditBox)widget.findWidget("Z"));
            z.MinValue = float.MinValue;
            z.MaxValue = float.MaxValue;
            z.AllowFloat = true;
            z.FloatValue = value.z;
            z.ValueChanged += new MyGUIEvent(editBox_ValueChanged);
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

                Vector3 euler = new Vector3(x.FloatValue, y.FloatValue, z.FloatValue);
                euler *= 0.0174532925f;
                Property.setValue(1, new Quaternion(euler.x, euler.y, euler.z));
                allowValueChanges = true;
            }
        }
    }
}
