using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class TeethController
    {
        static Dictionary<String, Tooth> teeth = new Dictionary<string, Tooth>();

        static TeethController()
        {
            HighlightContacts = false;
        }

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

        public static void adaptAllTeeth()
        {
            foreach (Tooth tooth in teeth.Values)
            {
                tooth.adapt();
            }
        }

        public static void adaptSingleTooth(String name)
        {
            teeth[name].adapt();
        }

        /// <summary>
        /// Returns true if all unextracted teeth are touching.
        /// </summary>
        /// <returns>True if all unextracted teeth are touching.</returns>
        public static bool allTeethTouching()
        {
            bool touching = true;
            foreach (Tooth tooth in teeth.Values)
            {
                if (!tooth.Extracted && !tooth.MakingContact)
                {
                    touching = false;
                    break;
                }
            }
            return touching;
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

        public static bool HighlightContacts { get; set; }
    }
}
