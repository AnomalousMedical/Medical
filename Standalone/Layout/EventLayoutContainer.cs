using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This class fires an event when a layout is changed.
    /// </summary>
    public class EventLayoutContainer : SingleChildConcreteLayoutContainer
    {
        public event Action<EventLayoutContainer> LayoutChanged;

        public override void layout()
        {
            if (LayoutChanged != null)
            {
                LayoutChanged.Invoke(this);
            }
            base.layout();
        }
    }
}
