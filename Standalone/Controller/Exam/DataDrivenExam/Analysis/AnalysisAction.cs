using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.Exam
{
    public interface AnalysisAction : Saveable
    {
        void execute(DataDrivenExam exam, DocumentBuilder docBuilder);
    }
}
