using Medical.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class MDIChainLink : LayoutChainLink
    {
        private MDILayoutManager mdiManager;
        private ActiveContainerTracker activeContainers = new ActiveContainerTracker();

        public MDIChainLink(String name, MDILayoutManager mdiManager)
            : base(name)
        {
            this.mdiManager = mdiManager;
        }

        public override void setLayoutItem(LayoutElementName elementName, LayoutContainer container, Action removedCallback)
        {
            activeContainers.add(container, removedCallback);
        }

        public override void removeLayoutItem(LayoutElementName elementName, LayoutContainer container)
        {
            activeContainers.remove(container);
        }

        public override LayoutContainer Container
        {
            get
            {
                return mdiManager;
            }
        }

        protected internal override void _setChildContainer(LayoutContainer layoutContainer)
        {
            mdiManager.Center = layoutContainer;
        }

        public override IEnumerable<LayoutElementName> ElementNames
        {
            get
            {
                yield return new MDILayoutElementName(Name, DockLocation.None);
            }
        }
    }
}
