using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// A TaskMenuItem that will fire a delegate.
    /// </summary>
    public class CallbackTask : Task
    {
        public delegate void ClickedCallback(CallbackTask item);

        public event ClickedCallback OnClicked;
        public event Action<CallbackTask, IntVector2> DragStarted;
        public event Action<CallbackTask, IntVector2> Dragged;
        public event Action<CallbackTask, IntVector2> DragEnded;
        private bool active = false;

        public CallbackTask(String uniqueName, String name, String iconName, String category, ClickedCallback callback = null)
            : this(uniqueName, name, iconName, category, DEFAULT_WEIGHT, true, callback)
        {

        }

        public CallbackTask(String uniqueName, String name, String iconName, String category, int weight, ClickedCallback callback = null)
            : this(uniqueName, name, iconName, category, weight, true, callback)
        {

        }

        public CallbackTask(String uniqueName, String name, String iconName, String category, bool showOnTaskbar, ClickedCallback callback = null)
            : this(uniqueName, name, iconName, category, DEFAULT_WEIGHT, showOnTaskbar, callback)
        {

        }

        public CallbackTask(String uniqueName, String name, String iconName, String category, int weight, bool showOnTaskbar, ClickedCallback callback = null)
            : base(uniqueName, name, iconName, category)
        {
            this.Weight = weight;
            this.ShowOnTaskbar = showOnTaskbar;
            if (callback != null)
            {
                this.OnClicked += callback;
            }
        }

        public override void clicked(TaskPositioner positioner)
        {
            if (OnClicked != null)
            {
                CurrentTaskPositioner = positioner;
                OnClicked.Invoke(this);
                CurrentTaskPositioner = null;
            }
        }

        public override void dragStarted(IntVector2 position)
        {
            if (DragStarted != null)
            {
                DragStarted.Invoke(this, position);
            }
        }

        public override void dragged(IntVector2 position)
        {
            if (Dragged != null)
            {
                Dragged.Invoke(this, position);
            }
        }

        public override void dragEnded(IntVector2 position)
        {
            if (DragEnded != null)
            {
                DragEnded.Invoke(this, position);
            }
        }

        public override bool Active
        {
            get { return active; }
        }

        public void setActive(bool value)
        {
            active = value;
        }

        public void setIcon(String name)
        {
            if (IconName != name)
            {
                IconName = name;
                fireIconChanged();
            }
        }

        public void closeTask()
        {
            this.fireItemClosed();
        }

        /// <summary>
        /// The TaskPositioner that was set when this item was clicked, only valid during an OnClicked event.
        /// </summary>
        public TaskPositioner CurrentTaskPositioner { get; private set; }
    }
}
