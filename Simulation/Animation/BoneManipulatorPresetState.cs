using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class BoneManipulatorPresetState : PresetState
    {
        private Dictionary<String, float> positions = new Dictionary<string, float>();

        public BoneManipulatorPresetState(String name, String category, String imageName)
            :base(name, category, imageName)
        {

        }

        public void addPosition(String manipulator, float position)
        {
            positions.Add(manipulator, position);
        }

        public override void applyToState(MedicalState state)
        {
            foreach (String position in positions.Keys)
            {
                state.BoneManipulator.addPosition(position, positions[position]);
            }
        }
    }
}
