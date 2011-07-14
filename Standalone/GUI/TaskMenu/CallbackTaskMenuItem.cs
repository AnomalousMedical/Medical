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
        public delegate void ClickedCallback();

        public event ClickedCallback OnClicked;

        public CallbackTaskMenuItem(String name, String iconName, String category)
            :base(name, iconName, category)
        {

        }

        public override void clicked()
        {
            if(OnClicked != null)
            {
                OnClicked.Invoke();
            }
        }
    }
}
