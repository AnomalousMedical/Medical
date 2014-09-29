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
    public class OffsetRoot : ProxyChainSegmentBehavior
    {
        [Editable]
        private String proxySimObjectName;

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

        protected override void computePosition()
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
