using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.Exam
{
    class BreakLine : WritingAction
    {
        public BreakLine()
        {

        }

        public override void execute(DataDrivenExam exam, DocumentBuilder docBuilder)
        {
            docBuilder.breakLine();
        }

        protected BreakLine(LoadInfo info)
            :base(info)
        {

        }
    }
}
