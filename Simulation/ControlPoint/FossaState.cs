using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class FossaState : Saveable
    {
        private Dictionary<String, float> positions = new Dictionary<string, float>();

        internal FossaState()
        {

        }

        public void addPosition(String fossa, float position)
        {
            if (positions.ContainsKey(fossa))
            {
                positions[fossa] = position;
            }
            else
            {
                positions.Add(fossa, position);
            }
        }

        public float getPosition(String fossa)
        {
            float position;
            positions.TryGetValue(fossa, out position);
            return position;
        }

        public void blend(FossaState target, float percent)
        {
            foreach (String key in positions.Keys)
            {
                Fossa fossa = FossaController.get(key);
                if (fossa != null)
                {
                    float start = positions[key];
                    float end = target.positions[key];
                    float delta = end - start;
                    fossa.setEminanceDistortion(start + delta * percent);
                }
            }
        }

        #region Saveable Members

        private const string POSITIONS = "Positions";

        protected FossaState(LoadInfo info)
        {
            info.RebuildDictionary<String, float>(POSITIONS, positions);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractDictionary<String, float>(POSITIONS, positions);
        }

        #endregion
    }
}
