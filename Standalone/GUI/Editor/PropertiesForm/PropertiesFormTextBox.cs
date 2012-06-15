using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using MyGUIPlugin;

namespace Medical.GUI
{
    class PropertiesFormTextBox : PropertiesFormLayoutComponent
    {
        private EditBox editBox;
        private bool allowValueChanges = true;

        public PropertiesFormTextBox(EditableProperty property, Widget parent)
            :base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormTextBox.layout")
        {
            widget.ForwardMouseWheelToParent = true;

            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);
            textBox.ForwardMouseWheelToParent = true;
            if (textBox.ClientWidget != null)
            {
                textBox.ClientWidget.ForwardMouseWheelToParent = true;
            }

            editBox = (EditBox)widget.findWidget("EditBox");
            editBox.OnlyText = property.getValue(1);
            editBox.ForwardMouseWheelToParent = true;
            editBox.KeyLostFocus += new MyGUIEvent(editBox_KeyLostFocus);
            editBox.EventEditSelectAccept += new MyGUIEvent(editBox_EventEditSelectAccept);
        }

        public override void refreshData()
        {
            editBox.OnlyText = Property.getValue(1);
        }

        void editBox_EventEditSelectAccept(Widget source, EventArgs e)
        {
            setValue();
        }

        void editBox_KeyLostFocus(Widget source, EventArgs e)
        {
            setValue();
        }

        private void setValue()
        {
            if (allowValueChanges)
            {
                allowValueChanges = false;
                String value = editBox.OnlyText;
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
