using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class TimelineActionProperties : Attribute
    {
        private Color color;
        private String typeName;

        public TimelineActionProperties(String typeName, float r, float g, float b)
        {
            this.typeName = typeName;
            this.color = new Color(r, g, b);
        }

        public Color Color
        {
            get
            {
                return color;
            }
        }

        public String TypeName
        {
            get
            {
                return typeName;
            }
        }

        public Type GUIType { get; set; }
    }
}
