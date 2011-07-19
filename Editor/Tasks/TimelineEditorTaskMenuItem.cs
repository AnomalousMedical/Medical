using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class TimelineEditorTask : Task
    {
        private TimelinePropertiesController timelinePropertiesController;

        public TimelineEditorTask(TimelinePropertiesController timelinePropertiesController)
            :base("Medical.TimelineEditor", "Timeline", "TimelineEditorIcon", TaskMenuCategories.Editor)
        {
            this.timelinePropertiesController = timelinePropertiesController;
            timelinePropertiesController.Closed += new EventHandler(timelinePropertiesController_Closed);
            timelinePropertiesController.VisibilityChanged += new Engine.EventDelegate<TimelinePropertiesController>(timelinePropertiesController_VisibilityChanged);
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

        void timelinePropertiesController_VisibilityChanged(TimelinePropertiesController source)
        {
            if (source.Visible && !OnTaskbar)
            {
                fireRequestShowInTaskbar();
            }
        }

        void timelinePropertiesController_Closed(object sender, EventArgs e)
        {
            fireItemClosed();
        }
    }
}
