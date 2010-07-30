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
        /// <summary>
        /// The container this window is currently inside of.
        /// Do not touch unless you are MDILayoutManager or MDILayoutContainer.
        /// </summary>
        internal MDILayoutContainer _ParentContainer { get; set; }
    }
}
