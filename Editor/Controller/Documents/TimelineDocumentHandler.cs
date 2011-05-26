using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;

namespace Medical
{
    class TimelineDocumentHandler : DocumentHandler
    {
        private TimelineProperties timelineProperties;

        public TimelineDocumentHandler(TimelineProperties timelineProperties)
        {
            this.timelineProperties = timelineProperties;
        }

        public bool canReadFile(string filename)
        {
            return filename.EndsWith(".tix") || filename.EndsWith(".tl") || filename.EndsWith(".tlp");
        }

        public bool processFile(string filename)
        {
            timelineProperties.openProject(filename);
            return true;
        }
    }
}
