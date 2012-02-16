using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class AnimationManipulatorPresetState : PresetState
    {
        private List<AnimationManipulatorStateEntry> positions = new List<AnimationManipulatorStateEntry>();

        public AnimationManipulatorPresetState(String name, String category, String imageName)
            :base(name, category, imageName)
        {

        }

        public void addPosition(AnimationManipulatorStateEntry entry)
        {
            positions.Add(entry);
        }

        public void captureFromState(AnimationManipulatorStateEntry state)
        {
            addPosition(state.clone());
        }

        public void captureFromState(AnimationManipulatorState animManipulator)
        {
            foreach (AnimationManipulatorStateEntry entry in animManipulator.Entries)
            {
                captureFromState(entry);
            }
        }

        public override void applyToState(MedicalState state)
        {
            foreach (AnimationManipulatorStateEntry position in positions)
            {
                state.BoneManipulator.addPosition(position.clone());
            }
        }

        /// <summary>
        /// Change the side of this manipulator. Assists in easy copying of data from one side to the other.
        /// </summary>
        /// <param name="oldName">The old base name.</param>
        /// <param name="newName">The new base name.</param>
        public void changeSide(String oldName, String newName)
        {
            foreach (AnimationManipulatorStateEntry entry in positions)
            {
                entry.changeSide(oldName, newName);
            }
        }

        #region Saveable Members

        private const String POSITION_BASE = "Position";

        protected AnimationManipulatorPresetState(LoadInfo info)
            :base(info)
        {
            info.RebuildList<AnimationManipulatorStateEntry>(POSITION_BASE, positions);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractList<AnimationManipulatorStateEntry>(POSITION_BASE, positions);
        }

        #endregion
    }
}
