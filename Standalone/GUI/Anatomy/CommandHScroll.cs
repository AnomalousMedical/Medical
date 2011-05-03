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

        private HScroll slider;
        private StaticText caption;
        private AnatomyCommand command;

        public CommandHScroll(AnatomyCommand command, Widget parentWidget)
        {
            this.command = command;
            command.NumericValueChanged += command_NumericValueChanged;

            caption = (StaticText)parentWidget.createWidgetT("StaticText", "StaticText", 0, 0, parentWidget.Width - SIDE_PADDING, 15, Align.Default, "");
            caption.Caption = command.UIText;

            slider = (HScroll)parentWidget.createWidgetT("HScroll", "HSlider", 0, 0, parentWidget.Width - SIDE_PADDING, 20, Align.Default, "");
            slider.ScrollChangePosition += new MyGUIEvent(slider_ScrollChangePosition);
            slider.UserObject = command;
            slider.ScrollRange = (int)SCROLL_MAX;
            slider.ScrollPosition = getSliderValueFromCommand(command);
            slider.ScrollIncrement = 1000;
        }

        public override void Dispose()
        {
            command.NumericValueChanged -= command_NumericValueChanged;
            Gui.Instance.destroyWidget(caption);
            Gui.Instance.destroyWidget(slider);
        }

        public override void layout()
        {
            caption.setCoord((int)Location.x, (int)Location.y, (int)WorkingSize.Width, caption.Height);
            slider.setCoord((int)Location.x, (int)Location.y + caption.Height, (int)WorkingSize.Width, slider.Height);
        }

        public override Size2 DesiredSize
        {
            get
            {
                return new Size2(caption.Width, caption.Height + slider.Height);
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
            HScroll scroll = (HScroll)source;
            float normalizedValue = scroll.ScrollPosition / SCROLL_MAX;
            if (scroll.ScrollPosition > SCROLL_MAX_RANGE_MIN)
            {
                normalizedValue = 1.0f;
            }
            AnatomyCommand command = (AnatomyCommand)scroll.UserObject;
            float range = command.NumericValueMax - command.NumericValueMin;
            command.NumericValue = range * normalizedValue + command.NumericValueMin;
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
