using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.Exam;
using Engine.Saving;

namespace Medical
{
    public class DataDrivenExamTextAnalyzer : ExamAnalyzer, Saveable
    {
        private ActionBlock analysis;

        public DataDrivenExamTextAnalyzer(String name, ActionBlock analysis)
        {
            this.Name = name;
            this.analysis = analysis;
        }

        public void analyzeExam(Exam exam, AnalysisDisplayProvider display)
        {
            DataDrivenExam ddExam = (DataDrivenExam)exam;
            DocumentBuilder docBuilder = new DocumentBuilder();
            analysis.execute(ddExam, docBuilder);
            display.showTextData(ddExam.PrettyName, docBuilder.ToString());
        }

        public string Name { get; private set; }

        public ActionBlock Analysis
        {
            get
            {
                return analysis;
            }
        }

        protected DataDrivenExamTextAnalyzer(LoadInfo info)
        {
            Name = info.GetString("Name");
            analysis = info.GetValue<ActionBlock>("Analysis");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Name", Name);
            info.AddValue("Analysis", analysis);
        }
    }
}
