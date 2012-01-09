using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.Exam
{
    class LessThanEqualTest : TestAction
    {
        public LessThanEqualTest(AnalysisAction successAction, AnalysisAction failureAction)
            : base(successAction, failureAction)
        {

        }

        protected override bool performTest(DataDrivenExam exam)
        {
            return Data.getData<decimal>(exam, DefaultDataValue) <= TestValue;
        }

        public decimal TestValue { get; set; }

        public DataRetriever Data { get; set; }

        public decimal DefaultDataValue { get; set; }
    }
}
