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
        private String offsetRootSimObjectName;

        [Editable]
        private String offsetRootName;

        [DoNotCopy]
        [DoNotSave]
        SimObject proxySimObject;

        [DoNotCopy]
        [DoNotSave]
        private OffsetRoot offsetRoot;

        protected override void link()
        {
            base.link();

            //Offset Root
            var offsetRootSimObject = Owner.getOtherSimObject(offsetRootSimObjectName);
            if (offsetRootSimObject == null)
            {
                blacklist("Cannot find offset root SimObject {0}.", offsetRootSimObjectName);
            }
            offsetRoot = offsetRootSimObject.getElement(offsetRootName) as OffsetRoot;
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

        protected override void computePosition()
        {
            Vector3 translation = proxySimObject.Translation + offsetRoot.ProxyTranslationOffset;
            Quaternion rotation = proxySimObject.Rotation *offsetRoot.ProxyRotationOffset;
            updatePosition(ref translation, ref rotation);
        }
    }
}
