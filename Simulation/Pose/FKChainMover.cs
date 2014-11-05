using BEPUikPlugin;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using Medical.Animation.Proxy;
using Medical.Pose.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class FKChainMover : BehaviorInterface
    {
        [Editable]
        private String chainStartSimObjectName = "this";

        [Editable]
        private String boneName = "IKBone";

        [Editable]
        private String fkElementName = "FKLink";

        [DoNotCopy]
        [DoNotSave]
        Vector3 lastTranslation;

        [DoNotCopy]
        [DoNotSave]
        Quaternion lastRotation;

        [DoNotCopy]
        [DoNotSave]
        private FKElement fkElement;

        [DoNotCopy]
        [DoNotSave]
        FKChainState chain = new FKChainState();

        [DoNotCopy]
        [DoNotSave]
        private bool allowFKUpdates = false;

        protected override void link()
        {
            base.link();

            var chainStartSimObject = Owner.getOtherSimObject(chainStartSimObjectName);
            if(chainStartSimObject == null)
            {
                blacklist("Cannot find the chain start SimObject '{0}'", chainStartSimObjectName);
            }
            var ikBone = chainStartSimObject.getElement(boneName) as BEPUikBone;
            if(ikBone == null)
            {
                blacklist("Cannot find the ik bone '{0}' in chain start SimObject '{1}'", boneName, chainStartSimObjectName);
            }

            fkElement = chainStartSimObject.getElement(fkElementName) as FKElement;
            if (fkElement == null)
            {
                blacklist("Cannot find the fk element '{0}' in chain start SimObject '{1}'", fkElementName, chainStartSimObjectName);
            }

            lastTranslation = Owner.Translation;
            lastRotation = Owner.Rotation;

            //Do a late link to update the chain, since we don't know if the entire thing has been built yet.
            registerLateLinkAction(() =>
                {
                    updateFKChain();
                    fkElement.ChainStateApplied += fkElement_ChainStateApplied;
                });
        }

        protected override void destroy()
        {
            fkElement.ChainStateApplied -= fkElement_ChainStateApplied;
            base.destroy();
        }

        protected override void positionUpdated()
        {
            //Need way to identify that we are moving as part of ik solving
            if(allowFKUpdates && (lastTranslation != Owner.Translation || lastRotation != Owner.Rotation))
            {
                //Update the element's children, skip the element itself it has already been set (and causes an infinite loop).
                foreach(var child in fkElement.Children)
                {
                    child.applyChainState(chain);
                }

                lastTranslation = Owner.Translation;
                lastRotation = Owner.Rotation;
            }
            base.positionUpdated();
        }

        public void updateFKChain()
        {
            fkElement.addToChainState(chain);
        }

        public bool AllowFKUpdates
        {
            get
            {
                return allowFKUpdates;
            }
            set
            {
                allowFKUpdates = value;
            }
        }

        void fkElement_ChainStateApplied(FKElement element, FKChainState chainState)
        {
            foreach (String name in chain.ChainStateNames)
            {
                var state = chainState[name];
                chain.setLinkState(name, state.LocalTranslation, state.LocalRotation);
            }
        }
    }
}
