using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.Exam
{
    public abstract class TestAction : AnalysisAction
    {
        public TestAction(AnalysisAction successAction, AnalysisAction failureAction)
        {
            this.SuccessAction = successAction;
            this.FailureAction = failureAction;
        }

        public void execute(DataDrivenExam exam, DocumentBuilder docBuilder)
        {
            if (performTest(exam))
            {
                if (SuccessAction != null)
                {
                    SuccessAction.execute(exam, docBuilder);
                }
            }
            else if(FailureAction != null)
            {
                FailureAction.execute(exam, docBuilder);
            }
        }

        protected abstract bool performTest(DataDrivenExam exam);

        public AnalysisAction SuccessAction { get; set; }

        public AnalysisAction FailureAction { get; set; }

        protected TestAction(LoadInfo info)
        {
            SuccessAction = info.GetValue<AnalysisAction>("SuccessAction");
            FailureAction = info.GetValue<AnalysisAction>("FailureAction");
        }

        public virtual void getInfo(SaveInfo info)
        {
            info.AddValue("SuccessAction", SuccessAction);
            info.AddValue("FailureAction", FailureAction);
        }
    }
}
