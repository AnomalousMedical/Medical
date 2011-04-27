using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class CommandHScroll : CommandUIElement
    {
        private const float SCROLL_MAX = 10000;
        private const float SCROLL_MAX_RANGE_MIN = SCROLL_MAX - 50;//The minimum postion of a scroll bar to count as the max value.

        private HScroll slider;

        public CommandHScroll(AnatomyCommand command, Widget parentWidget)
        {
            slider = (HScroll)parentWidget.createWidgetT("HScroll", "HSlider", 0, 0, parentWidget.Width - SIDE_PADDING, 20, Align.Default, "");
            slider.ScrollChangePosition += new MyGUIEvent(slider_ScrollChangePosition);
            slider.UserObject = command;
            slider.ScrollRange = (int)SCROLL_MAX;
            slider.ScrollPosition = getSliderValueFromCommand(command);
            LayoutContainer = new MyGUILayoutContainer(slider);
        }

        public override void Dispose()
        {
            Gui.Instance.destroyWidget(slider);
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
    }
}
