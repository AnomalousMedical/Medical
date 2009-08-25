using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class BoneManipulatorState : Saveable
    {
        private Dictionary<String, BoneManipulatorStateEntry> positions = new Dictionary<string, BoneManipulatorStateEntry>();

        internal BoneManipulatorState()
        {

        }

        public void addPosition(BoneManipulatorStateEntry entry)
        {
            positions.Add(entry.Name, entry);
        }

        public void blend(BoneManipulatorState target, float percent)
        {
            foreach (String key in positions.Keys)
            {
                positions[key].blend(target.positions[key], percent);
            }
        }

        #region Saveable Members

        private const string POSITIONS = "Positions";

        protected BoneManipulatorState(LoadInfo info)
        {
            info.RebuildDictionary<String, BoneManipulatorStateEntry>(POSITIONS, positions);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractDictionary<String, BoneManipulatorStateEntry>(POSITIONS, positions);
        }

        #endregion
    }
}
