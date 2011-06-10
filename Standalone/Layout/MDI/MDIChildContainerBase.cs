using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    /// <summary>
    /// This is an MDIContainer that has children
    /// </summary>
    public abstract class MDIChildContainerBase : MDIContainerBase
    {
        public MDIChildContainerBase(DockLocation currentLocation)
            :base(currentLocation)
        {

        }

        public abstract void addChild(MDIWindow window);

        public abstract void addChild(MDIWindow window, MDIWindow previous, WindowAlignment alignment);

        public abstract void removeChild(MDIWindow window);

        internal abstract MDILayoutContainer.LayoutType Layout { get; }

        internal abstract void insertChild(MDIWindow child, MDIWindow previous, bool after);

        internal abstract void swapAndRemove(MDIContainerBase newChild, MDIContainerBase oldChild);

        internal abstract void promoteChild(MDIContainerBase mdiContainerBase, MDILayoutContainer mdiLayoutContainer);
    }
}
