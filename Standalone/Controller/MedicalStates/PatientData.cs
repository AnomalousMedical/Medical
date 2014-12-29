using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class PatientData : Saveable
    {
        private SavedMedicalStates savedMedicalStates;

        public PatientData()
        {

        }

        public SavedMedicalStates MedicalStates
        {
            get
            {
                return savedMedicalStates;
            }
            set
            {
                savedMedicalStates = value;
            }
        }

        #region Saveable Members

        protected PatientData(LoadInfo info)
        {
            savedMedicalStates = info.GetValue<SavedMedicalStates>("MedicalStates");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("MedicalStates", savedMedicalStates);
        }

        #endregion
    }
}
