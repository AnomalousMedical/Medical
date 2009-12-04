using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class TeethPresetState : PresetState
    {
        private List<ToothState> teeth = new List<ToothState>();

        public TeethPresetState(String name, String category, String imageName)
            :base(name, category, imageName)
        {

        }

        public void captureFromState(TeethState teethState)
        {
            foreach (ToothState tooth in teethState.StateEnum)
            {
                teeth.Add(new ToothState(tooth));
            }
        }

        public override void applyToState(MedicalState state)
        {
            foreach (ToothState tooth in teeth)
            {
                state.Teeth.addPosition(new ToothState(tooth));
            }
        }

        #region Saveable Members

        private const string TEETH = "Teeth";

        protected TeethPresetState(LoadInfo info)
            :base(info)
        {
            info.RebuildList<ToothState>(TEETH, teeth);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractList<ToothState>(TEETH, teeth);
        }

        #endregion
    }
}
