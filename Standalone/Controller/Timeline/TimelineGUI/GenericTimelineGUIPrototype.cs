using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class GenericTimelineGUIFactoryPrototype<PrototypeType> : TimelineGUIFactoryPrototype
        where PrototypeType : TimelineGUI, new()
    {
        public GenericTimelineGUIFactoryPrototype(String name)
        {
            this.Name = name;
        }

        public TimelineGUI getGUI()
        {
            return new PrototypeType();
        }

        public string Name { get; private set; }
    }
}
