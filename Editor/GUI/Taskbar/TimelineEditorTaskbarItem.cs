using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class TimelineEditorTaskbarItem : TaskbarItem
    {
        private TimelinePropertiesController timelinePropertiesController;

        public TimelineEditorTaskbarItem(TimelinePropertiesController timelinePropertiesController)
            :base("Timeline", "TimelineEditorIcon")
        {
            this.timelinePropertiesController = timelinePropertiesController;
        }

        public override void clicked(Widget source, EventArgs e)
        {
            this.taskbarButton.StateCheck = timelinePropertiesController.Visible = !timelinePropertiesController.Visible;
        }
    }
}
