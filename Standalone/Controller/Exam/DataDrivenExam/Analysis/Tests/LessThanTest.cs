using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.Exam
{
    class LessThanTest : TestAction
    {
        public LessThanTest(AnalysisAction successAction, AnalysisAction failureAction)
            : base(successAction, failureAction)
        {

        }

        protected override bool performTest(DataDrivenExam exam)
        {
            return Data.getData<decimal>(exam, DefaultDataValue) < TestValue;
        }

        public decimal TestValue { get; set; }

        public DataRetriever Data { get; set; }

        public decimal DefaultDataValue { get; set; }

        protected LessThanTest(LoadInfo info)
            :base(info)
        {
            TestValue = info.GetDecimal("TestValue");
            Data = info.GetValue<DataRetriever>("Data");
            DefaultDataValue = info.GetDecimal("DefaultDataValue");
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
