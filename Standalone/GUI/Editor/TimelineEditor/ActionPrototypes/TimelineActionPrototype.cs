using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    class TimelineActionPrototype
    {
        public TimelineActionPrototype(String typeName, Type type, Color normalColor, Color selectedColor)
        {
            this.NormalColor = normalColor;
            this.SelectedColor = selectedColor;
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

        public Color NormalColor { get; private set; }

        public Color SelectedColor { get; set; }

        public String TypeName { get; private set; }

        public Type Type { get; set; }
    }
}
