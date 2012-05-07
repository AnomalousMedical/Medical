using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using Medical.Editor;

namespace Medical
{
    public partial class TimelineEntry : Saveable
    {
        public TimelineEntry()
        {
            
        }

        public TimelineEntry(String name, String timeline, String imageKey)
        {
            this.Timeline = timeline;
        }

        public String Timeline { get; set; }

        public String Name { get; set; }

        public String ImageKey { get; set; }

        protected TimelineEntry(LoadInfo info)
        {
            Timeline = info.GetString("Timeline", Timeline);
            Name = info.GetString("Name", "");
            ImageKey = info.GetString("ImageKey", "");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Timeline", Timeline);
            info.AddValue("Name", Name);
            info.AddValue("ImageKey", ImageKey);
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
            switch (column)
            {
                case 0:
                    return Name;
                case 1:
                    return Timeline;
                case 2:
                    return ImageKey;
                default:
                    return "";
            }
        }

        public void setValueStr(int column, string value)
        {
            switch (column)
            {
                case 0:
                    Name = value;
                    break;
                case 1:
                    Timeline = value;
                    break;
                case 2:
                    ImageKey = value;
                    break;
            }
        }

        public bool hasBrowser(int column)
        {
            switch (column)
            {
                case 0:
                    return false;
                case 1:
                    return true;
                case 2:
                    return false;
                default:
                    return false;
            }
        }

        public Browser getBrowser(int column)
        {
            switch (column)
            {
                case 0:
                    return null;
                case 1:
                    return BrowserWindowController.createFileBrowser(BrowserWindowController.TimelineSearchPattern);
                case 2:
                    return null;
                default:
                    return null;
            }
        }
    }
}
