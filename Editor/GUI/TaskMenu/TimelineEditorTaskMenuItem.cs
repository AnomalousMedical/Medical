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
            timelinePropertiesController.Closed += new EventHandler(timelinePropertiesController_Closed);
        }

        public override void clicked()
        {
            if (timelinePropertiesController.Visible)
            {
                timelinePropertiesController.Visible = false;
                fireItemClosed();
            }
            else
            {
                timelinePropertiesController.Visible = true;
            }
            ShowOnTaskbar = timelinePropertiesController.Visible;
        }

        void timelinePropertiesController_Closed(object sender, EventArgs e)
        {
            fireItemClosed();
        }
    }
}
