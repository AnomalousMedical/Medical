using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Ribbon;

namespace Medical.GUI
{
    public partial class MandibleControlSlider : UserControl
    {
        public event EventHandler ValueChanged;

        private float minimum = 0.0f;
        private float maximum = 0.0f;
        private float sequentialChange = 0.0f;

        private KryptonRibbonGroupButton previousButton;
        private KryptonRibbonGroupButton nextButton;

        public MandibleControlSlider()
        {
            InitializeComponent();
            amountTrackBar.TimeChanged += new TimeChanged(amountTrackBar_TimeChanged);
        }

        public float Minimum
        {
            get
            {
                return minimum;
            }
            set
            {
                float oldTime = amountTrackBar.CurrentTime + minimum;
                minimum = value;
                amountTrackBar.CurrentTime = oldTime - minimum;
                amountTrackBar.MaximumTime = maximum - minimum;
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
                amountTrackBar.MaximumTime = maximum - minimum;
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
                return amountTrackBar.CurrentTime + minimum;
            }
            set
            {
                amountTrackBar.CurrentTime = value - minimum;
            }
        }

        public KryptonRibbonGroupButton PreviousButton
        {
            get
            {
                return previousButton;
            }
            set
            {
                if (previousButton != null)
                {
                    previousButton.Click -= previousButton_Click;
                }
                previousButton = value;
                if (previousButton != null)
                {
                    previousButton.Click += previousButton_Click;
                }
            }
        }

        public KryptonRibbonGroupButton NextButton
        {
            get
            {
                return nextButton;
            }
            set
            {
                if (nextButton != null)
                {
                    nextButton.Click -= nextButton_Click;
                }
                nextButton = value;
                if (nextButton != null)
                {
                    nextButton.Click += nextButton_Click;
                }
            }
        }

        void amountTrackBar_TimeChanged(TimeTrackBar trackBar, double currentTime)
        {
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this, null);
            }
        }

        void previousButton_Click(object sender, EventArgs e)
        {
            float time = amountTrackBar.CurrentTime - sequentialChange;
            if (time < 0.0f)
            {
                time = 0.0f;
            }
            amountTrackBar.CurrentTime = time;
        }

        void nextButton_Click(object sender, EventArgs e)
        {
            float time = amountTrackBar.CurrentTime + sequentialChange;
            if (time > amountTrackBar.MaximumTime)
            {
                time = amountTrackBar.MaximumTime;
            }
            amountTrackBar.CurrentTime = time;
        }
    }
}
