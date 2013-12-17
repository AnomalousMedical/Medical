using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class ActiveContainerTracker
    {
        private Dictionary<LayoutContainer, Action> activePanels = new Dictionary<LayoutContainer, Action>();

        public void add(LayoutContainer container, Action removedCallback)
        {
            if (container != null && removedCallback != null)
            {
                activePanels.Add(container, removedCallback);
            }
        }

        public void remove(LayoutContainer container)
        {
            Action removedCallback;
            if (container != null && activePanels.TryGetValue(container, out removedCallback))
            {
                removedCallback.Invoke();
                activePanels.Remove(container);
            }
        }
    }
}
