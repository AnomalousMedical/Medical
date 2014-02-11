using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine;

namespace Medical.GUI
{
    class PropertiesFormIntVector2 : ConstrainableFormComponent
    {
        private Int32NumericEdit x;
        private Int32NumericEdit y;
        private bool allowValueChanges = true;

        public PropertiesFormIntVector2(EditableProperty property, Widget parent)
            : base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormXY.layout")
        {
            widget.ForwardMouseWheelToParent = true;

            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);
            textBox.ForwardMouseWheelToParent = true;

            IntVector2 value = (IntVector2)property.getRealValue(1);

            x = new Int32NumericEdit((EditBox)widget.findWidget("X"));
            x.Value = value.x;
            x.ValueChanged += new MyGUIEvent(editBox_ValueChanged);

            y = new Int32NumericEdit((EditBox)widget.findWidget("Y"));
            y.Value = value.y;
            y.ValueChanged += new MyGUIEvent(editBox_ValueChanged);
        }

        public override void refreshData()
        {
            IntVector2 value = (IntVector2)Property.getRealValue(1);
            x.Value = value.x;
            y.Value = value.y;
        }

        public override void setConstraints(ReflectedMinMaxEditableProperty minMaxProp)
        {
            x.MinValue = minMaxProp.MinValue.AsInt32;
            x.MaxValue = minMaxProp.MaxValue.AsInt32;
            x.Increment = minMaxProp.Increment.AsInt32;

            y.MinValue = minMaxProp.MinValue.AsInt32;
            y.MaxValue = minMaxProp.MaxValue.AsInt32;
            y.Increment = minMaxProp.Increment.AsInt32;
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

                Property.setValue(1, new IntVector2(x.Value, y.Value));
                allowValueChanges = true;
            }
        }
    }
}
