using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Logging;

namespace Medical.GUI
{
    class AdvancedMandibleControlSlider
    {
        public event EventHandler ValueChanged;

        private float minimum = 0.0f;
        private float maximum = 0.0f;
        private float sequentialChange = 0.0f;

        private VScroll scrollBar;
        private NumericEdit numericEdit;

        public AdvancedMandibleControlSlider(VScroll scrollBar, Edit edit)
        {
            this.scrollBar = scrollBar;
            scrollBar.ScrollChangePosition += new MyGUIEvent(scrollBar_ScrollChangePosition);

            numericEdit = new NumericEdit(edit);
            numericEdit.AllowFloat = true;
            numericEdit.ValueChanged += new MyGUIEvent(numericEdit_ValueChanged);
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
                synchronize(this, oldTime - minimum);
                this.MaximumTime = maximum - minimum;
                numericEdit.MinValue = value;
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
                numericEdit.MaxValue = value - 0.0001f;
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
                numericEdit.Increment = value;
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
                synchronize(this, value - minimum);
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

        void numericEdit_ValueChanged(Widget source, EventArgs e)
        {
            synchronize(numericEdit, numericEdit.FloatValue - minimum);
            fireValueChanged();
        }

        void scrollBar_ScrollChangePosition(Widget source, EventArgs e)
        {
            synchronize(scrollBar, CurrentTime);
            fireValueChanged();
        }

        private void fireValueChanged()
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
            synchronize(this, time);
        }

        void nextButton_Click(object sender, EventArgs e)
        {
            float time = this.CurrentTime + sequentialChange;
            if (time > this.MaximumTime)
            {
                time = this.MaximumTime;
            }
            synchronize(this, time);
        }

        private void synchronize(Object source, float currentTime)
        {
            if (source != numericEdit)
            {
                numericEdit.FloatValue = currentTime + minimum;
            }
            if (source != scrollBar)
            {
                scrollBar.ScrollPosition = (uint)(currentTime * 1000.0f);
            }
        }

        private float CurrentTime
        {
            get
            {
                return scrollBar.ScrollPosition / 1000.0f;
            }
        }

        private float MaximumTime
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
