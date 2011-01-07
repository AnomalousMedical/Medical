using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    /// <summary>
    /// The base class for items that go into the MDILayoutContainer.
    /// </summary>
    public abstract class MDIContainerBase : LayoutContainer
    {
        protected MDIContainerBase()
        {
            Scale = 100.0f;
        }

        /// <summary>
        /// The scale of this window in its parent container. The scaling can be
        /// any arbitray number, which will be added up by LayoutContainers. So
        /// the final size of the MDI element will be the percentage this number
        /// is of the whole row.
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// The container this window is currently inside of.
        /// Do not touch unless you are MDILayoutManager or MDILayoutContainer.
        /// </summary>
        internal MDILayoutContainer _ParentContainer { get; set; }

        public abstract MDIWindow findWindowAtPosition(float mouseX, float mouseY);
    }
}
