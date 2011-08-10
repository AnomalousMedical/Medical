using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// Displays the raw data for an exam.
    /// </summary>
    public class RawDataAnalyzer : ExamAnalyzer
    {
        private static RawDataAnalyzer instance;

        public static RawDataAnalyzer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RawDataAnalyzer();
                }
                return instance;
            }
        }

        private RawDataAnalyzer()
        {

        }

        public void analyzeExam(Exam exam, AnalysisDisplayProvider display)
        {
            display.showRawData(exam);
        }

        public string Name
        {
            get
            {
                return "View Raw Data";
            }
        }
    }
}
