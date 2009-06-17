using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class DiscState
    {
        private Dictionary<String, Vector3> positions = new Dictionary<string, Vector3>();

        public void addPosition(String disc, Vector3 position)
        {
            positions.Add(disc, position);
        }

        public void blend(DiscState target, float percent)
        {
            foreach (String key in positions.Keys)
            {
                Vector3 start = positions[key];
                Vector3 end = target.positions[key];
                DiscController.getDisc(key).setOffset(start.lerp(ref end, ref percent));
            }
        }
    }
}
