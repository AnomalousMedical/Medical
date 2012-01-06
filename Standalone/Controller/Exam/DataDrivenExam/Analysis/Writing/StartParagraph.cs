using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.Exam
{
    public class StartParagraph : WritingAction
    {
        public override void execute(DataDrivenExam exam, DocumentBuilder docBuilder)
        {
            docBuilder.startParagraph();
        }
    }
}
