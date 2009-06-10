using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class TeethController
    {
        static Dictionary<String, Tooth> teeth = new Dictionary<string, Tooth>();

        public static void addTooth(String name, Tooth tooth)
        {
            teeth.Add(name, tooth);
        }

        public static void removeTooth(String name)
        {
            teeth.Remove(name);
        }

        public static bool hasTooth(String name)
        {
            return teeth.ContainsKey(name);
        }

        public static Tooth getTooth(String name)
        {
            Tooth ret;
            teeth.TryGetValue(name, out ret);
            return ret;
        }
    }
}
