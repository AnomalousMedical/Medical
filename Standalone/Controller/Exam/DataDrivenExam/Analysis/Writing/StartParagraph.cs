using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.Exam
{
    public class StartParagraph : WritingAction
    {
        public StartParagraph()
        {

        }

        public override void execute(DataDrivenExam exam, DocumentBuilder docBuilder)
        {
            docBuilder.startParagraph();
        }

        protected StartParagraph(LoadInfo info)
            :base(info)
        {

        }
    }
}
