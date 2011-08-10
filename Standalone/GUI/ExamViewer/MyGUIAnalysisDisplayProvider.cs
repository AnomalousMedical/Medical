using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class MyGUIAnalysisDisplayProvider : AnalysisDisplayProvider
    {
        public void showRawData(Exam exam)
        {
            GenericExamOverview viewer = new GenericExamOverview(exam);
            viewer.open(false);
        }

        public void showTextData(String title, String text)
        {
            TextAnalysisViewer viewer = new TextAnalysisViewer(title, text);
            viewer.open(false);
        }
    }
}
