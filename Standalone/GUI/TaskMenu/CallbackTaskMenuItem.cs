using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    /// <summary>
    /// A TaskMenuItem that will fire a delegate.
    /// </summary>
    public class CallbackTaskMenuItem : TaskMenuItem
    {
        public delegate void ClickedCallback(CallbackTaskMenuItem item);

        public event ClickedCallback OnClicked;

        public CallbackTaskMenuItem(String uniqueName, String name, String iconName, String category)
            : this(name, uniqueName, iconName, category, DEFAULT_WEIGHT, true)
        {

        }

        public CallbackTaskMenuItem(String uniqueName, String name, String iconName, String category, int weight)
            : this(name, uniqueName, iconName, category, weight, true)
        {
            
        }

        public CallbackTaskMenuItem(String uniqueName, String name, String iconName, String category, bool showOnTaskbar)
            : this(name, uniqueName, iconName, category, DEFAULT_WEIGHT, showOnTaskbar)
        {
            
        }

        public CallbackTaskMenuItem(String uniqueName, String name, String iconName, String category, int weight, bool showOnTaskbar)
            : base(uniqueName, name, iconName, category)
        {
            this.Weight = weight;
            this.ShowOnTaskbar = showOnTaskbar;
        }

        public override void clicked()
        {
            if(OnClicked != null)
            {
                OnClicked.Invoke(this);
            }
        }
    }
}
