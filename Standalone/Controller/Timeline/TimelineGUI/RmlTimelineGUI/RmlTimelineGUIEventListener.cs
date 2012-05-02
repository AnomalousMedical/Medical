using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;

namespace Medical
{
    class RmlTimelineGUIEventListener : EventListener
    {
        private String name;

        public RmlTimelineGUIEventListener(String name)
        {
            this.name = name;
        }

        public override void ProcessEvent(Event evt)
        {
            Logging.Log.Debug(name);
        }
    }
}
