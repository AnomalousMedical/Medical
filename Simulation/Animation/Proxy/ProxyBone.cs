using Engine;
using Engine.Attributes;
using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Animation.Proxy
{
    class ProxyBone : BehaviorInterface
    {
        [Editable]
        private String spineRootSimObjectName = "Pelvis";

        [Editable]
        private String spineRootName = "SpineRoot";

        [DoNotCopy]
        [DoNotSave]
        ProxyRoot spineRoot;

        protected override void link()
        {
            base.link();

            var spineRootSimObject = Owner.getOtherSimObject(spineRootSimObjectName);
            if (spineRootSimObject == null)
            {
                blacklist("Cannot find parent Spine Root SimObject '{0}'.", spineRootSimObjectName);
            }

            spineRoot = spineRootSimObject.getElement(spineRootName) as ProxyRoot;
            if (spineRoot == null)
            {
                blacklist("Cannot find SpineRoot '{0}' on SimObject '{1}'.", spineRootName, spineRootSimObjectName);
            }
        }

        protected override void positionUpdated()
        {
            spineRoot.alertSpineMoved();
        }
    }
}
