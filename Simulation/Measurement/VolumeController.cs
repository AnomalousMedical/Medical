using Engine.Editing;
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
            volumes.Add(volumeCalculator.VolumeName, volumeCalculator);
        }

        internal static void removeVolume(VolumeCalculator volumeCalculator)
        {
            volumes.Remove(volumeCalculator.VolumeName);
        }

        public static bool tryGetCalculator(String name, out VolumeCalculator calculator)
        {
            return volumes.TryGetValue(name, out calculator);
        }

        public static Browser Browser
        {
            get
            {
                Browser browser = new Browser("Volumes", "Choose a Volume");
                foreach (var volume in volumes.Values)
                {
                    browser.addNode(volume.Category, new BrowserNode(volume.PrettyName, volume.VolumeName));
                }
                return browser;
            }
        }

        public static IEnumerable<VolumeCalculator> Volumes
        {
            get
            {
                return volumes.Values;
            }
        }
    }
}
