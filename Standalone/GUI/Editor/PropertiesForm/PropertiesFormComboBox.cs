using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using MyGUIPlugin;

namespace Medical.GUI
{
    class PropertiesFormComboBox : PropertiesFormLayoutComponent
    {
        private ComboBox comboBox;
        private bool allowValueChanges = true;

        public PropertiesFormComboBox(EditableProperty property, Widget parent, IEnumerable<String> options)
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
            foreach (String option in options)
            {
                comboBox.addItem(option);
            }
            comboBox.SelectedIndex = comboBox.findItemIndexWith(property.getValue(1));
            //comboBox.ForwardMouseWheelToParent = true;
            comboBox.KeyLostFocus += new MyGUIEvent(editBox_KeyLostFocus);
            comboBox.EventComboChangePosition += new MyGUIEvent(comboBox_EventComboChangePosition);
            comboBox.EventComboAccept += new MyGUIEvent(comboBox_EventComboAccept);
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
                String value = comboBox.SelectedItemName;
                String errorMessage = null;
                if (Property.canParseString(1, value, out errorMessage))
                {
                    Property.setValueStr(1, value);
                }
                else
                {
                    MessageBox.show(errorMessage, "Parse Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                }
                allowValueChanges = true;
            }
        }
    }
}
