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

        internal SavedMedicalStates(List<MedicalState> states, String sceneFile)
        {
            this.states = states;
            this.SceneName = sceneFile;
        }

        public String SceneName { get; set; }

        #region Saveable Members

        private const string STATES = "States";
        private const string SCENE_NAME = "SceneName";

        protected SavedMedicalStates(LoadInfo info)
        {
            info.RebuildList<MedicalState>(STATES, states);
            if (info.hasValue(SCENE_NAME))
            {
                SceneName = info.GetString(SCENE_NAME);
            }
            else
            {
                SceneName = "Male.sim.xml";
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList<MedicalState>(STATES, states);
            info.AddValue(SCENE_NAME, SceneName);
        }

        internal List<MedicalState> getStates()
        {
            return states;
        }

        #endregion
    }
}
