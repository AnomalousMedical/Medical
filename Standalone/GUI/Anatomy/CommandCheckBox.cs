using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class CommandCheckBox : CommandUIElement
    {
        private Button checkButton;
        private AnatomyCommand command;

        public CommandCheckBox(AnatomyCommand command, Widget parentWidget)
        {
            this.command = command;
            command.BooleanValueChanged += command_BooleanValueChanged;

            checkButton = (Button)parentWidget.createWidgetT("Button", "CheckBox", 0, 0, parentWidget.Width - SIDE_PADDING, 20, Align.Default, "");
            checkButton.MouseButtonClick += new MyGUIEvent(checkButton_MouseButtonClick);
            checkButton.Caption = command.UIText;
            checkButton.StateCheck = command.BooleanValue;
        }

        void checkButton_MouseButtonClick(Widget source, EventArgs e)
        {
            command.BooleanValue = checkButton.StateCheck = !checkButton.StateCheck;
        }

        void command_BooleanValueChanged(AnatomyCommand command, bool value)
        {
            checkButton.StateCheck = value;
        }

        public override void Dispose()
        {
            command.BooleanValueChanged -= command_BooleanValueChanged;
            Gui.Instance.destroyWidget(checkButton);
        }

        public override void layout()
        {
            checkButton.setCoord((int)Location.x, (int)Location.y, (int)WorkingSize.Width, checkButton.Height);
        }

        public override Engine.Size2 DesiredSize
        {
            get { return new Size2(checkButton.Width, checkButton.Height); }
        }

        public override bool Visible
        {
            get
            {
                return checkButton.Visible;
            }
            set
            {
                checkButton.Visible = value;
            }
        }
    }
}
