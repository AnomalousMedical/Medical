using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class BoneManipulatorState
    {
        private Dictionary<String, float> positions = new Dictionary<string, float>();

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
                BoneManipulatorController.getManipulator(key).setPosition(start + delta * percent);
            }
        }
    }
}
