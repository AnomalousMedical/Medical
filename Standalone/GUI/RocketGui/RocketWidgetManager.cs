using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    /// <summary>
    /// This class provides some bookkeeping functions for the RocketWidgets, it
    /// is not intended to be used outside that class.
    /// </summary>
    static class RocketWidgetManager
    {
        private const String NameBase = "RocketWidget_{0}";

        private static HashSet<String> rocketWidgetNames = new HashSet<string>();

        public static String generateRocketWidgetName()
        {
            String name = String.Format(NameBase, UniqueKeyGenerator.generateStringKey());
            while (rocketWidgetNames.Contains(name))
            {
                name = String.Format(NameBase, UniqueKeyGenerator.generateStringKey());
            }
            rocketWidgetNames.Add(name);
            return name;
        }

        public static void rocketWidgetDisposed(String name)
        {
            rocketWidgetNames.Remove(name);
        }
    }
}
