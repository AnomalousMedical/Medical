using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    interface PromptAnswerAction : Saveable
    {
        void execute(TimelineController timelineController);
    }
}
