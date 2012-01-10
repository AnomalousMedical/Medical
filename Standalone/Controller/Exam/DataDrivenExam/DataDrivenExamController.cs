using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class DataDrivenExamController
    {
        private static DataDrivenExamController instance;

        public static DataDrivenExamController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataDrivenExamController();
                }
                return instance;
            }
        }

        DataDrivenExam currentExam;
        DataDrivenExam previousExamVersion;
        CopySaver copySaver = new CopySaver();
        private Stack<DataDrivenExamSection> sectionStack = new Stack<DataDrivenExamSection>();

        public DataDrivenExam createOrRetrieveExam(String name)
        {
            if (currentExam == null)
            {
                currentExam = new DataDrivenExam(name);
                currentExam.Date = DateTime.Now;
            }
            return currentExam;
        }

        public void pushCurrentSection(DataDrivenExamSection section)
        {
            sectionStack.Push(section);
        }

        public DataDrivenExamSection popCurrentSection()
        {
            return sectionStack.Pop();
        }

        public DataDrivenExamSection CurrentSection
        {
            get
            {
                return sectionStack.Peek();
            }
        }

        public bool HasCurrentSection
        {
            get
            {
                return sectionStack.Count > 0;
            }
        }

        /// <summary>
        /// Open the given exam for review. Will create a copy of the exam. Upon calling saveAndClear this copy will replace the original.
        /// </summary>
        /// <param name="exam">The exam to review.</param>
        public void openForReview(DataDrivenExam exam)
        {
            previousExamVersion = exam;
            exam._SavePreviousExams = false;
            currentExam = copySaver.copy<DataDrivenExam>(exam);
            exam._SavePreviousExams = true;
            currentExam.Date = DateTime.Now;
        }

        /// <summary>
        /// Save this exam and clear the current exam.
        /// </summary>
        public void saveAndClear()
        {
            if (previousExamVersion != null)
            {
                ExamController.Instance.replaceExam(previousExamVersion, currentExam);
                currentExam.PreviousExam = previousExamVersion;
            }
            else
            {
                ExamController.Instance.addExam(currentExam);
            }
            clear();
        }

        /// <summary>
        /// Clear the current exam without saving it.
        /// </summary>
        public void clear()
        {
            currentExam = null;
            previousExamVersion = null;
            sectionStack.Clear();
        }

        public DataDrivenExamTextAnalyzer TEMP_InjectedExamAnalyzer { get; set; }
    }
}
