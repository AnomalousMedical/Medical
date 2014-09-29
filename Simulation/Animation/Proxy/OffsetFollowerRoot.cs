using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Spine
{
    public class OffsetFollowerRoot : BehaviorInterface, ProxyChainSegment
    {
        [Editable]
        private String parentSegmentSimObjectName;

        [Editable]
        private String parentSegmentName = "Follower";

        [Editable]
        private String proxySimObjectName;

        [DoNotCopy]
        [DoNotSave]
        private ProxyChainSegment childSegment;

        [DoNotCopy]
        [DoNotSave]
        SimObject parentSegmentSimObject;

        [DoNotCopy]
        [DoNotSave]
        SimObject proxySimObject;

        [DoNotCopy]
        [DoNotSave]
        Vector3 translationOffset;

        [DoNotCopy]
        [DoNotSave]
        Quaternion rotationOffset;

        [DoNotCopy]
        [DoNotSave]
        Vector3 proxyTranslationOffset;

        [DoNotCopy]
        [DoNotSave]
        Quaternion proxyRotationOffset;

        protected override void link()
        {
            base.link();

            //Parent segment
            parentSegmentSimObject = Owner.getOtherSimObject(parentSegmentSimObjectName);
            if (parentSegmentSimObject == null)
            {
                blacklist("Cannot find parent segment SimObject {0}.", parentSegmentSimObjectName);
            }
            var parentSegment = parentSegmentSimObject.getElement(parentSegmentName) as ProxyChainSegment;
            if (parentSegment == null)
            {
                blacklist("Cannot find segment {0} on parent segment SimObject {1}.", parentSegmentName, parentSegmentSimObjectName);
            }
            parentSegment.setChildSegment(this);

            proxySimObject = Owner.getOtherSimObject(proxySimObjectName);
            if (proxySimObject == null)
            {
                blacklist("Cannot find proxy SimObject {0}.", proxySimObjectName);
            }

            Quaternion inverseTargetRot = parentSegmentSimObject.Rotation.inverse();

            translationOffset = Owner.Translation - parentSegmentSimObject.Translation;
            translationOffset = Quaternion.quatRotate(inverseTargetRot, translationOffset);

            rotationOffset = inverseTargetRot * Owner.Rotation;

            computeProxyOffsets(Owner.Translation, Owner.Rotation);
        }

        public void setChildSegment(ProxyChainSegment segment)
        {
            if (childSegment == null)
            {
                childSegment = segment;
            }
            else if (childSegment is MultiChildSegment)
            {
                childSegment.setChildSegment(segment);
            }
            else
            {
                var oldChild = childSegment;
                childSegment = new MultiChildSegment();
                childSegment.setChildSegment(oldChild);
                childSegment.setChildSegment(segment);
            }
        }

        public void updatePosition()
        {
            computePosition();
            if (childSegment != null)
            {
                childSegment.updatePosition();
            }
        }

        public Vector3 ProxyTranslationOffset
        {
            get
            {
                return proxyTranslationOffset;
            }
        }

        public Quaternion ProxyRotationOffset
        {
            get
            {
                return proxyRotationOffset;
            }
        }

        private void computePosition()
        {
            Vector3 trans = parentSegmentSimObject.Translation + Quaternion.quatRotate(parentSegmentSimObject.Rotation, translationOffset);
            Quaternion rotation = parentSegmentSimObject.Rotation * rotationOffset;

            computeProxyOffsets(trans, rotation);

            updatePosition(ref trans, ref rotation);
        }

        private void computeProxyOffsets(Vector3 trans, Quaternion rotation)
        {
            proxyTranslationOffset = trans - proxySimObject.Translation;
            proxyRotationOffset = proxySimObject.Rotation.inverse() * rotation;
        }
    }
}
