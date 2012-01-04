using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class DataDrivenExamTextAnalyzer : ExamAnalyzer
    {
        public DataDrivenExamTextAnalyzer(String name)
        {
            this.Name = name;
        }

        public void analyzeExam(Exam exam, AnalysisDisplayProvider display)
        {
            DataDrivenExam ddExam = (DataDrivenExam)exam;

            DocumentBuilder docBuilder = new DocumentBuilder();
            docBuilder.startParagraph();
            docBuilder.writeSentence("Hello, this is a test paragraph in the doc builder.");
            docBuilder.writeSentence("If the doc builder works correctly this will be nicely formatted.");
            docBuilder.writeSentence("I hope it does cause that is less work I will have to do.");
            docBuilder.endParagraph();
            docBuilder.startParagraph();
            docBuilder.writeSentence("This is a second paragraph.");
            docBuilder.writeSentence("It is equally awesome.");
            docBuilder.endParagraph();

            display.showTextData(ddExam.PrettyName, docBuilder.ToString());
        }

        public string Name { get; private set; }
    }
}
