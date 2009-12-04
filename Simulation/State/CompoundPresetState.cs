using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class CompoundPresetState : PresetState
    {
        private List<PresetState> subStates = new List<PresetState>();

        public CompoundPresetState(String name, String category, String imageName)
            : base(name, category, imageName)
        {

        }

        public void addSubState(PresetState state)
        {
            subStates.Add(state);
        }

        public void removeSubState(PresetState state)
        {
            subStates.Remove(state);
        }

        public override void applyToState(MedicalState state)
        {
            foreach(PresetState subState in subStates)
            {
                subState.applyToState(state);
            }
        }

        #region Saveable

        private const String SUB_STATE = "SubState";

        protected CompoundPresetState(LoadInfo info)
            :base(info)
        {
            info.RebuildList<PresetState>(SUB_STATE, subStates);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractList<PresetState>(SUB_STATE, subStates);
        }

        #endregion Saveable
    }
}
