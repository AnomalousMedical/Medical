using Engine;
using Medical.GUI;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public class DragAndDropTaskManager<ItemType>
        where ItemType : DragAndDropItem
    {
        public event Action<ItemType, IntVector2> DragStarted;
        public event Action<ItemType, IntVector2> Dragging;
        public event Action<ItemType, IntVector2> DragEnded;
        public event Action<ItemType> ItemActivated;

        private List<CallbackTaskWithObject<ItemType>> tasks = new List<CallbackTaskWithObject<ItemType>>();
        private ImageBox dragIconPreview;
        private IntVector2 dragMouseStartPosition;
        private bool firstDrag;

        public DragAndDropTaskManager()
        {
            Category = "DragDrop";
        }

        public DragAndDropTaskManager(params ItemType[] items)
            :this()
        {
            foreach (ItemType item in items)
            {
                AddItem(item);
            }
        }

        public void CreateIconPreview()
        {
            if (dragIconPreview == null)
            {
                dragIconPreview = (ImageBox)Gui.Instance.createWidgetT("ImageBox", "ImageBox", 0, 0, ScaleHelper.Scaled(32), ScaleHelper.Scaled(32), Align.Default, "Info", "DragAndDropPreview");
                dragIconPreview.Visible = false;
            }
        }

        public void DestroyIconPreview()
        {
            if (dragIconPreview != null)
            {
                Gui.Instance.destroyWidget(dragIconPreview);
                dragIconPreview = null;
            }
        }

        public void AddItem(ItemType item)
        {
            CallbackTaskWithObject<ItemType> task = new CallbackTaskWithObject<ItemType>(item.Name, String.Format("Add {0}", item.Name), item.Icon, Category)
            {
                Dragable = true,
                UserObject = item
            };
            task.OnClicked += task_OnClicked;
            task.DragStarted += taskDragStarted;
            task.Dragged += taskDragged;
            task.DragEnded += taskDragEnded;

            tasks.Add(task);
        }

        public IEnumerable<Task> Tasks
        {
            get
            {
                return tasks;
            }
        }

        public String Category { get; set; }

        void task_OnClicked(CallbackTask task)
        {
            if (firstDrag)
            {
                _fireItemActivated(((CallbackTaskWithObject<ItemType>)task).UserObject);
            }
        }

        void taskDragStarted(Task task, IntVector2 position)
        {
            dragMouseStartPosition = position;
            firstDrag = true;
        }

        void taskDragged(Task task, IntVector2 position)
        {
            ItemType item = ((CallbackTaskWithObject<ItemType>)task).UserObject;
            dragIconPreview.setPosition(position.x - (dragIconPreview.Width / 2), position.y - (int)(dragIconPreview.Height * .75f));
            if (!dragIconPreview.Visible && (Math.Abs(dragMouseStartPosition.x - position.x) > 5 || Math.Abs(dragMouseStartPosition.y - position.y) > 5))
            {
                dragIconPreview.Visible = true;
                dragIconPreview.setItemResource(task.IconName);
                LayerManager.Instance.upLayerItem(dragIconPreview);
                if (firstDrag)
                {
                    _fireDragStarted(item, position);
                    firstDrag = false;
                }
            }
            _fireDragging(item, position);
        }

        void taskDragEnded(Task task, IntVector2 position)
        {
            if (dragIconPreview.Visible)
            {
                dragIconPreview.Visible = false;
                _fireDragEnded(((CallbackTaskWithObject<ItemType>)task).UserObject, position);
            }
        }

        void _fireDragStarted(ItemType item, IntVector2 position)
        {
            if (DragStarted != null)
            {
                DragStarted.Invoke(item, position);
            }
        }

        void _fireDragging(ItemType item, IntVector2 position)
        {
            if (Dragging != null)
            {
                Dragging.Invoke(item, position);
            }
        }

        void _fireDragEnded(ItemType item, IntVector2 position)
        {
            if (DragEnded != null)
            {
                DragEnded.Invoke(item, position);
            }
        }

        void _fireItemActivated(ItemType item)
        {
            if (ItemActivated != null)
            {
                ItemActivated.Invoke(item);
            }
        }
    }
}
