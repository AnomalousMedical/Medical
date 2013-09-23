using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    class TimelineActionPrototype
    {
        public TimelineActionPrototype(String typeName, Type type)
        {
            this.TypeName = typeName;
            this.Type = type;
        }

        public virtual TimelineActionData createData(TimelineAction action)
        {
            return new TimelineActionData(action);
        }

        internal TimelineAction createInstance()
        {
            return (TimelineAction)Activator.CreateInstance(Type);
        }

        public String TypeName { get; private set; }

        public Type Type { get; set; }
    }
}
