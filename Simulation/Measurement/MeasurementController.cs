using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class MeasurementController
    {
        private static List<Measurement> measurements = new List<Measurement>();
        private static MeasurementDrawer measurementDrawer;

        private MeasurementController()
        {

        }

        internal static void addMesurement(Measurement measurement)
        {
            measurements.Add(measurement);
        }

        internal static void removeMeasurement(Measurement measurement)
        {
            measurements.Remove(measurement);
        }

        internal static void setMeasurementDrawer(MeasurementDrawer drawer)
        {
            measurementDrawer = drawer;
        }

        public static IEnumerable<Measurement> Measurements
        {
            get
            {
                return measurements;
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
