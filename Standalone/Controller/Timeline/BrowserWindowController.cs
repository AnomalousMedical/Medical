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
    public class BrowserWindowController
    {
        public const String TimelineSearchPattern = "*.tl";
        public const String RmlSearchPattern = "*.rml";

        private static TimelineController timelineController;

        public static void setTimelineController(TimelineController timelineController)
        {
            BrowserWindowController.timelineController = timelineController;
        }

        public static Browser createBrowser(String searchPattern)
        {
            Browser browser = new Browser("Timelines");
            if (timelineController != null)
            {
                foreach (String timeline in timelineController.listResourceFiles(searchPattern))
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

        private BrowserWindowController()
        {

        }
    }
}
