using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class AnatomyTransparencySlider : IDisposable
    {
        private const float SCROLL_MAX = 10000;
        private const float SCROLL_MAX_RANGE_MIN = SCROLL_MAX - 50;//The minimum postion of a scroll bar to count as the max value.

        private HScroll slider;
        private AnatomyCommand command;

        public AnatomyTransparencySlider(HScroll slider)
        {
            this.slider = slider;
            slider.ScrollChangePosition += new MyGUIEvent(slider_ScrollChangePosition);
            slider.UserObject = command;
            slider.ScrollRange = (int)SCROLL_MAX;
            slider.ScrollIncrement = 1000;
        }

        public void Dispose()
        {
            if (command != null)
            {
                command.NumericValueChanged -= command_NumericValueChanged;
            }
        }

        public AnatomyCommand Command
        {
            get
            {
                return command;
            }
            set
            {
                if (command != null)
                {
                    command.NumericValueChanged -= command_NumericValueChanged;
                }
                command = value;
                if (command != null)
                {
                    command.NumericValueChanged += command_NumericValueChanged;
                    slider.ScrollPosition = getSliderValueFromCommand(command);
                }
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
