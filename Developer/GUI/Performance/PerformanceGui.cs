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
    class PerformanceGui : MDIDialog
    {
        private TextBox text;
        private CheckButton enabled;
        private StandaloneController standaloneController;

        public PerformanceGui(StandaloneController standaloneController)
            :base("Developer.GUI.Performance.PerformanceGui.layout")
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
                standaloneController.MedicalController.OnLoopUpdate += MedicalController_OnLoopUpdate;
            }
            else
            {
                standaloneController.MedicalController.OnLoopUpdate -= MedicalController_OnLoopUpdate;
            }
        }

        void MedicalController_OnLoopUpdate(Clock time)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Timelapse timelapse in PerformanceMonitor.Timelapses)
            {
                sb.AppendFormat("{0}: {1}ms   |   Min {2}ms   |   Max {3}ms   |   Avg {4}ms\n", timelapse.Name, timelapse.Duration, timelapse.Min, timelapse.Max, timelapse.Average);
            }
            text.Caption = sb.ToString();
        }

        void reset_MouseButtonClick(Widget source, EventArgs e)
        {
            foreach (Timelapse timelapse in PerformanceMonitor.Timelapses)
            {
                timelapse.resetStats();
            }
        }
    }
}
