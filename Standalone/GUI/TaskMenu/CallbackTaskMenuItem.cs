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

        public CallbackTaskMenuItem(String name, String iconName, String category)
            : this(name, iconName, category, DEFAULT_WEIGHT, true)
        {

        }

        public CallbackTaskMenuItem(String name, String iconName, String category, int weight)
            : this(name, iconName, category, weight, true)
        {
            
        }

        public CallbackTaskMenuItem(String name, String iconName, String category, bool showOnTaskbar)
            : this(name, iconName, category, DEFAULT_WEIGHT, showOnTaskbar)
        {
            
        }

        public CallbackTaskMenuItem(String name, String iconName, String category, int weight, bool showOnTaskbar)
            : base(name, iconName, category)
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
