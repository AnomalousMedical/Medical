using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class ControlPointController
    {
        static Dictionary<String, ControlPointBehavior> controlPoints = new Dictionary<string, ControlPointBehavior>();

        public static void addControlPoint(ControlPointBehavior cp)
        {
            controlPoints.Add(cp.Owner.Name, cp);
        }

        public static void removeControlPoint(ControlPointBehavior cp)
        {
            controlPoints.Remove(cp.Owner.Name);
        }

        public static ControlPointBehavior getControlPoint(String name)
        {
            ControlPointBehavior ret;
            controlPoints.TryGetValue(name, out ret);
            return ret;
        }
    }
}
