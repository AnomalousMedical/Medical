using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class SavedMedicalStates : Saveable
    {
        private List<MedicalState> states = new List<MedicalState>();

        internal SavedMedicalStates(List<MedicalState> states)
        {
            this.states = states;
        }

        #region Saveable Members

        private const string STATES = "States";

        protected SavedMedicalStates(LoadInfo info)
        {
            info.RebuildList<MedicalState>(STATES, states);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList<MedicalState>(STATES, states);
        }

        internal List<MedicalState> getStates()
        {
            return states;
        }

        #endregion
    }
}
