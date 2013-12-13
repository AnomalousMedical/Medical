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

        public MDIChainLink(String name, MDILayoutManager mdiManager)
            : base(name)
        {
            this.mdiManager = mdiManager;
        }

        public override void setLayoutItem(LayoutElementName elementName, LayoutContainer container, AnimationCompletedDelegate animationCompleted = null)
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
                return mdiManager;
            }
        }

        protected internal override void _setChildContainer(LayoutContainer layoutContainer)
        {
            //mdiManager.changeCenterParent(layoutContainer, (center) =>
            //{
            //    editorPreviewBorderLayout.Center = center;
            //});
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
