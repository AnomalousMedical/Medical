using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

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

        #region Saveable Members

        private const String POSITION_BASE = "Position";

        protected BoneManipulatorPresetState(LoadInfo info)
            :base(info)
        {
            info.RebuildDictionary<String, float>(POSITION_BASE, positions);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractDictionary<String, float>(POSITION_BASE, positions);
        }

        #endregion
    }
}
