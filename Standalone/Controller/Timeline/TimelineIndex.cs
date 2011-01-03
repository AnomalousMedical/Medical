using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class TimelineIndex : Saveable
    {
        private List<TimelineIndexItem> items = new List<TimelineIndexItem>();

        public TimelineIndex()
        {

        }

        public void addItem(TimelineIndexItem item)
        {
            items.Add(item);
        }

        public void removeItem(TimelineIndexItem item)
        {
            items.Remove(item);
        }

        public IEnumerable<TimelineIndexItem> Items
        {
            get
            {
                return items;
            }
        }

        #region Saveable Members

        private const String ITEM = "Item";

        protected TimelineIndex(LoadInfo info)
        {
            info.RebuildList<TimelineIndexItem>(ITEM, items);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList<TimelineIndexItem>(ITEM, items);
        }

        #endregion
    }
}
