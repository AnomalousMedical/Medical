using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.Exam
{
    public abstract class WritingAction : AnalysisAction
    {
        public abstract void execute(DataDrivenExam exam, DocumentBuilder docBuilder);
    }
}
