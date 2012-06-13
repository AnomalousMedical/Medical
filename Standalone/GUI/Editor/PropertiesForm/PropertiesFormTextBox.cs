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
        public PropertiesFormTextBox(EditableProperty property, Widget parent)
            :base(property, parent, "Medical.GUI.Editor.PropertiesForm.PropertiesFormTextBox.layout")
        {
            TextBox textBox = (TextBox)widget.findWidget("TextBox");
            textBox.Caption = property.getValue(0);

            EditBox editBox = (EditBox)widget.findWidget("EditBox");
            editBox.Caption = property.getValue(1);
        }
    }
}
