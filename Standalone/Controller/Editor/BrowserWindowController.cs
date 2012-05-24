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
    public static class BrowserWindowController
    {
        public const String TimelineSearchPattern = "*.tl";
        public const String RmlSearchPattern = "*.rml";

        private static ResourceProvider resourceProvider;
        private static AnomalousMvcContext currentEditingContext;

        public static void setResourceProvider(ResourceProvider resourceProvider)
        {
            BrowserWindowController.resourceProvider = resourceProvider;
        }

        public static ResourceProvider getResourceProvider()
        {
            return resourceProvider;
        }

        public static void setCurrentEditingMvcContext(AnomalousMvcContext context)
        {
            currentEditingContext = context;
        }

        public static AnomalousMvcContext getCurrentEditingMvcContext()
        {
            return currentEditingContext;
        }

        public static Browser createFileBrowser(String searchPattern)
        {
            Browser browser = new Browser("Files");
            if (resourceProvider != null)
            {
                foreach (String timeline in resourceProvider.listFiles(searchPattern, "", true))
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

        public static Browser createModelBrowser(Type assignableFromType)
        {
            Browser browser = new Browser("Models");
            if (currentEditingContext != null)
            {
                foreach (MvcModel model in currentEditingContext.Models)
                {
                    if (assignableFromType.IsAssignableFrom(model.GetType()))
                    {
                        browser.addNode("", null, new BrowserNode(model.Name, model.Name));
                    }
                }
            }
            return browser;
        }
    }
}
