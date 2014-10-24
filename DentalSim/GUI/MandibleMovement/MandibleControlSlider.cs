using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Logging;

namespace DentalSim.GUI
{
    class MandibleControlSlider
    {
        /// <summary>
        /// This event is fired when the value changes
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// This event fires when the value change starts, it will fire before ValueChanged is fired.
        /// </summary>
        public event Action<MandibleControlSlider> ValueChangeStarted;

        /// <summary>
        /// This event is fired when the value change ends, it will fire after ValueChanged is fired.
        /// </summary>
        public event Action<MandibleControlSlider> ValueChangeEnded;

        private float minimum = 0.0f;
        private float maximum = 0.0f;
        private float sequentialChange = 0.0f;

        private ScrollBar scrollBar;

        public MandibleControlSlider(ScrollBar scrollBar)
        {
            this.scrollBar = scrollBar;
            scrollBar.ScrollIncrement = 0;
            scrollBar.MouseButtonPressed += scrollBar_MouseButtonPressed;
            scrollBar.MouseButtonReleased += scrollBar_MouseButtonReleased;
            scrollBar.ScrollChangePosition += scrollBar_ScrollChangePosition;
        }

        public float Minimum
        {
            get
            {
                return minimum;
            }
            set
            {
                float oldTime = this.CurrentTime + minimum;
                minimum = value;
                this.CurrentTime = oldTime - minimum;
                this.MaximumTime = maximum - minimum;
            }
        }

        public float Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                maximum = value;
                this.MaximumTime = maximum - minimum;
            }
        }

        public float SequentialChange
        {
            get
            {
                return sequentialChange;
            }
            set
            {
                sequentialChange = value;
            }
        }

        public float Value
        {
            get
            {
                return this.CurrentTime + minimum;
            }
            set
            {
                this.CurrentTime = value - minimum;
            }
        }

        public bool Enabled
        {
            get
            {
                return scrollBar.Enabled;
            }
            set
            {
                scrollBar.Enabled = value;
            }
        }

        void scrollBar_ScrollChangePosition(Widget source, EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this, EventArgs.Empty);
            }
        }

        private float CurrentTime
        {
            get
            {
                return scrollBar.ScrollPosition / 1000.0f;
            }
            set
            {
                scrollBar.ScrollPosition = (uint)(value * 1000.0f);
            }
        }

        public float MaximumTime
        {
            get
            {
                return scrollBar.ScrollRange / 1000.0f;
            }
            set
            {
                scrollBar.ScrollRange = (uint)(value * 1000.0f);
            }
        }

        void scrollBar_MouseButtonReleased(Widget source, EventArgs e)
        {
            if (ValueChangeEnded != null)
            {
                ValueChangeEnded.Invoke(this);
            }
        }

        void scrollBar_MouseButtonPressed(Widget source, EventArgs e)
        {
            if(ValueChangeStarted != null)
            {
                ValueChangeStarted.Invoke(this);
            }
        }
    }
}
