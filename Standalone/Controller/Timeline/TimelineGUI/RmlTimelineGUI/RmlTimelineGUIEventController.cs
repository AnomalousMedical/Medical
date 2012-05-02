using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using libRocketPlugin;
using Medical.RmlTimeline.Actions;

namespace Medical
{
    class RmlTimelineGUIEventController : RocketEventController
    {
        private RmlTimelineGUI rmlGui;

        public RmlTimelineGUIEventController(RmlTimelineGUI rmlGui)
        {
            this.rmlGui = rmlGui;
        }

        public EventListener createEventListener(string name)
        {
            return new RmlTimelineGUIEventListener(name, rmlGui);
        }
    }
}
