using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    public abstract class MDIContainerBase : LayoutContainer
    {
        protected MDILayoutManager layoutManager;

        /// <summary>
        /// Set the MDILayoutManager.
        /// Do not touch unless you are MDILayoutManager.
        /// </summary>
        internal void _setMDILayoutManager(MDILayoutManager layoutManager)
        {
            this.layoutManager = layoutManager;
        }

        /// <summary>
        /// The container this window is currently inside of.
        /// Do not touch unless you are MDILayoutManager or MDILayoutContainer.
        /// </summary>
        internal MDILayoutContainer _CurrentContainer { get; set; }
    }
}
