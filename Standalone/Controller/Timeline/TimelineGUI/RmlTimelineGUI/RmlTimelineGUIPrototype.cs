using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class RmlTimelineGUIPrototype : TimelineGUIFactoryPrototype
    {
        public TimelineGUI getGUI()
        {
            return new RmlTimelineGUI();
        }

        public TimelineGUIData getGUIData()
        {
            return new RmlTimelineGUIData();
        }

        public string Name
        {
            get
            {
                return "RmlGui";
            }
        }
    }
}
