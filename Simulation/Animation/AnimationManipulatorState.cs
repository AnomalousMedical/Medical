using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class AnimationManipulatorState : Saveable
    {
        private Dictionary<String, AnimationManipulatorStateEntry> positions = new Dictionary<string, AnimationManipulatorStateEntry>();

        internal AnimationManipulatorState()
        {

        }

        public void addPosition(AnimationManipulatorStateEntry entry)
        {
            if (positions.ContainsKey(entry.Name))
            {
                positions[entry.Name] = entry;
            }
            else
            {
                positions.Add(entry.Name, entry);
            }
        }

        public void blend(AnimationManipulatorState target, float percent)
        {
            foreach (String key in positions.Keys)
            {
                positions[key].blend(target.positions[key], percent);
            }
        }

        public AnimationManipulatorStateEntry getEntry(String name)
        {
            AnimationManipulatorStateEntry ret;
            positions.TryGetValue(name, out ret);
            return ret;
        }

        #region Saveable Members

        private const string POSITIONS = "Positions";

        protected AnimationManipulatorState(LoadInfo info)
        {
            info.RebuildDictionary<String, AnimationManipulatorStateEntry>(POSITIONS, positions);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractDictionary<String, AnimationManipulatorStateEntry>(POSITIONS, positions);
        }

        #endregion

        public IEnumerable<AnimationManipulatorStateEntry> Entries
        {
            get
            {
                return positions.Values;
            }
        }
    }
}
