using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public static class VolumeController
    {
        private static List<VolumeCalculator> volumes = new List<VolumeCalculator>();

        internal static void addVolume(VolumeCalculator volumeCalculator)
        {
            volumes.Add(volumeCalculator);
        }

        internal static void removeMeasurement(VolumeCalculator volumeCalculator)
        {
            volumes.Remove(volumeCalculator);
        }
    }
}
