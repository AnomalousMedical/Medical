using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI
{
    public class PropTimelineFactory : ViewHostComponentFactory
    {
        private SaveableClipboard clipboard;

        public PropTimelineFactory(SaveableClipboard clipboard)
        {
            this.clipboard = clipboard;
        }

        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is PropTimelineView)
            {
                PropTimeline propTimeline = new PropTimeline(clipboard, ((PropTimelineView)view).PropEditController, viewHost);
                return propTimeline;
            }
            else if (view is OpenPropManagerView)
            {
                OpenPropManager openPropManager = new OpenPropManager(((OpenPropManagerView)view).PropEditController, viewHost);
                return openPropManager;
            }
            return null;
        }

        public void createViewBrowser(Engine.Editing.Browser browser)
        {
            
        }
    }
}
