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
    public partial class MandibleControlSlider : UserControl
    {
        public event EventHandler ValueChanged;

        public MandibleControlSlider()
        {
            InitializeComponent();
            amountTrackBar.ValueChanged += new EventHandler(amountTrackBar_ValueChanged);
        }

        void amountTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this, e);
            }
        }

        public Image ClosedImage
        {
            get
            {
                return closedPicturePanel.BackgroundImage;
            }
            set
            {
                closedPicturePanel.BackgroundImage = value;
            }
        }

        public Image OpenImage 
        { 
            get
            {
                return openPicturePanel.BackgroundImage;
            }
            set
            {
                openPicturePanel.BackgroundImage = value;
            }
        }

        public int Minimum
        {
            get
            {
                return amountTrackBar.Minimum;
            }
            set
            {
                amountTrackBar.Minimum = value;
            }
        }

        public int Maximum
        {
            get
            {
                return amountTrackBar.Maximum;
            }
            set
            {
                amountTrackBar.Maximum = value;
            }
        }

        public int SmallChange
        {
            get
            {
                return amountTrackBar.SmallChange;
            }
            set
            {
                amountTrackBar.SmallChange = value;
            }
        }

        public int LargeChange
        {
            get
            {
                return amountTrackBar.LargeChange;
            }
            set
            {
                amountTrackBar.LargeChange = value;
            }
        }

        public int Value
        {
            get
            {
                return amountTrackBar.Value;
            }
            set
            {
                amountTrackBar.Value = value;
            }
        }
    }
}
