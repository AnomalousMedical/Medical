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
    class ProxyOffsetFollower : BehaviorInterface, ProxyChainSegment
    {
        [Editable]
        private String proxySimObjectName;

        [Editable]
        private String parentSegmentSimObjectName;

        [Editable]
        private String parentSegmentName = "Follower";

        [Editable]
        private String offsetRootSimObjectName;

        [Editable]
        private String offsetRootName;

        [DoNotCopy]
        [DoNotSave]
        SimObject proxySimObject;

        [DoNotCopy]
        [DoNotSave]
        private ProxyChainSegment childSegment;

        [DoNotCopy]
        [DoNotSave]
        private OffsetFollowerRoot offsetRoot;

        protected override void link()
        {
            base.link();

            //Parent segment
            var parentSegmentSimObject = Owner.getOtherSimObject(parentSegmentSimObjectName);
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

            //Offset Root
            var offsetRootSimObject = Owner.getOtherSimObject(offsetRootSimObjectName);
            if (offsetRootSimObject == null)
            {
                blacklist("Cannot find offset root SimObject {0}.", offsetRootSimObjectName);
            }
            offsetRoot = offsetRootSimObject.getElement(offsetRootName) as OffsetFollowerRoot;
            if (offsetRoot == null)
            {
                blacklist("Cannot find offset root {0} on SimObject {1}.", offsetRootName, offsetRootSimObjectName);
            }

            //Proxy sim object
            proxySimObject = Owner.getOtherSimObject(proxySimObjectName);
            if (proxySimObject == null)
            {
                blacklist("Cannot find proxy SimObject {0}.", proxySimObjectName);
            }
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

        private void computePosition()
        {
            Vector3 translation = proxySimObject.Translation + offsetRoot.ProxyTranslationOffset;
            Quaternion rotation = proxySimObject.Rotation *offsetRoot.ProxyRotationOffset;
            updatePosition(ref translation, ref rotation);
        }
    }
}
