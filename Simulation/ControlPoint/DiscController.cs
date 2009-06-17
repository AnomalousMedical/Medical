using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class DiscController
    {
        static Dictionary<String, Disc> discs = new Dictionary<string, Disc>();

        public static void addDisc(Disc disc)
        {
            discs.Add(disc.Owner.Name, disc);
        }

        public static void removeDisc(Disc disc)
        {
            discs.Remove(disc.Owner.Name);
        }

        public static Disc getDisc(String name)
        {
            Disc ret;
            discs.TryGetValue(name, out ret);
            return ret;
        }

        public static DiscState createDiscState()
        {
            DiscState state = new DiscState();
            foreach (Disc disc in discs.Values)
            {
                state.addPosition(disc.Owner.Name, disc.getOffset(0));
            }
            return state;
        }
    }
}
