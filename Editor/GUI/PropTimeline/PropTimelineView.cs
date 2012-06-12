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
        public PropTimelineView(String name)
            : base(name)
        {
            IsWindow = true;
            ViewLocation = ViewLocations.Top;
        }

        public PropTimelineView(LoadInfo info)
            :base(info)
        {

        }
    }
}
