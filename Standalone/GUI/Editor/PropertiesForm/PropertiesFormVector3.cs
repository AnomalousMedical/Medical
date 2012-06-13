using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine;

namespace Medical.GUI
{
    class PropertiesFormVector3 : PropertiesFormLayoutComponent
    {
        private NumericEdit x;
        private NumericEdit y;
        private NumericEdit z;
        private bool allowValueChanges = true;

        public PropertiesFormVector3(EditableProperty property, Widget parent)
            : base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormXYZ.layout")
        {
            widget.ForwardMouseWheelToParent = true;

            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);
            textBox.ForwardMouseWheelToParent = true;

            Vector3 value = (Vector3)property.getRealValue(1);

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
                
                Property.setValue(1, new Vector3(x.FloatValue, y.FloatValue, z.FloatValue));
                allowValueChanges = true;
            }
        }
    }
}
