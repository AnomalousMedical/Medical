using Engine.Attributes;
using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose.Commands
{
    class ActivateFKMoverCommand : PoseCommandActionBase
    {
        [Editable]
        private String chainMoverSimObjectName = "this";

        [Editable]
        private String chainMoverName = "FKChainMover";

        [DoNotCopy]
        [DoNotSave]
        private FKChainMover chainMover;

        protected override void link()
        {
            var chainParentSimObject = Owner.getOtherSimObject(chainMoverSimObjectName);
            if (chainParentSimObject == null)
            {
                blacklist("Cannot find FK Chain Mover SimObject '{0}'", chainMoverSimObjectName);
            }

            chainMover = chainParentSimObject.getElement(chainMoverName) as FKChainMover;
            if (chainMover == null)
            {
                blacklist("Cannot find FKChainMover '{0}' on SimObject '{1}'", chainMoverName, chainMoverSimObjectName);
            }

            base.link();
        }

        public override void posingEnded()
        {
            chainMover.AllowFKUpdates = false;
        }

        public override void posingStarted()
        {
            chainMover.AllowFKUpdates = true;
        }
    }
}
