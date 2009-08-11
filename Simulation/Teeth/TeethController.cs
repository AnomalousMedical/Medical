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

        public static TeethState createTeethState()
        {
            TeethState state = new TeethState();
            foreach (Tooth tooth in teeth.Values)
            {
                state.addPosition(new ToothState(tooth.Owner.Name, tooth.Extracted, tooth.Offset, tooth.Rotation));
            }
            return state;
        }

        public static void setTeethLoose(bool loose)
        {
            foreach (Tooth tooth in teeth.Values)
            {
                tooth.Loose = loose;
            }
        }
    }
}
