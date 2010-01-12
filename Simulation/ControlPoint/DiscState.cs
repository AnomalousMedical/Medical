using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving;

namespace Medical
{
    public class DiscState : Saveable
    {
        private Dictionary<String, DiscStateProperties> discs = new Dictionary<String, DiscStateProperties>();

        internal DiscState()
        {

        }

        public void addPosition(Disc disc)
        {
            if (discs.ContainsKey(disc.Owner.Name))
            {
                discs[disc.Owner.Name] = new DiscStateProperties(disc);
            }
            else
            {
                discs.Add(disc.Owner.Name, new DiscStateProperties(disc));
            }
        }

        public void addPosition(DiscStateProperties prop)
        {
            if (discs.ContainsKey(prop.DiscName))
            {
                discs[prop.DiscName] = prop;
            }
            else
            {
                discs.Add(prop.DiscName, prop);
            }
        }

        public void blend(DiscState target, float percent)
        {
            foreach (String key in discs.Keys)
            {
                discs[key].blend(target.discs[key], percent);
            }
        }

        public DiscStateProperties getPosition(String name)
        {
            DiscStateProperties prop;
            discs.TryGetValue(name, out prop);
            return prop;
        }

        #region Saveable Members

        private const string POSITIONS = "Positions";

        protected DiscState(LoadInfo info)
        {
            info.RebuildDictionary<String, DiscStateProperties>(POSITIONS, discs);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractDictionary<String, DiscStateProperties>(POSITIONS, discs);
        }

        #endregion
    }
}
