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
    public partial class MeasurementView : UserControl
    {
        private const String labelFormat = "{0} mm";

        Measurement measurement;

        public MeasurementView(Measurement measurement)
        {
            InitializeComponent();
            this.measurement = measurement;
            nameLabel.Text = measurement.MeasurementName;
            measurement.MeasurementChanged += measurement_MeasurementChanged;
        }

        void measurement_MeasurementChanged(Measurement src)
        {
            measurementLabel.Text = String.Format(labelFormat, src.CurrentDelta);
        }
    }
}
