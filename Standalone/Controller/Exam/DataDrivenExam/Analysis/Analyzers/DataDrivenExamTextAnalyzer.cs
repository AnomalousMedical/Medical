using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.Exam;

namespace Medical
{
    class DataDrivenExamTextAnalyzer : ExamAnalyzer
    {
        private ActionBlock analysis;

        public DataDrivenExamTextAnalyzer(String name)
        {
            this.Name = name;

            analysis = new ActionBlock();
            analysis.addAction(new StartParagraph());
            Write sentence1 = new Write();
            sentence1.Text = "This is a test text block. It has some data in it here {0} and here {1}. In fact, quite a long string is written out as one paragraph here. That is useful I guess and you can keep writing till you need logic this way.";
            DataRetriever retriever = new DataRetriever();
            retriever.addExamSection("Menu Item");
            retriever.addExamSection("Numbers");
            retriever.DataPoint = "Number1";
            sentence1.addData(retriever);
            retriever = new DataRetriever();
            retriever.addExamSection("Menu Item");
            retriever.addExamSection("Numbers");
            retriever.DataPoint = "Number2";
            sentence1.addData(retriever);
            analysis.addAction(sentence1);
            analysis.addAction(new EndParagraph());
            analysis.addAction(new StartParagraph());
            analysis.addAction(new Write("If there were some notes they will be the next sentence. {0}", new DataRetriever("Notes", "Menu Item", "Notes")));
            analysis.addAction(new EndParagraph());
            analysis.addAction(new StartParagraph());
            analysis.addAction(new Write("This data field here '{0}' should be blank because we are making it fail on purpose.", new DataRetriever("Fail", "Fail")));
            analysis.addAction(new EndParagraph());
        }

        public void analyzeExam(Exam exam, AnalysisDisplayProvider display)
        {
            DataDrivenExam ddExam = (DataDrivenExam)exam;
            DocumentBuilder docBuilder = new DocumentBuilder();
            analysis.execute(ddExam, docBuilder);
            display.showTextData(ddExam.PrettyName, docBuilder.ToString());
        }

        public string Name { get; private set; }
    }
}
