using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Logging;

namespace Medical
{
    /// <summary>
    /// This class can create Browser objects from TimeilneController information.
    /// </summary>
    public class TimelineBrowserController
    {
        private static TimelineController timelineController;

        public static void setTimelineController(TimelineController timelineController)
        {
            TimelineBrowserController.timelineController = timelineController;
        }

        public static Browser createBrowser()
        {
            Browser browser = new Browser("Timelines");
            if (timelineController != null)
            {
                foreach (String timeline in timelineController.listResourceFiles("*.tl"))
                {
                    browser.addNode("", null, new BrowserNode(timeline, timeline));
                }
            }
            else
            {
                Log.Warning("No TimelineController registered with the TimelineBrowserController");
            }
            return browser;
        }

        private TimelineBrowserController()
        {

        }
    }
}
