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

        private ScrollBar slider;
        private List<AnatomyCommand> transparencyCommands = new List<AnatomyCommand>();
        private LayerState undoState;

        public event Action<LayerState> RecordUndo;

        public AnatomyTransparencySlider(ScrollBar slider)
        {
            this.slider = slider;
            slider.ScrollChangePosition += new MyGUIEvent(slider_ScrollChangePosition);
            slider.ScrollRange = (int)SCROLL_MAX;
            slider.ScrollWheelPage = 1000;
            slider.MouseButtonPressed += slider_MouseButtonPressed;
            slider.MouseButtonReleased += slider_MouseButtonReleased;
        }

        public void Dispose()
        {
            clearCommands();
        }

        public void addCommand(AnatomyCommand command)
        {
            if(transparencyCommands.Count == 0)
            {
                command.NumericValueChanged += command_NumericValueChanged;
                slider.ScrollPosition = getSliderValueFromCommand(command);
            }
            transparencyCommands.Add(command);
        }

        public void clearCommands()
        {
            if(transparencyCommands.Count > 0)
            {
                transparencyCommands[0].NumericValueChanged -= command_NumericValueChanged;
            }
            transparencyCommands.Clear();
        }

        void slider_ScrollChangePosition(Widget source, EventArgs e)
        {
            foreach(var command in transparencyCommands)
            {
                ScrollBar scroll = (ScrollBar)source;
                float normalizedValue = scroll.ScrollPosition / SCROLL_MAX;
                if (scroll.ScrollPosition > SCROLL_MAX_RANGE_MIN)
                {
                    normalizedValue = 1.0f;
                }
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

        void slider_MouseButtonPressed(Widget source, EventArgs e)
        {
            undoState = LayerState.CreateAndCapture();
        }

        void slider_MouseButtonReleased(Widget source, EventArgs e)
        {
            if(RecordUndo != null)
            {
                RecordUndo.Invoke(undoState);
            }
            undoState = null;
        }
    }
}
