using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class FossaState
    {
        private Dictionary<String, float> positions = new Dictionary<string, float>();

        public void addPosition(String fossa, float position)
        {
            positions.Add(fossa, position);
        }

        public void blend(FossaState target, float percent)
        {
            foreach (String key in positions.Keys)
            {
                float start = positions[key];
                float end = target.positions[key];
                float delta = end - start;
                FossaController.get(key).setEminanceDistortion(start + delta * percent);
            }
        }
    }
}
