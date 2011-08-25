using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical
{
    class CheckBoxDataControl : DataControl
    {
        private CheckButton checkButton;
        private String valueName;

        public CheckBoxDataControl(Widget parentWidget, BooleanDataField field)
        {
            checkButton = new CheckButton((Button)parentWidget.createWidgetT("Button", "CheckBox", 0, 0, 1000, 15, Align.Default, ""));
            checkButton.Button.Caption = field.Name;
            IntCoord textRegion = checkButton.Button.getTextRegion();
            Size2 textSize = checkButton.Button.getTextSize();
            checkButton.Button.setSize((int)(textRegion.left + textSize.Width), checkButton.Button.Height);
            checkButton.Checked = field.StartingValue;

            valueName = field.Name;
        }

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(checkButton.Button);
        }

        public override void captureData(DataDrivenExamSection examSection)
        {
            examSection.setValue<bool>(valueName, checkButton.Checked);
        }

        public override void displayData(DataDrivenExamSection examSection)
        {
            checkButton.Checked = examSection.getValue<bool>(valueName, checkButton.Checked);
        }

        public override void bringToFront()
        {
            
        }

        public override void setAlpha(float alpha)
        {
            
        }

        public override void layout()
        {
            checkButton.Button.setPosition((int)Location.x, (int)Location.y);
            checkButton.Button.setSize((int)WorkingSize.Width, checkButton.Button.Height);
        }

        public override Size2 DesiredSize
        {
            get 
            {
                return new Size2(checkButton.Button.Width, checkButton.Button.Height);
            }
        }

        public override bool Visible
        {
            get
            {
                return checkButton.Button.Visible;
            }
            set
            {
                checkButton.Button.Visible = value;
            }
        }
    }
}
