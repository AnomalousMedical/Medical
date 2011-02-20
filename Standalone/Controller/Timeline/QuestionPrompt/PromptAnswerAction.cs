using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public interface PromptAnswerAction : Saveable
    {
        void execute(TimelineController timelineController);

        void dumpToLog();

        void findFileReference(TimelineStaticInfo info, String answerText);
    }
}
