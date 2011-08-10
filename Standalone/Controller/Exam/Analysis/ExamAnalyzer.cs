using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public interface ExamAnalyzer
    {
        void analyzeExam(Exam exam, AnalysisDisplayProvider display);

        String Name { get; }
    }
}
