using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class FossaController
    {
        static Dictionary<String, Fossa> fossas = new Dictionary<string, Fossa>();

        public static void add(Fossa fossa)
        {
            fossas.Add(fossa.Owner.Name, fossa);
        }

        public static void remove(Fossa fossa)
        {
            fossas.Remove(fossa.Owner.Name);
        }

        public static Fossa get(String name)
        {
            Fossa ret;
            fossas.TryGetValue(name, out ret);
            return ret;
        }

        public static FossaState createState()
        {
            FossaState state = new FossaState();
            foreach (Fossa fossa in fossas.Values)
            {
                state.addPosition(fossa.Owner.Name, fossa.getEminanceDistortion());
            }
            return state;
        }
    }
}
