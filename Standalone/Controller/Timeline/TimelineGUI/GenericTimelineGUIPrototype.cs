using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class GenericTimelineGUIFactoryPrototype<GUIType, GUIDataType> : TimelineGUIFactoryPrototype
        where GUIType : TimelineGUI, new()
        where GUIDataType : TimelineGUIData, new()
    {
        public GenericTimelineGUIFactoryPrototype(String name)
        {
            this.Name = name;
        }

        public TimelineGUI getGUI()
        {
            return new GUIType();
        }

        public TimelineGUIData getGUIData()
        {
            return new GUIDataType();
        }

        public string Name { get; private set; }
    }
}
