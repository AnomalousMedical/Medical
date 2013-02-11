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

        static String[] categoryOrder = new String[] { "FOUNDATIONAL DIMENSION", "OCCLUSAL HORIZONTAL CHANGES", "OCCLUSAL VERTICAL CHANGES" };
        const String spaceString = "                                 ";

        void MedicalController_FixedLoopUpdate(Clock time)
        {
            Dictionary<String, List<Measurement>> categories = new Dictionary<String, List<Measurement>>();
            StringBuilder sb = new StringBuilder();
            foreach (Measurement measurement in MeasurementController.Measurements)
            {
                List<Measurement> category;
                if(!categories.TryGetValue(measurement.Category, out category))
                {
                    category = new List<Measurement>();
                    categories.Add(measurement.Category, category);
                }
                category.Add(measurement);
            }
            String categoryFormat = "#000000{0}\n\n";
            bool firstCategory = true;
            foreach (String categoryKey in categoryOrder)
            {
                sb.AppendFormat(categoryFormat, categoryKey);
                if (firstCategory)
                {
                    firstCategory = false;
                    categoryFormat = "\n" + categoryFormat;
                }
                List<Measurement> category = categories[categoryKey];
                category.Sort((left, right) =>
                    {
                        return left.Index - right.Index;
                    });
                
                foreach (Measurement measurement in category)
                {
                    String deltaString = measurement.CurrentDelta.ToString("f2");
                    int spaceCount = spaceString.Length - measurement.MeasurementName.Length - deltaString.Length;
                    if (spaceCount < 0)
                    {
                        spaceCount = 0;
                    }
                    if (measurement.MeasurementName == "AOB")
                    {
                        spaceCount += 5;
                    }
                    if (measurement.MeasurementName == "Midline")
                    {
                        spaceCount += 2;
                    }
                    if (measurement.MeasurementName == "Right Fossa Ramus")
                    {
                        spaceCount -= 1;
                    }
                    if (measurement.MeasurementName == "Left Molar Oblique")
                    {
                        spaceCount -= 3;
                    }
                    if (measurement.MeasurementName == "Right Molar Oblique")
                    {
                        spaceCount -= 4;
                    }
                    if (measurement.MeasurementName == "Fissure to 3")
                    {
                        spaceCount += 1;
                    }
                    
                    sb.AppendFormat(" *  #555566{0}{2}#000000{1} mm\n", measurement.MeasurementName, deltaString, spaceString.Substring(0, spaceCount));
                }
            }
            text.Caption = sb.ToString();
        }

        void showLines_CheckedChanged(Widget source, EventArgs e)
        {
            MeasurementController.ShowingMeasurements = showLines.Checked;
        }
    }
}
