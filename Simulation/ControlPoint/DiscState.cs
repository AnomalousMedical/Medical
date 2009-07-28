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
        private Dictionary<String, Vector3> positions = new Dictionary<string, Vector3>();

        internal DiscState()
        {

        }

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

        #region Saveable Members

        private const string POSITIONS = "Positions";

        protected DiscState(LoadInfo info)
        {
            info.RebuildDictionary<String, Vector3>(POSITIONS, positions);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractDictionary<String, Vector3>(POSITIONS, positions);
        }

        #endregion
    }
}
