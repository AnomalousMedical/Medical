﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Logging;

namespace DentalSim.GUI
{
    class MandibleControlSlider
    {
        public event EventHandler ValueChanged;

        private float minimum = 0.0f;
        private float maximum = 0.0f;
        private float sequentialChange = 0.0f;

        private ScrollBar scrollBar;

        public MandibleControlSlider(ScrollBar scrollBar)
        {
            this.scrollBar = scrollBar;
            scrollBar.ScrollChangePosition += new MyGUIEvent(scrollBar_ScrollChangePosition);
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

        void previousButton_Click(object sender, EventArgs e)
        {
            float time = this.CurrentTime - sequentialChange;
            if (time < 0.0f)
            {
                time = 0.0f;
            }
            this.CurrentTime = time;
        }

        void nextButton_Click(object sender, EventArgs e)
        {
            float time = this.CurrentTime + sequentialChange;
            if (time > this.MaximumTime)
            {
                time = this.MaximumTime;
            }
            this.CurrentTime = time;
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
    }
}
