using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public static class VolumeController
    {
        private static Dictionary<String, VolumeCalculator> volumes = new Dictionary<String, VolumeCalculator>();

        internal static void addVolume(VolumeCalculator volumeCalculator)
        {
            volumes.Add(volumeCalculator.Name, volumeCalculator);
        }

        internal static void removeVolume(VolumeCalculator volumeCalculator)
        {
            volumes.Remove(volumeCalculator.Name);
        }

        public static bool tryGetCalculator(String name, out VolumeCalculator calculator)
        {
            return volumes.TryGetValue(name, out calculator);
        }
    }
}
