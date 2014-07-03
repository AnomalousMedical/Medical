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
    public class FKLink : Interface, FKElement
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
        private FKElement parent;

        [DoNotCopy]
        [DoNotSave]
        private List<FKElement> children = new List<FKElement>();

        protected override void link()
        {
            base.link();
            SimObject parentSimObject = Owner.getOtherSimObject(parentSimObjectName);
            if (parentSimObject == null)
            {
                blacklist("Cannot find Parent SimObject named '{0}'", parentSimObjectName);
            }

            parent = parentSimObject.getElement(parentSimObjectLinkName) as FKElement;
            if (parent == null)
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

            parent.addChild(this);
        }

        protected override void destroy()
        {
            if (parent != null)
            {
                parent.removeChild(this);
            }
            base.destroy();
        }

        public void addChild(FKElement child)
        {
            children.Add(child);
        }

        public void removeChild(FKElement child)
        {
            children.Remove(child);
        }

        public Vector3 Translation
        {
            get
            {
                return Owner.Translation;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return Owner.Rotation;
            }
        }

        public void addToChainState(FKChainState chain)
        {
            Quaternion inverseParentRot = parent.Rotation.inverse();
            Vector3 parentTrans = parent.Translation;

            Quaternion ourRotation = Owner.Rotation;

            //Figure out the translation in parent space, first, however, we must transform the center of rotation offset by the current rotation.
            //This makes the recorded translation offsets relative to the center of rotation point in world space instead of the center of this SimObject.
            Vector3 localTranslation = Owner.Translation + Quaternion.quatRotate(ref ourRotation, ref centerOfRotationOffset) - parentTrans;
            localTranslation = Quaternion.quatRotate(inverseParentRot, localTranslation);

            Quaternion localRotation = inverseParentRot * ourRotation;
            chain.setLinkState(Owner.Name, localTranslation, localRotation);

            foreach(var child in children)
            {
                child.addToChainState(chain);
            }
        }

        public void applyChainState(FKChainState chain)
        {
            Vector3 startTranslation = parent.Translation;
            Quaternion startRotation = parent.Rotation;

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
