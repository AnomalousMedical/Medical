using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class ArgumentCallbackTask<T> : ArgumentTask<T>
    {
        public event Action<ArgumentCallbackTask<T>> OnClicked;
        private bool active = false;

        public ArgumentCallbackTask(String uniqueName, String name, String iconName, String category)
            : this(name, uniqueName, iconName, category, DEFAULT_WEIGHT, true)
        {

        }

        public ArgumentCallbackTask(String uniqueName, String name, String iconName, String category, int weight)
            : this(name, uniqueName, iconName, category, weight, true)
        {

        }

        public ArgumentCallbackTask(String uniqueName, String name, String iconName, String category, bool showOnTaskbar)
            : this(name, uniqueName, iconName, category, DEFAULT_WEIGHT, showOnTaskbar)
        {

        }

        public ArgumentCallbackTask(String uniqueName, String name, String iconName, String category, int weight, bool showOnTaskbar, Action<ArgumentCallbackTask<T>> callback = null)
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
                OnClicked.Invoke(this);
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
    }
}
