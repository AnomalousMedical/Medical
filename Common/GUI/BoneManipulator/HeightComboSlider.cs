using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI
{
    public partial class HeightComboSlider : UserControl
    {
        AnimationManipulator condyle;
        AnimationManipulator ramus;

        public HeightComboSlider()
        {
            InitializeComponent();
            valueTrackBar.ValueChanged += new EventHandler(valueTrackBar_ValueChanged);
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

        public String LabelText
        {
            get
            {
                return sliderNameLabel.Text;
            }
            set
            {
                sliderNameLabel.Text = value;
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
            }
        }

        void valueTrackBar_ValueChanged(object sender, EventArgs e)
        {
            condyle.Position = Value + condyle.DefaultPosition;
            ramus.Position = Value + ramus.DefaultPosition;
        }
    }
}
