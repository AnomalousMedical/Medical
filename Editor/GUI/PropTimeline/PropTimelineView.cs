using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI
{
    class PropTimelineView : MyGUIView
    {
        public event Action<PropTimelineView, PropTimeline> ComponentCreated;

        public PropTimelineView(String name)
            : base(name)
        {
            IsWindow = true;
            ViewLocation = ViewLocations.Top;
        }

        internal void _fireComponentCreated(PropTimeline timeline)
        {
            if (ComponentCreated != null)
            {
                ComponentCreated.Invoke(this, timeline);
            }
        }

        public PropTimelineView(LoadInfo info)
            :base(info)
        {

        }
    }
}
