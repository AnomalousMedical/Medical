using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.Exam
{
    class StringEqualTest : TestAction
    {
        public StringEqualTest(AnalysisAction successAction, AnalysisAction failureAction)
            :base(successAction, failureAction)
        {

        }

        protected override bool performTest(DataDrivenExam exam)
        {
            return Data.getData<String>(exam, DefaultDataValue) == TestValue;
        }

        public String TestValue { get; set; }

        public DataRetriever Data { get; set; }

        public String DefaultDataValue { get; set; }
    }
}
