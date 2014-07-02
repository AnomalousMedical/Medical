using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.Editing.Renderers;
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

        /// <summary>
        /// The SimObject to use as the center of rotation, this can be blank to use this object's center of rotation instead.
        /// </summary>
        [Editable]
        private String jointSimObjectName;

        [DoNotCopy]
        [DoNotSave]
        private Vector3 centerOfRotationOffset = Vector3.Zero;

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

                if(!String.IsNullOrEmpty(jointSimObjectName))
                {
                    SimObject jointSimObject = Owner.getOtherSimObject(jointSimObjectName);
                    if(jointSimObject == null)
                    {
                        blacklist("Cannot find Joint SimObject named '{0}'", jointSimObjectName);
                    }
                    //Find the center of rotation in this object's local space
                    centerOfRotationOffset = jointSimObject.Translation - Owner.Translation;
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

                Quaternion ourRotation = Owner.Rotation;

                //Figure out the translation in parent space, first, however, we must transform the center of rotation offset by the current rotation.
                //This makes the recorded translation offsets relative to the center of rotation point in world space instead of the center of this SimObject.
                Vector3 localTranslation = Owner.Translation + Quaternion.quatRotate(ref ourRotation, ref centerOfRotationOffset) - parentTrans;
                localTranslation = Quaternion.quatRotate(inverseParentRot, localTranslation);

                Quaternion localRotation = inverseParentRot * ourRotation;
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

            //Figure out the new position using the parent's position.
            Vector3 newTrans = startTranslation + Quaternion.quatRotate(startRotation, linkState.LocalTranslation);
            Quaternion newRot = startRotation * linkState.LocalRotation;

            //Transform to the real center point from the center of rotation
            newTrans -= Quaternion.quatRotate(ref newRot, ref centerOfRotationOffset);

            this.updatePosition(ref newTrans, ref newRot);

            foreach (var child in children)
            {
                child.applyChainState(chain);
            }
        }
    }
}
