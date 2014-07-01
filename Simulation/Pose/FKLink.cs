using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class FKLink : Interface
    {
        [Editable]
        private String parentSimObjectName;

        [Editable]
        private String parentSimObjectLinkName = "FKLink";

        [DoNotCopy]
        [DoNotSave]
        private FKLink parentLink;

        [DoNotCopy]
        [DoNotSave]
        private List<FKLink> children = new List<FKLink>();

        protected override void link()
        {
            base.link();
            if (!String.IsNullOrEmpty(parentSimObjectName))
            {
                SimObject parentSimObject = Owner.getOtherSimObject(parentSimObjectName);
                if (parentSimObject == null)
                {
                    blacklist("Cannot find Parent SimObject named '{0}'", parentSimObjectName);
                }

                parentLink = parentSimObject.getElement(parentSimObjectLinkName) as FKLink;
                if (parentLink == null)
                {
                    blacklist("Cannot find FKLink '{0}' on Parent SimObject '{1}'", parentSimObjectLinkName, parentSimObjectName);
                }

                parentLink.addChild(this);
            }
            else
            {
                PoseableObjectsManager.addFkChainRoot(Owner.Name, this);
            }
        }

        protected override void destroy()
        {
            if (parentLink != null)
            {
                parentLink.removeChild(this);
            }
            else
            {
                PoseableObjectsManager.removeFkChainRoot(Owner.Name);
            }
            base.destroy();
        }

        public void addChild(FKLink child)
        {
            children.Add(child);
        }

        public void removeChild(FKLink child)
        {
            children.Remove(child);
        }

        public void addToChainState(FKChainState chain)
        {
            if(parentLink != null)
            {
                Quaternion inverseParentRot = parentLink.Owner.Rotation.inverse();
                Vector3 parentTrans = parentLink.Owner.Translation;

                Vector3 localTranslation = Owner.Translation - parentTrans;
                localTranslation = Quaternion.quatRotate(inverseParentRot, localTranslation);

                Quaternion localRotation = inverseParentRot * Owner.Rotation;
                chain.setLinkState(Owner.Name, localTranslation, localRotation);
            }
            else
            {
                chain.setLinkState(Owner.Name, Owner.Translation, Owner.Rotation);
            }

            foreach(var child in children)
            {
                child.addToChainState(chain);
            }
        }

        public void applyChainState(FKChainState chain)
        {
            Vector3 startTranslation = Vector3.Zero;
            Quaternion startRotation = Quaternion.Identity;
            if(parentLink != null)
            {
                startTranslation = parentLink.Owner.Translation;
                startRotation = parentLink.Owner.Rotation;
            }

            FKLinkState linkState = chain[Owner.Name];

            Vector3 newTrans = startTranslation + Quaternion.quatRotate(startRotation, linkState.LocalTranslation);
            Quaternion newRot = startRotation * linkState.LocalRotation;

            this.updatePosition(ref newTrans, ref newRot);

            foreach (var child in children)
            {
                child.applyChainState(chain);
            }
        }
    }
}
