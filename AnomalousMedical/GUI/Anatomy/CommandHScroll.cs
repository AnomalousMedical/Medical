using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class CommandHScroll : CommandUIElement
    {
        private const float SCROLL_MAX = 10000;
        private const float SCROLL_MAX_RANGE_MIN = SCROLL_MAX - 50;//The minimum postion of a scroll bar to count as the max value.

        private ScrollBar slider;
        private TextBox caption;
        private List<AnatomyCommand> commands = new List<AnatomyCommand>();

        public CommandHScroll(Widget parentWidget)
        {
            caption = (TextBox)parentWidget.createWidgetT("TextBox", "TextBox", 0, 0, parentWidget.Width - SIDE_PADDING, ScaleHelper.Scaled(15), Align.Default, "");

            slider = (ScrollBar)parentWidget.createWidgetT("HScroll", "HSlider", 0, 0, parentWidget.Width - SIDE_PADDING, ScaleHelper.Scaled(20), Align.Default, "");
            slider.ScrollChangePosition += new MyGUIEvent(slider_ScrollChangePosition);
            slider.ScrollRange = (int)SCROLL_MAX;
            slider.ScrollIncrement = 1000;
        }

        public override void Dispose()
        {
            clearCommands();
            Gui.Instance.destroyWidget(caption);
            Gui.Instance.destroyWidget(slider);
        }

        public override void addCommand(AnatomyCommand command)
        {
            if(commands.Count == 0)
            {
                caption.Caption = command.UIText;
                slider.ScrollPosition = getSliderValueFromCommand(command);
            }
            commands.Add(command);
            command.NumericValueChanged += command_NumericValueChanged;
        }

        public override void clearCommands()
        {
            foreach (var command in commands)
            {
                command.NumericValueChanged -= command_NumericValueChanged;
            }
        }

        public override void layout()
        {
            caption.setCoord((int)Location.x, (int)Location.y, (int)WorkingSize.Width, caption.Height);
            slider.setCoord((int)Location.x, (int)Location.y + caption.Height, (int)WorkingSize.Width, slider.Height);
        }

        public override IntSize2 DesiredSize
        {
            get
            {
                return new IntSize2(caption.Width, caption.Height + slider.Height);
            }
        }

        public override bool Visible
        {
            get
            {
                return slider.Visible;
            }
            set
            {
                slider.Visible = value;
            }
        }

        void slider_ScrollChangePosition(Widget source, EventArgs e)
        {
            ScrollBar scroll = (ScrollBar)source;
            float normalizedValue = scroll.ScrollPosition / SCROLL_MAX;
            if (scroll.ScrollPosition > SCROLL_MAX_RANGE_MIN)
            {
                normalizedValue = 1.0f;
            }
            foreach (var command in commands)
            {
                float range = command.NumericValueMax - command.NumericValueMin;
                command.NumericValue = range * normalizedValue + command.NumericValueMin;
            }
        }

        private static uint getSliderValueFromCommand(AnatomyCommand command)
        {
            float range = command.NumericValueMax - command.NumericValueMin;
            float normalizedCommandValue = (command.NumericValue - command.NumericValueMin) / range;
            return (uint)(normalizedCommandValue * SCROLL_MAX) - 1;
        }

        void command_NumericValueChanged(AnatomyCommand command, float value)
        {
            slider.ScrollPosition = getSliderValueFromCommand(command);
        }
    }
}
