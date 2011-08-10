using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class ExamAnalyzerCollection
    {
        private List<ExamAnalyzer> analyzers = new List<ExamAnalyzer>();

        public ExamAnalyzerCollection()
        {

        }

        public void addAnalyzer(ExamAnalyzer analyzer)
        {
            analyzers.Add(analyzer);
        }

        public void removeAnalyzer(ExamAnalyzer analyzer)
        {
            analyzers.Remove(analyzer);
        }

        public IEnumerable<ExamAnalyzer> Analyzers
        {
            get
            {
                return analyzers.AsReadOnly();
            }
        }
    }
}
