using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.Exam
{
    class TrueTest : TestAction
    {
        public TrueTest(AnalysisAction successAction, AnalysisAction failureAction)
            : base(successAction, failureAction)
        {

        }

        protected override bool performTest(DataDrivenExam exam)
        {
            return Data.getData<bool>(exam, DefaultDataValue);
        }

        public DataRetriever Data { get; set; }

        public bool DefaultDataValue { get; set; }
    }
}
