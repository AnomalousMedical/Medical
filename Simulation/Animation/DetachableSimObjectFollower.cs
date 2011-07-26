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
        public String TargetSimObjectName { get; set; }

        [Editable]
        bool attached;

        SimObject targetSimObject;
        Vector3 translationOffset;
        Quaternion rotationOffset;

        protected override void link()
        {
            targetSimObject = Owner.getOtherSimObject(TargetSimObjectName);
            if (targetSimObject == null)
            {
                blacklist("Cannot find target SimObject {0}.", TargetSimObjectName);
            }
            if (attached)
            {
                attach();
            }
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

        public void attach()
        {
            attached = true;
            translationOffset = Owner.Translation - targetSimObject.Translation;
            translationOffset = Quaternion.quatRotate(targetSimObject.Rotation.inverse(), translationOffset);
            rotationOffset = Owner.Rotation;
        }

        public void detach()
        {
            attached = false;
        }
    }
}
