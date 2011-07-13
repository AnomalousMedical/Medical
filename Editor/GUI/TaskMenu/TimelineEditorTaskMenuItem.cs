using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class TimelineEditorTaskMenuItem : TaskMenuItem
    {
        private TimelinePropertiesController timelinePropertiesController;

        public TimelineEditorTaskMenuItem(TimelinePropertiesController timelinePropertiesController)
            :base("Timeline", "TimelineEditorIcon", TaskMenuCategories.Editor)
        {
            this.timelinePropertiesController = timelinePropertiesController;
        }

        public override void clicked()
        {
            timelinePropertiesController.Visible = !timelinePropertiesController.Visible;
        }
    }
}
