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
            :base(name)
        {
            this.mdiManager = mdiManager;
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
    }
}
