using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This class represents a chain of layout containers that can be mixed and matched
    /// as needed to provide various gui layouts.
    /// </summary>
    public abstract class LayoutChainLink
    {
        public LayoutChainLink(String name)
        {
            this.Name = name;
        }

        public abstract void setLayoutItem(LayoutElementName elementName, LayoutContainer container, Action removedCallback);

        public abstract void removeLayoutItem(LayoutElementName elementName, LayoutContainer container);

        public String Name { get; private set; }

        public abstract LayoutContainer Container { get; }

        public abstract IEnumerable<LayoutElementName> ElementNames { get; }

        protected internal abstract void _setChildContainer(LayoutContainer layoutContainer);
    }
}
