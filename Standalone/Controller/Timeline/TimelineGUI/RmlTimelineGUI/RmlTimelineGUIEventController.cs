using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using libRocketPlugin;

namespace Medical
{
    class RmlTimelineGUIEventController : RocketEventController
    {
        public EventListener createEventListener(string name)
        {
            return new RmlTimelineGUIEventListener(name);
        }
    }
}
