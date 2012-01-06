using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.Exam
{
    public interface AnalysisAction
    {
        void execute(DataDrivenExam exam, DocumentBuilder docBuilder);
    }
}
