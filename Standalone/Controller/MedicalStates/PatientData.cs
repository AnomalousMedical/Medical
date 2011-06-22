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
        private List<Exam> exams = new List<Exam>();

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

        public void addExam(Exam exam)
        {
            exams.Add(exam);
        }

        public void removeExam(Exam exam)
        {
            exams.Remove(exam);
        }

        public IEnumerable<Exam> Exams
        {
            get
            {
                return exams;
            }
        }

        #region Saveable Members

        protected PatientData(LoadInfo info)
        {
            savedMedicalStates = info.GetValue<SavedMedicalStates>("MedicalStates");
            info.RebuildList<Exam>("Exam", exams);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("MedicalStates", savedMedicalStates);
            info.ExtractList<Exam>("Exam", exams);
        }

        #endregion
    }
}
