using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;

namespace Medical
{
    public class DataDrivenExam : DataDrivenExamSection, Exam
    {
        private static ExamAnalyzerCollection analyzers;

        static DataDrivenExam()
        {
            analyzers = new ExamAnalyzerCollection();
            analyzers.addAnalyzer(RawDataAnalyzer.Instance);
        }

        [DoNotSave]
        private DataDrivenExam previousExam;

        [DoNotSave]
        private bool savePreviousExams = true; //This can be turned off when the exam is being copied to save pointlessly copying the previous editions with the copy saver.

        public DataDrivenExam(String prettyName)
            :base(prettyName)
        {
            
        }

        public ExamAnalyzerCollection Analyzers
        {
            get
            {
                return analyzers;
            }
        }

        public DateTime Date { get; set; }

        public Exam PreviousExam { get; set; }

        /// <summary>
        /// DO NOT TOUCH unless you are the DataDrivenExamController.
        /// </summary>
        internal bool _SavePreviousExams
        {
            get
            {
                return savePreviousExams;
            }
            set
            {
                savePreviousExams = value;
            }
        }

        protected DataDrivenExam(LoadInfo info)
            :base(info)
        {
            previousExam = info.GetValue<DataDrivenExam>("ExamReserved_Previous", previousExam);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            if (savePreviousExams)
            {
                info.AddValue("ExamReserved_Previous", previousExam);
            }
        }
    }
}
