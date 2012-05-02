using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;
using Medical.RmlTimeline.Actions;

namespace Medical
{
    class RmlTimelineGUIEventListener : EventListener
    {
        private String name;
        private RmlTimelineGUI rmlGui;

        public RmlTimelineGUIEventListener(String name, RmlTimelineGUI rmlGui)
        {
            this.name = name;
            this.rmlGui = rmlGui;
        }

        public override void ProcessEvent(Event evt)
        {
            rmlGui.runAction(name);
        }
    }
}
