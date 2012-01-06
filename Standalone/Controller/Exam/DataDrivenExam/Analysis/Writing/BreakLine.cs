using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.Exam
{
    class BreakLine : WritingAction
    {
        public override void execute(DataDrivenExam exam, DocumentBuilder docBuilder)
        {
            docBuilder.breakLine();
        }
    }
}
