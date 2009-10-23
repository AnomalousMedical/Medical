using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.ObjectManagement;

namespace Medical.GUI
{
    public partial class MeasurementGUI : GUIElement
    {
        public MeasurementGUI()
        {
            InitializeComponent();
        }

        protected override void sceneLoaded(SimScene scene)
        {
            base.sceneLoaded(scene);
            foreach (Measurement measure in MeasurementController.Measurements)
            {
                measurementPanel.Controls.Add(new MeasurementView(measure));
            }
        }

        protected override void sceneUnloading()
        {
            base.sceneUnloading();
            foreach (Control control in measurementPanel.Controls)
            {
                control.Dispose();
            }
            measurementPanel.Controls.Clear();
        }

        private void showLines_CheckedChanged(object sender, EventArgs e)
        {
            MeasurementController.ShowingMeasurements = showLines.Checked;
        }
    }
}
