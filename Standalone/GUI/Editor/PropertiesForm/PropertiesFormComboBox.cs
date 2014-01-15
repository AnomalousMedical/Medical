using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class PropertiesFormComboBox : PropertiesFormLayoutComponent
    {
        private ComboBox comboBox;
        private bool allowValueChanges = true;

        public PropertiesFormComboBox(EditableProperty property, Widget parent, IEnumerable<Pair<String, Object>> options)
            : base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormComboBox.layout")
        {
            widget.ForwardMouseWheelToParent = true;

            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);
            textBox.ForwardMouseWheelToParent = true;
            if (textBox.ClientWidget != null)
            {
                textBox.ClientWidget.ForwardMouseWheelToParent = true;
            }

            comboBox = (ComboBox)widget.findWidget("ComboBox");
            foreach (var option in options)
            {
                comboBox.addItem(option.First, option.Second);
            }
            refreshData();
            //comboBox.ForwardMouseWheelToParent = true;
            comboBox.KeyLostFocus += new MyGUIEvent(editBox_KeyLostFocus);
            comboBox.EventComboChangePosition += new MyGUIEvent(comboBox_EventComboChangePosition);
            comboBox.EventComboAccept += new MyGUIEvent(comboBox_EventComboAccept);
        }

        public override void refreshData()
        {
            uint index = comboBox.findItemIndexWith(Property.getValue(1));
            if (index != ComboBox.Invalid)
            {
                comboBox.SelectedIndex = index;
            }
            else
            {
                comboBox.SelectedIndex = 0;
            }
        }

        void editBox_KeyLostFocus(Widget source, EventArgs e)
        {
            setValue();
        }

        void comboBox_EventComboAccept(Widget source, EventArgs e)
        {
            setValue();
        }

        void comboBox_EventComboChangePosition(Widget source, EventArgs e)
        {
            setValue();
        }

        private void setValue()
        {
            if (allowValueChanges)
            {
                allowValueChanges = false;
                Property.setValue(1, comboBox.SelectedItemData);
                allowValueChanges = true;
            }
        }
    }
}
