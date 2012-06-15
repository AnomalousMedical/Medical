using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Logging;
using Engine;
using System.Globalization;

namespace Medical.GUI
{
    class PropertiesFormColor : PropertiesFormLayoutComponent
    {
        private EditBox editBox;
        private bool allowValueChanges = true;

        public PropertiesFormColor(EditableProperty property, Widget parent)
            :base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormTextBoxBrowser.layout")
        {
            widget.ForwardMouseWheelToParent = true;

            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);
            textBox.ForwardMouseWheelToParent = true;
            if (textBox.ClientWidget != null)
            {
                textBox.ClientWidget.ForwardMouseWheelToParent = true;
            }

            Color color = (Color)property.getRealValue(1);
            String value = String.Format("{0:X2}{1:X2}{2:X2}{3:X2}", (int)(color.r * 0xff), (int)(color.g * 0xff), (int)(color.b * 0xff), (int)(color.a * 0xff));

            editBox = (EditBox)widget.findWidget("EditBox");
            editBox.OnlyText = value;
            editBox.ForwardMouseWheelToParent = true;
            editBox.KeyLostFocus += new MyGUIEvent(editBox_KeyLostFocus);
            editBox.EventEditSelectAccept += new MyGUIEvent(editBox_EventEditSelectAccept);

            Button browseButton = (Button)widget.findWidget("Browse");
            browseButton.ForwardMouseWheelToParent = true;
            browseButton.MouseButtonClick += new MyGUIEvent(browseButton_MouseButtonClick);
        }

        public override void refreshData()
        {
            Color color = (Color)Property.getRealValue(1);
            String value = String.Format("{0:X2}{1:X2}{2:X2}{3:X2}", (int)(color.r * 0xff), (int)(color.g * 0xff), (int)(color.b * 0xff), (int)(color.a * 0xff));
            editBox.OnlyText = value;
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ColorMenu.ShowColorMenu(source.AbsoluteLeft, source.AbsoluteTop + source.Height, color =>
            {
                String value = String.Format("{0:X2}{1:X2}{2:X2}{3:X2}", (int)(color.a * 0xff), (int)(color.r * 0xff), (int)(color.g * 0xff), (int)(color.b * 0xff));
                editBox.OnlyText = value;
                setValue();
            });
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
                int color;
                if (int.TryParse(value, NumberStyles.HexNumber, null, out color))
                {
                    Property.setValue(1, Color.FromARGB(color));
                }
                else
                {
                    MessageBox.show("Invalid color, format is ARGB in hex, 00000000 through FFFFFFFF", "Color Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                }
                allowValueChanges = true;
            }
        }
    }
}
