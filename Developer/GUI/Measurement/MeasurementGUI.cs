using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;
using Engine;
using Medical;
using Engine.Performance;
using Engine.Platform;

namespace Developer.GUI
{
    class MeasurementGUI : MDIDialog
    {
        private TextBox text;
        private CheckButton enabled;
        private CheckButton showLines;
        private StandaloneController standaloneController;

        public MeasurementGUI(StandaloneController standaloneController)
            : base("Developer.GUI.Measurement.MeasurementGUI.layout")
        {
            this.standaloneController = standaloneController;

            text = (TextBox)window.findWidget("Text");

            enabled = new CheckButton((Button)window.findWidget("Enabled"));
            enabled.CheckedChanged += new MyGUIEvent(enabled_CheckedChanged);
            enabled.Checked = false;

            showLines = new CheckButton((Button)window.findWidget("Show"));
            showLines.Checked = MeasurementController.ShowingMeasurements;
            showLines.CheckedChanged += showLines_CheckedChanged;
        }

        void enabled_CheckedChanged(Widget source, EventArgs e)
        {
            PerformanceMonitor.Enabled = enabled.Checked;
            if (enabled.Checked)
            {
                standaloneController.MedicalController.FixedLoopUpdate += MedicalController_FixedLoopUpdate;
            }
            else
            {
                standaloneController.MedicalController.FixedLoopUpdate -= MedicalController_FixedLoopUpdate;
            }
        }

        void MedicalController_FixedLoopUpdate(Clock time)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Measurement measurement in MeasurementController.Measurements)
            {
                sb.AppendFormat("{1:f2} mm - {0}\n", measurement.MeasurementName, measurement.CurrentDelta);
            }
            text.Caption = sb.ToString();
        }

        void showLines_CheckedChanged(Widget source, EventArgs e)
        {
            MeasurementController.ShowingMeasurements = showLines.Checked;
        }
    }
}
