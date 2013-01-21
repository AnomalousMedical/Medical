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
        private StandaloneController standaloneController;

        public MeasurementGUI(StandaloneController standaloneController)
            : base("Developer.GUI.Measurement.MeasurementGUI.layout")
        {
            this.standaloneController = standaloneController;

            text = (TextBox)window.findWidget("Text");
            enabled = new CheckButton((Button)window.findWidget("Enabled"));
            enabled.CheckedChanged += new MyGUIEvent(enabled_CheckedChanged);
            enabled.Checked = PerformanceMonitor.Enabled;

            Button reset = (Button)window.findWidget("ResetButton");
            reset.MouseButtonClick += new MyGUIEvent(reset_MouseButtonClick);
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

        void reset_MouseButtonClick(Widget source, EventArgs e)
        {
            foreach (Timelapse timelapse in PerformanceMonitor.Timelapses)
            {
                timelapse.resetMinMax();
            }
        }
    }
}
