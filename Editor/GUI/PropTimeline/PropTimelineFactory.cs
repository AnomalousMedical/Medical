using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI
{
    class PropTimelineFactory : ViewHostComponentFactory
    {
        private SaveableClipboard clipboard;
        private PropEditController propEditController;

        public PropTimelineFactory(SaveableClipboard clipboard, PropEditController propEditController)
        {
            this.clipboard = clipboard;
            this.propEditController = propEditController;
        }

        public ViewHostComponent createViewHostComponent(MyGUIView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
        {
            if (view is PropTimelineView)
            {
                return new PropTimeline(clipboard, propEditController, viewHost);
            }
            return null;
        }

        public void createViewBrowser(Engine.Editing.Browser browser)
        {
            
        }
    }
}
