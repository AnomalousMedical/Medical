using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.Exam
{
    public class StringNotEqualTest : TestAction
    {
        public StringNotEqualTest(AnalysisAction successAction, AnalysisAction failureAction)
            : base(successAction, failureAction)
        {

        }

        public StringNotEqualTest(AnalysisAction successAction, AnalysisAction failureAction, String testValue, DataRetriever data, String defaultValue)
            : this(successAction, failureAction)
        {
            this.TestValue = testValue;
            this.Data = data;
            this.DefaultDataValue = defaultValue;
        }

        protected override bool performTest(DataDrivenExam exam)
        {
            return Data.getData<String>(exam, DefaultDataValue) != TestValue;
        }

        public String TestValue { get; set; }

        public DataRetriever Data { get; set; }

        public String DefaultDataValue { get; set; }

        protected StringNotEqualTest(LoadInfo info)
            : base(info)
        {
            TestValue = info.GetString("TestValue");
            Data = info.GetValue<DataRetriever>("Data");
            DefaultDataValue = info.GetString("DefaultDataValue");
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("TestValue", TestValue);
            info.AddValue("Data", Data);
            info.AddValue("DefaultDataValue", DefaultDataValue);
        }
    }
}
