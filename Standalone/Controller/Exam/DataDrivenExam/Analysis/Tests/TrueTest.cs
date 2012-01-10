using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

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

        protected TrueTest(LoadInfo info)
            :base(info)
        {
            Data = info.GetValue<DataRetriever>("Data");
            DefaultDataValue = info.GetBoolean("DefaultDataValue");
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.AddValue("Data", Data);
            info.AddValue("DefaultDataValue", DefaultDataValue);
        }
    }
}
