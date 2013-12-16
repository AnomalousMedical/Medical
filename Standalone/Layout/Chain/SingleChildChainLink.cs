using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class SingleChildChainLink : LayoutChainLink
    {
        private SingleChildLayoutContainer singleChildContainer;

        public SingleChildChainLink(String name, SingleChildLayoutContainer layoutContainer)
            :base(name)
        {
            this.singleChildContainer = layoutContainer;
        }

        public override void setLayoutItem(LayoutElementName elementName, LayoutContainer container, Action removedCallback)
        {
            //This will never be called because this class does not expose any names
        }

        public override void removeLayoutItem(LayoutElementName elementName, LayoutContainer container)
        {
            //This will never be called because this class does not expose any names
        }

        protected internal override void _setChildContainer(LayoutContainer layoutContainer)
        {
            singleChildContainer.Child = layoutContainer;
        }

        public override LayoutContainer Container
        {
            get
            {
                return singleChildContainer;
            }
        }

        public override IEnumerable<LayoutElementName> ElementNames
        {
            get
            {
                return IEnumerableUtil<LayoutElementName>.EmptyIterator;
            }
        }
    }
}
