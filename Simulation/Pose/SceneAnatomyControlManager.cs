using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public static class SceneAnatomyControlManager
    {
        private static List<SceneAnatomyControl> controls = new List<SceneAnatomyControl>();

        internal static void addControl(SceneAnatomyControl control)
        {
            controls.Add(control);
        }

        internal static void removeControl(SceneAnatomyControl control)
        {
            controls.Remove(control);
        }

        public static IEnumerable<SceneAnatomyControl> Controls
        {
            get
            {
                return controls;
            }
        }
    }
}
