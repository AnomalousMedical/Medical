using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class MeasurementController
    {
        private static Dictionary<String, Measurement> measurements = new Dictionary<String, Measurement>();
        private static MeasurementDrawer measurementDrawer;

        private MeasurementController()
        {

        }

        internal static void addMesurement(Measurement measurement)
        {
            measurements.Add(measurement.MeasurementName, measurement);
        }

        internal static void removeMeasurement(Measurement measurement)
        {
            measurements.Remove(measurement.MeasurementName);
        }

        internal static void setMeasurementDrawer(MeasurementDrawer drawer)
        {
            measurementDrawer = drawer;
        }

        public static bool tryGetCalculator(String name, out Measurement measurement)
        {
            return measurements.TryGetValue(name, out measurement);
        }

        public static IEnumerable<Measurement> Measurements
        {
            get
            {
                return measurements.Values;
            }
        }

        public static bool ShowingMeasurements
        {
            get
            {
                return measurementDrawer != null ? measurementDrawer.DrawLines : false;
            }
            set
            {
                if (measurementDrawer != null)
                {
                    measurementDrawer.DrawLines = value;
                }
            }
        }
    }
}
