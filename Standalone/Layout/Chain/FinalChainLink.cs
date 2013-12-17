using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This is a chain link for a LayoutContainer that has no children. It does not have to be used to have a
    /// valid layout chain, but you can use this for your final element. Attempting to add a link after this
    /// one will cause an exception to be thrown.
    /// </summary>
    class FinalChainLink : LayoutChainLink
    {
        private LayoutContainer layoutContainer;

        public FinalChainLink(String name, LayoutContainer layoutContainer)
            :base(name)
        {
            this.layoutContainer = layoutContainer;
        }

        public override void setLayoutItem(LayoutElementName elementName, LayoutContainer container, Action removedCallback)
        {
            //This will never be called because this class does not expose any names
        }

        public override void removeLayoutItem(LayoutElementName elementName, LayoutContainer container)
        {
            //This will never be called because this class does not expose any names
        }

        public override LayoutContainer Container
        {
            get
            {
                return layoutContainer;
            }
        }

        public override IEnumerable<LayoutElementName> ElementNames
        {
            get
            {
                return IEnumerableUtil<LayoutElementName>.EmptyIterator;
            }
        }

        protected internal override void _setChildContainer(LayoutContainer layoutContainer)
        {
            throw new NotImplementedException();
        }
    }
}
