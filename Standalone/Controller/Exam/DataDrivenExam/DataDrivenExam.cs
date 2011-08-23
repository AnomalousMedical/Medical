using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    public class DataDrivenExam : Exam
    {
        private String prettyName;
        private EditInterface editInterface;
        private ExamAnalyzerCollection analyzers;
        private Dictionary<String, Saveable> values;

        public DataDrivenExam(String prettyName)
        {
            this.prettyName = prettyName;
            analyzers = new ExamAnalyzerCollection();
            values = new Dictionary<String, Saveable>();
        }

        public ExamAnalyzerCollection Analyzers
        {
            get
            {
                return analyzers;
            }
        }

        public DateTime Date { get; set; }

        public string PrettyName
        {
            get
            {
                return prettyName;
            }
        }

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = new EditInterface(prettyName);
                }
                return editInterface;
            }
        }

        public Exam PreviousExam { get; set; }

        protected DataDrivenExam(LoadInfo info)
        {
            prettyName = info.GetValue("PrettyName", prettyName);
            info.RebuildDictionary<String, Saveable>("ExamValue", values);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("PrettyName", prettyName);
            info.ExtractDictionary<String, Saveable>("ExamValue", values);
        }
    }
}
