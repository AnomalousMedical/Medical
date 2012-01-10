using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.Exam
{
    public class EndParagraph : WritingAction
    {
        public EndParagraph()
        {

        }

        public override void execute(DataDrivenExam exam, DocumentBuilder docBuilder)
        {
            docBuilder.endParagraph();
        }

        protected EndParagraph(LoadInfo info)
            :base(info)
        {

        }
    }
}
