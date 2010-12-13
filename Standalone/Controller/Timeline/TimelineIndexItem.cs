using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    class TimelineIndexItem : Saveable
    {
        public TimelineIndexItem(String timelineName)
        {

            this.TimelineName = timelineName;
        }

        public String Name { get; set; }

        public String Description { get; set; }

        public String TimelineName { get; private set; }

        #region Saveable Members

        private const String NAME = "Name";
        private const String DESCRIPTION = "Description";
        private const String TIMELINE_NAME = "TimelineName";

        protected TimelineIndexItem(LoadInfo info)
        {
            Name = info.GetString(NAME, null);
            Description = info.GetString(DESCRIPTION, null);
            TimelineName = info.GetString(TIMELINE_NAME);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(NAME, Name);
            info.AddValue(DESCRIPTION, Description);
            info.AddValue(TIMELINE_NAME, TimelineName);
        }

        #endregion
    }
}
