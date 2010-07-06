using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class HeightComboSlider
    {
        AnimationManipulator condyle;
        AnimationManipulator ramus;

        private MinMaxScroll valueTrackBar;

        public HeightComboSlider(VScroll scrollBar)
        {
            valueTrackBar = new MinMaxScroll(scrollBar);
            valueTrackBar.ScrollChangePosition += new MyGUIEvent(valueTrackBar_ScrollChangePosition);
            valueTrackBar.Minimum = 0;
            valueTrackBar.Maximum = 1000;
        }

        public void initialize(AnimationManipulator condyle, AnimationManipulator ramus)
        {
            this.condyle = condyle;
            this.ramus = ramus;
        }

        public void clearManipulator()
        {
            if (condyle != null)
            {
                this.condyle = null;
                this.ramus = null;
            }
        }

        public void setToDefault()
        {
            if (condyle != null)
            {
                valueTrackBar.Value = 0;
            }
        }

        public void getPositionFromScene()
        {
            int value = (int)((ramus.Position - ramus.DefaultPosition) * valueTrackBar.Maximum);
            if (value > valueTrackBar.Minimum)
            {
                if (value < valueTrackBar.Maximum)
                {
                    valueTrackBar.Value = value;
                }
                else
                {
                    valueTrackBar.Value = valueTrackBar.Maximum;
                }
            }
            else
            {
                valueTrackBar.Value = valueTrackBar.Minimum;
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
                valueTrackBar_ScrollChangePosition(null, null);
            }
        }

        void valueTrackBar_ScrollChangePosition(Widget source, EventArgs e)
        {
            condyle.Position = Value + condyle.DefaultPosition;
            ramus.Position = Value + ramus.DefaultPosition;
        }
    }
}
