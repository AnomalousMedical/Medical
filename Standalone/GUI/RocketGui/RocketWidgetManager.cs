using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using OgreWrapper;

namespace Medical.GUI
{
    /// <summary>
    /// This class provides some bookkeeping functions for the RocketWidgets, it
    /// is not intended to be used outside that class.
    /// </summary>
    static class RocketWidgetManager
    {
        private const String NameBase = "RocketWidget_{0}";

        private static Dictionary<String, RocketWidget> rocketWidgetNames = new Dictionary<String, RocketWidget>();

        public static String generateRocketWidgetName(RocketWidget widget)
        {
            String name = String.Format(NameBase, UniqueKeyGenerator.generateStringKey());
            while (rocketWidgetNames.ContainsKey(name))
            {
                name = String.Format(NameBase, UniqueKeyGenerator.generateStringKey());
            }
            rocketWidgetNames.Add(name, widget);
            return name;
        }

        public static void rocketWidgetDisposed(String name)
        {
            rocketWidgetNames.Remove(name);
        }

        internal static void deviceRestored()
        {
            foreach (RocketWidget widget in rocketWidgetNames.Values)
            {
                widget.renderOnNextFrame();
            }
        }
    }
}
