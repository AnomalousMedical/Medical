using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using MyGUIPlugin;

namespace Medical.GUI
{
    class PropertiesFormCheckBox : PropertiesFormComponent
    {
        private MyGUILayoutContainer layoutContainer;
        private EditableProperty editableProperty;
        private CheckButton checkButton;

        public PropertiesFormCheckBox(EditableProperty property, Widget parent)
        {
            this.editableProperty = property;
            checkButton = new CheckButton((Button)parent.createWidgetT("Button", "CheckBox", 0, 0, 100, 25, Align.Stretch, ""));
            checkButton.Button.Caption = property.getValue(0);
            checkButton.Checked = (bool)property.getRealValue(1);
            checkButton.CheckedChanged += new MyGUIEvent(checkButton_CheckedChanged);
            layoutContainer = new MyGUILayoutContainer(checkButton.Button);
        }

        public void Dispose()
        {
            Gui.Instance.destroyWidget(checkButton.Button);
        }

        public LayoutContainer Container
        {
            get
            {
                return layoutContainer;
            }
        }

        public EditableProperty Property
        {
            get
            {
                return editableProperty;
            }
        }

        void checkButton_CheckedChanged(Widget source, EventArgs e)
        {
            Property.setValue(1, checkButton.Checked);
        }
    }
}
