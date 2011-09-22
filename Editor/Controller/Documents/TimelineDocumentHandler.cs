using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;

namespace Medical
{
    class TimelineDocumentHandler : DocumentHandler
    {
        private TimelinePropertiesController timelinePropertiesController;

        public TimelineDocumentHandler(TimelinePropertiesController timelinePropertiesController)
        {
            this.timelinePropertiesController = timelinePropertiesController;
        }

        public bool canReadFile(string filename)
        {
            return filename.EndsWith(".tl") || filename.EndsWith(".tlp");
        }

        public bool processFile(string filename)
        {
            timelinePropertiesController.openProject(filename);
            timelinePropertiesController.Visible = true;
            return true;
        }

        public string getPrettyName(string filename)
        {
            if (filename.EndsWith(".tl"))
            {
                return "Timeline";
            }
            return "Timeline Project";
        }

        public string getIcon(string filename)
        {
            return "TimelineEditorIcon";
        }
    }
}
