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
            String value = color.toRGBA().ToString("X8");

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
            String value = color.toRGBA().ToString("X8");
            editBox.OnlyText = value;
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ColorMenu.ShowColorMenu(source.AbsoluteLeft, source.AbsoluteTop + source.Height, color =>
            {
                String value = color.toRGBA().ToString("X8");
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
                int rgba;
                Color color = Color.White;
                bool failed = true;
                if (int.TryParse(value, NumberStyles.HexNumber, null, out rgba))
                {
                    if (value.Length == 6)
                    {
                        color = Color.FromRGB(rgba);
                        failed = false;
                    }
                    else if(value.Length == 8)
                    {
                        color = Color.FromRGBA(rgba);
                        failed = false;
                    }
                    if (!failed)
                    {
                        Property.setValue(1, color);
                    }
                }
                if(failed)
                {
                    MessageBox.show("Invalid color, format is RGBA in hex, 00000000 through FFFFFFFF", "Color Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                }
                allowValueChanges = true;
            }
        }
    }
}
