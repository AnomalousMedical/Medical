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
    public delegate void ChangeTransparency(float value);

    public partial class LayerSlider : UserControl
    {
        public event ChangeTransparency TransparencyChanged;

        public LayerSlider()
        {
            InitializeComponent();
            transparencySlider.ValueChanged += new EventHandler(transparencySlider_ValueChanged);
        }

        /// <summary>
        /// Move the slider to the next value.
        /// </summary>
        public void toggle()
        {
            transparencySlider.Value = (transparencySlider.Value + 1) % (transparencySlider.Maximum + 1);
        }

        public String LabelText
        {
            get
            {
                return nameLabel.Text;
            }
            set
            {
                nameLabel.Text = value;
            }
        }

        void transparencySlider_ValueChanged(object sender, EventArgs e)
        {
            if (TransparencyChanged != null)
            {
                switch (transparencySlider.Value)
                {
                    case 0:
                        TransparencyChanged.Invoke(1.0f);
                        break;
                    case 1:
                        TransparencyChanged.Invoke(0.7f);
                        break;
                    case 2:
                        TransparencyChanged.Invoke(0.0f);
                        break;
                }
            }
        }
    }
}
