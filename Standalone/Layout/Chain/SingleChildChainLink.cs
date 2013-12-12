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

        public override void setLayoutItem(string positionHint, LayoutContainer container, AnimationCompletedDelegate animationCompleted = null)
        {
            //This only has one child and does not support this, so just fire that we are done with that container.
            if (animationCompleted != null)
            {
                animationCompleted.Invoke(container);
            }
        }

        public override void removeLayoutItem(string positionHint)
        {
            //Does nothing
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
    }
}
