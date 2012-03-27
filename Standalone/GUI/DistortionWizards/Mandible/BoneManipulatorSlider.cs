using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class BoneManipulatorSlider
    {
        public event EventHandler ValueChanged;

        private AnimationManipulator manipulator;
        private bool allowSynchronization = true;

        private MinMaxScroll valueTrackBar;

        public BoneManipulatorSlider(ScrollBar scroll)
        {
            valueTrackBar = new MinMaxScroll(scroll);
            valueTrackBar.Minimum = 0;
            valueTrackBar.Maximum = 1000;
            valueTrackBar.ScrollChangePosition += new MyGUIEvent(valueTrackBar_ScrollChangePosition);
            BoneManipulator = scroll.getUserString("BoneManipulator");
        }

        public void initialize(AnimationManipulator manipulator)
        {
            this.manipulator = manipulator;
            synchronizeValue(manipulator, manipulator.Position);
        }

        public void updateFromScene()
        {
            synchronizeValue(manipulator, manipulator.Position);
        }

        public AnimationManipulatorStateEntry createStateEntry()
        {
            return manipulator.createStateEntry();
        }

        public void clearManipulator()
        {
            if (manipulator != null)
            {
                this.manipulator = null;
            }
        }

        public void setToDefault()
        {
            if (manipulator != null)
            {
                synchronizeValue(this, manipulator.DefaultPosition);
            }
        }

        public float Value
        {
            get
            {
                return (float)valueTrackBar.Value / valueTrackBar.Maximum;
            }
            set
            {
                synchronizeValue(this, value);
            }
        }

        /// <summary>
        /// The name of the BoneManipulator that will be modified by this slider.
        /// </summary>
        public String BoneManipulator { get; set; }

        /// <summary>
        /// The value of the slider when a wizard panel was opened. Not used
        /// internally, can be used to track info by another class.
        /// </summary>
        public float OpeningValue { get; set; }

        void valueTrackBar_ScrollChangePosition(Widget source, EventArgs e)
        {
            synchronizeValue(valueTrackBar, (float)valueTrackBar.Value / (float)valueTrackBar.Maximum);
        }

        private void synchronizeValue(object sender, float value)
        {
            if (allowSynchronization)
            {
                allowSynchronization = false;
                if (manipulator != null && sender != manipulator)
                {
                    manipulator.Position = value;
                }
                if (sender != valueTrackBar)
                {
                    int newVal = (int)(value * valueTrackBar.Maximum);
                    if (newVal > valueTrackBar.Maximum)
                    {
                        valueTrackBar.Value = valueTrackBar.Maximum;
                    }
                    else if (newVal < valueTrackBar.Minimum)
                    {
                        valueTrackBar.Value = valueTrackBar.Minimum;
                    }
                    else
                    {
                        valueTrackBar.Value = newVal;
                    }
                }
                if (ValueChanged != null)
                {
                    ValueChanged.Invoke(this, EventArgs.Empty);
                }
                allowSynchronization = true;
            }
        }
    }
}
