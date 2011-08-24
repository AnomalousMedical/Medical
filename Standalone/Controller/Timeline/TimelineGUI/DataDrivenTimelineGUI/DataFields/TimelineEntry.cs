using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical
{
    public partial class TimelineEntry : Saveable
    {
        public TimelineEntry()
        {

        }

        public TimelineEntry(String timeline)
        {
            this.Timeline = timeline;
        }

        public String Timeline { get; set; }

        protected TimelineEntry(LoadInfo info)
        {
            Timeline = info.GetString("Timeline", Timeline);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Timeline", Timeline);
        }
    }

    partial class TimelineEntry : EditableProperty
    {
        public bool canParseString(int column, string value, out string errorMessage)
        {
            errorMessage = "";
            return true;
        }

        public Type getPropertyType(int column)
        {
            return typeof(String);
        }

        public string getValue(int column)
        {
            return Timeline;
        }

        public void setValueStr(int column, string value)
        {
            Timeline = value;
        }
    }
}
