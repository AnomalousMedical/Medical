using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine;

namespace Medical.GUI
{
    class PropertiesFormIntSize2 : ConstrainableFormComponent
    {
        private Int32NumericEdit width;
        private Int32NumericEdit height;
        private bool allowValueChanges = true;

        public PropertiesFormIntSize2(EditableProperty property, Widget parent)
            : base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormWidthHeight.layout")
        {
            widget.ForwardMouseWheelToParent = true;

            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);
            textBox.ForwardMouseWheelToParent = true;

            IntSize2 value = (IntSize2)property.getRealValue(1);

            width = new Int32NumericEdit((EditBox)widget.findWidget("Width"));
            width.Value = value.Width;
            width.ValueChanged += new MyGUIEvent(editBox_ValueChanged);

            height = new Int32NumericEdit((EditBox)widget.findWidget("Height"));
            height.Value = value.Height;
            height.ValueChanged += new MyGUIEvent(editBox_ValueChanged);
        }

        public override void refreshData()
        {
            IntSize2 value = (IntSize2)Property.getRealValue(1);
            width.Value = value.Width;
            height.Value = value.Height;
        }

        public override void setConstraints(ReflectedMinMaxEditableProperty minMaxProp)
        {
            width.MinValue = (Int32)minMaxProp.MinValue;
            width.MaxValue = (Int32)minMaxProp.MaxValue;
            width.Increment = (Int32)minMaxProp.Increment;

            height.MinValue = (Int32)minMaxProp.MinValue;
            height.MaxValue = (Int32)minMaxProp.MaxValue;
            height.Increment = (Int32)minMaxProp.Increment;
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

                Property.setValue(1, new IntSize2(width.Value, height.Value));
                allowValueChanges = true;
            }
        }
    }
}
