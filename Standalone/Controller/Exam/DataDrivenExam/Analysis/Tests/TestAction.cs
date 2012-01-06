using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.Exam
{
    public abstract class TestAction : AnalysisAction
    {
        private AnalysisAction successAction;
        private AnalysisAction failureAction;

        public TestAction(AnalysisAction successAction, AnalysisAction failureAction)
        {
            this.successAction = successAction;
            this.failureAction = failureAction;
        }

        public void execute(DataDrivenExam exam, DocumentBuilder docBuilder)
        {
            if (performTest(exam))
            {
                successAction.execute(exam, docBuilder);
            }
            else
            {
                failureAction.execute(exam, docBuilder);
            }
        }

        protected abstract bool performTest(DataDrivenExam exam);
    }
}
