using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This class provides a way for exams in plugins to talk back to the main system.
    /// </summary>
    public class ExamController
    {
        public delegate void ExamControllerEvent(ExamController examController, Exam exam);

        private static ExamController instance;

        public event ExamControllerEvent ExamAdded;
        public event ExamControllerEvent ExamRemoved;
        public event ExamControllerEvent ExamsCleared;

        internal static ExamController Instance
        {
            get
            {
                return instance;
            }
        }

        private List<Exam> exams = new List<Exam>();

        public ExamController()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                throw new Exception("Only create one instance of the ExamController");
            }
        }

        public void addExam(Exam exam)
        {
            exams.Add(exam);
            if (ExamAdded != null)
            {
                ExamAdded.Invoke(this, exam);
            }
        }

        public void removeExam(Exam exam)
        {
            int index = exams.IndexOf(exam);
            if (index >= 0)
            {
                exams.RemoveAt(index);
                if (ExamRemoved != null)
                {
                    ExamRemoved.Invoke(this, exam);
                }
            }
        }

        public void clear()
        {
            exams.Clear();
            if (ExamsCleared != null)
            {
                ExamsCleared.Invoke(this, null);
            }
        }

        public void addExamsToData(PatientData patientData)
        {
            foreach (Exam exam in exams)
            {
                patientData.addExam(exam);
            }
        }

        public void setExamsFromData(PatientData patientData)
        {
            clear();
            foreach (Exam exam in patientData.Exams)
            {
                addExam(exam);
            }
        }

        public int Count
        {
            get
            {
                return exams.Count;
            }
        }

        public IEnumerable<Exam> Exams
        {
            get
            {
                return exams;
            }
        }
    }
}
