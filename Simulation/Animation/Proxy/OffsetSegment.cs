using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Animation.Proxy
{
    class OffsetSegment : ProxyChainSegmentBehavior
    {
        [Editable]
        private String proxySimObjectName;

        [Editable]
        private String proxyParentSimObjectName;

        [DoNotCopy]
        [DoNotSave]
        SimObject proxySimObject;

        [DoNotCopy]
        [DoNotSave]
        SimObject proxyParentSimObject;

        protected override void link()
        {
            //Proxy sim object
            proxySimObject = Owner.getOtherSimObject(proxySimObjectName);
            if (proxySimObject == null)
            {
                blacklist("Cannot find proxy SimObject {0}.", proxySimObjectName);
            }

            //Proxy Parent SimObject
            proxyParentSimObject = Owner.getOtherSimObject(proxyParentSimObjectName);
            if (proxyParentSimObject == null)
            {
                blacklist("Cannot find proxy parent SimObject {0}.", proxyParentSimObjectName);
            }

            base.link();
        }

        protected override void computePosition()
        {
            Quaternion inverseTargetRot = proxyParentSimObject.Rotation.inverse();

            Vector3 translationOffset = proxySimObject.Translation - proxyParentSimObject.Translation;
            translationOffset = Quaternion.quatRotate(inverseTargetRot, translationOffset);

            Quaternion rotationOffset = inverseTargetRot * proxySimObject.Rotation;

            Vector3 translation = parentSegmentSimObject.Translation + Quaternion.quatRotate(parentSegmentSimObject.Rotation, translationOffset);
            Quaternion rotation = parentSegmentSimObject.Rotation * rotationOffset;

            updatePosition(ref translation, ref rotation);
        }
    }
}
