using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine;

namespace Medical.GUI
{
    class PropertiesFormSize2 : ConstrainableFormComponent
    {
        private SingleNumericEdit width;
        private SingleNumericEdit height;
        private bool allowValueChanges = true;

        public PropertiesFormSize2(EditableProperty property, Widget parent)
            : base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormWidthHeight.layout")
        {
            widget.ForwardMouseWheelToParent = true;

            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);
            textBox.ForwardMouseWheelToParent = true;

            Size2 value = (Size2)property.getRealValue(1);

            width = new SingleNumericEdit((EditBox)widget.findWidget("Width"));
            width.Value = value.Width;
            width.ValueChanged += new MyGUIEvent(editBox_ValueChanged);

            height = new SingleNumericEdit((EditBox)widget.findWidget("Height"));
            height.Value = value.Height;
            height.ValueChanged += new MyGUIEvent(editBox_ValueChanged);
        }

        public override void refreshData()
        {
            Size2 value = (Size2)Property.getRealValue(1);
            width.Value = value.Width;
            height.Value = value.Height;
        }

        public override void setConstraints(ReflectedMinMaxEditableProperty minMaxProp)
        {
            width.MinValue = minMaxProp.MinValue.AsSingle;
            width.MaxValue = minMaxProp.MaxValue.AsSingle;
            width.Increment = minMaxProp.Increment.AsSingle;

            height.MinValue = minMaxProp.MinValue.AsSingle;
            height.MaxValue = minMaxProp.MaxValue.AsSingle;
            height.Increment = minMaxProp.Increment.AsSingle;
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

                Property.setValue(1, new Size2(width.Value, height.Value));
                allowValueChanges = true;
            }
        }
    }
}
