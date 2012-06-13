﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine;

namespace Medical.GUI
{
    class PropertiesFormSize2 : PropertiesFormLayoutComponent
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

            Single min = Single.MinValue;
            Single max = Single.MaxValue;
            Single inc = 1;

            if (property is ReflectedMinMaxEditableProperty)
            {
                ReflectedMinMaxEditableProperty minMaxProp = (ReflectedMinMaxEditableProperty)property;
                min = minMaxProp.MinValue;
                max = minMaxProp.MaxValue;
                inc = minMaxProp.Increment;
            }

            width = new SingleNumericEdit((EditBox)widget.findWidget("Width"));
            width.MinValue = min;
            width.MaxValue = max;
            width.Increment = inc;
            width.Value = value.Width;
            width.ValueChanged += new MyGUIEvent(editBox_ValueChanged);

            height = new SingleNumericEdit((EditBox)widget.findWidget("Height"));
            height.MinValue = min;
            height.MaxValue = max;
            height.Increment = inc;
            height.Value = value.Height;
            height.ValueChanged += new MyGUIEvent(editBox_ValueChanged);
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