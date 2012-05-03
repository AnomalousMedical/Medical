using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Editor
{
    public abstract class SaveableEditableItemCollection<ItemType> : EditableItemCollection<ItemType>, Saveable
        where ItemType : SaveableEditableItem
    {
        protected SaveableEditableItemCollection()
        {

        }

        protected SaveableEditableItemCollection(LoadInfo info)
        {
            info.RebuildList<ItemType>("Items", items);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractList<ItemType>("Items", items);
        }
    }
}
