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
    class PropertiesFormColorNullable : PropertiesFormLayoutComponent
    {
        private Color? color;
        private Widget colorPreview;
        private bool allowValueChanges = true;

        public PropertiesFormColorNullable(EditableProperty property, Widget parent)
            :base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormColorButton.layout")
        {
            widget.ForwardMouseWheelToParent = true;

            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);
            textBox.ForwardMouseWheelToParent = true;
            if (textBox.ClientWidget != null)
            {
                textBox.ClientWidget.ForwardMouseWheelToParent = true;
            }

            Button colorButton = (Button)widget.findWidget("ColorButton");
            colorButton.MouseButtonClick += colorButton_MouseButtonClick;
            colorButton.ForwardMouseWheelToParent = true;

            colorPreview = widget.findWidget("ColorPreview");
            colorPreview.ForwardMouseWheelToParent = true;

            refreshData();
        }

        public override void refreshData()
        {
            color = (Color?)Property.getRealValue(1);
            syncColorToButton();
        }

        void colorButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ColorMenu.ShowColorMenu(source.AbsoluteLeft, source.AbsoluteTop + source.Height, color =>
            {
                this.color = color;
                syncColorToButton();
                setValue();
            },
            () =>
            {
                this.color = null;
                syncColorToButton();
                setValue();
            });
        }

        private void setValue()
        {
            if (allowValueChanges)
            {
                allowValueChanges = false;
                Property.setValue(1, color);
                allowValueChanges = true;
            }
        }

        private void syncColorToButton()
        {
            if (color.HasValue)
            {
                colorPreview.Visible = true;
                colorPreview.setColour(color.Value);
            }
            else
            {
                colorPreview.Visible = false;
            }
        }
    }
}
