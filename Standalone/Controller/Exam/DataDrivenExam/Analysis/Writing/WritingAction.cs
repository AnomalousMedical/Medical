using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.Exam
{
    public abstract class WritingAction : AnalysisAction
    {
        public WritingAction()
        {

        }

        public abstract void execute(DataDrivenExam exam, DocumentBuilder docBuilder);

        protected WritingAction(LoadInfo info)
        {

        }

        public virtual void getInfo(Engine.Saving.SaveInfo info)
        {
            
        }
    }
}
