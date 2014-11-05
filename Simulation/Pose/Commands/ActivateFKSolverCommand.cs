using Engine.Attributes;
using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose.Commands
{
    class ActivateFKSolverCommand : PoseCommandActionBase
    {
        [Editable]
        private String chainParentSimObjectName = "this";

        [Editable]
        private String chainParentName = "FKChainParent";

        [DoNotCopy]
        [DoNotSave]
        private IKChainParentBehavior chainParent;

        protected override void link()
        {
            var chainParentSimObject = Owner.getOtherSimObject(chainParentSimObjectName);
            if(chainParentSimObject == null)
            {
                blacklist("Cannot find Chain Parent SimObject '{0}'", chainParentSimObjectName);
            }

            chainParent = chainParentSimObject.getElement(chainParentName) as IKChainParentBehavior;
            if(chainParent == null)
            {
                blacklist("Cannot find Chain Parent '{0}' on SimObject '{1}'", chainParentName, chainParentSimObjectName);
            }

            base.link();
        }

        public override void posingEnded()
        {
            chainParent.AllowFKUpdates = false;
        }

        public override void posingStarted()
        {
            chainParent.AllowFKUpdates = true;
        }
    }
}
