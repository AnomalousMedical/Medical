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

        public DataDrivenExamTextAnalyzer(String name)
        {
            this.Name = name;

            ActionBlock analysis = new ActionBlock();
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

            ActionBlock ifStatementBlock = new ActionBlock();
            ifStatementBlock.addAction(new StartParagraph());
            ifStatementBlock.addAction(new Write("Number1 Had a value greater than 4"));
            ifStatementBlock.addAction(new EndParagraph());
            GreaterThanTest gtTest = new GreaterThanTest(ifStatementBlock, null);
            gtTest.Data = new DataRetriever("Number1", "Menu Item", "Numbers");
            gtTest.TestValue = 4;
            analysis.addAction(gtTest);

            this.analysis = analysis;
        }

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
