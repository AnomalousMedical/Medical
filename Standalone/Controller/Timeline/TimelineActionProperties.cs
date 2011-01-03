using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    /// <summary>
    /// This class is used to identify a GUI to a timeline editing type. It
    /// should be initialzed with the same type name specified in the
    /// ActionProperty this provides a type for.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class TimelineActionProperties : Attribute
    {
        private String typeName;

        public TimelineActionProperties(String typeName)
        {
            this.typeName = typeName;
        }

        public String TypeName
        {
            get
            {
                return typeName;
            }
        }
    }
}
