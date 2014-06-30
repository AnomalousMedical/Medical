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
        private List<AnatomyCommand> commands = new List<AnatomyCommand>();

        public CommandCheckBox(Widget parentWidget)
        {
            checkButton = (Button)parentWidget.createWidgetT("Button", "CheckBox", 0, 0, parentWidget.Width - SIDE_PADDING, ScaleHelper.Scaled(20), Align.Default, "");
            checkButton.MouseButtonClick += new MyGUIEvent(checkButton_MouseButtonClick);
        }

        public override void Dispose()
        {
            clearCommands();
            Gui.Instance.destroyWidget(checkButton);
        }

        public override void addCommand(AnatomyCommand command)
        {
            if (commands.Count == 0)
            {
                checkButton.Caption = command.UIText;
                checkButton.Selected = command.BooleanValue;
            }
            commands.Add(command);
            command.BooleanValueChanged += command_BooleanValueChanged;
        }

        public override void clearCommands()
        {
            foreach (var command in commands)
            {
                command.BooleanValueChanged -= command_BooleanValueChanged;
            }
        }

        public override void layout()
        {
            checkButton.setCoord((int)Location.x, (int)Location.y, (int)WorkingSize.Width, checkButton.Height);
        }

        public override Engine.IntSize2 DesiredSize
        {
            get { return new IntSize2(checkButton.getTextRegion().left + checkButton.getTextSize().Width, checkButton.Height); }
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

        void checkButton_MouseButtonClick(Widget source, EventArgs e)
        {
            bool val = !checkButton.Selected;
            checkButton.Selected = val;
            foreach (var command in commands)
            {
                command.BooleanValue = val;
            }
        }

        void command_BooleanValueChanged(AnatomyCommand command, bool value)
        {
            checkButton.Selected = value;
        }
    }
}
