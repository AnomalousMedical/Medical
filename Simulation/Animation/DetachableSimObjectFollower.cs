using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Platform;

namespace Medical
{
    public class DetachableSimObjectFollower : Behavior
    {
        [Editable]
        bool attached;

        SimObject targetSimObject;
        Vector3 translationOffset;
        Quaternion rotationOffset;

        protected override void link()
        {
            
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            if (attached)
            {
                Vector3 trans = targetSimObject.Translation + Quaternion.quatRotate(targetSimObject.Rotation, translationOffset);
                Quaternion rotation = targetSimObject.Rotation * rotationOffset;
                updatePosition(ref trans, ref rotation);
            }
        }

        public void attach(String targetObject)
        {
            targetSimObject = Owner.getOtherSimObject(targetObject);
            if (targetSimObject != null)
            {
                attached = true;
                translationOffset = Owner.Translation - targetSimObject.Translation;
                translationOffset = Quaternion.quatRotate(targetSimObject.Rotation.inverse(), translationOffset);
                rotationOffset = Owner.Rotation;
            }
        }

        public void detach()
        {
            attached = false;
        }
    }
}
