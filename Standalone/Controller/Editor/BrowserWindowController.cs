using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Logging;
using Medical.Controller.AnomalousMvc;

namespace Medical.Editor
{
    /// <summary>
    /// This class can create Browser objects from TimeilneController information.
    /// </summary>
    public class BrowserWindowController
    {
        public const String TimelineSearchPattern = "*.tl";
        public const String RmlSearchPattern = "*.rml";

        private static TimelineController timelineController;
        private static AnomalousMvcContext currentEditingContext;

        public static void setTimelineController(TimelineController timelineController)
        {
            BrowserWindowController.timelineController = timelineController;
        }

        public static void setCurrentEditingMvcContext(AnomalousMvcContext context)
        {
            currentEditingContext = context;
        }

        public static Browser createFileBrowser(String searchPattern)
        {
            Browser browser = new Browser("Files");
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

        public static Browser createViewBrowser()
        {
            Browser browser = new Browser("Views");
            if (currentEditingContext != null)
            {
                foreach (View view in currentEditingContext.Views)
                {
                    browser.addNode("", null, new BrowserNode(view.Name, view.Name));
                }
            }
            return browser;
        }

        public static Browser createActionBrowser()
        {
            Browser browser = new Browser("Action");
            if (currentEditingContext != null)
            {
                foreach (MvcController controller in currentEditingContext.Controllers)
                {
                    BrowserNode controllerNode = new BrowserNode(controller.Name, null);
                    foreach (ControllerAction action in controller.Actions)
                    {
                        controllerNode.addChild(new BrowserNode(action.Name, String.Format("{0}/{1}", controller.Name, action.Name)));
                    }
                    browser.addNode("", null, controllerNode);
                }
            }
            return browser;
        }

        private BrowserWindowController()
        {

        }
    }
}
