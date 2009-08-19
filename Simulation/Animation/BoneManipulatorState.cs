using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class BoneManipulatorState : Saveable
    {
        private Dictionary<String, float> positions = new Dictionary<string, float>();

        internal BoneManipulatorState()
        {

        }

        public void addPosition(String manipulator, float position)
        {
            positions.Add(manipulator, position);
        }

        public void blend(BoneManipulatorState target, float percent)
        {
            foreach (String key in positions.Keys)
            {
                float start = positions[key];
                float end = target.positions[key];
                float delta = end - start;
                BoneManipulatorController.getManipulator(key).Position = start + delta * percent;
            }
        }

        #region Saveable Members

        private const string POSITIONS = "Positions";

        protected BoneManipulatorState(LoadInfo info)
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
