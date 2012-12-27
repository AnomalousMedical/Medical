using Engine;
using Engine.Saving;
using Medical.GUI.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public abstract class DragAndDropViewBase : MyGUIView
    {
        public DragAndDropViewBase(String name)
            :base(name)
        {
            this.ViewLocation = Controller.AnomalousMvc.ViewLocations.Floating;
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
        }

        protected DragAndDropViewBase(LoadInfo info)
            : base(info)
        {
            
        }

        public abstract IEnumerable<DragAndDropItem> BaseItems
        {
            get;
        }

        internal abstract void _fireDragStarted(DragAndDropItem item, IntVector2 position);

        internal abstract void _fireDragging(DragAndDropItem item, IntVector2 position);

        internal abstract void _fireDragEnded(DragAndDropItem item, IntVector2 position);
    }

    public class DragAndDropView<ItemType> : DragAndDropViewBase
        where ItemType : DragAndDropItem
    {
        private List<ItemType> items;

        public event Action<ItemType, IntVector2> DragStarted;
        public event Action<ItemType, IntVector2> Dragging;
        public event Action<ItemType, IntVector2> DragEnded;

        public DragAndDropView(String name, params ItemType[] items)
            :base(name)
        {
            this.items = new List<ItemType>(items);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractList("DragAndDropItem", items);
        }

        protected DragAndDropView(LoadInfo info)
            : base(info)
        {
            info.RebuildList("DragAndDropItem", items);
        }

        public void AddItem(ItemType item)
        {
            items.Add(item);
        }

        public void RemoveItem(ItemType item)
        {
            items.Remove(item);
        }

        public override IEnumerable<DragAndDropItem> BaseItems
        {
            get
            {
                return items;
            }
        }

        public IEnumerable<DragAndDropItem> Items
        {
            get
            {
                return items;
            }
        }

        internal override void _fireDragStarted(DragAndDropItem item, IntVector2 position)
        {
            if (DragStarted != null)
            {
                DragStarted.Invoke((ItemType)item, position);
            }
        }

        internal override void _fireDragging(DragAndDropItem item, IntVector2 position)
        {
            if (Dragging != null)
            {
                Dragging.Invoke((ItemType)item, position);
            }
        }

        internal override void _fireDragEnded(DragAndDropItem item, IntVector2 position)
        {
            if (DragEnded != null)
            {
                DragEnded.Invoke((ItemType)item, position);
            }
        }
    }
}
