using Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecture
{
    class TimelineTypeController : SaveableTypeController<Timeline>
    {
        public TimelineTypeController(EditorController editorController)
            :base(".tl", editorController)
        {

        }

        public void createNewTimeline(String filePath)
        {
            Timeline timeline = new Timeline();
            creatingNewFile(filePath);
            saveObject(filePath, timeline);
        }
    }
}
