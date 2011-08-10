using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public abstract class AbstractExamAnalyzer<ExamType> : ExamAnalyzer
        where ExamType : Exam
    {
        private String name;

        public AbstractExamAnalyzer(String name)
        {
            this.name = name;
        }

        public void analyzeExam(Exam exam, AnalysisDisplayProvider display)
        {
            doAnalysis((ExamType)exam, display);
        }

        protected abstract void doAnalysis(ExamType exam, AnalysisDisplayProvider display);

        public string Name
        {
            get
            {
                return name;
            }
        }
    }
}
