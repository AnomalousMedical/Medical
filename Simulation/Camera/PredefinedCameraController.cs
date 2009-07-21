using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;

namespace Medical
{
    public class PredefinedCameraController
    {
        static Dictionary<String, PredefinedCamera> cameras = new Dictionary<string, PredefinedCamera>();

        internal static void add(PredefinedCamera camera)
        {
            if (!cameras.ContainsKey(camera.CameraName))
            {
                cameras.Add(camera.CameraName, camera);
            }
            else
            {
                Log.Default.sendMessage("Attempted to add a duplicate camera named {0}. Duplicate ignored.", LogLevel.Error, "Simulation");
            }
        }

        internal static void remove(PredefinedCamera camera)
        {
            cameras.Remove(camera.CameraName);
        }

        public static bool contains(String name)
        {
            return cameras.ContainsKey(name);
        }

        public static PredefinedCamera get(String name)
        {
            PredefinedCamera ret;
            cameras.TryGetValue(name, out ret);
            return ret;
        }

        public static IEnumerable<String> getCameraNameList()
        {
            return cameras.Keys;
        }
    }
}
